<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
    validate?: boolean;
    verified?: boolean;
  }>(),
  {
    disabled: false,
    id: "emailAddress",
    label: "users.email.address.label",
    placeholder: "users.email.address.placeholder",
    required: false,
    validate: false,
    verified: false,
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
    type="email"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="verified" #append>
      <span class="input-group-text bg-info text-white"><font-awesome-icon icon="fas fa-check" />&nbsp;{{ t("users.email.verified") }}</span>
    </template>
  </form-input>
</template>
