<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import ProviderTypeSelect from "./ProviderTypeSelect.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import SendGridSettings from "./SendGridSettings.vue";
import SetDefaultSender from "./SetDefaultSender.vue";
import type { ApiError } from "@/types/api";
import type { Configuration } from "@/types/configuration";
import type { ProviderSetting, ProviderSettingModification, ProviderType, Sender } from "@/types/senders";
import type { Realm } from "@/types/realms";
import type { ToastUtils } from "@/types/components";
import { createSender, readSender, updateSender } from "@/api/senders";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRealm } from "@/api/realms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const defaults = {
  emailAddress: "",
  displayName: "",
  description: "",
  settings: [],
};

const configuration = ref<Configuration>();
const description = ref<string>(defaults.description);
const displayName = ref<string>(defaults.displayName);
const emailAddress = ref<string>(defaults.emailAddress);
const hasLoaded = ref<boolean>(false);
const provider = ref<ProviderType>();
const realm = ref<Realm>();
const realmId = ref<string>();
const sender = ref<Sender>();
const settings = ref<ProviderSetting[]>(defaults.settings);

const hasChanges = computed<boolean>(() => {
  const model = sender.value ?? defaults;
  return (
    realm.value?.id !== sender.value?.realm?.id ||
    emailAddress.value !== model.emailAddress ||
    displayName.value !== (model.displayName ?? "") ||
    description.value !== (model.description ?? "") ||
    JSON.stringify(settings.value) !== JSON.stringify(sender.value?.settings ?? [])
  );
});
const title = computed<string>(() => {
  if (sender.value) {
    return sender.value.displayName ? `${sender.value.displayName} <${sender.value.emailAddress}>` : sender.value.emailAddress;
  }
  return t("senders.title.new");
});

function setModel(model: Sender): void {
  sender.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  emailAddress.value = model.emailAddress;
  provider.value = model.provider;
  realm.value = model.realm;
  realmId.value = model.realm?.id;
  settings.value = model.settings.map((item) => ({ ...item }));
}

function getSettingModifications(source: ProviderSetting[], destination: ProviderSetting[]): ProviderSettingModification[] {
  const modifications: ProviderSettingModification[] = [];

  const destinationKeys = new Set(destination.map(({ key }) => key));
  for (const setting of source) {
    const key = setting.key;
    if (!destinationKeys.has(key)) {
      modifications.push({ key });
    }
  }

  const sourceMap = new Map(source.map(({ key, value }) => [key, value]));
  for (const setting of destination) {
    if (sourceMap.get(setting.key) !== setting.value) {
      modifications.push(setting);
    }
  }

  return modifications;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (sender.value) {
      const settingModifications = getSettingModifications(sender.value.settings, settings.value);
      const updatedSender = await updateSender(sender.value.id, {
        emailAddress: emailAddress.value !== sender.value.emailAddress ? emailAddress.value : undefined,
        displayName: displayName.value !== (sender.value.displayName ?? "") ? { value: displayName.value } : undefined,
        description: description.value !== (sender.value.description ?? "") ? { value: description.value } : undefined,
        settings: settingModifications.length ? settingModifications : undefined,
      });
      setModel(updatedSender);
      toasts.success("senders.updated");
    } else {
      const createdSender = await createSender({
        realm: realmId.value,
        emailAddress: emailAddress.value,
        displayName: displayName.value,
        description: description.value,
        provider: provider.value as ProviderType,
        settings: settings.value,
      });
      setModel(createdSender);
      toasts.success("senders.created");
      router.replace({ name: "SenderEdit", params: { id: createdSender.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

function onRealmSelected(selected?: Realm) {
  realm.value = selected;
  realmId.value = selected?.id;
}

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
    configuration.value = await readConfiguration();
    const id = route.params.id?.toString();
    const realmIdQuery = route.query.realm?.toString();
    if (id) {
      const sender = await readSender(id);
      setModel(sender);
    } else if (realmIdQuery) {
      const foundRealm = await readRealm(realmIdQuery);
      realm.value = foundRealm;
      realmId.value = foundRealm.id;
      provider.value = (route.query.provider?.toString() as ProviderType) || undefined;
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
          <icon-button class="mx-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
          <SetDefaultSender v-if="sender" class="ms-1" :sender="sender" @error="handleError" @success="onSetDefault" />
        </div>
        <div class="row">
          <RealmSelect class="col-lg-6" :disabled="Boolean(sender)" :model-value="realmId" @realm-selected="onRealmSelected" />
          <ProviderTypeSelect class="col-lg-6" :disabled="Boolean(sender)" required v-model="provider" />
        </div>
        <div class="row">
          <EmailAddressInput class="col-lg-6" required validate v-model="emailAddress" />
          <display-name-input class="col-lg-6" validate v-model="displayName" />
        </div>
        <description-textarea v-model="description" />
        <h3>{{ t("settings.title") }}</h3>
        <SendGridSettings v-if="provider === 'SendGrid'" v-model="settings" />
      </form>
    </template>
  </main>
</template>
