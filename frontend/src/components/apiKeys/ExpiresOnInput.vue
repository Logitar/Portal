<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    maxValue?: Date;
    modelValue?: Date;
    required?: boolean;
    validate?: boolean;
  }>(),
  {
    disabled: false,
    id: "expiresOn",
    label: "apiKeys.expiresOn",
    required: false,
    validate: false,
  },
);

const min = new Date();
const max = new Date(min);
max.setFullYear(max.getFullYear() + 100);

const isExpired = computed<boolean>(() => (props.modelValue ? new Date(props.modelValue) <= new Date() : false));

defineEmits<{
  (e: "update:model-value", value?: Date): void;
}>();
</script>

<template>
  <date-time-input
    :disabled="disabled"
    :id="id"
    :label="label"
    :max="validate ? maxValue ?? max : undefined"
    :min="validate ? min : undefined"
    :model-value="modelValue"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="isExpired" #append>
      <span class="input-group-text bg-info text-white">{{ t("apiKeys.expired") }}</span>
    </template>
  </date-time-input>
</template>
