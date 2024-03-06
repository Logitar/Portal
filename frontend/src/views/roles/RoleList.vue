<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import type { Role, RoleSort, SearchRolesPayload } from "@/types/roles";
import type { SelectOption, ToastUtils } from "@/types/components";
import { deleteRole, searchRoles } from "@/api/roles";
import { formatRole } from "@/helpers/displayUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const roles = ref<Role[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => Number(route.query.count) || 10);
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const page = computed<number>(() => Number(route.query.page) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("roles.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const parameters: SearchRolesPayload = {
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as RoleSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchRoles(parameters);
    if (now === timestamp.value) {
      roles.value = data.results;
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

async function onDelete(role: Role, hideModal: () => void): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await deleteRole(role.id);
      hideModal();
      toasts.success("roles.delete.success");
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
    if (route.name === "RoleList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
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
    <h1>{{ t("roles.title.list") }}</h1>
    <div class="my-2">
      <icon-button class="me-1" :disabled="isLoading" icon="fas fa-rotate" :loading="isLoading" text="actions.refresh" @click="refresh()" />
      <icon-button class="ms-1" icon="fas fa-plus" text="actions.create" :to="{ name: 'CreateRole' }" variant="success" />
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
    <template v-if="roles.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("roles.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("roles.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("roles.sort.options.UpdatedOn") }}</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="role in roles" :key="role.id">
            <td>
              <RouterLink :to="{ name: 'RoleEdit', params: { id: role.id } }"><font-awesome-icon icon="fas fa-edit" />{{ role.uniqueName }}</RouterLink>
            </td>
            <td>{{ role.displayName ?? "—" }}</td>
            <td><status-block :actor="role.updatedBy" :date="role.updatedOn" /></td>
            <td>
              <icon-button
                :disabled="isLoading"
                icon="trash"
                text="actions.delete"
                variant="danger"
                data-bs-toggle="modal"
                :data-bs-target="`#deleteModal_${role.id}`"
              />
              <delete-modal
                confirm="roles.delete.confirm"
                :display-name="formatRole(role)"
                :id="`deleteModal_${role.id}`"
                :loading="isLoading"
                title="roles.delete.title"
                @ok="onDelete(role, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <app-pagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
    </template>
    <p v-else>{{ t("roles.empty") }}</p>
  </main>
</template>
