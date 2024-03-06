<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentTypeSelect from "@/components/templates/ContentTypeSelect.vue";
import type { SelectOption, ToastUtils } from "@/types/components";
import type { Template, TemplateSort, SearchTemplatesPayload } from "@/types/templates";
import { deleteTemplate, searchTemplates } from "@/api/templates";
import { formatTemplate } from "@/helpers/displayUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const templates = ref<Template[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const contentType = computed<string>(() => route.query.contentType?.toString() ?? "");
const count = computed<number>(() => Number(route.query.count) || 10);
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const page = computed<number>(() => Number(route.query.page) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("templates.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const parameters: SearchTemplatesPayload = {
    contentType: contentType.value || undefined,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as TemplateSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchTemplates(parameters);
    if (now === timestamp.value) {
      templates.value = data.results;
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

async function onDelete(template: Template, hideModal: () => void): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await deleteTemplate(template.id);
      hideModal();
      toasts.success("templates.delete.success");
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
    case "contentType":
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
    if (route.name === "TemplateList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                contentType: "",
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
    <h1>{{ t("templates.title.list") }}</h1>
    <div class="my-2">
      <icon-button class="me-1" :disabled="isLoading" icon="fas fa-rotate" :loading="isLoading" text="actions.refresh" @click="refresh()" />
      <icon-button class="ms-1" icon="fas fa-plus" text="actions.create" :to="{ name: 'CreateTemplate' }" variant="success" />
    </div>
    <div class="row">
      <ContentTypeSelect class="col-lg-6" :model-value="contentType" @update:model-value="setQuery('contentType', $event)" />
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
    <template v-if="templates.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("templates.sort.options.UniqueKey") }}</th>
            <th scope="col">{{ t("templates.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("templates.contentType.label") }}</th>
            <th scope="col">{{ t("templates.sort.options.UpdatedOn") }}</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="template in templates" :key="template.id">
            <td>
              <RouterLink :to="{ name: 'TemplateEdit', params: { id: template.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ template.uniqueKey }}
              </RouterLink>
            </td>
            <td>{{ template.displayName ?? "—" }}</td>
            <td>{{ t(`templates.contentType.options.${template.content.type}`) }}</td>
            <td><status-block :actor="template.updatedBy" :date="template.updatedOn" /></td>
            <td>
              <icon-button
                :disabled="isLoading"
                icon="trash"
                text="actions.delete"
                variant="danger"
                data-bs-toggle="modal"
                :data-bs-target="`#deleteModal_${template.id}`"
              />
              <delete-modal
                confirm="templates.delete.confirm"
                :display-name="formatTemplate(template)"
                :id="`deleteModal_${template.id}`"
                :loading="isLoading"
                title="templates.delete.title"
                @ok="onDelete(template, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <app-pagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
    </template>
    <p v-else>{{ t("templates.empty") }}</p>
  </main>
</template>
