<script setup lang="ts">
// import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { MailgunSettings } from "@/types/senders";

const { t } = useI18n();
// type SettingKeys = "ApiKey";

withDefaults(
  defineProps<{
    id?: string;
    modelValue?: MailgunSettings[];
  }>(),
  {
    id: "mailgun-settings",
    modelValue: () => [],
  },
);

// const showApiKey = ref<boolean>(false);

// function findSetting(key: SettingKeys): MailgunSettings | undefined {
//   return props.modelValue.find((setting) => setting.key === key);
// }
// const apiKey = computed<string | undefined>(() => findSetting("ApiKey")?.value);

// const emit = defineEmits<{
//   (e: "update:model-value", value: MailgunSettings[]): void;
// }>();

// function setSetting(key: SettingKeys, value: string): void {
//   const settings = [...props.modelValue];
//   const index = settings.findIndex((setting) => setting.key === key);
//   if (index >= 0) {
//     settings.splice(index, 1, { key, value });
//   } else {
//     settings.push({ key, value });
//   }
//   emit("update:model-value", settings);
// }

// TODO(fpion): DomainName
// TODO(fpion): implement
</script>

<template>
  <div :id="id">
    <h3>{{ t("senders.providers.mailgun.settings") }}</h3>
    <!-- <form-input
      id="apiKey"
      label="senders.providers.mailgun.apiKey.label"
      :model-value="apiKey"
      placeholder="senders.providers.mailgun.apiKey.placeholder"
      required
      :type="showApiKey ? 'text' : 'password'"
      @update:model-value="setSetting('ApiKey', $event)"
    >
      <template #append>
        <icon-button :icon="showApiKey ? 'eye-slash' : 'eye'" variant="info" @click="showApiKey = !showApiKey" />
      </template>
    </form-input> -->
  </div>
</template>
