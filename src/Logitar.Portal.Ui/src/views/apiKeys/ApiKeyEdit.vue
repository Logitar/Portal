<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import ExpiresOnInput from "@/components/apiKeys/ExpiresOnInput.vue";
import FormInput from "@/components/shared/FormInput.vue";
import ManageRoles from "@/components/roles/ManageRoles.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { ApiError } from "@/types/api";
import type { ApiKey } from "@/types/apiKeys";
import type { CollectionAction } from "@/types/modifications";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { ToastUtils } from "@/types/components";
import { createApiKey, readApiKey, updateApiKey } from "@/api/apiKeys";
import { getCustomAttributeModifications } from "@/helpers/customAttributeUtils";
import { handleErrorKey, registerTooltipsKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRealm } from "@/api/realms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const registerTooltips = inject(registerTooltipsKey) as () => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { d, t } = useI18n();

const defaults = {
  displayName: "",
  description: "",
  customAttributes: [],
};

const apiKey = ref<ApiKey>();
const clipboardRef = ref<InstanceType<typeof FormInput> | null>(null);
const configuration = ref<Configuration>();
const customAttributes = ref<CustomAttribute[]>(defaults.customAttributes);
const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const expiresOn = ref<Date>();
const hasLoaded = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const realm = ref<Realm>();
const realmId = ref<string>();
const showString = ref<boolean>(false);
const xApiKey = ref<string>();

const hasChanges = computed<boolean>(() => {
  const model = apiKey.value ?? defaults;
  return (
    displayName.value !== (model.displayName ?? "") ||
    description.value !== (model.description ?? "") ||
    expiresOn.value?.getTime() !== (apiKey.value?.expiresOn ? new Date(apiKey.value.expiresOn) : undefined)?.getTime() ||
    JSON.stringify(customAttributes.value) !== JSON.stringify(model.customAttributes)
  );
});
const isExpired = computed<boolean>(() => (apiKey.value?.expiresOn ? new Date(apiKey.value.expiresOn) < new Date() : false));
const maxExpiration = computed<Date | undefined>(() => (apiKey.value?.expiresOn ? new Date(apiKey.value.expiresOn) : undefined));
const title = computed<string>(() => apiKey.value?.displayName ?? t("apiKeys.title.new"));

function setModel(model: ApiKey): void {
  apiKey.value = model;
  customAttributes.value = model.customAttributes.map((item) => ({ ...item }));
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  expiresOn.value = model.expiresOn ? new Date(model.expiresOn) : undefined;
  realm.value = model.realm;
  realmId.value = model.realm?.id;
  xApiKey.value = model.xApiKey;
  showString.value = Boolean(model.xApiKey);
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (apiKey.value) {
      const customAttributeModifications = getCustomAttributeModifications(apiKey.value.customAttributes, customAttributes.value);
      const updatedApiKey = await updateApiKey(apiKey.value.id, {
        displayName: displayName.value !== (apiKey.value.displayName ?? "") ? displayName.value : undefined,
        description: description.value !== (apiKey.value.description ?? "") ? { value: description.value } : undefined,
        expiresOn:
          expiresOn.value?.getTime() !== (apiKey.value.expiresOn ? new Date(apiKey.value.expiresOn) : undefined)?.getTime() ? expiresOn.value : undefined,
        customAttributes: customAttributeModifications.length ? customAttributeModifications : undefined,
      });
      setModel(updatedApiKey);
      toasts.success("apiKeys.updated");
    } else {
      const createdApiKey = await createApiKey({
        realm: realmId.value,
        displayName: displayName.value,
        description: description.value,
        expiresOn: expiresOn.value,
        customAttributes: customAttributes.value,
      });
      setModel(createdApiKey);
      toasts.success("apiKeys.created");
      router.replace({ name: "ApiKeyEdit", params: { id: createdApiKey.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

function copyToClipboard(): void {
  if (clipboardRef.value && xApiKey.value) {
    clipboardRef.value.focus();
    navigator.clipboard.writeText(xApiKey.value);
  }
}

function onRealmSelected(selected?: Realm) {
  realm.value = selected;
  realmId.value = selected?.id;
}

async function onRoleToggled(role: Role, action: CollectionAction): Promise<void> {
  if (apiKey.value && !isLoading.value) {
    isLoading.value = true;
    try {
      const updatedApiKey = await updateApiKey(apiKey.value.id, {
        roles: [{ role: role.id, action }],
      });
      setModel(updatedApiKey);
      toasts.success("apiKeys.updated");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

onMounted(async () => {
  try {
    configuration.value = await readConfiguration();
    const id = route.params.id?.toString();
    const realmIdQuery = route.query.realm?.toString();
    if (id) {
      const apiKey = await readApiKey(id);
      setModel(apiKey);
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
      <app-alert dismissible variant="warning" v-model="showString">
        <strong>{{ t("apiKeys.string.heading") }}</strong>
        <br />
        <FormInput id="clipboard" :model-value="xApiKey" readonly ref="clipboardRef" @focus="$event.target.select()">
          <template #append>
            <icon-button icon="fas fa-clipboard" text="apiKeys.clipboard" variant="warning" @click="copyToClipboard()" />
          </template>
        </FormInput>
        {{ t("apiKeys.string.warning") }}
      </app-alert>
      <status-detail v-if="apiKey" :aggregate="apiKey" />
      <p v-if="apiKey?.authenticatedOn">
        {{ t("apiKeys.authenticatedOnFormat", { date: d(apiKey.authenticatedOn, "medium") }) }}
      </p>
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <icon-submit
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="apiKey ? 'save' : 'plus'"
            :loading="isSubmitting"
            :text="apiKey ? 'actions.save' : 'actions.create'"
            :variant="apiKey ? undefined : 'success'"
          />
          <icon-button class="ms-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <app-tabs>
          <app-tab active title="general">
            <RealmSelect :disabled="Boolean(apiKey)" :model-value="realmId" @realm-selected="onRealmSelected" />
            <div class="row">
              <display-name-input class="col-lg-6" required validate v-model="displayName" />
              <ExpiresOnInput class="col-lg-6" :disabled="isExpired" :max-value="maxExpiration" :validate="!isExpired" v-model="expiresOn" />
            </div>
            <description-textarea v-model="description" />
          </app-tab>
          <app-tab title="customAttributes.title">
            <custom-attribute-list v-model="customAttributes" />
          </app-tab>
          <app-tab v-if="apiKey" title="roles.title.list">
            <ManageRoles
              :loading="isLoading"
              :realm="realm"
              :roles="apiKey.roles"
              @role-added="onRoleToggled($event, 'Add')"
              @role-removed="onRoleToggled($event, 'Remove')"
            />
          </app-tab>
        </app-tabs>
      </form>
    </template>
  </main>
</template>
