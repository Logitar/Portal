<script setup lang="ts">
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
    id: "profile",
    label: "users.profile.label",
    placeholder: "users.profile.placeholder",
    required: false,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <url-input
    :disabled="disabled"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :placeholder="placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
