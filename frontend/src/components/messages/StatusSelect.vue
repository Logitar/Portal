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
    isDemo?: boolean;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    disabled: false,
    id: "gender",
    isDemo: false,
    label: "messages.status.label",
    placeholder: "messages.status.placeholder",
    required: false,
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("messages.status.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "is-demo", value: boolean): void;
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <form-select
    :disabled="disabled"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :no-state="!required"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <form-checkbox id="demo" label="messages.demo.label" :model-value="isDemo" @update:model-value="$emit('is-demo', $event)" />
    </template>
  </form-select>
</template>
