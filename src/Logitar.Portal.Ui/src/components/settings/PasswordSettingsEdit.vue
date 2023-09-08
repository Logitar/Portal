<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { PasswordSettings } from "@/types/settings";
import { assign } from "@/helpers/objectUtils";

const { t } = useI18n();

const props = defineProps<{
  modelValue: PasswordSettings;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: PasswordSettings): void;
}>();
function onUpdate(changes: object): void {
  const settings: PasswordSettings = { ...props.modelValue };
  for (const [key, value] of Object.entries(changes)) {
    assign(settings, key as keyof PasswordSettings, value);
  }
  emit("update:model-value", settings);
}
</script>

<template>
  <div>
    <h5>{{ t("settings.password.title") }}</h5>
    <div class="row">
      <form-input
        class="col-lg-6"
        id="requiredLength"
        label="settings.password.requiredLength.label"
        :min-value="1"
        :model-value="modelValue.requiredLength.toString()"
        placeholder="settings.password.requiredLength.placeholder"
        required
        type="number"
        @update:model-value="onUpdate({ requiredLength: Number($event) })"
      />
      <form-input
        class="col-lg-6"
        id="requiredUniqueChars"
        label="settings.password.requiredUniqueChars.label"
        :min-value="1"
        :max-value="modelValue.requiredLength"
        :model-value="modelValue.requiredUniqueChars.toString()"
        placeholder="settings.password.requiredUniqueChars.placeholder"
        required
        type="number"
        @update:model-value="onUpdate({ requiredUniqueChars: Number($event) })"
      />
    </div>
    <div class="mb-3">
      <form-checkbox
        id="requireLowercase"
        label="settings.password.requireLowercase"
        :model-value="modelValue.requireLowercase"
        @update:model-value="onUpdate({ requireLowercase: $event })"
      />
      <form-checkbox
        id="requireUppercase"
        label="settings.password.requireUppercase"
        :model-value="modelValue.requireUppercase"
        @update:model-value="onUpdate({ requireUppercase: $event })"
      />
      <form-checkbox
        id="requireDigit"
        label="settings.password.requireDigit"
        :model-value="modelValue.requireDigit"
        @update:model-value="onUpdate({ requireDigit: $event })"
      />
      <form-checkbox
        id="requireNonAlphanumeric"
        label="settings.password.requireNonAlphanumeric"
        :model-value="modelValue.requireNonAlphanumeric"
        @update:model-value="onUpdate({ requireNonAlphanumeric: $event })"
      />
    </div>
  </div>
</template>
