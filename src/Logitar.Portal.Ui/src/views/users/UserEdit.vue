<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import AuthenticationInformation from "@/components/users/AuthenticationInformation.vue";
import ContactInformation from "@/components/users/ContactInformation.vue";
import PasswordInput from "@/components/users/PasswordInput.vue";
import PersonalInformation from "@/components/users/PersonalInformation.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import SignOutUser from "@/components/sessions/SignOutUser.vue";
import StatusInfo from "@/components/shared/StatusInfo.vue";
import ToggleUserStatus from "@/components/users/ToggleUserStatus.vue";
import type { ApiError, ErrorDetail } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/customAttributes";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { Realm } from "@/types/realms";
import type { ToastUtils } from "@/types/components";
import type { User, UserUpdatedEvent } from "@/types/users";
import { createUser, readUser } from "@/api/users";
import { handleErrorKey, registerTooltipsKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRealm } from "@/api/realms";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const registerTooltips = inject(registerTooltipsKey) as () => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { d, t } = useI18n();

const configuration = ref<Configuration>();
const confirm = ref<string>("");
const customAttributes = ref<CustomAttribute[]>([]);
const hasLoaded = ref<boolean>(false);
const password = ref<string>("");
const realm = ref<Realm>();
const realmId = ref<string>();
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);
const user = ref<User>();

const hasChanges = computed<boolean>(() => !user.value && (Boolean(realm) || Boolean(uniqueName) || Boolean(password) || Boolean(confirm)));
const isPasswordRequired = computed<boolean>(() => Boolean(password.value) || Boolean(confirm.value));
const passwordSettings = computed<PasswordSettings | undefined>(() => realm.value?.passwordSettings ?? configuration.value?.passwordSettings);
const title = computed<string>(() => user.value?.fullName ?? user.value?.uniqueName ?? t("users.title.new"));
const uniqueNameSettings = computed<UniqueNameSettings | undefined>(() => realm.value?.uniqueNameSettings ?? configuration.value?.uniqueNameSettings);

function setModel(model: User): void {
  user.value = model;
  customAttributes.value = model.customAttributes.map((item) => ({ ...item }));
  realm.value = model.realm;
  realmId.value = model.realm?.id;
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onCreate = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    const user = await createUser({
      realm: realmId.value,
      uniqueName: uniqueName.value,
      password: password.value || undefined,
    });
    setModel(user);
    toasts.success("users.created");
    router.replace({ name: "UserEdit", params: { id: user.id } }); // TODO(fpion): the realm is not set correctly
  } catch (e: unknown) {
    const { data, status } = e as ApiError;
    if (status === 409 && (data as ErrorDetail)?.errorCode === "UniqueNameAlreadyUsed") {
      uniqueNameAlreadyUsed.value = true;
    } else {
      handleError(e);
    }
  }
});

function onRealmSelected(selected?: Realm) {
  realm.value = selected;
  realmId.value = selected?.id;
}

function onStatusToggled(updatedUser: User) {
  if (user.value) {
    user.value.disabledBy = updatedUser.disabledBy;
    user.value.disabledOn = updatedUser.disabledOn;
    user.value.isDisabled = updatedUser.isDisabled;
  }
}

function onUserSignedOut(signedOut: User): void {
  if (user.value) {
    user.value.updatedBy = signedOut.updatedBy;
    user.value.updatedOn = signedOut.updatedOn;
    user.value.version = signedOut.version;
  }
}

function onUserUpdated(event: UserUpdatedEvent): void {
  user.value = event.user;
  if (account.authenticated?.id === event.user.id) {
    account.setUser(event.user);
  }
  toasts.success(event.toast ?? "users.updated");
}

onMounted(async () => {
  try {
    configuration.value = await readConfiguration();
    const id = route.params.id?.toString();
    const realmIdQuery = route.query.realm?.toString();
    if (id) {
      const user = await readUser(id);
      setModel(user);
    } else if (realmIdQuery) {
      const foundRealm = await readRealm(realmIdQuery);
      realm.value = foundRealm;
      realmId.value = foundRealm.id;
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
  registerTooltips();
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <template v-if="hasLoaded">
      <h1>{{ title }}</h1>
      <app-alert dismissible variant="warning" v-model="uniqueNameAlreadyUsed">
        <strong>{{ t("uniqueName.alreadyUsed.lead") }}</strong> {{ t("uniqueName.alreadyUsed.help") }}
      </app-alert>
      <template v-if="user">
        <status-detail :aggregate="user" />
        <p v-if="user.authenticatedOn">
          {{ t("users.authenticatedOnFormat", { date: d(user.authenticatedOn, "medium") }) }}
          <br />
          <RouterLink
            :to="{
              name: 'SessionList',
              query: { realm: user.realm?.id ?? '', user: user.id, sort: 'UpdatedOn', isDescending: 'true', page: '1', count: '10' },
            }"
          >
            {{ t("sessions.view") }}
          </RouterLink>
        </p>
        <p v-if="user.disabledBy && user.disabledOn">
          <StatusInfo :actor="user.disabledBy" class="text-danger" :date="user.disabledOn" format="users.disabledFormat" />
        </p>
        <div class="mb-3">
          <icon-button class="me-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
          <ToggleUserStatus class="mx-1" :user="user" @user-updated="onStatusToggled" />
          <SignOutUser class="ms-1" :user="user" @signed-out="onUserSignedOut" />
        </div>
        <RealmSelect disabled :model-value="realmId" />
        <app-tabs>
          <app-tab active title="users.tabs.authentication">
            <AuthenticationInformation
              :password-settings="passwordSettings"
              :unique-name-settings="uniqueNameSettings"
              :user="user"
              @user-updated="onUserUpdated"
            />
          </app-tab>
          <app-tab title="users.tabs.personal">
            <PersonalInformation :user="user" @user-updated="onUserUpdated" />
          </app-tab>
          <app-tab title="users.tabs.contact">
            <ContactInformation :user="user" @user-updated="onUserUpdated" />
          </app-tab>
          <app-tab title="roles.title.list">
            <p>TODO(fpion): implement user roles</p>
          </app-tab>
          <app-tab title="users.identifiers.title">
            <p>TODO(fpion): implement user identifiers</p>
          </app-tab>
          <app-tab title="customAttributes.title">
            <!-- TODO(fpion): how to save -->
            <custom-attribute-list v-model="customAttributes" />
          </app-tab>
        </app-tabs>
      </template>
      <form v-else @submit.prevent="onCreate">
        <div class="mb-3">
          <icon-submit
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            icon="fas fa-plus"
            :loading="isSubmitting"
            text="actions.create"
            variant="success"
          />
          <icon-button class="ms-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <RealmSelect :model-value="realmId" @realm-selected="onRealmSelected" />
        <unique-name-input required :settings="uniqueNameSettings" validate v-model="uniqueName" />
        <div class="row">
          <PasswordInput
            class="col-lg-6"
            label="users.password.new.label"
            placeholder="users.password.new.placeholder"
            :required="isPasswordRequired"
            :settings="passwordSettings"
            :validate="isPasswordRequired"
            v-model="password"
          />
          <PasswordInput
            class="col-lg-6"
            :confirm="{ value: password, label: 'users.password.new.label' }"
            id="confirm"
            label="users.password.confirm.label"
            placeholder="users.password.confirm.placeholder"
            :required="isPasswordRequired"
            :validate="isPasswordRequired"
            v-model="confirm"
          />
        </div>
      </form>
    </template>
  </main>
</template>
