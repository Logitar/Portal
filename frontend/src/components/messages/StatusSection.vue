<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import StatusBadge from "./StatusBadge.vue";
import type { Message } from "@/types/messages";
import { formatRealm, formatSender } from "@/helpers/displayUtils";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const resultData = computed<object | undefined>(() =>
  props.message?.resultData.length ? Object.fromEntries(props.message.resultData.map(({ key, value }) => [key, value])) : undefined,
);
</script>

<template>
  <section>
    <status-detail :aggregate="message" />
    <p>
      <template v-if="message.realm">
        {{ t("messages.sent.realm") }}
        <RouterLink :to="{ name: 'RealmEdit', params: { uniqueSlug: message.realm.uniqueSlug } }" target="_blank">
          {{ formatRealm(message.realm) }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" /> </RouterLink
        >.
      </template>
      <template v-else>
        {{ t("messages.sent.portal") }} <strong>{{ t("brand") }}</strong
        >.
      </template>
      <br />
      {{ t("messages.sent.sender") }}
      <RouterLink v-if="message.sender.version" :to="{ name: 'SenderEdit', params: { id: message.sender.id } }" target="_blank">
        {{ formatSender(message.sender) }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
      </RouterLink>
      <strong v-else>{{ formatSender(message.sender) }}</strong>
      {{ t("messages.sent.provider") }}
      <strong>{{ t(`senders.providers.options.${message.sender.provider}`) }}</strong
      >.
      <br />
      {{ t("messages.status.format") }} <StatusBadge :message="message" />
      <template v-if="!message.isDemo">
        <br />
        <app-badge>{{ t("messages.demo.label") }}</app-badge> <i class="text-info">{{ t("messages.demo.hint") }}</i>
      </template>
    </p>
    <template v-if="resultData">
      <h3>{{ t("messages.resultData") }}</h3>
      <json-viewer boxed copyable expanded :value="resultData" />
    </template>
  </section>
</template>
