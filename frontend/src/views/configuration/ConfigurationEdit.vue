<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import JwtSecretField from "@/components/settings/JwtSecretField.vue";
import LoggingSettingsEdit from "@/components/configuration/LoggingSettingsEdit.vue";
import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { Configuration, LoggingExtent, LoggingSettings } from "@/types/configuration";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { ToastUtils } from "@/types/components";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readConfiguration, replaceConfiguration } from "@/api/configuration";
import { useConfigurationStore } from "@/stores/configuration";

const configurationStore = useConfigurationStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const defaults = {
  loggingSettings: {
    extent: "ActivityOnly" as LoggingExtent,
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
    hashingStrategy: "PBKDF2",
  },
  requireUniqueEmail: false,
  secret: "",
};

const configuration = ref<Configuration>();
const defaultLocale = ref<string>();
const loggingSettings = ref<LoggingSettings>(defaults.loggingSettings);
const passwordSettings = ref<PasswordSettings>(defaults.passwordSettings);
const requireUniqueEmail = ref<boolean>(defaults.requireUniqueEmail);
const secret = ref<string>(defaults.secret);
const uniqueNameSettings = ref<UniqueNameSettings>(defaults.uniqueNameSettings);

const hasChanges = computed<boolean>(() => {
  const model = configuration.value ?? defaults;
  return (
    (defaultLocale.value ?? "") !== (configuration.value?.defaultLocale?.code ?? "") ||
    JSON.stringify(loggingSettings.value) !== JSON.stringify(model.loggingSettings) ||
    JSON.stringify(passwordSettings.value) !== JSON.stringify(model.passwordSettings) ||
    requireUniqueEmail.value !== model.requireUniqueEmail ||
    secret.value !== model.secret ||
    JSON.stringify(uniqueNameSettings.value) !== JSON.stringify(model.uniqueNameSettings)
  );
});

function setModel(model: Configuration): void {
  configuration.value = model;
  defaultLocale.value = model.defaultLocale?.code;
  loggingSettings.value = model.loggingSettings;
  passwordSettings.value = model.passwordSettings;
  requireUniqueEmail.value = model.requireUniqueEmail;
  secret.value = model.secret;
  uniqueNameSettings.value = model.uniqueNameSettings;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const updatedConfiguration = await replaceConfiguration(
      {
        defaultLocale: defaultLocale.value,
        secret: secret.value,
        uniqueNameSettings: uniqueNameSettings.value,
        passwordSettings: passwordSettings.value,
        requireUniqueEmail: requireUniqueEmail.value,
        loggingSettings: loggingSettings.value,
      },
      configuration.value?.version,
    );
    setModel(updatedConfiguration);
    toasts.success("configuration.updated");
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  if (configurationStore.toast) {
    toasts.success(configurationStore.toast);
    configurationStore.toast = undefined;
  }
  try {
    const configuration = configurationStore.configuration ?? (await readConfiguration());
    configurationStore.configuration = undefined;
    setModel(configuration);
  } catch (e: unknown) {
    handleError(e);
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
