<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { BadgeVariant } from "@/types/components";
import type { Message } from "@/types/messages";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const variant = computed<BadgeVariant>(() => {
  switch (props.message.status) {
    case "Failed":
      return "danger";
    case "Succeeded":
      return "success";
    default:
      return "secondary";
  }
});
</script>

<template>
  <app-badge :variant="variant">{{ t(`messages.status.options.${props.message.status}`) }}</app-badge>
</template>
