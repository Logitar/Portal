<script setup lang="ts">
import type { RouteLocationRaw } from "vue-router";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { User } from "@/types/users";

const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const to = computed<RouteLocationRaw>(() => ({
  name: "SessionList",
  query: { user: props.user.id, sort: "UpdatedOn", isDescending: "true", page: "1", count: "10" },
}));
</script>

<template>
  <RouterLink :to="to">{{ t("sessions.view.label") }}</RouterLink>
</template>
