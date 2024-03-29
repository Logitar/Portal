<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import SenderProviderSelect from "./SenderProviderSelect.vue";
import SetDefaultSender from "./SetDefaultSender.vue";
import type { ApiError, Error } from "@/types/api";
import type { Sender, SenderProvider, SenderSort, SearchSendersPayload } from "@/types/senders";
import type { SelectOption, ToastUtils } from "@/types/components";
import { deleteSender, searchSenders } from "@/api/senders";
import { formatSender } from "@/helpers/displayUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { rt, t, tm } = useI18n();

const cannotDeleteDefaultSender = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const senders = ref<Sender[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => Number(route.query.count) || 10);
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const page = computed<number>(() => Number(route.query.page) || 1);
const provider = computed<string>(() => route.query.provider?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("senders.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const parameters: SearchSendersPayload = {
    provider: (provider.value as SenderProvider) || undefined,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as SenderSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchSenders(parameters);
    if (now === timestamp.value) {
      senders.value = data.items;
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

async function onDelete(sender: Sender, hideModal: () => void): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    cannotDeleteDefaultSender.value = false;
    try {
      await deleteSender(sender.id);
      hideModal();
      toasts.success("senders.delete.success");
    } catch (e: unknown) {
      const { data, status } = e as ApiError;
      if (status === 400 && (data as Error).code === "CannotDeleteDefaultSender") {
        cannotDeleteDefaultSender.value = true;
        hideModal();
      } else {
        handleError(e);
      }
      return;
    } finally {
      isLoading.value = false;
    }
    await refresh();
  }
}

function onSetDefault(): void {
  refresh();
  toasts.success("senders.default.success");
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "provider":
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
    if (route.name === "SenderList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                provider: "",
                search: "",
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
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("senders.title.list") }}</h1>
    <app-alert dismissible variant="warning" v-model="cannotDeleteDefaultSender">
      <strong>{{ t("senders.cannotDeleteDefaultSender.lead") }}</strong> {{ t("senders.cannotDeleteDefaultSender.help") }}
    </app-alert>
    <div class="my-2">
      <icon-button class="me-1" :disabled="isLoading" icon="fas fa-rotate" :loading="isLoading" text="actions.refresh" @click="refresh()" />
      <icon-button
        class="ms-1"
        icon="fas fa-plus"
        text="actions.create"
        :to="{ name: 'CreateSender', query: { provider: provider || undefined } }"
        variant="success"
      />
    </div>
    <SenderProviderSelect :model-value="provider" @update:model-value="setQuery('provider', $event)" />
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
    <template v-if="senders.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("senders.sort.options.EmailAddress") }}</th>
            <th scope="col">{{ t("senders.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("senders.providers.label") }}</th>
            <th scope="col">{{ t("senders.sort.options.UpdatedOn") }}</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="sender in senders" :key="sender.id">
            <td>
              <RouterLink :to="{ name: 'SenderEdit', params: { id: sender.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ sender.emailAddress }}
              </RouterLink>
            </td>
            <td>{{ sender.displayName ?? "—" }}</td>
            <td>{{ t(`senders.providers.options.${sender.provider}`) }}</td>
            <td><status-block :actor="sender.updatedBy" :date="sender.updatedOn" /></td>
            <td>
              <SetDefaultSender class="me-1" :sender="sender" @error="handleError" @success="onSetDefault" />
              <icon-button
                class="ms-1"
                :disabled="isLoading"
                icon="trash"
                text="actions.delete"
                variant="danger"
                data-bs-toggle="modal"
                :data-bs-target="`#deleteModal_${sender.id}`"
              />
              <delete-modal
                confirm="senders.delete.confirm"
                :display-name="formatSender(sender)"
                :id="`deleteModal_${sender.id}`"
                :loading="isLoading"
                title="senders.delete.title"
                @ok="onDelete(sender, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <app-pagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
    </template>
    <p v-else>{{ t("senders.empty") }}</p>
  </main>
</template>
