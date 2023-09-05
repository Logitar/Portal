<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Role } from "@/types/roles";
import type { ToastUtils } from "@/types/components";
import { createRole, readRole, updateRole } from "@/api/roles";
import { getCustomAttributeModifications } from "@/helpers/customAttributeUtils";
import { handleErrorKey, registerTooltipsKey, toastsKey } from "@/inject/App";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const registerTooltips = inject(registerTooltipsKey) as () => void;
const toasts = inject(toastsKey) as ToastUtils;

const defaults = {
  uniqueName: "",
  displayName: "",
  description: "",
  customAttributes: [],
};

const customAttributes = ref<CustomAttribute[]>(defaults.customAttributes);
const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const hasLoaded = ref<boolean>(false);
const role = ref<Role>();
const uniqueName = ref<string>(defaults.uniqueName);

const hasChanges = computed<boolean>(() => {
  const model = role.value ?? defaults;
  return (
    displayName.value !== (model.displayName ?? "") ||
    uniqueName.value !== model.uniqueName ||
    description.value !== (model.description ?? "") ||
    JSON.stringify(customAttributes.value) !== JSON.stringify(model.customAttributes)
  );
});
const title = computed<string>(() => role.value?.displayName ?? role.value?.uniqueName ?? t("roles.title.new"));

function setModel(model: Role): void {
  role.value = model;
  customAttributes.value = model.customAttributes.map((item) => ({ ...item }));
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
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
    handleError(e);
  }
});

onMounted(async () => {
  const id = route.params.id?.toString();
  if (id) {
    try {
      const role = await readRole(id);
      setModel(role);
    } catch (e: unknown) {
      handleError(e);
    }
  }
  registerTooltips();
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <h1 v-show="hasLoaded">{{ title }}</h1>
    <status-detail v-if="role" :aggregate="role" />
    <form v-show="hasLoaded" @submit.prevent="onSubmit">
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
            <display-name-input class="col-lg-6" validate v-model="displayName" />
            <slug-input
              :base-value="displayName"
              class="col-lg-6"
              :disabled="Boolean(role)"
              id="uniqueName"
              label="roles.uniqueName.label"
              placeholder="roles.uniqueName.placeholder"
              :required="!role"
              :validate="!role"
              v-model="uniqueName"
            />
          </div>
          <description-textarea v-model="description" />
        </app-tab>
        <app-tab title="customAttributes.title">
          <custom-attribute-list v-model="customAttributes" />
        </app-tab>
      </app-tabs>
    </form>
  </main>
</template>
