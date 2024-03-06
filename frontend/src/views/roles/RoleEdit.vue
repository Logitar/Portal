<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import type { ApiError, ErrorDetail } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Role } from "@/types/roles";
import type { ToastUtils } from "@/types/components";
import type { UniqueNameSettings } from "@/types/settings";
import { createRole, readRole, replaceRole } from "@/api/roles";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";

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
const role = ref<Role>();
const uniqueName = ref<string>(defaults.uniqueName);
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() => {
  const model = role.value ?? defaults;
  return (
    uniqueName.value !== model.uniqueName ||
    displayName.value !== (model.displayName ?? "") ||
    description.value !== (model.description ?? "") ||
    JSON.stringify(customAttributes.value) !== JSON.stringify(model.customAttributes)
  );
});
const title = computed<string>(() => role.value?.displayName ?? role.value?.uniqueName ?? t("roles.title.new"));
const uniqueNameSettings = computed<UniqueNameSettings>(
  () => configuration.value?.uniqueNameSettings ?? { allowedCharacters: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+" },
); // TODO(fpion): current realm

function setModel(model: Role): void {
  role.value = model;
  customAttributes.value = model.customAttributes.map((item) => ({ ...item }));
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    if (role.value) {
      const updatedRole = await replaceRole(
        role.value.id,
        {
          uniqueName: uniqueName.value,
          displayName: displayName.value,
          description: description.value,
          customAttributes: customAttributes.value,
        },
        role.value.version,
      );
      setModel(updatedRole);
      toasts.success("roles.updated");
    } else {
      const createdRole = await createRole({
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

onMounted(async () => {
  try {
    configuration.value = await readConfiguration();
    const id = route.params.id?.toString();
    if (id) {
      const role = await readRole(id);
      setModel(role);
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
