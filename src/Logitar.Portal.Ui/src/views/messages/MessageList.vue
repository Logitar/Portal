<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import RealmSelect from "@/components/realms/RealmSelect.vue";
import StatusBadge from "@/components/messages/StatusBadge.vue";
import StatusSelect from "@/components/messages/StatusSelect.vue";
import TemplateSelect from "@/components/templates/TemplateSelect.vue";
import type { Message, MessageSort, MessageStatus, SearchMessagesPayload } from "@/types/messages";
import type { Realm } from "@/types/realms";
import type { SelectOption } from "@/types/components";
import type { Template } from "@/types/templates";
import { formatSender, formatTemplate } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { isEmpty } from "@/helpers/objectUtils";
import { orderBy } from "@/helpers/arrayUtils";
import { searchMessages } from "@/api/messages";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const messages = ref<Message[]>([]);
const selectedRealm = ref<Realm>();
const selectedTemplate = ref<Template>();
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => Number(route.query.count) || 10);
const isDemo = computed<boolean>(() => route.query.isDemo === "true");
const isDescending = computed<boolean>(() => route.query.isDescending === "true");
const page = computed<number>(() => Number(route.query.page) || 1);
const realm = computed<string>(() => route.query.realm?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const status = computed<string>(() => route.query.status?.toString() ?? "");
const template = computed<string>(() => route.query.template?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("messages.sort.options"))).map(([value, text]) => ({ text, value } as SelectOption)),
    "text"
  )
);

async function refresh(): Promise<void> {
  const parameters: SearchMessagesPayload = {
    realm: realm.value || undefined,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    isDemo: isDemo.value,
    status: (status.value as MessageStatus) || undefined,
    template: template.value || undefined,
    sort: sort.value ? [{ field: sort.value as MessageSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const data = await searchMessages(parameters);
    if (now === timestamp.value) {
      messages.value = data.results;
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
    case "isDemo":
    case "search":
    case "status":
    case "template":
    case "count":
      query.page = "1";
      break;
    case "realm":
      query.page = "1";
      query.template = "";
      break;
  }
  router.replace({ ...route, query });
}

function onRealmSelected(realm?: Realm): void {
  selectedRealm.value = realm;
  selectedTemplate.value = undefined;
  setQuery("realm", realm?.id ?? "");
}
function onTemplateSelected(template?: Template): void {
  selectedTemplate.value = template;
  setQuery("template", template?.id ?? "");
}

watch(
  () => route,
  (route) => {
    if (route.name === "MessageList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                realm: "",
                isDemo: "false",
                status: "",
                template: "",
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
  { deep: true, immediate: true }
);
</script>

<template>
  <main>
    <div class="container">
      <h1>{{ t("messages.title.list") }}</h1>
      <div class="my-2">
        <icon-button :disabled="isLoading" icon="fas fa-rotate" :loading="isLoading" text="actions.refresh" @click="refresh()" />
      </div>
      <div class="row">
        <RealmSelect class="col-lg-4" :model-value="realm" @realm-selected="onRealmSelected" />
        <TemplateSelect class="col-lg-4" :model-value="template" :realm="selectedRealm" @template-selected="onTemplateSelected" />
        <StatusSelect
          class="col-lg-4"
          :is-demo="isDemo"
          :model-value="status"
          @is-demo="setQuery('isDemo', $event.toString())"
          @update:model-value="setQuery('status', $event)"
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
        <p v-if="messages.length === 0">{{ t("messages.empty") }}</p>
      </div>
    </div>
    <div v-if="messages.length" class="container-fluid">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("messages.sort.options.Subject") }}</th>
            <th scope="col">{{ t("messages.sort.options.RecipientCount") }}</th>
            <th scope="col">{{ t("senders.select.label") }}</th>
            <th scope="col">{{ t("templates.select.label") }}</th>
            <th scope="col">{{ t("messages.status.label") }}</th>
            <th scope="col">{{ t("messages.sent.on") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="message in messages" :key="message.id">
            <td>
              <RouterLink :to="{ name: 'MessageView', params: { id: message.id } }"><font-awesome-icon icon="fas fa-eye" />{{ message.subject }}</RouterLink>
              {{ " " }}
              <app-badge v-if="!message.isDemo">{{ t("messages.demo.label") }}</app-badge>
            </td>
            <td>{{ message.recipientCount }}</td>
            <td>
              <RouterLink v-if="message.sender.version" :to="{ name: 'SenderEdit', params: { id: message.sender.id } }" target="_blank">
                {{ formatSender(message.sender) }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
              </RouterLink>
              <template v-else>{{ formatSender(message.sender) }}</template>
              <template v-if="message.sender.isDefault">
                {{ " " }}
                <app-badge>{{ t("senders.default.submit") }}</app-badge>
              </template>
            </td>
            <td>
              <RouterLink v-if="message.template.version" :to="{ name: 'TemplateEdit', params: { id: message.template.id } }" target="_blank">
                {{ formatTemplate(message.template) }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
              </RouterLink>
              <template v-else>{{ formatTemplate(message.template) }}</template>
            </td>
            <td>
              <StatusBadge :message="message" />
            </td>
            <td><status-block :actor="message.updatedBy" :date="message.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <app-pagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
    </div>
  </main>
</template>
