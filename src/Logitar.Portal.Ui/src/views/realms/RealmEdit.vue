<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import ClaimMappingList from "@/components/realms/ClaimMappingList.vue";
import JwtSecretField from "@/components/settings/JwtSecretField.vue";
import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { ClaimMapping, Realm } from "@/types/realms";
import type { CustomAttribute } from "@/types/customAttributes";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { ToastUtils } from "@/types/components";
import { createRealm, readRealm, updateRealm } from "@/api/realms";
import { handleErrorKey, registerTooltipsKey, toastsKey } from "@/inject/App";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const registerTooltips = inject(registerTooltipsKey) as () => void;
const toasts = inject(toastsKey) as ToastUtils;

const defaults = {
  uniqueSlug: "",
  displayName: "",
  defaultLocale: "",
  url: "",
  description: "",
  requireConfirmedAccount: true,
  requireUniqueEmail: true,
  uniqueNameSettings: { allowedCharacters: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+" },
  passwordSettings: {
    requiredLength: 8,
    requiredUniqueChars: 8,
    requireNonAlphanumeric: true,
    requireLowercase: true,
    requireUppercase: true,
    requireDigit: true,
    strategy: "PBKDF2",
  },
  secret: "",
  claimMappings: [],
  customAttributes: [],
};

const claimMappings = ref<ClaimMapping[]>(defaults.claimMappings);
const customAttributes = ref<CustomAttribute[]>(defaults.customAttributes);
const defaultLocale = ref<string>(defaults.defaultLocale);
const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const hasLoaded = ref<boolean>(false);
const passwordSettings = ref<PasswordSettings>(defaults.passwordSettings);
const realm = ref<Realm>();
const requireConfirmedAccount = ref<boolean>(defaults.requireConfirmedAccount);
const requireUniqueEmail = ref<boolean>(defaults.requireUniqueEmail);
const secret = ref<string>(defaults.secret);
const uniqueSlug = ref<string>(defaults.uniqueSlug);
const url = ref<string>(defaults.url);
const uniqueNameSettings = ref<UniqueNameSettings>(defaults.uniqueNameSettings);

const hasChanges = computed<boolean>(() => {
  const model = realm.value ?? defaults;
  return (
    displayName.value !== (model.displayName ?? "") ||
    uniqueSlug.value !== model.uniqueSlug ||
    defaultLocale.value !== (model.defaultLocale ?? "") ||
    url.value !== (model.url ?? "") ||
    description.value !== (model.description ?? "") ||
    requireConfirmedAccount.value !== model.requireConfirmedAccount ||
    requireUniqueEmail.value !== model.requireUniqueEmail ||
    JSON.stringify(uniqueNameSettings.value) !== JSON.stringify(model.uniqueNameSettings) ||
    JSON.stringify(passwordSettings.value) !== JSON.stringify(model.passwordSettings) ||
    secret.value !== model.secret ||
    JSON.stringify(claimMappings.value) !== JSON.stringify(model.claimMappings) ||
    JSON.stringify(customAttributes.value) !== JSON.stringify(model.customAttributes)
  );
});
const title = computed<string>(() => realm.value?.displayName ?? realm.value?.uniqueSlug ?? t("realms.title.new"));

function setModel(model: Realm): void {
  realm.value = model;
  claimMappings.value = model.claimMappings.map((item) => ({ ...item }));
  customAttributes.value = model.customAttributes.map((item) => ({ ...item }));
  defaultLocale.value = model.defaultLocale ?? "";
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  passwordSettings.value = model.passwordSettings;
  requireConfirmedAccount.value = model.requireConfirmedAccount;
  requireUniqueEmail.value = model.requireUniqueEmail;
  secret.value = model.secret;
  uniqueSlug.value = model.uniqueSlug;
  url.value = model.url ?? "";
  uniqueNameSettings.value = model.uniqueNameSettings;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (realm.value) {
      const updatedRealm = await updateRealm(realm.value.id, {
        uniqueSlug: uniqueSlug.value !== realm.value.uniqueSlug ? uniqueSlug.value : undefined,
        displayName: displayName.value !== (realm.value.displayName ?? "") ? { value: displayName.value } : undefined,
        description: description.value !== (realm.value.description ?? "") ? { value: description.value } : undefined,
        defaultLocale: defaultLocale.value !== (realm.value.defaultLocale ?? "") ? { value: defaultLocale.value } : undefined,
        secret: secret.value !== realm.value.secret ? secret.value : undefined,
        url: url.value !== (realm.value.url ?? "") ? { value: url.value } : undefined,
        requireUniqueEmail: requireUniqueEmail.value !== realm.value.requireUniqueEmail ? requireUniqueEmail.value : undefined,
        requireConfirmedAccount: requireConfirmedAccount.value !== realm.value.requireConfirmedAccount ? requireConfirmedAccount.value : undefined,
        uniqueNameSettings: JSON.stringify(uniqueNameSettings.value) !== JSON.stringify(realm.value.uniqueNameSettings) ? uniqueNameSettings.value : undefined,
        passwordSettings: JSON.stringify(passwordSettings.value) !== JSON.stringify(realm.value.passwordSettings) ? passwordSettings.value : undefined,
        // claimMappings: claimMappings.value, // TODO(fpion): implement
        // customAttributes: customAttributes.value, // TODO(fpion): implement
      });
      setModel(updatedRealm);
      toasts.success("realms.updated");
    } else {
      const createdRealm = await createRealm({
        uniqueSlug: uniqueSlug.value,
        displayName: displayName.value,
        description: description.value,
        defaultLocale: defaultLocale.value,
        secret: secret.value,
        url: url.value,
        requireConfirmedAccount: requireConfirmedAccount.value,
        requireUniqueEmail: requireConfirmedAccount.value,
        uniqueNameSettings: uniqueNameSettings.value,
        passwordSettings: passwordSettings.value,
        claimMappings: claimMappings.value,
        customAttributes: customAttributes.value,
      });
      setModel(createdRealm);
      toasts.success("realms.created");
      router.replace({ name: "RealmEdit", params: { uniqueSlug: createdRealm.uniqueSlug } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  const uniqueSlug = route.params.uniqueSlug?.toString();
  if (uniqueSlug) {
    try {
      const realm = await readRealm(uniqueSlug);
      setModel(realm);
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
    <status-detail v-if="realm" :aggregate="realm" />
    <form v-show="hasLoaded" @submit.prevent="onSubmit">
      <div class="mb-3">
        <icon-submit
          class="me-1"
          :disabled="isSubmitting || !hasChanges"
          :icon="realm ? 'save' : 'plus'"
          :loading="isSubmitting"
          :text="realm ? 'actions.save' : 'actions.create'"
          :variant="realm ? undefined : 'success'"
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
              :disabled="Boolean(realm)"
              id="uniqueSlug"
              label="realms.uniqueSlug.label"
              placeholder="realms.uniqueSlug.placeholder"
              :required="!realm"
              :validate="!realm"
              v-model="uniqueSlug"
            />
          </div>
          <div class="row">
            <locale-select class="col-lg-6" id="defaultLocale" label="realms.defaultLocale" v-model="defaultLocale" />
            <url-input class="col-lg-6" id="url" label="realms.url.label" placeholder="realms.url.placeholder" validate v-model="url" />
          </div>
          <description-textarea v-model="description" />
        </app-tab>
        <app-tab title="settings.title">
          <form-checkbox class="mb-3" id="requireConfirmedAccount" v-model="requireConfirmedAccount">
            <template #label>
              <span data-bs-toggle="tooltip" :data-bs-title="t('realms.requireConfirmedAccount.info')">
                {{ t("realms.requireConfirmedAccount.label") }} <font-awesome-icon icon="fas fa-circle-info" />
              </span>
            </template>
          </form-checkbox>
          <form-checkbox class="mb-3" id="requireUniqueEmail" v-model="requireUniqueEmail">
            <template #label>
              <span data-bs-toggle="tooltip" :data-bs-title="t('realms.requireUniqueEmail.info')">
                {{ t("realms.requireUniqueEmail.label") }} <font-awesome-icon icon="fas fa-circle-info" />
              </span>
            </template>
          </form-checkbox>
          <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
          <PasswordSettingsEdit v-model="passwordSettings" />
          <h5>{{ t("settings.jwt.title") }}</h5>
          <JwtSecretField :old-value="realm?.secret" validate v-model="secret" warning="realms.jwt.secret.warning" />
        </app-tab>
        <app-tab title="realms.claimMappings.title">
          <ClaimMappingList v-model="claimMappings" />
        </app-tab>
        <app-tab title="customAttributes.title">
          <custom-attribute-list v-model="customAttributes" />
        </app-tab>
      </app-tabs>
    </form>
  </main>
</template>
