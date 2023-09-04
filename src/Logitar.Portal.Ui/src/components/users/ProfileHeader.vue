<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import type { User } from "@/types/users";

const { d, t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const addressLines = computed<string[]>(() => props.user.address?.formatted.split("\n") ?? []);
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
        <td>{{ d(user.authenticatedOn, "medium") }}</td>
      </tr>
    </tbody>
  </table>
</template>
