<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import type { SelectOption } from "@/types/components";
import { orderBy } from "@/helpers/arrayUtils";

const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    onlyErrors: boolean;
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    disabled: false,
    id: "extent",
    label: "configuration.logging.extent.label",
    onlyErrors: false,
    placeholder: "configuration.logging.extent.placeholder",
    required: false,
  }
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("configuration.logging.extent.options"))).map(([value, text]) => ({ text, value } as SelectOption)),
    "text"
  )
);

const emit = defineEmits<{
  (e: "only-errors", value: boolean): void;
  (e: "update:model-value", value: string): void;
}>();
function onModelValueUpdate(value: string) {
  emit("update:model-value", value);
}
</script>

<template>
  <form-select
    :disabled="disabled"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #after>
      <form-checkbox
        :disabled="modelValue === 'None'"
        id="onlyErrors"
        label="configuration.logging.extent.onlyErrors"
        :model-value="onlyErrors"
        @update:model-value="$emit('only-errors', $event)"
      />
    </template>
  </form-select>
</template>
