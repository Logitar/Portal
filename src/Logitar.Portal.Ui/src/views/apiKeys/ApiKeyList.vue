<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { ApiKey, ApiKeySort, SearchApiKeysPayload } from "@/types/apiKeys";
import type { SelectOption, ToastUtils } from "@/types/components";
import { deleteApiKey, searchApiKeys } from "@/api/apiKeys";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { d, rt, t, tm } = useI18n();

const apiKeys = ref<ApiKey[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => Number(route.query.count) || 10);
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const isExpired = computed<boolean | undefined>(() => (route.query.isExpired ? route.query.isExpired === "true" : undefined));
const page = computed<number>(() => Number(route.query.page) || 1);
const realm = computed<string>(() => route.query.realm?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("apiKeys.sort.options"))).map(([value, text]) => ({ text, value } as SelectOption)),
    "text"
  )
);

function isApiKeyExpired(apiKey: ApiKey): boolean {
  return Boolean(apiKey.expiresOn && new Date(apiKey.expiresOn) <= new Date());
}

async function refresh(): Promise<void> {
  const parameters: SearchApiKeysPayload = {
    realm: realm.value || undefined,
    status: typeof isExpired.value === "boolean" ? { isExpired: isExpired.value } : undefined,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ApiKeySort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchApiKeys(parameters);
    if (now === timestamp.value) {
      apiKeys.value = data.results;
      total.value = data.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

async function onDelete(apiKey: ApiKey, hideModal: () => void): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await deleteApiKey(apiKey.id);
      hideModal();
      toasts.success("apiKeys.delete.success");
    } catch (e: unknown) {
      handleError(e);
      return;
    } finally {
      isLoading.value = false;
    }
    await refresh();
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "isExpired":
    case "realm":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ApiKeyList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                realm: "",
                status: "",
                search: "",
                isExpired: "",
                sort: "UpdatedOn",
                isDescending: "true",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true }
);
</script>

<template>
  <main class="container">
    <h1>{{ t("apiKeys.title.list") }}</h1>
    <div class="my-2">
      <icon-button class="me-1" :disabled="isLoading" icon="fas fa-rotate" :loading="isLoading" text="actions.refresh" @click="refresh()" />
      <icon-button
        class="ms-1"
        icon="fas fa-plus"
        text="actions.create"
        :to="{ name: 'CreateApiKey', query: { realm: realm || undefined } }"
        variant="success"
      />
    </div>
    <div class="row">
      <RealmSelect class="col-lg-6" :model-value="realm" @update:model-value="setQuery('realm', $event)" />
      <yes-no-select
        class="col-lg-6"
        id="isExpired"
        label="apiKeys.isExpired.label"
        placeholder="apiKeys.isExpired.placeholder"
        :model-value="isExpired?.toString()"
        @update:model-value="setQuery('isExpired', $event)"
      />
    </div>
    <div class="row">
      <search-input class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <sort-select
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event)"
        @update:model-value="setQuery('sort', $event)"
      />
      <count-select class="col-lg-4" :model-value="count" @update:model-value="setQuery('count', $event)" />
    </div>
    <template v-if="apiKeys.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("apiKeys.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.ExpiresOn") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.AuthenticatedOn") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.UpdatedOn") }}</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="apiKey in apiKeys" :key="apiKey.id">
            <td>
              <RouterLink :to="{ name: 'ApiKeyEdit', params: { id: apiKey.id } }">{{ apiKey.displayName }}</RouterLink>
            </td>
            <td>
              <app-badge v-if="isApiKeyExpired(apiKey)">{{ t("apiKeys.expired") }}</app-badge>
              <template v-else-if="apiKey.expiresOn">{{ d(apiKey.expiresOn, "medium") }}</template>
              <template v-else>{{ "—" }}</template>
            </td>
            <td>{{ apiKey.authenticatedOn ? d(apiKey.authenticatedOn, "medium") : "—" }}</td>
            <td><status-block :actor="apiKey.updatedBy" :date="apiKey.updatedOn" /></td>
            <td>
              <icon-button
                :disabled="isLoading"
                icon="trash"
                text="actions.delete"
                variant="danger"
                data-bs-toggle="modal"
                :data-bs-target="`#deleteModal_${apiKey.id}`"
              />
              <delete-modal
                confirm="apiKeys.delete.confirm"
                :display-name="apiKey.displayName"
                :id="`deleteModal_${apiKey.id}`"
                :loading="isLoading"
                title="apiKeys.delete.title"
                @ok="onDelete(apiKey, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <app-pagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
    </template>
    <p v-else>{{ t("apiKeys.empty") }}</p>
  </main>
</template>
