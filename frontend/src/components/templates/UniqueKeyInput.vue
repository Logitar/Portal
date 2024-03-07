<script setup lang="ts">
withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
    validate?: boolean;
  }>(),
  {
    disabled: false,
    id: "uniqueKey",
    label: "templates.uniqueKey.label",
    placeholder: "templates.uniqueKey.placeholder",
    required: false,
    validate: false,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <form-input
    :disabled="disabled"
    :id="id"
    :label="label"
    :max-length="validate ? 255 : null"
    :model-value="modelValue"
    :placeholder="placeholder"
    :required="required"
    :rules="validate ? { identifier: true } : undefined"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
