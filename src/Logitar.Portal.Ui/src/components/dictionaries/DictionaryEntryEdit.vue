<script setup lang="ts">
import type { DictionaryEntry } from "@/types/dictionaries";
import { assign } from "@/helpers/objectUtils";

const props = defineProps<{
  dictionaryEntry: DictionaryEntry;
  id: string;
}>();

const emit = defineEmits<{
  (e: "remove"): void;
  (e: "update", attribute: DictionaryEntry): void;
}>();
function onUpdate(changes: object): void {
  const dictionaryEntry: DictionaryEntry = { ...props.dictionaryEntry };
  for (const [key, value] of Object.entries(changes)) {
    assign(dictionaryEntry, key as keyof DictionaryEntry, value);
  }
  emit("update", dictionaryEntry);
}
</script>

<template>
  <div>
    <div class="col">
      <form-input
        :id="`${id}_key`"
        label="dictionaries.entries.key.label"
        :max-length="255"
        :model-value="dictionaryEntry.key"
        no-label
        placeholder="dictionaries.entries.key.placeholder"
        required
        :rules="{ identifier: true }"
        @update:model-value="onUpdate({ key: $event })"
      >
        <template #prepend>
          <icon-button icon="times" variant="danger" @click="$emit('remove')" />
        </template>
      </form-input>
    </div>
    <form-input
      class="col"
      :id="`${id}_value`"
      label="dictionaries.entries.value.label"
      :model-value="dictionaryEntry.value"
      no-label
      placeholder="dictionaries.entries.value.placeholder"
      required
      @update:model-value="onUpdate({ value: $event })"
    />
  </div>
</template>
