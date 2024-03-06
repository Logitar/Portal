<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { ApiError, ErrorDetail } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { ToastUtils } from "@/types/components";
import type { UniqueNameSettings } from "@/types/settings";
import { createRole, readRole, updateRole } from "@/api/roles";
import { getCustomAttributeModifications } from "@/helpers/customAttributeUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRealm } from "@/api/realms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const defaults = {
  uniqueName: "",
  displayName: "",
  description: "",
  customAttributes: [],
};

const configuration = ref<Configuration>();
const customAttributes = ref<CustomAttribute[]>(defaults.customAttributes);
const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const hasLoaded = ref<boolean>(false);
const realm = ref<Realm>();
const realmId = ref<string>();
const role = ref<Role>();
const uniqueName = ref<string>(defaults.uniqueName);
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() => {
  const model = role.value ?? defaults;
  return (
    realm.value?.id !== role.value?.realm?.id ||
    displayName.value !== (model.displayName ?? "") ||
    uniqueName.value !== model.uniqueName ||
    description.value !== (model.description ?? "") ||
    JSON.stringify(customAttributes.value) !== JSON.stringify(model.customAttributes)
  );
});
const title = computed<string>(() => role.value?.displayName ?? role.value?.uniqueName ?? t("roles.title.new"));
const uniqueNameSettings = computed<UniqueNameSettings>(
  () =>
    realm.value?.uniqueNameSettings ??
    configuration.value?.uniqueNameSettings ?? { allowedCharacters: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+" },
);

function setModel(model: Role): void {
  role.value = model;
  customAttributes.value = model.customAttributes.map((item) => ({ ...item }));
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  realm.value = model.realm;
  realmId.value = model.realm?.id;
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    if (role.value) {
      const customAttributeModifications = getCustomAttributeModifications(role.value.customAttributes, customAttributes.value);
      const updatedRole = await updateRole(role.value.id, {
        uniqueName: uniqueName.value !== role.value.uniqueName ? uniqueName.value : undefined,
        displayName: displayName.value !== (role.value.displayName ?? "") ? { value: displayName.value } : undefined,
        description: description.value !== (role.value.description ?? "") ? { value: description.value } : undefined,
        customAttributes: customAttributeModifications.length ? customAttributeModifications : undefined,
      });
      setModel(updatedRole);
      toasts.success("roles.updated");
    } else {
      const createdRole = await createRole({
        realm: realmId.value,
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
        customAttributes: customAttributes.value,
      });
      setModel(createdRole);
      toasts.success("roles.created");
      router.replace({ name: "RoleEdit", params: { id: createdRole.id } });
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

function onRealmSelected(selected?: Realm) {
  realm.value = selected;
  realmId.value = selected?.id;
}

onMounted(async () => {
  try {
    configuration.value = await readConfiguration();
    const id = route.params.id?.toString();
    const realmIdQuery = route.query.realm?.toString();
    if (id) {
      const role = await readRole(id);
      setModel(role);
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
      <status-detail v-if="role" :aggregate="role" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <icon-submit
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="role ? 'save' : 'plus'"
            :loading="isSubmitting"
            :text="role ? 'actions.save' : 'actions.create'"
            :variant="role ? undefined : 'success'"
          />
          <icon-button class="ms-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <app-tabs>
          <app-tab active title="general">
            <RealmSelect :disabled="Boolean(role)" :model-value="realmId" @realm-selected="onRealmSelected" />
            <div class="row">
              <unique-name-input class="col-lg-6" required :settings="uniqueNameSettings" validate v-model="uniqueName" />
              <display-name-input class="col-lg-6" validate v-model="displayName" />
            </div>
            <description-textarea v-model="description" />
          </app-tab>
          <app-tab title="customAttributes.title">
            <custom-attribute-list v-model="customAttributes" />
          </app-tab>
        </app-tabs>
      </form>
    </template>
  </main>
</template>
