<script setup lang="ts">
import { useI18n } from "vue-i18n";
import LoggingExtentSelect from "./LoggingExtentSelect.vue";
import type { LoggingSettings } from "@/types/configuration";
import { assign } from "@/helpers/objectUtils";

const { t } = useI18n();

const props = defineProps<{
  modelValue?: LoggingSettings;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: LoggingSettings): void;
}>();
function onUpdate(changes: object): void {
  const settings: LoggingSettings = { ...props.modelValue };
  for (const [key, value] of Object.entries(changes)) {
    assign(settings, key as keyof LoggingSettings, value);
  }
  if (settings.extent === "None") {
    settings.onlyErrors = false;
  }
  emit("update:model-value", settings);
}
</script>

<template>
  <div>
    <h5>{{ t("configuration.logging.title") }}</h5>
    <LoggingExtentSelect
      :model-value="modelValue?.extent"
      :only-errors="modelValue?.onlyErrors"
      required
      @only-errors="onUpdate({ onlyErrors: $event })"
      @update:model-value="onUpdate({ extent: $event })"
    />
  </div>
</template>
