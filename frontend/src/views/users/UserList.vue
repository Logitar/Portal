<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ToggleUserStatus from "@/components/users/ToggleUserStatus.vue";
import type { SearchUsersPayload, User, UserSort } from "@/types/users";
import type { SelectOption, ToastUtils } from "@/types/components";
import { deleteUser, searchUsers } from "@/api/users";
import { formatUser } from "@/helpers/displayUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { d, rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);
const users = ref<User[]>([]);

const count = computed<number>(() => Number(route.query.count) || 10);
const isConfirmed = computed<boolean | undefined>(() => (route.query.isConfirmed ? route.query.isConfirmed === "true" : undefined));
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const isDisabled = computed<boolean | undefined>(() => (route.query.isDisabled ? route.query.isDisabled === "true" : undefined));
const page = computed<number>(() => Number(route.query.page) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("users.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const parameters: SearchUsersPayload = {
    isConfirmed: isConfirmed.value,
    isDisabled: isDisabled.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as UserSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchUsers(parameters);
    if (now === timestamp.value) {
      users.value = data.results;
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

async function onDelete(user: User, hideModal: () => void): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await deleteUser(user.id);
      hideModal();
      toasts.success("users.delete.success");
    } catch (e: unknown) {
      handleError(e);
      return;
    } finally {
      isLoading.value = false;
    }
    await refresh();
  }
}

function onUserUpdated(user: User): void {
  const index = users.value.findIndex(({ id }) => id === user.id);
  if (index >= 0) {
    users.value.splice(index, 1, user);
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "isConfirmed":
    case "isDisabled":
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
    if (route.name === "UserList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                isConfirmed: "",
                isDisabled: "",
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
  <main>
    <div class="container">
      <h1>{{ t("users.title.list") }}</h1>
      <div class="my-2">
        <icon-button class="me-1" :disabled="isLoading" icon="fas fa-rotate" :loading="isLoading" text="actions.refresh" @click="refresh()" />
        <icon-button class="ms-1" icon="fas fa-plus" text="actions.create" :to="{ name: 'CreateUser' }" variant="success" />
      </div>
      <div class="row">
        <yes-no-select
          class="col-lg-6"
          id="isConfirmed"
          label="users.isConfirmed.label"
          placeholder="users.isConfirmed.placeholder"
          :model-value="isConfirmed?.toString()"
          @update:model-value="setQuery('isConfirmed', $event)"
        />
        <yes-no-select
          class="col-lg-6"
          id="isDisabled"
          label="users.isDisabled.label"
          placeholder="users.isDisabled.placeholder"
          :model-value="isDisabled?.toString()"
          @update:model-value="setQuery('isDisabled', $event)"
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
      <p v-show="users.length === 0">{{ t("users.empty") }}</p>
    </div>
    <div v-if="users.length" class="container-fluid">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("users.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("users.sort.options.FullName") }}</th>
            <th scope="col">{{ t("users.tabs.contact") }}</th>
            <th scope="col">{{ t("users.sort.options.AuthenticatedOn") }}</th>
            <th scope="col">{{ t("users.sort.options.UpdatedOn") }}</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>
              <RouterLink :to="{ name: 'UserEdit', params: { id: user.id } }">
                <app-avatar :display-name="user.fullName ?? user.uniqueName" :email-address="user.email?.address" :url="user.picture" />
              </RouterLink>
              {{ " " }}
              <RouterLink :to="{ name: 'UserEdit', params: { id: user.id } }"><font-awesome-icon icon="fas fa-edit" />{{ user.uniqueName }}</RouterLink>
            </td>
            <td>{{ user.fullName ?? "—" }}</td>
            <td>
              <span v-if="user.email">
                <font-awesome-icon icon="fas fa-at" /> {{ user.email.address }}
                <app-badge v-if="user.email.isVerified">{{ t("users.email.verified") }}</app-badge>
              </span>
              <br v-if="user.email && user.phone" />
              <span v-if="user.phone">
                <font-awesome-icon icon="fas fa-phone" /> {{ user.phone.e164Formatted }}
                <app-badge v-if="user.phone.isVerified">{{ t("users.phone.verified") }}</app-badge>
              </span>
              <span v-if="!user.email && !user.phone">{{ "—" }}</span>
            </td>
            <td>{{ user.authenticatedOn ? d(user.authenticatedOn, "medium") : "—" }}</td>
            <td><status-block :actor="user.updatedBy" :date="user.updatedOn" /></td>
            <td>
              <ToggleUserStatus class="me-1" :disabled="account.authenticated?.id === user.id" :user="user" @user-updated="onUserUpdated" />
              <icon-button
                class="ms-1"
                :disabled="isLoading || account.authenticated?.id === user.id"
                icon="trash"
                text="actions.delete"
                variant="danger"
                data-bs-toggle="modal"
                :data-bs-target="`#deleteModal_${user.id}`"
              />
              <delete-modal
                confirm="users.delete.confirm"
                :display-name="formatUser(user)"
                :id="`deleteModal_${user.id}`"
                :loading="isLoading"
                title="users.delete.title"
                @ok="onDelete(user, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <app-pagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
    </div>
  </main>
</template>
