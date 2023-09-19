<script setup lang="ts">
import { useI18n } from "vue-i18n";

import VariableEdit from "./VariableEdit.vue";
import type { Variable } from "@/types/messages";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue?: Variable[];
  }>(),
  {
    id: "variables",
    modelValue: () => [],
  }
);

const emit = defineEmits<{
  (e: "update:model-value", value: Variable[]): void;
}>();

function add(): void {
  const value = [...props.modelValue];
  value.push({ key: "", value: "" });
  emit("update:model-value", value);
}
function remove(index: number): void {
  const value = [...props.modelValue];
  value.splice(index, 1);
  emit("update:model-value", value);
}
function update(index: number, attribute: Variable): void {
  const value = [...props.modelValue];
  value.splice(index, 1, attribute);
  emit("update:model-value", value);
}
</script>

<template>
  <div :id="id">
    <div class="mb-3">
      <icon-button icon="plus" text="actions.add" variant="success" @click="add" />
    </div>
    <template v-if="modelValue.length">
      <div class="row">
        <h5 class="col">{{ t("messages.contents.variables.key.label") }}</h5>
        <h5 class="col">{{ t("messages.contents.variables.value.label") }}</h5>
      </div>
      <VariableEdit
        v-for="(variable, index) in modelValue"
        :key="index"
        class="row"
        :id="[id, index].join('_')"
        :variable="variable"
        @remove="remove(index)"
        @update="update(index, $event)"
      />
    </template>
    <p v-else>{{ t("messages.contents.variables.empty") }}</p>
  </div>
</template>
