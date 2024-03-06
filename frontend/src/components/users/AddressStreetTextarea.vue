<script setup lang="ts">
withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
    rows?: number;
    validate?: boolean;
  }>(),
  {
    disabled: false,
    id: "street",
    label: "users.address.street.label",
    placeholder: "users.address.street.placeholder",
    required: false,
    validate: false,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <form-textarea
    :disabled="disabled"
    :id="id"
    :label="label"
    :max-length="validate ? 255 : undefined"
    :model-value="modelValue"
    :placeholder="placeholder"
    :required="required"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  >
  </form-textarea>
</template>
