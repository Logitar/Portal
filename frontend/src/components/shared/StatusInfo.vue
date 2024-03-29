<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Actor } from "@/types/actor";

const { d, t } = useI18n();

const props = defineProps<{
  actor: Actor;
  date: string;
  format: string;
}>();

const displayName = computed<string>(() => {
  const { displayName, type } = props.actor;
  return type === "System" ? t("system") : displayName;
});
const href = computed<string | undefined>(() => {
  const { id, isDeleted, type } = props.actor;
  if (!isDeleted) {
    switch (type) {
      case "ApiKey":
        return `/api-keys/${id}`;
      case "User":
        return `/users/${id}`;
    }
  }
  return undefined;
});
const icon = computed<string | undefined>(() => {
  switch (props.actor.type) {
    case "ApiKey":
      return "key";
    case "System":
      return "robot";
    case "User":
      return "user";
  }
  return undefined;
});
const variant = computed<string | undefined>(() => (props.actor.type === "ApiKey" ? "info" : undefined));
</script>

<template>
  <span>
    {{ t(format, { date: d(date, "medium") }) }}
    <template v-if="actor.type === 'System' || actor.isDeleted">
      <app-avatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :size="24" :url="actor.pictureUrl" :variant="variant" />
      {{ displayName }}
    </template>
    <a v-else :href="href" target="_blank">
      <app-avatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :size="24" :url="actor.pictureUrl" :variant="variant" />
      {{ displayName }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
    </a>
  </span>
</template>
