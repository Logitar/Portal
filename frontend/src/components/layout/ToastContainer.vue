<script setup lang="ts">
import { nanoid } from "nanoid";
import { ref } from "vue";

import AppToast from "./AppToast.vue";
import type { ToastOptions } from "@/types/components";

const toasts = ref<Map<string, ToastOptions>>(new Map<string, ToastOptions>());

function add(toast: ToastOptions): void {
  const key = ["toast", nanoid()].join("_");
  toasts.value.set(key, toast);
}

function onHidden(key: string): void {
  toasts.value.delete(key);
}

defineExpose({ add });
</script>

<template>
  <div class="toast-container position-fixed top-0 end-0 p-3">
    <AppToast
      v-for="[key, toast] in toasts"
      :key="key"
      :id="key"
      :message="toast.message"
      :solid="toast.solid"
      :title="toast.title"
      :variant="toast.variant"
      @hidden="onHidden(key)"
    />
  </div>
</template>
