<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { MailgunSettings } from "@/types/senders";
type SettingsKey = "ApiKey" | "DomainName";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: MailgunSettings;
  }>(),
  {
    id: "mailgun-settings",
  },
);

const showApiKey = ref<boolean>(false);

const emit = defineEmits<{
  (e: "update:model-value", value: MailgunSettings): void;
}>();

function updateSetting(key: SettingsKey, value: string): void {
  const settings = { ...props.modelValue };
  switch (key) {
    case "ApiKey":
      settings.apiKey = value;
      break;
    case "DomainName":
      settings.domainName = value;
      break;
  }
  emit("update:model-value", settings);
}
</script>

<template>
  <div :id="id">
    <h3>{{ t("senders.providers.mailgun.settings") }}</h3>
    <div class="row">
      <form-input
        class="col-lg-6"
        id="api-key"
        label="senders.providers.mailgun.apiKey.label"
        :model-value="modelValue.apiKey"
        placeholder="senders.providers.mailgun.apiKey.placeholder"
        required
        :type="showApiKey ? 'text' : 'password'"
        @update:model-value="updateSetting('ApiKey', $event)"
      >
        <template #append>
          <icon-button :icon="showApiKey ? 'eye-slash' : 'eye'" variant="info" @click="showApiKey = !showApiKey" />
        </template>
      </form-input>
      <form-input
        class="col-lg-6"
        id="domain-name"
        label="senders.providers.mailgun.domainName.label"
        :model-value="modelValue.domainName"
        placeholder="senders.providers.mailgun.domainName.placeholder"
        required
        @update:model-value="updateSetting('DomainName', $event)"
      />
    </div>
  </div>
</template>
