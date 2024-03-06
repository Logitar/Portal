<script setup lang="ts">
import ClaimTypeSelect from "@/components/tokens/ClaimTypeSelect.vue";
import type { ClaimMapping } from "@/types/realms";
import { assign } from "@/helpers/objectUtils";

const props = defineProps<{
  claimMapping: ClaimMapping;
  id: string;
}>();

const emit = defineEmits<{
  (e: "remove"): void;
  (e: "update", claimMapping: ClaimMapping): void;
}>();
function onUpdate(changes: object): void {
  const claimMapping: ClaimMapping = { ...props.claimMapping };
  for (const [key, value] of Object.entries(changes)) {
    assign(claimMapping, key as keyof ClaimMapping, value);
  }
  emit("update", claimMapping);
}
</script>

<template>
  <div>
    <div class="col">
      <form-input
        :id="`${id}_key`"
        label="customAttributes.key.label"
        :max-length="255"
        :model-value="claimMapping.key"
        no-label
        placeholder="customAttributes.key.placeholder"
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
      :id="`${id}_name`"
      label="tokens.claims.name.label"
      :model-value="claimMapping.name"
      no-label
      placeholder="tokens.claims.name.placeholder"
      required
      @update:model-value="onUpdate({ name: $event })"
    />
    <ClaimTypeSelect class="col" :id="`${id}_type`" :model-value="claimMapping.type" no-label @update:model-value="onUpdate({ type: $event || null })" />
  </div>
</template>
