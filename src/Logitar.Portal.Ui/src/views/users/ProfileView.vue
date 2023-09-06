<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import AuthenticationInformation from "@/components/users/AuthenticationInformation.vue";
import ContactInformation from "@/components/users/ContactInformation.vue";
import PersonalInformation from "@/components/users/PersonalInformation.vue";
import ProfileHeader from "@/components/users/ProfileHeader.vue";
import type { Configuration } from "@/types/configuration";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { ToastUtils } from "@/types/components";
import type { User, UserUpdatedEvent } from "@/types/users";
import { getProfile } from "@/api/account";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const configuration = ref<Configuration>();
const user = ref<User>();

const passwordSettings = computed<PasswordSettings | undefined>(() => user.value?.realm?.passwordSettings ?? configuration.value?.passwordSettings);
const uniqueNameSettings = computed<UniqueNameSettings | undefined>(() => user.value?.realm?.uniqueNameSettings ?? configuration.value?.uniqueNameSettings);

function onUserUpdated(event: UserUpdatedEvent): void {
  user.value = event.user;
  account.signIn(event.user);
  toasts.success(event.toast ?? "users.profile.updated");
}

onMounted(async () => {
  try {
    user.value = await getProfile();
    if (!user.value.realm) {
      configuration.value = await readConfiguration();
    }
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <h1>{{ t("users.profile.title") }}</h1>
    <template v-if="user">
      <ProfileHeader :user="user" />
      <app-tabs>
        <app-tab active title="users.tabs.personal">
          <PersonalInformation is-profile :user="user" @user-updated="onUserUpdated" />
        </app-tab>
        <app-tab title="users.tabs.contact">
          <ContactInformation is-profile :user="user" @user-updated="onUserUpdated" />
        </app-tab>
        <app-tab title="users.tabs.authentication">
          <AuthenticationInformation
            is-profile
            :password-settings="passwordSettings"
            :unique-name-settings="uniqueNameSettings"
            :user="user"
            @user-updated="onUserUpdated"
          />
        </app-tab>
      </app-tabs>
    </template>
  </main>
</template>
