<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DemoMessage from "@/components/messages/DemoMessage.vue";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import SenderProviderSelect from "./SenderProviderSelect.vue";
import SetDefaultSender from "./SetDefaultSender.vue";
import type { ApiError } from "@/types/api";
import type { MailgunSettings, Sender, SenderProvider, SendGridSettings } from "@/types/senders";
import type { ToastUtils } from "@/types/components";
import { createSender, readSender, replaceSender } from "@/api/senders";
import { formatSender } from "@/helpers/displayUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const defaults = {
  emailAddress: "",
  displayName: "",
  description: "",
  mailgun: {
    apiKey: "",
    domainName: "",
  },
  sendGrid: {
    apiKey: "",
  },
};

const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const emailAddress = ref<string>(defaults.emailAddress);
const hasLoaded = ref<boolean>(false);
const mailgun = ref<MailgunSettings>(defaults.mailgun);
const provider = ref<SenderProvider>();
const sender = ref<Sender>();
const sendGrid = ref<SendGridSettings>(defaults.sendGrid);

const hasChanges = computed<boolean>(() => {
  const model = sender.value ?? defaults;
  return (
    emailAddress.value !== model.emailAddress ||
    displayName.value !== (model.displayName ?? "") ||
    description.value !== (model.description ?? "") ||
    (provider.value === "Mailgun" &&
      (mailgun.value.apiKey !== (model.mailgun?.apiKey ?? "") || mailgun.value.domainName !== (model.mailgun?.domainName ?? ""))) ||
    (provider.value === "SendGrid" && sendGrid.value.apiKey !== (model.sendGrid?.apiKey ?? ""))
  );
});
const title = computed<string>(() => (sender.value ? formatSender(sender.value) : t("senders.title.new")));

function setModel(model: Sender): void {
  sender.value = model;
  emailAddress.value = model.emailAddress;
  displayName.value = model.displayName ?? "";
  description.value = model.description ?? "";
  provider.value = model.provider;
  mailgun.value = model.mailgun ?? defaults.mailgun;
  sendGrid.value = model.sendGrid ?? defaults.sendGrid;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (sender.value) {
      const updatedSender = await replaceSender(
        sender.value.id,
        {
          emailAddress: emailAddress.value,
          displayName: displayName.value,
          description: description.value,
          mailgun: provider.value === "Mailgun" ? mailgun.value : undefined,
          sendGrid: provider.value === "SendGrid" ? sendGrid.value : undefined,
        },
        sender.value.version,
      );
      setModel(updatedSender);
      toasts.success("senders.updated");
    } else {
      const createdSender = await createSender({
        emailAddress: emailAddress.value,
        displayName: displayName.value,
        description: description.value,
        mailgun: provider.value === "Mailgun" ? mailgun.value : undefined,
        sendGrid: provider.value === "SendGrid" ? sendGrid.value : undefined,
      });
      setModel(createdSender);
      toasts.success("senders.created");
      router.replace({ name: "SenderEdit", params: { id: createdSender.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

function onSetDefault(model: Sender): void {
  if (sender.value) {
    sender.value.updatedBy = model.updatedBy;
    sender.value.updatedOn = model.updatedOn;
    sender.value.version = model.version;
    sender.value.isDefault = model.isDefault;
  }
  toasts.success("senders.default.success");
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const sender = await readSender(id);
      setModel(sender);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <template v-if="hasLoaded">
      <h1>{{ title }}</h1>
      <status-detail v-if="sender" :aggregate="sender" />
      <div class="mb-3">
        <icon-button icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
      </div>
      <app-tabs>
        <app-tab active title="general">
          <form @submit.prevent="onSubmit">
            <div class="mb-3">
              <icon-submit
                class="me-1"
                :disabled="isSubmitting || !hasChanges"
                :icon="sender ? 'save' : 'plus'"
                :loading="isSubmitting"
                :text="sender ? 'actions.save' : 'actions.create'"
                :variant="sender ? undefined : 'success'"
              />
              <SetDefaultSender v-if="sender" class="ms-1" :sender="sender" @error="handleError" @success="onSetDefault" />
            </div>
            <SenderProviderSelect :disabled="Boolean(sender)" required v-model="provider" />
            <div class="row">
              <EmailAddressInput class="col-lg-6" required validate v-model="emailAddress" />
              <display-name-input class="col-lg-6" validate v-model="displayName" />
            </div>
            <description-textarea v-model="description" />
            <!-- <MailgunSettingsEdit v-if="provider === 'Mailgun'" v-model="mailgun" /> -->
            <!-- <SendGridSettingsEdit v-else-if="provider === 'SendGrid'" v-model="sendGrid" /> -->
          </form>
        </app-tab>
        <app-tab :disabled="!sender" title="messages.demo.label">
          <DemoMessage v-if="sender" :sender="sender" @error="handleError" />
        </app-tab>
      </app-tabs>
    </template>
  </main>
</template>
