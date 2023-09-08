<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import SignOutSession from "@/components/sessions/SignOutSession.vue";
import SignOutUser from "@/components/sessions/SignOutUser.vue";
import type { ApiError } from "@/types/api";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Session } from "@/types/sessions";
import type { User } from "@/types/users";
import { handleErrorKey } from "@/inject/App";
import { readSession } from "@/api/sessions";
import { useRoute, useRouter } from "vue-router";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const session = ref<Session>();

const additionalInformation = computed<string | undefined>(() => session.value?.customAttributes.find(({ key }) => key === "AdditionalInformation")?.value);
const customAttributes = computed<CustomAttribute[]>(
  () => session.value?.customAttributes.filter(({ key }) => key !== "AdditionalInformation" && key !== "IpAddress") ?? []
);
const ipAddress = computed<string | undefined>(() => session.value?.customAttributes.find(({ key }) => key === "IpAddress")?.value);

function onSessionSignedOut(signedOut: Session): void {
  session.value = signedOut;
}
function onUserSignedOut(user: User): void {
  if (session.value) {
    session.value.user = user;
  }
}

onMounted(async () => {
  try {
    const id = route.params.id.toString();
    const data = await readSession(id);
    session.value = data;
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
  <main class="container">
    <h1>{{ t("sessions.title.view") }}</h1>
    <template v-if="session">
      <status-detail :aggregate="session" />
      <div class="mb-3">
        <icon-button class="me-1" icon="chevron-left" text="actions.back" variant="secondary" @click="router.back()" />
        <SignOutSession class="mx-1" :session="session" @signed-out="onSessionSignedOut" />
        <SignOutUser class="ms-1" :disabled="!session.isActive" :user="session.user" @signed-out="onUserSignedOut" />
      </div>
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
      <app-tabs>
        <app-tab active title="sessions.additionalInformation.title">
          <json-viewer v-if="additionalInformation" boxed copyable expanded :value="JSON.parse(additionalInformation)" />
          <p v-else>{{ t("sessions.additionalInformation.empty") }}</p>
        </app-tab>
        <app-tab title="customAttributes.title">
          <table v-if="customAttributes.length > 0" class="table table-striped">
            <thead>
              <tr>
                <th scope="col">{{ t("customAttributes.key.label") }}</th>
                <th scope="col">{{ t("customAttributes.value.label") }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="customAttribute in customAttributes" :key="customAttribute.key">
                <td>{{ customAttribute.key }}</td>
                <td>{{ customAttribute.value }}</td>
              </tr>
            </tbody>
          </table>
          <p v-else>{{ t("customAttributes.empty") }}</p>
        </app-tab>
      </app-tabs>
    </template>
  </main>
</template>
