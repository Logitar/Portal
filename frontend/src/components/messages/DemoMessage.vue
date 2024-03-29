<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import SenderSelect from "@/components/senders/SenderSelect.vue";
import StatusBadge from "./StatusBadge.vue";
import TemplateSelect from "@/components/templates/TemplateSelect.vue";
import VariableList from "./VariableList.vue";
import type { ApiError } from "@/types/api";
import type { ContentType, Template } from "@/types/templates";
import type { Message, RecipientPayload, Variable } from "@/types/messages";
import type { Sender, SenderProvider, SenderType } from "@/types/senders";
import type { User } from "@/types/users";
import { getSenderType } from "@/helpers/senderUtils";
import { readDefaultSender } from "@/api/senders";
import { readMessage, sendMessage } from "@/api/messages";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const { t } = useI18n();
type DemoError = "InvalidSmsMessageContentType" | "RealmHasNoDefaultSender" | "TemplateIsRequired" | "UserHasNoEmail" | "UserHasNoPhone";

const props = defineProps<{
  sender?: Sender;
  template?: Template;
}>();
if ((!props.sender && !props.template) || (props.sender && props.template)) {
  throw new Error("Only one of the following properties must be specified: sender, template.");
}

const defaultSender = ref<Sender>();
const ignoreUserLocale = ref<boolean>(false);
const locale = ref<string | undefined>(account.authenticated?.locale?.code);
const message = ref<Message>();
const selectedSender = ref<Sender>();
const selectedTemplate = ref<Template>();
const showStatus = ref<boolean>(false);
const variables = ref<Variable[]>([]);

const error = computed<DemoError | undefined>(() => {
  const contentType: ContentType | undefined = props.template?.content.type ?? selectedTemplate.value?.content.type;
  if (!props.sender && !selectedSender.value && !defaultSender.value) {
    return "RealmHasNoDefaultSender";
  } else if (!props.template && !selectedTemplate.value) {
    return "TemplateIsRequired";
  } else if (senderType.value === "Email" && !account.authenticated?.email) {
    return "UserHasNoEmail";
  } else if (senderType.value === "Sms" && !account.authenticated?.phone) {
    return "UserHasNoPhone";
  } else if (senderType.value === "Sms" && contentType !== "text/plain") {
    return "InvalidSmsMessageContentType";
  }
  return undefined;
});
const senderType = computed<SenderType | undefined>(() => {
  const senderProvider: SenderProvider | undefined = selectedSender.value?.provider ?? props.sender?.provider ?? defaultSender.value?.provider;
  return senderProvider ? getSenderType(senderProvider) : undefined;
});
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

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();
const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  showStatus.value = false;
  const user: User | undefined = account.authenticated;
  const realmId: string | undefined = props.sender?.realm?.id ?? props.template?.realm?.id;
  const recipient: RecipientPayload = { type: "To" };
  if (user?.realm?.id === realmId) {
    recipient.userId = user?.id;
  } else if (senderType.value === "Email") {
    recipient.address = user?.email?.address;
    recipient.displayName = user?.fullName;
  } else if (senderType.value === "Sms") {
    recipient.phoneNumber = user?.phone?.number;
  }
  try {
    const sentMessages = await sendMessage({
      senderId: props.sender?.id ?? selectedSender.value?.id,
      template: props.template?.id ?? selectedTemplate.value?.id ?? "",
      recipients: [recipient],
      ignoreUserLocale: ignoreUserLocale.value,
      locale: locale.value,
      variables: variables.value,
      isDemo: true,
    });
    message.value = await readMessage(sentMessages.ids[0]);
    showStatus.value = true;
  } catch (e: unknown) {
    emit("error", e);
  }
});

onMounted(async () => {
  try {
    if (!props.sender) {
      defaultSender.value = await readDefaultSender();
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (!status || status !== 404) {
      emit("error", e);
    }
  }
});
</script>

<template>
  <form @submit.prevent="onSubmit">
    {{ senderType }}
    <app-alert v-if="message" dismissible v-model="showStatus" :variant="variant">
      <StatusBadge :message="message" />
      {{ t(`messages.demo.status.${message.status}`) }}
      <RouterLink :to="{ name: 'MessageView', params: { id: message.id } }" target="_blank">
        {{ t("messages.demo.view") }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
      </RouterLink>
    </app-alert>
    <div class="mb-3">
      <icon-submit class="me-1" :disabled="Boolean(error) || isSubmitting" icon="fas fa-paper-plane" :loading="isSubmitting" text="messages.demo.submit" />
      <span v-if="error === 'RealmHasNoDefaultSender'" class="ms-1 text-danger">{{ t("messages.demo.realmHasNoDefaultSender") }}</span>
      <span v-else-if="error === 'UserHasNoEmail'" class="ms-1 text-danger">{{ t("messages.demo.emailRequired") }}</span>
      <span v-else-if="error === 'UserHasNoPhone'" class="ms-1 text-danger">{{ t("messages.demo.phoneRequired") }}</span>
      <span v-else-if="error === 'InvalidSmsMessageContentType'" class="ms-1 text-danger">{{ t("messages.demo.invalidSmsMessageContentType") }}</span>
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
