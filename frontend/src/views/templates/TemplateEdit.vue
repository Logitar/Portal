<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentTypeSelect from "@/components/templates/ContentTypeSelect.vue";
import DemoMessage from "@/components/messages/DemoMessage.vue";
import type { ApiError, ErrorDetail } from "@/types/api";
import type { Template } from "@/types/templates";
import type { ToastUtils } from "@/types/components";
import { createTemplate, readTemplate, replaceTemplate } from "@/api/templates";
import { handleErrorKey, toastsKey } from "@/inject/App";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const defaults = {
  uniqueKey: "",
  displayName: "",
  description: "",
  subject: "",
  content: {
    type: "",
    text: "",
  },
};

const contentType = ref<string>(defaults.content.type);
const contents = ref<string>(defaults.content.text);
const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const hasLoaded = ref<boolean>(false);
const subject = ref<string>(defaults.subject);
const template = ref<Template>();
const uniqueKey = ref<string>(defaults.uniqueKey);
const uniqueKeyAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() => {
  const model = template.value ?? defaults;
  return (
    uniqueKey.value !== model.uniqueKey ||
    displayName.value !== (model.displayName ?? "") ||
    description.value !== (model.description ?? "") ||
    subject.value !== (model.subject ?? "") ||
    contentType.value !== (model.content.type ?? "") ||
    contents.value !== (model.content.text ?? "")
  );
});
const title = computed<string>(() => template.value?.displayName ?? template.value?.uniqueKey ?? t("templates.title.new"));

function setModel(model: Template): void {
  template.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  uniqueKey.value = model.uniqueKey;
  subject.value = model.subject;
  contentType.value = model.content.type;
  contents.value = model.content.text;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueKeyAlreadyUsed.value = false;
  try {
    if (template.value) {
      const updatedTemplate = await replaceTemplate(
        template.value.id,
        {
          uniqueKey: uniqueKey.value,
          displayName: displayName.value,
          description: description.value,
          subject: subject.value,
          content: {
            type: contentType.value,
            text: contents.value,
          },
        },
        template.value.version,
      );
      setModel(updatedTemplate);
      toasts.success("templates.updated");
    } else {
      const createdTemplate = await createTemplate({
        uniqueKey: uniqueKey.value,
        displayName: displayName.value,
        description: description.value,
        subject: subject.value,
        content: {
          type: contentType.value,
          text: contents.value,
        },
      });
      setModel(createdTemplate);
      toasts.success("templates.created");
      router.replace({ name: "TemplateEdit", params: { id: createdTemplate.id } });
    }
  } catch (e: unknown) {
    const { data, status } = e as ApiError;
    if (status === 409 && (data as ErrorDetail)?.errorCode === "UniqueKeyAlreadyUsed") {
      uniqueKeyAlreadyUsed.value = true;
    } else {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const template = await readTemplate(id);
      setModel(template);
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
      <app-alert dismissible variant="warning" v-model="uniqueKeyAlreadyUsed">
        <strong>{{ t("templates.uniqueKeyAlreadyUsed.lead") }}</strong> {{ t("templates.uniqueKeyAlreadyUsed.help") }}
      </app-alert>
      <status-detail v-if="template" :aggregate="template" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <icon-submit
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="template ? 'save' : 'plus'"
            :loading="isSubmitting"
            :text="template ? 'actions.save' : 'actions.create'"
            :variant="template ? undefined : 'success'"
          />
          <icon-button class="ms-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <app-tabs>
          <app-tab active title="general">
            <ContentTypeSelect required v-model="contentType" />
            <div class="row">
              <!-- TODO(fpion): implement UniqueKeyInput.vue -->
              <unique-key-input class="col-lg-6" required validate v-model="uniqueKey" />
              <display-name-input class="col-lg-6" validate v-model="displayName" />
            </div>
            <form-input id="subject" label="templates.subject.label" :maxLength="255" placeholder="templates.subject.placeholder" required v-model="subject" />
            <form-textarea id="contents" label="templates.contents.label" placeholder="templates.contents.placeholder" required :rows="20" v-model="contents" />
          </app-tab>
          <app-tab title="description.label">
            <description-textarea no-label v-model="description" />
          </app-tab>
          <app-tab :disabled="!template" title="messages.demo.label">
            <DemoMessage v-if="template" :template="template" @error="handleError" />
          </app-tab>
        </app-tabs>
      </form>
    </template>
  </main>
</template>
