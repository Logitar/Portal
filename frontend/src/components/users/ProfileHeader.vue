<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import ViewSessionsLink from "@/components/sessions/ViewSessionsLink.vue";
import type { User } from "@/types/users";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const router = useRouter();
const { d, t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const addressLines = computed<string[]>(() => props.user.address?.formatted.split("\n") ?? []);

function viewSessions(): void {
  account.setRealm(undefined);
  router.push({
    name: "SessionList",
    query: { user: props.user.id, sort: "UpdatedOn", isDescending: "true", page: "1", count: "10" },
  });
}
</script>

<template>
  <table class="table table-striped">
    <tbody>
      <tr v-if="user.fullName">
        <th scope="row">{{ t("users.name.full") }}</th>
        <td>{{ user.fullName }}</td>
      </tr>
      <tr v-if="user.email">
        <th scope="row">{{ t("users.email.address.label") }}</th>
        <td>
          {{ user.email.address }}
          <app-badge v-if="user.email.isVerified">{{ t("users.email.verified") }}</app-badge>
        </td>
      </tr>
      <tr v-if="user.phone">
        <th scope="row">{{ t("users.phone.e164") }}</th>
        <td>
          {{ user.phone.e164Formatted }}
          <app-badge v-if="user.phone.isVerified">{{ t("users.phone.verified") }}</app-badge>
        </td>
      </tr>
      <tr v-if="user.address">
        <th scope="row">{{ t("users.address.title") }}</th>
        <td>
          <template v-for="(line, index) in addressLines" :key="index"><br v-if="index > 0" />{{ line }}</template>
          <template v-if="user.address.isVerified">
            {{ " " }}
            <app-badge>{{ t("users.address.verified") }}</app-badge>
          </template>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.createdOn") }}</th>
        <td>{{ d(user.createdOn, "medium") }}</td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.updatedOn") }}</th>
        <td>{{ d(user.updatedOn, "medium") }}</td>
      </tr>
      <tr v-if="user.authenticatedOn">
        <th scope="row">{{ t("users.authenticatedOn") }}</th>
        <td>
          {{ d(user.authenticatedOn, "medium") }}
          <br />
          <template v-if="account.currentRealm">
            <icon-button class="me-1" icon="fas fa-chess-rook" text="sessions.view.label" variant="warning" @click="viewSessions" />
            <i class="ms-1 text-warning"><font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("sessions.view.warning") }}</i>
          </template>
          <ViewSessionsLink v-else :user="user" />
        </td>
      </tr>
    </tbody>
  </table>
</template>
