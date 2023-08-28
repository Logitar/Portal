<script setup lang="ts">
import { useI18n } from "vue-i18n";
import type { UniqueNameSettings } from "@/types/settings";
import { assign } from "@/helpers/objectUtils";

const { t } = useI18n();

const props = defineProps<{
  modelValue: UniqueNameSettings;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: UniqueNameSettings): void;
}>();
function onUpdate(changes: object): void {
  const settings: UniqueNameSettings = { ...props.modelValue };
  for (const [key, value] of Object.entries(changes)) {
    assign(settings, key as keyof UniqueNameSettings, value);
  }
  emit("update:model-value", settings);
}
</script>

<template>
  <div>
    <h5>{{ t("settings.uniqueName.title") }}</h5>
    <form-input
      id="allowedCharacters"
      label="settings.uniqueName.allowedCharacters.label"
      placeholder="settings.uniqueName.allowedCharacters.placeholder"
      :model-value="modelValue?.allowedCharacters"
      @update:model-value="onUpdate({ allowedCharacters: $event })"
    />
  </div>
</template>
