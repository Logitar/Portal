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
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    disabled: false,
    id: "provider",
    label: "senders.providers.label",
    placeholder: "senders.providers.placeholder",
    required: false,
  }
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("senders.providers.options"))).map(([value, text]) => ({ text, value } as SelectOption)),
    "text"
  )
);

defineEmits<{
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
  />
</template>
