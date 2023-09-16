<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean;
    id: string;
    label?: string;
    modelValue?: boolean;
    switch?: boolean;
  }>(),
  {
    disabled: false,
    switch: false,
  }
);

const classes = computed<string[]>(() => {
  const classes = ["form-check"];
  if (props.switch) {
    classes.push("form-switch");
  }
  return classes;
});

const emit = defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
function onChange($event: Event): void {
  if ($event.target) {
    emit("update:model-value", ($event.target as HTMLInputElement).checked);
  }
}
</script>

<template>
  <div :class="classes">
    <input type="checkbox" :checked="modelValue" class="form-check-input" :disabled="disabled" :id="id" @change="onChange" />
    <label class="form-check-label" :for="id">
      <template v-if="label">{{ t(label) }}</template>
      <slot name="label"></slot>
    </label>
  </div>
</template>
