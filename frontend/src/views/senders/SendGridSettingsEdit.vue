<script setup lang="ts">
// import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { SendGridSettings } from "@/types/senders";

const { t } = useI18n();
// type SettingKeys = "ApiKey";

withDefaults(
  defineProps<{
    id?: string;
    modelValue?: SendGridSettings[];
  }>(),
  {
    id: "sendgrid-settings",
    modelValue: () => [],
  },
);

// const showApiKey = ref<boolean>(false);

// function findSetting(key: SettingKeys): SendGridSettings | undefined {
//   return props.modelValue.find((setting) => setting.key === key);
// }
// const apiKey = computed<string | undefined>(() => findSetting("ApiKey")?.value);

// const emit = defineEmits<{
//   (e: "update:model-value", value: SendGridSettings[]): void;
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
</script>

<template>
  <div :id="id">
    <h3>{{ t("senders.providers.sendGrid.settings") }}</h3>
    <!-- <form-input
      id="apiKey"
      label="senders.providers.sendGrid.apiKey.label"
      :model-value="apiKey"
      placeholder="senders.providers.sendGrid.apiKey.placeholder"
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
