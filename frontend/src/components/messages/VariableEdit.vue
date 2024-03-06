<script setup lang="ts">
import type { Variable } from "@/types/messages";
import { assign } from "@/helpers/objectUtils";

const props = defineProps<{
  variable: Variable;
  id: string;
}>();

const emit = defineEmits<{
  (e: "remove"): void;
  (e: "update", attribute: Variable): void;
}>();
function onUpdate(changes: object): void {
  const variable: Variable = { ...props.variable };
  for (const [key, value] of Object.entries(changes)) {
    assign(variable, key as keyof Variable, value);
  }
  emit("update", variable);
}
</script>

<template>
  <div>
    <div class="col">
      <form-input
        :id="`${id}_key`"
        label="messages.contents.variables.key.label"
        :max-length="255"
        :model-value="variable.key"
        no-label
        placeholder="messages.contents.variables.key.placeholder"
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
      label="messages.contents.variables.value.label"
      :model-value="variable.value"
      no-label
      placeholder="messages.contents.variables.value.placeholder"
      required
      @update:model-value="onUpdate({ value: $event })"
    />
  </div>
</template>
