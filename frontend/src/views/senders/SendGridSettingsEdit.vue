<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { SendGridSettings } from "@/types/senders";
type SettingsKey = "ApiKey";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: SendGridSettings;
  }>(),
  {
    id: "sendgrid-settings",
  },
);

const showApiKey = ref<boolean>(false);

const emit = defineEmits<{
  (e: "update:model-value", value: SendGridSettings): void;
}>();

function updateSetting(key: SettingsKey, value: string): void {
  const settings = { ...props.modelValue };
  switch (key) {
    case "ApiKey":
      settings.apiKey = value;
      break;
  }
  emit("update:model-value", settings);
}
</script>

<template>
  <div :id="id">
    <h3>{{ t("senders.providers.sendGrid.settings") }}</h3>
    <form-input
      id="api-key"
      label="senders.providers.sendGrid.apiKey.label"
      :model-value="modelValue.apiKey"
      placeholder="senders.providers.sendGrid.apiKey.placeholder"
      required
      :type="showApiKey ? 'text' : 'password'"
      @update:model-value="updateSetting('ApiKey', $event)"
    >
      <template #append>
        <icon-button :icon="showApiKey ? 'eye-slash' : 'eye'" variant="info" @click="showApiKey = !showApiKey" />
      </template>
    </form-input>
  </div>
</template>
