<script setup lang="ts">
import ClaimTypeSelect from "./ClaimTypeSelect.vue";
import type { Claim } from "@/types/tokens";
import { assign } from "@/helpers/objectUtils";

const props = defineProps<{
  claim: Claim;
  id: string;
}>();

const emit = defineEmits<{
  (e: "remove"): void;
  (e: "update", claim: Claim): void;
}>();
function onUpdate(changes: object): void {
  const claim: Claim = { ...props.claim };
  for (const [key, value] of Object.entries(changes)) {
    assign(claim, key as keyof Claim, value);
  }
  emit("update", claim);
}
</script>

<template>
  <div>
    <div class="col">
      <form-input
        :id="`${id}_name`"
        label="tokens.claims.name.label"
        :max-length="255"
        :model-value="claim.name"
        no-label
        placeholder="tokens.claims.name.placeholder"
        required
        @update:model-value="onUpdate({ name: $event })"
      >
        <template #prepend>
          <icon-button icon="times" variant="danger" @click="$emit('remove')" />
        </template>
      </form-input>
    </div>
    <form-input
      class="col"
      :id="`${id}_value`"
      label="tokens.claims.value.label"
      :model-value="claim.value"
      no-label
      placeholder="tokens.claims.value.placeholder"
      required
      @update:model-value="onUpdate({ value: $event })"
    />
    <ClaimTypeSelect class="col" :id="`${id}_type`" :model-value="claim.type" no-label @update:model-value="onUpdate({ type: $event || null })" />
  </div>
</template>
