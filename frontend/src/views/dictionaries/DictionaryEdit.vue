<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DictionaryEntryList from "@/components/dictionaries/DictionaryEntryList.vue";
import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import type { ApiError } from "@/types/api";
import type { Dictionary, DictionaryEntry } from "@/types/dictionaries";
import type { ToastUtils } from "@/types/components";
import { createDictionary, readDictionary, replaceDictionary } from "@/api/dictionaries";
import { formatDictionary } from "@/helpers/displayUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const dictionary = ref<Dictionary>();
const entries = ref<DictionaryEntry[]>([]);
const hasLoaded = ref<boolean>(false);
const locale = ref<string>("");

const hasChanges = computed<boolean>(() => {
  return locale.value !== dictionary.value?.locale.code || JSON.stringify(entries.value) !== JSON.stringify(dictionary.value?.entries ?? []);
});
const title = computed<string>(() => (dictionary.value ? formatDictionary(dictionary.value) : t("dictionaries.title.new")));

function setModel(model: Dictionary): void {
  dictionary.value = model;
  entries.value = model.entries.map((item) => ({ ...item }));
  locale.value = model.locale.code;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    if (dictionary.value) {
      const updatedDictionary = await replaceDictionary(
        dictionary.value.id,
        {
          locale: locale.value,
          entries: entries.value,
        },
        dictionary.value.version,
      );
      setModel(updatedDictionary);
      toasts.success("dictionaries.updated");
    } else if (locale.value) {
      const createdDictionary = await createDictionary({
        locale: locale.value,
      });
      setModel(createdDictionary);
      toasts.success("dictionaries.created");
      router.replace({ name: "DictionaryEdit", params: { id: createdDictionary.id } });
    }
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const dictionary = await readDictionary(id);
      setModel(dictionary);
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
        <LocaleSelect :disabled="Boolean(dictionary)" required v-model="locale" />
        <template v-if="dictionary">
          <h3>{{ t("dictionaries.entries.title") }}</h3>
          <DictionaryEntryList v-model="entries" />
        </template>
      </form>
    </template>
  </main>
</template>
