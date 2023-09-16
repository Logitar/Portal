<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { Recipient } from "@/types/messages";
import { formatUser } from "@/helpers/displayUtils";

const { t } = useI18n();

defineProps<{
  recipients: Recipient[];
}>();
</script>

<template>
  <section>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("messages.recipients.type.label") }}</th>
          <th scope="col">{{ t("users.email.address.label") }}</th>
          <th scope="col">{{ t("displayName.label") }}</th>
          <th scope="col">{{ t("messages.recipients.associatedUser") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(recipient, index) in recipients" :key="index">
          <td>{{ t(`messages.recipients.type.options.${recipient.type}`) }}</td>
          <td>{{ recipient.user?.email?.address ?? recipient.address }}</td>
          <td>{{ recipient.user?.fullName ?? recipient.displayName }}</td>
          <td>
            <template v-if="recipient.user?.version">
              <RouterLink :to="{ name: 'UserEdit', params: { id: recipient.user.id } }" target="_blank">
                <app-avatar
                  :displayName="recipient.user.fullName || recipient.user.uniqueName"
                  :emailAddress="recipient.user.email?.address"
                  :url="recipient.user.picture"
                />
              </RouterLink>
              {{ " " }}
              <RouterLink :to="{ name: 'UserEdit', params: { id: recipient.user.id } }" target="_blank">
                {{ recipient.user.uniqueName }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
              </RouterLink>
            </template>
            <template v-else-if="recipient.user"><font-awesome-icon icon="fas fa-user-slash" /> {{ formatUser(recipient.user) }}</template>
            <template v-else>{{ "—" }}</template>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>
