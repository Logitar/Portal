<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import AuthenticationInformation from "@/components/users/AuthenticationInformation.vue";
import ContactInformation from "@/components/users/ContactInformation.vue";
import PersonalInformation from "@/components/users/PersonalInformation.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { ApiError, ErrorDetail } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/customAttributes";
import type { ProfileUpdatedEvent, User } from "@/types/users";
import type { Realm } from "@/types/realms";
import type { ToastUtils } from "@/types/components";
import type { UniqueNameSettings } from "@/types/settings";
import { createUser, readUser, updateUser } from "@/api/users";
import { getCustomAttributeModifications } from "@/helpers/customAttributeUtils";
import { handleErrorKey, registerTooltipsKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const registerTooltips = inject(registerTooltipsKey) as () => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { d, t } = useI18n();

const defaults = {
  uniqueName: "",
  customAttributes: [],
};

const configuration = ref<Configuration>();
const customAttributes = ref<CustomAttribute[]>(defaults.customAttributes);
const hasLoaded = ref<boolean>(false);
const realm = ref<Realm>();
const realmId = ref<string>();
const user = ref<User>();
const uniqueName = ref<string>(defaults.uniqueName);
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() => {
  const model = user.value ?? defaults;
  return uniqueName.value !== model.uniqueName || JSON.stringify(customAttributes.value) !== JSON.stringify(model.customAttributes);
});
const title = computed<string>(() => user.value?.fullName ?? user.value?.uniqueName ?? t("users.title.new"));
const uniqueNameSettings = computed<UniqueNameSettings>(() => realm.value?.uniqueNameSettings ?? configuration.value?.uniqueNameSettings ?? {});

function setModel(model: User): void {
  user.value = model;
  customAttributes.value = model.customAttributes.map((item) => ({ ...item }));
  realm.value = model.realm;
  realmId.value = model.realm?.id;
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    if (user.value) {
      const customAttributeModifications = getCustomAttributeModifications(user.value.customAttributes, customAttributes.value);
      const updatedUser = await updateUser(user.value.id, {
        uniqueName: uniqueName.value !== user.value.uniqueName ? uniqueName.value : undefined,
        customAttributes: customAttributeModifications.length ? customAttributeModifications : undefined,
      });
      setModel(updatedUser);
      toasts.success("users.updated");
    } else {
      const createdUser = await createUser({
        realm: realmId.value,
        uniqueName: uniqueName.value,
        isDisabled: false, // TODO(fpion): implement
        customAttributes: customAttributes.value,
        roles: [], // TODO(fpion): implement
      });
      setModel(createdUser);
      toasts.success("users.created");
      router.replace({ name: "UserEdit", params: { id: createdUser.id } });
    }
  } catch (e: unknown) {
    const { data, status } = e as ApiError;
    if (status === 409 && (data as ErrorDetail)?.errorCode === "UniqueNameAlreadyUsed") {
      uniqueNameAlreadyUsed.value = true;
    } else {
      handleError(e);
    }
  }
});

function onProfileUpdated(event: ProfileUpdatedEvent): void {
  user.value = event.user;

  if (account.authenticated?.id === event.user.id) {
    account.signIn(event.user);
  }

  if (event.toast ?? true) {
    toasts.success("users.profile.updated");
  }
}

function onRealmSelected(selected?: Realm) {
  realm.value = selected;
  realmId.value = selected?.id;
}

onMounted(async () => {
  try {
    configuration.value = await readConfiguration();
  } catch (e: unknown) {
    handleError(e);
  }
  if (route.query.realm?.toString()) {
    realmId.value = route.query.realm?.toString();
  }
  const id = route.params.id?.toString();
  if (id) {
    try {
      const user = await readUser(id);
      setModel(user);
    } catch (e: unknown) {
      const { status } = e as ApiError;
      if (status === 404) {
        router.push({ path: "/not-found" });
      } else {
        handleError(e);
      }
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
      <status-detail v-if="user" :aggregate="user" />
      <p v-if="user?.authenticatedOn">
        {{ t("users.authenticatedOnFormat", { date: d(user.authenticatedOn, "medium") }) }}
        <br />
        TODO(fpion): View sessions
      </p>
      <!-- TODO(fpion): disabled on & disable button ? -->
      <template v-if="user">
        <!-- TODO(fpion): <ProfileHeader :user="user" /> -->
        <RealmSelect disabled :model-value="realmId" @realm-selected="onRealmSelected" />
        <app-tabs>
          <app-tab active title="users.tabs.authentication">
            <AuthenticationInformation :user="user" @profile-updated="onProfileUpdated" />
            <!-- TODO(fpion): "Avoid changing an user's password without its consent." -->
          </app-tab>
          <app-tab title="users.tabs.personal">
            <PersonalInformation :user="user" @profile-updated="onProfileUpdated" />
          </app-tab>
          <app-tab title="users.tabs.contact">
            <ContactInformation :user="user" @profile-updated="onProfileUpdated" />
          </app-tab>
          <app-tab title="roles.title.list">
            <p>TODO(fpion): implement user roles</p>
          </app-tab>
          <app-tab title="users.identifiers.title">
            <p>TODO(fpion): implement user identifiers</p>
          </app-tab>
          <app-tab title="customAttributes.title">
            <custom-attribute-list v-model="customAttributes" />
            <!-- TODO(fpion): how to save -->
          </app-tab>
        </app-tabs>
      </template>
      <form v-else @submit.prevent="onSubmit">
        <div class="mb-3">
          <icon-submit
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="user ? 'save' : 'plus'"
            :loading="isSubmitting"
            :text="user ? 'actions.save' : 'actions.create'"
            :variant="user ? undefined : 'success'"
          />
          <icon-button class="ms-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <!-- TODO(fpion): implement -->
        <RealmSelect :model-value="realmId" @realm-selected="onRealmSelected" />
        <unique-name-input required :settings="uniqueNameSettings" validate v-model="uniqueName" />
      </form>
    </template>
  </main>
</template>
