<script setup lang="ts">
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import SenderSelect from "@/components/senders/SenderSelect.vue";
import StatusBadge from "./StatusBadge.vue";
import TemplateSelect from "@/components/templates/TemplateSelect.vue";
import VariableList from "./VariableList.vue";
import type { Message, Variable } from "@/types/messages";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import { sendDemoMessage } from "@/api/messages";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const { t } = useI18n();

const props = defineProps<{
  sender?: Sender;
  template?: Template;
}>();

const locale = ref<string | undefined>(account.authenticated?.locale?.code);
const message = ref<Message>();
const selectedSender = ref<Sender>();
const selectedTemplate = ref<Template>();
const showStatus = ref<boolean>(false);
const variables = ref<Variable[]>([]);

const variant = computed<string>(() => {
  switch (message.value?.status) {
    case "Failed":
      return "danger";
    case "Succeeded":
      return "success";
    case "Unsent":
      return "warning";
    default:
      return "secondary";
  }
});
const hasEmail = computed<boolean>(() => Boolean(account.authenticated?.email));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    message.value = await sendDemoMessage({
      senderId: props.sender?.id ?? selectedSender.value?.id,
      templateId: props.template?.id ?? selectedTemplate.value?.id ?? "",
      locale: locale.value,
      variables: variables.value,
    });
    showStatus.value = true;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <form @submit.prevent="onSubmit">
    <app-alert v-if="message" dismissible v-model="showStatus" :variant="variant">
      <StatusBadge :message="message" />
      {{ t(`messages.demo.status.${message.status}`) }}
      <RouterLink :to="{ name: 'MessageView', params: { id: message.id } }" target="_blank">
        {{ t("messages.demo.view") }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
      </RouterLink>
    </app-alert>
    <div class="mb-3">
      <icon-submit class="me-1" :disabled="!hasEmail || isSubmitting" icon="fas fa-paper-plane" :loading="isSubmitting" text="messages.demo.submit" />
      <span v-if="!hasEmail" class="ms-1 text-danger">{{ t("messages.demo.emailRequired") }}</span>
    </div>
    <div class="row">
      <TemplateSelect
        v-if="sender"
        class="col-lg-6"
        :model-value="selectedTemplate?.id"
        :realm="sender.realm"
        required
        @template-selected="selectedTemplate = $event"
      />
      <SenderSelect
        v-else-if="template"
        class="col-lg-6"
        :model-value="selectedSender?.id"
        placeholder="senders.select.useDefault"
        :realm="template.realm"
        @sender-selected="selectedSender = $event"
      />
      <locale-select class="col-lg-6" v-model="locale" />
      <h3>{{ t("messages.contents.variables.title") }}</h3>
      <VariableList v-model="variables" />
    </div>
  </form>
</template>
