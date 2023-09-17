<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentTypeSelect from "@/components/templates/ContentTypeSelect.vue";
import DemoMessage from "@/components/messages/DemoMessage.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { ApiError, ErrorDetail } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { Realm } from "@/types/realms";
import type { Template } from "@/types/templates";
import type { ToastUtils } from "@/types/components";
import type { UniqueNameSettings } from "@/types/settings";
import { createTemplate, readTemplate, updateTemplate } from "@/api/templates";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRealm } from "@/api/realms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const defaults = {
  uniqueName: "",
  displayName: "",
  description: "",
  subject: "",
  contentType: "",
  contents: "",
};

const configuration = ref<Configuration>();
const contentType = ref<string>(defaults.contentType);
const contents = ref<string>(defaults.contents);
const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const hasLoaded = ref<boolean>(false);
const realm = ref<Realm>();
const realmId = ref<string>();
const subject = ref<string>(defaults.subject);
const template = ref<Template>();
const uniqueName = ref<string>(defaults.uniqueName);
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() => {
  const model = template.value ?? defaults;
  return (
    realm.value?.id !== template.value?.realm?.id ||
    displayName.value !== (model.displayName ?? "") ||
    uniqueName.value !== model.uniqueName ||
    description.value !== (model.description ?? "") ||
    subject.value !== (model.subject ?? "") ||
    contentType.value !== (model.contentType ?? "") ||
    contents.value !== (model.contents ?? "")
  );
});
const title = computed<string>(() => template.value?.displayName ?? template.value?.uniqueName ?? t("templates.title.new"));
const uniqueNameSettings = computed<UniqueNameSettings>(
  () =>
    realm.value?.uniqueNameSettings ??
    configuration.value?.uniqueNameSettings ?? { allowedCharacters: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+" }
);

function setModel(model: Template): void {
  template.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  realm.value = model.realm;
  realmId.value = model.realm?.id;
  uniqueName.value = model.uniqueName;
  subject.value = model.subject;
  contentType.value = model.contentType;
  contents.value = model.contents;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    if (template.value) {
      const updatedTemplate = await updateTemplate(template.value.id, {
        uniqueName: uniqueName.value !== template.value.uniqueName ? uniqueName.value : undefined,
        displayName: displayName.value !== (template.value.displayName ?? "") ? { value: displayName.value } : undefined,
        description: description.value !== (template.value.description ?? "") ? { value: description.value } : undefined,
        subject: subject.value !== template.value.subject ? subject.value : undefined,
        contentType: contentType.value !== template.value.contentType ? contentType.value : undefined,
        contents: contents.value !== template.value.contents ? contents.value : undefined,
      });
      setModel(updatedTemplate);
      toasts.success("templates.updated");
    } else {
      const createdTemplate = await createTemplate({
        realm: realmId.value,
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
        subject: subject.value,
        contentType: contentType.value,
        contents: contents.value,
      });
      setModel(createdTemplate);
      toasts.success("templates.created");
      router.replace({ name: "TemplateEdit", params: { id: createdTemplate.id } });
    }
  } catch (e: unknown) {
    const { data, status } = e as ApiError;
    if (status === 409 && (data as ErrorDetail)?.errorCode === "UniqueNameAlreadyUsed") {
      uniqueNameAlreadyUsed.value = true;
    } else {
      handleError(e);
    }
  }
});

function onRealmSelected(selected?: Realm) {
  realm.value = selected;
  realmId.value = selected?.id;
}

onMounted(async () => {
  try {
    configuration.value = await readConfiguration();
    const id = route.params.id?.toString();
    const realmIdQuery = route.query.realm?.toString();
    if (id) {
      const template = await readTemplate(id);
      setModel(template);
    } else if (realmIdQuery) {
      const foundRealm = await readRealm(realmIdQuery);
      realm.value = foundRealm;
      realmId.value = foundRealm.id;
      contentType.value = route.query.contentType?.toString() || defaults.contentType;
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
      <app-alert dismissible variant="warning" v-model="uniqueNameAlreadyUsed">
        <strong>{{ t("uniqueName.alreadyUsed.lead") }}</strong> {{ t("uniqueName.alreadyUsed.help") }}
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
            <div class="row">
              <RealmSelect class="col-lg-6" :disabled="Boolean(template)" :model-value="realmId" @realm-selected="onRealmSelected" />
              <ContentTypeSelect class="col-lg-6" required v-model="contentType" />
            </div>
            <div class="row">
              <unique-name-input class="col-lg-6" required :settings="uniqueNameSettings" validate v-model="uniqueName" />
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
