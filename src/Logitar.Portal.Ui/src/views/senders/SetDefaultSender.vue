<script setup lang="ts">
import { computed, ref } from "vue";

import type { ButtonVariant } from "@/types/components";
import type { Sender } from "@/types/senders";
import { setDefaultSender } from "@/api/senders";

const props = defineProps<{
  sender: Sender;
}>();

const isLoading = ref<boolean>(false);

const isDisabled = computed<boolean>(() => isLoading.value || props.sender.isDefault);
const variant = computed<ButtonVariant>(() => (props.sender.isDefault ? "info" : "warning"));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "success", value: Sender): void;
}>();
async function onClick(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const sender = await setDefaultSender(props.sender.id);
      emit("success", sender);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <icon-button :disabled="isDisabled" icon="fas fa-star" :loading="isLoading" text="senders.default.submit" :variant="variant" @click="onClick" />
</template>
