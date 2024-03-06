<script setup lang="ts">
withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    name?: string;
    noLabel?: boolean;
    placeholder?: string;
    rows?: number;
    required?: boolean;
  }>(),
  {
    disabled: false,
    id: "description",
    label: "description.label",
    noLabel: false,
    placeholder: "description.placeholder",
    required: false,
    rows: 15,
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
    :model-value="modelValue"
    :name="name"
    :no-label="noLabel"
    :placeholder="placeholder"
    :required="required"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
