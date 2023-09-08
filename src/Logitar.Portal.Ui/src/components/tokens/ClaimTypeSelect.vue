<script setup lang="ts">
import { computed } from "vue";
import claimTypes from "@/resources/claimTypes.json";
import type { SelectOption } from "@/types/components";
import { orderBy } from "@/helpers/arrayUtils";

withDefaults(
  defineProps<{
    disabled?: boolean;
    id: string;
    label?: string;
    modelValue?: string;
    noLabel?: boolean;
    noState?: boolean;
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    disabled: false,
    id: "type",
    label: "tokens.claims.type.label",
    noLabel: false,
    noState: false,
    placeholder: "tokens.claims.type.placeholder",
    required: false,
  }
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    claimTypes.map(({ id, name }) => ({ value: id, text: name })),
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
    :no-label="noLabel"
    :no-state="noState"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
