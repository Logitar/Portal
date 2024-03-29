<script setup lang="ts">
import type { Result, Translations } from "maz-ui/components/MazPhoneNumberInput";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    countryCode?: string;
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    name?: string;
    placeholder?: string;
    required?: boolean;
    verified?: boolean;
  }>(),
  {
    disabled: false,
    id: "phoneNumber",
    label: "users.phone.number.label",
    placeholder: "users.phone.number.placeholder",
    required: false,
    verified: false,
  },
);

const isValid = ref<boolean>();

const classes = computed<string[]>(() => {
  const classes = ["input"];
  if (props.verified) {
    classes.push("verified");
  }
  return classes;
});

const inputName = computed<string>(() => props.name ?? props.id);
const inputPlaceholder = computed<string | undefined>(() => (props.modelValue ? undefined : t(props.placeholder)));

type MazState = "success" | "warning" | "error";
const state = computed<MazState | undefined>(() => {
  switch (isValid.value) {
    case true:
      return "success";
    case false:
      return "error";
    default:
      return undefined;
  }
});

const translations = computed<Translations>(() => ({
  countrySelector: {
    error: t("users.phone.countryCode.placeholder"),
    placeholder: t("users.phone.countryCode.label"),
    searchPlaceholder: "",
  },
  phoneInput: {
    example: t("users.phone.number.example"),
    placeholder: "",
  },
}));

function onUpdate(e: Partial<Result>): void {
  isValid.value = props.modelValue ? e.isValid : undefined;
}

defineExpose({ isValid });

defineEmits<{
  (e: "country-code", value: string): void;
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <div class="mb-3">
    <label v-if="label" :for="id" class="form-label">
      <template v-if="required"><span class="text-danger">*</span></template> {{ t(label) }}
    </label>
    <slot name="before"></slot>
    <div class="input-group">
      <slot name="prepend"></slot>
      <maz-phone-number-input
        :class="classes"
        :defaultCountryCode="countryCode || 'CA'"
        :disabled="disabled"
        :error="state === 'error'"
        :id="id"
        :model-value="modelValue"
        :name="inputName"
        :placeholder="inputPlaceholder"
        size="sm"
        :success="state === 'success'"
        :translations="translations"
        @country-code="$emit('country-code', $event)"
        @update="onUpdate"
        @update:model-value="$emit('update:model-value', $event)"
      />
      <span v-if="verified" class="input-group-text bg-info text-white"><font-awesome-icon icon="fas fa-check" />&nbsp;{{ t("users.phone.verified") }}</span>
      <slot name="append"></slot>
    </div>
    <slot name="after"></slot>
    <div v-if="isValid === false" class="invalid-feedback">{{ t("users.phone.invalid") }}</div>
  </div>
</template>

<style scoped>
.input {
  flex: 1 1 auto;
}

.verified :deep(.m-input-wrapper) {
  border-top-right-radius: 0 !important;
  border-bottom-right-radius: 0 !important;
}
</style>
