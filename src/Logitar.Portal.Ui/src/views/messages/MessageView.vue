<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentSection from "@/components/messages/ContentSection.vue";
import RecipientSection from "@/components/messages/RecipientSection.vue";
import StatusSection from "@/components/messages/StatusSection.vue";
import type { ApiError } from "@/types/api";
import type { Message } from "@/types/messages";
import { handleErrorKey } from "@/inject/App";
import { readMessage } from "@/api/messages";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const message = ref<Message>();

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      message.value = await readMessage(id);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="message">
      <h1>{{ t("messages.title.view") }}</h1>
      <div class="mb-3">
        <icon-button class="ms-1" icon="chevron-left" text="actions.back" variant="secondary" @click="router.back()" />
      </div>
      <app-tabs>
        <app-tab active title="messages.status.label">
          <StatusSection :message="message" />
        </app-tab>
        <app-tab title="templates.contents.label">
          <ContentSection :message="message" />
        </app-tab>
        <app-tab title="messages.recipients.label">
          <RecipientSection :recipients="message.recipients" />
        </app-tab>
      </app-tabs>
    </template>
  </main>
</template>
