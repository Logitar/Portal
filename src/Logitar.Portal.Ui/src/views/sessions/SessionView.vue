<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import type { Session } from "@/types/sessions";
import { handleErrorKey, registerTooltipsKey } from "@/inject/App";
import { readSession } from "@/api/sessions";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const registerTooltips = inject(registerTooltipsKey) as () => void;
const route = useRoute();
const { t } = useI18n();

const session = ref<Session>();

const additionalInformation = computed<string | undefined>(() => session.value?.customAttributes.find(({ key }) => key === "AdditionalInformation")?.value);
const ipAddress = computed<string | undefined>(() => session.value?.customAttributes.find(({ key }) => key === "IpAddress")?.value);

onMounted(async () => {
  try {
    const id = route.params.id.toString();
    const data = await readSession(id);
    session.value = data;
  } catch (e: unknown) {
    handleError(e);
  }
  registerTooltips();
});
</script>

<template>
  <main class="container">
    <h1>{{ t("sessions.title.view") }}</h1>
    <template v-if="session">
      <status-detail :aggregate="session" />
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">{{ t("users.select.label") }}</th>
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
          </tr>
          <tr>
            <th scope="row">{{ t("sessions.isPersistent.label") }}</th>
            <td>{{ t(session?.isPersistent ? "yes" : "no") }}</td>
          </tr>
          <tr>
            <th scope="row">{{ t("sessions.sort.options.SignedOutOn") }}</th>
            <td>
              <status-block v-if="session.signedOutBy && session.signedOutOn" :actor="session.signedOutBy" :date="session.signedOutOn" />
              <app-badge v-else>{{ t("sessions.isActive.label") }}</app-badge>
            </td>
          </tr>
          <tr v-if="ipAddress">
            <th scope="row">{{ t("sessions.ipAddress") }}</th>
            <td>{{ ipAddress }}</td>
          </tr>
        </tbody>
      </table>
      <template v-if="additionalInformation">
        <h3>{{ t("sessions.additionalInformation") }}</h3>
        <json-viewer :value="JSON.parse(additionalInformation)" :expand-depth="5" copyable boxed />
      </template>
      <!-- TODO(fpion): custom attributes -->
    </template>
  </main>
</template>
