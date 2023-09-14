<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import locales from "@/resources/locales.json";
import type { ApiError } from "@/types/api";
import type { Dictionary, DictionaryEntry } from "@/types/dictionaries";
import type { Locale } from "@/types/i18n";
import type { Realm } from "@/types/realms";
import type { ToastUtils } from "@/types/components";
import { createDictionary, readDictionary, updateDictionary } from "@/api/dictionaries";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readRealm } from "@/api/realms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const dictionary = ref<Dictionary>();
const entries = ref<DictionaryEntry[]>([]);
const hasLoaded = ref<boolean>(false);
const locale = ref<string>();
const realm = ref<Realm>();
const realmId = ref<string>();

const hasChanges = computed<boolean>(() => {
  return (
    realm.value?.id !== dictionary.value?.realm?.id ||
    locale.value !== dictionary.value?.locale ||
    JSON.stringify(entries.value) !== JSON.stringify(dictionary.value?.entries ?? [])
  );
});
const title = computed<string>(() => {
  if (dictionary.value) {
    const locale: Locale | undefined = locales.find(({ code }) => code === dictionary.value?.locale);
    return [
      dictionary.value.realm ? dictionary.value.realm.displayName ?? dictionary.value.realm.uniqueSlug : t("brand"),
      locale?.nativeName ?? dictionary.value.locale,
    ].join(" | ");
  }
  return t("dictionaries.title.new");
});

function setModel(model: Dictionary): void {
  dictionary.value = model;
  entries.value = model.entries.map((item) => ({ ...item }));
  locale.value = model.locale;
  realm.value = model.realm;
  realmId.value = model.realm?.id;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (dictionary.value) {
      const updatedDictionary = await updateDictionary(dictionary.value.id, {
        entries: [], // TODO(fpion): implement
      });
      setModel(updatedDictionary);
      toasts.success("dictionaries.updated");
    } else if (locale.value) {
      const createdDictionary = await createDictionary({
        realm: realmId.value,
        locale: locale.value,
        entries: entries.value,
      });
      setModel(createdDictionary);
      toasts.success("dictionaries.created");
      router.replace({ name: "DictionaryEdit", params: { id: createdDictionary.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

function onRealmSelected(selected?: Realm) {
  realm.value = selected;
  realmId.value = selected?.id;
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    const realmIdQuery = route.query.realm?.toString();
    if (id) {
      const dictionary = await readDictionary(id);
      setModel(dictionary);
    } else if (realmIdQuery) {
      const foundRealm = await readRealm(realmIdQuery);
      realm.value = foundRealm;
      realmId.value = foundRealm.id;
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
      <status-detail v-if="dictionary" :aggregate="dictionary" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <icon-submit
            class="me-1"
            :disabled="isSubmitting || !hasChanges"
            :icon="dictionary ? 'save' : 'plus'"
            :loading="isSubmitting"
            :text="dictionary ? 'actions.save' : 'actions.create'"
            :variant="dictionary ? undefined : 'success'"
          />
          <icon-button class="ms-1" icon="chevron-left" text="actions.back" :variant="hasChanges ? 'danger' : 'secondary'" @click="router.back()" />
        </div>
        <div class="row">
          <RealmSelect class="col-lg-6" :disabled="Boolean(dictionary)" :model-value="realmId" @realm-selected="onRealmSelected" />
          <LocaleSelect class="col-lg-6" :disabled="Boolean(dictionary)" required v-model="locale" />
        </div>
      </form>
    </template>
  </main>
</template>
