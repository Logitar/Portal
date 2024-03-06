<script setup lang="ts">
import { useI18n } from "vue-i18n";

import DictionaryEntryEdit from "./DictionaryEntryEdit.vue";
import type { DictionaryEntry } from "@/types/dictionaries";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue?: DictionaryEntry[];
  }>(),
  {
    id: "dictionaryEntries",
    modelValue: () => [],
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: DictionaryEntry[]): void;
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
function update(index: number, attribute: DictionaryEntry): void {
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
        <h5 class="col">{{ t("dictionaries.entries.key.label") }}</h5>
        <h5 class="col">{{ t("dictionaries.entries.value.label") }}</h5>
      </div>
      <DictionaryEntryEdit
        v-for="(entry, index) in modelValue"
        :key="index"
        class="row"
        :dictionary-entry="entry"
        :id="[id, index].join('_')"
        @remove="remove(index)"
        @update="update(index, $event)"
      />
    </template>
    <p v-else>{{ t("dictionaries.entries.empty") }}</p>
  </div>
</template>
