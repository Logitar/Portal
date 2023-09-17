<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Message } from "@/types/messages";
import { formatTemplate } from "@/helpers/displayUtils";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const viewAsHtml = ref<boolean>(props.message.template.contentType === "text/html");

const variables = computed<object | undefined>(() =>
  props.message.variables.length ? Object.fromEntries(props.message.variables.map(({ key, value }) => [key, value])) : undefined
);
</script>

<template>
  <section>
    <p>
      {{ t("messages.contents.generated") }}
      <template v-if="message.template.version">
        <RouterLink :to="{ name: 'TemplateEdit', params: { id: message.template.id } }" target="_blank">
          {{ formatTemplate(message.template) }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" /> </RouterLink
        >.
      </template>
      <strong v-else>{{ formatTemplate(message.template) }}.</strong>
    </p>
    <h3>{{ message.subject }}</h3>
    <p>
      <i class="text-warning">{{ t("messages.contents.warning") }}</i>
    </p>
    <app-accordion>
      <app-accordion-item :title="`messages.contents.type.options.${message.template.contentType}`">
        <div class="mb-3">
          <form-checkbox id="viewAsHtml" label="messages.contents.viewAsHtml" switch v-model="viewAsHtml" />
        </div>
        <div v-if="viewAsHtml" v-html="message.body"></div>
        <div v-else v-text="message.body"></div>
      </app-accordion-item>
      <app-accordion-item title="messages.contents.localization">
        <div class="mb-3">
          <form-checkbox disabled id="ignoreUserLocale" label="messages.contents.ignoreUserLocale" :model-value="message.ignoreUserLocale" />
        </div>
        <locale-select disabled :model-value="message.locale?.code" />
      </app-accordion-item>
      <app-accordion-item title="messages.contents.variables.title">
        <json-viewer v-if="variables" boxed copyable expanded :value="variables" />
        <p v-else>{{ t("messages.contents.variables.empty") }}</p>
      </app-accordion-item>
    </app-accordion>
  </section>
</template>
