<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Message } from "@/types/messages";
import { formatTemplate } from "@/helpers/displayUtils";
import { isEmpty } from "@/helpers/objectUtils";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const viewAsHtml = ref<boolean>(props.message.body.type === "text/html");

const variables = computed<object>(() => Object.fromEntries(props.message.variables.map(({ key, value }) => [key, value])));
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
      <app-accordion-item :title="`messages.contents.type.options.${message.body.type}`">
        <div class="mb-3">
          <form-checkbox id="viewAsHtml" label="messages.contents.viewAsHtml" switch v-model="viewAsHtml" />
        </div>
        <div v-if="viewAsHtml" v-html="message.body.text"></div>
        <div v-else v-text="message.body.text"></div>
      </app-accordion-item>
      <app-accordion-item title="messages.contents.localization">
        <div class="mb-3">
          <form-checkbox disabled id="ignoreUserLocale" label="messages.contents.ignoreUserLocale" :model-value="message.ignoreUserLocale" />
        </div>
        <locale-select disabled :model-value="message.locale?.code" />
      </app-accordion-item>
      <app-accordion-item title="messages.contents.variables.title">
        <p v-if="isEmpty(variables)">{{ t("messages.contents.variables.empty") }}</p>
        <json-viewer v-else boxed copyable expanded :value="variables" />
      </app-accordion-item>
    </app-accordion>
  </section>
</template>
