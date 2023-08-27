<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import JwtSecretField from "@/components/settings/JwtSecretField.vue";
import LoggingSettingsEdit from "@/components/configuration/LoggingSettingsEdit.vue";
import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { Configuration, LoggingSettings } from "@/types/configuration";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { ToastUtils } from "@/types/components";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readConfiguration, updateConfiguration } from "@/api/configuration";

const { t } = useI18n();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = inject(toastsKey) as ToastUtils;

const defaults = {
  defaultLocale: "",
  loggingSettings: {
    extent: "ActivityOnly",
    onlyErrors: false,
  },
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
};

const configuration = ref<Configuration>();
const defaultLocale = ref<string>(defaults.defaultLocale);
const loggingSettings = ref<LoggingSettings>(defaults.loggingSettings);
const passwordSettings = ref<PasswordSettings>(defaults.passwordSettings);
const secret = ref<string>(defaults.secret);
const uniqueNameSettings = ref<UniqueNameSettings>(defaults.uniqueNameSettings);

const hasChanges = computed<boolean>(() => {
  const model = configuration.value ?? defaults;
  return (
    defaultLocale.value !== (model.defaultLocale ?? "") ||
    JSON.stringify(loggingSettings.value) !== JSON.stringify(model.loggingSettings) ||
    JSON.stringify(uniqueNameSettings.value) !== JSON.stringify(model.uniqueNameSettings) ||
    JSON.stringify(passwordSettings.value) !== JSON.stringify(model.passwordSettings) ||
    secret.value !== model.secret
  );
});

function setModel(model: Configuration): void {
  configuration.value = model;
  defaultLocale.value = model.defaultLocale;
  loggingSettings.value = model.loggingSettings;
  uniqueNameSettings.value = model.uniqueNameSettings;
  passwordSettings.value = model.passwordSettings;
  secret.value = model.secret;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const updatedConfiguration = await updateConfiguration({
      defaultLocale: defaultLocale.value !== configuration.value?.defaultLocale ? defaultLocale.value : undefined,
      secret: secret.value !== configuration.value?.secret ? secret.value : undefined,
      uniqueNameSettings:
        JSON.stringify(uniqueNameSettings.value) !== JSON.stringify(configuration.value?.uniqueNameSettings) ? uniqueNameSettings.value : undefined,
      passwordSettings: JSON.stringify(passwordSettings.value) !== JSON.stringify(configuration.value?.passwordSettings) ? passwordSettings.value : undefined,
      loggingSettings: JSON.stringify(loggingSettings.value) !== JSON.stringify(configuration.value?.loggingSettings) ? loggingSettings.value : undefined,
    });
    setModel(updatedConfiguration);
    toasts.success("configuration.updated");
  } catch (e: unknown) {
    handleError(e);
  }
});

const route = useRoute();
onMounted(async () => {
  try {
    const configuration = await readConfiguration();
    setModel(configuration);
  } catch (e) {
    handleError(e);
  }
  if (route.query.status === "initialized") {
    toasts.success("configuration.initialized");
  }
});
</script>

<template>
  <main class="container">
    <h1>{{ t("configuration.title") }}</h1>
    <status-detail v-if="configuration" :aggregate="configuration" />
    <form v-if="configuration" @submit.prevent="onSubmit">
      <div class="mb-3">
        <icon-submit class="me-1" :disabled="!hasChanges || isSubmitting" icon="fas fa-save" :loading="isSubmitting" text="actions.save" />
      </div>
      <locale-select label="configuration.defaultLocale" required v-model="defaultLocale" />
      <LoggingSettingsEdit v-model="loggingSettings" />
      <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
      <PasswordSettingsEdit v-model="passwordSettings" />
      <h5>{{ t("settings.jwt.title") }}</h5>
      <JwtSecretField :old-value="configuration.secret" validate warning="configuration.jwt.secret.warning" v-model="secret" />
    </form>
  </main>
</template>
