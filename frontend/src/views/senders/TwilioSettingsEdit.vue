<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { TwilioSettings } from "@/types/senders";
type SettingsKey = "AccountSid" | "AuthenticationToken";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: TwilioSettings;
  }>(),
  {
    id: "twilio-settings",
  },
);

const showAuthenticationToken = ref<boolean>(false);

const emit = defineEmits<{
  (e: "update:model-value", value: TwilioSettings): void;
}>();

function updateSetting(key: SettingsKey, value: string): void {
  const settings = { ...props.modelValue };
  switch (key) {
    case "AccountSid":
      settings.accountSid = value;
      break;
    case "AuthenticationToken":
      settings.authenticationToken = value;
      break;
  }
  emit("update:model-value", settings);
}
</script>

<template>
  <div :id="id">
    <h3>{{ t("senders.providers.twilio.settings") }}</h3>
    <div class="row">
      <form-input
        class="col-lg-6"
        id="account-sid"
        label="senders.providers.twilio.accountSid.label"
        :model-value="modelValue.accountSid"
        placeholder="senders.providers.twilio.accountSid.placeholder"
        required
        @update:model-value="updateSetting('AccountSid', $event)"
      />
      <form-input
        class="col-lg-6"
        id="api-key"
        label="senders.providers.twilio.authenticationToken.label"
        :model-value="modelValue.authenticationToken"
        placeholder="senders.providers.twilio.authenticationToken.placeholder"
        required
        :type="showAuthenticationToken ? 'text' : 'password'"
        @update:model-value="updateSetting('AuthenticationToken', $event)"
      >
        <template #append>
          <icon-button :icon="showAuthenticationToken ? 'eye-slash' : 'eye'" variant="info" @click="showAuthenticationToken = !showAuthenticationToken" />
        </template>
      </form-input>
    </div>
  </div>
</template>
