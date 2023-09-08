<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import RealmSelect from "@/components/realms/RealmSelect.vue";
import SignOutSession from "@/components/sessions/SignOutSession.vue";
import SignOutUser from "@/components/sessions/SignOutUser.vue";
import UserSelect from "@/components/users/UserSelect.vue";
import type { ApiError } from "@/types/api";
import type { Realm } from "@/types/realms";
import type { SelectOption } from "@/types/components";
import type { Session, SessionSort, SearchSessionsPayload } from "@/types/sessions";
import type { User } from "@/types/users";
import { handleErrorKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { readRealm } from "@/api/realms";
import { readUser } from "@/api/users";
import { searchSessions } from "@/api/sessions";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { d, rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const selectedRealm = ref<Realm>();
const selectedUser = ref<User>();
const sessions = ref<Session[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => Number(route.query.count) || 10);
const isActive = computed<boolean | undefined>(() => (route.query.isActive ? route.query.isActive === "true" : undefined));
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const isPersistent = computed<boolean | undefined>(() => (route.query.isPersistent ? route.query.isPersistent === "true" : undefined));
const page = computed<number>(() => Number(route.query.page) || 1);
const realm = computed<string>(() => route.query.realm?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const user = computed<string>(() => route.query.user?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("sessions.sort.options"))).map(([value, text]) => ({ text, value } as SelectOption)),
    "text"
  )
);

function onSessionSignedOut(session: Session): void {
  const index = sessions.value.findIndex(({ id }) => id === session.id);
  if (index >= 0) {
    sessions.value.splice(index, 1, session);
  }
}
function onUserSignedOut(): void {
  refresh();
}

async function refresh(): Promise<void> {
  const parameters: SearchSessionsPayload = {
    realm: realm.value || undefined,
    userId: user.value || undefined,
    isActive: isActive.value,
    isPersistent: isPersistent.value,
    sort: sort.value ? [{ field: sort.value as SessionSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchSessions(parameters);
    if (now === timestamp.value) {
      sessions.value = data.results;
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

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "isActive":
    case "isPersistent":
    case "user":
    case "count":
      query.page = "1";
      break;
    case "realm":
      query.page = "1";
      query.user = "";
      break;
  }
  router.replace({ ...route, query });
}

function onRealmSelected(realm?: Realm): void {
  selectedRealm.value = realm;
  selectedUser.value = undefined;
  setQuery("realm", realm?.id ?? "");
}
function onUserSelected(user?: User): void {
  selectedUser.value = user;
  setQuery("user", user?.id ?? "");
}

watch(
  () => route,
  (route) => {
    if (route.name === "SessionList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                realm: "",
                user: "",
                isActive: "",
                isPersistent: "",
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

onMounted(async () => {
  try {
    const realm = route.query.realm?.toString();
    const user = route.query.user?.toString();
    if (user) {
      const foundUser = await readUser(user);
      selectedUser.value = foundUser;
      selectedRealm.value = foundUser.realm;
    } else if (realm) {
      selectedRealm.value = await readRealm(realm);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main>
    <div class="container">
      <h1>{{ t("sessions.title.list") }}</h1>
      <div class="my-2">
        <icon-button class="me-1" :disabled="isLoading" icon="fas fa-rotate" :loading="isLoading" text="actions.refresh" @click="refresh()" />
        <SignOutUser
          v-if="selectedUser"
          class="ms-1"
          :disabled="sessions.every(({ isActive }) => !isActive)"
          :user="selectedUser"
          @signed-out="onUserSignedOut"
        />
      </div>
      <div class="row">
        <RealmSelect class="col-lg-4" :model-value="realm" @realm-selected="onRealmSelected" />
        <UserSelect class="col-lg-4" :model-value="user" :realm="selectedRealm" @user-selected="onUserSelected" />
        <yes-no-select
          class="col-lg-4"
          id="isPersistent"
          label="sessions.isPersistent.label"
          placeholder="sessions.isPersistent.placeholder"
          :model-value="isPersistent?.toString()"
          @update:model-value="setQuery('isPersistent', $event)"
        />
      </div>
      <div class="row">
        <yes-no-select
          class="col-lg-4"
          id="isActive"
          label="sessions.isActive.label"
          placeholder="sessions.isActive.placeholder"
          :model-value="isActive?.toString()"
          @update:model-value="setQuery('isActive', $event)"
        />
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
      <template v-if="sessions.length">
        <table class="table table-striped">
          <thead>
            <tr>
              <th scope="col">{{ t("sessions.sort.options.UpdatedOn") }}</th>
              <th scope="col">{{ t("users.select.label") }}</th>
              <th scope="col">{{ t("sessions.sort.options.SignedOutOn") }}</th>
              <th scope="col">{{ t("sessions.isPersistent.label") }}</th>
              <th scope="col"></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="session in sessions" :key="session.id">
              <td>
                <RouterLink :to="{ name: 'SessionEdit', params: { id: session.id } }">{{ d(session.updatedOn, "medium") }}</RouterLink>
              </td>
              <td>
                <RouterLink :to="{ name: 'UserEdit', params: { id: session.user.id } }" target="_blank">
                  <app-avatar
                    :display-name="session.user.fullName ?? session.user.uniqueName"
                    :email-address="session.user.email?.address"
                    :url="session.user.picture"
                  />
                </RouterLink>
                {{ " " }}
                <RouterLink :to="{ name: 'UserEdit', params: { id: session.user.id } }" target="_blank">
                  {{ session.user.fullName ?? session.user.uniqueName }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
                </RouterLink>
              </td>
              <td>
                <status-block v-if="session.signedOutBy && session.signedOutOn" :actor="session.signedOutBy" :date="session.signedOutOn" />
                <app-badge v-else>{{ t("sessions.isActive.label") }}</app-badge>
              </td>
              <td>{{ t(session.isPersistent ? "yes" : "no") }}</td>
              <td><SignOutSession :session="session" @signed-out="onSessionSignedOut" /></td>
            </tr>
          </tbody>
        </table>
        <app-pagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
      </template>
      <p v-else>{{ t("sessions.empty") }}</p>
    </div>
  </main>
</template>
