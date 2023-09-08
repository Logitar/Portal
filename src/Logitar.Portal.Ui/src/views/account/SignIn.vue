<script setup lang="ts">
import { inject, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import PasswordInput from "@/components/users/PasswordInput.vue";
import type { ApiError, ErrorDetail } from "@/types/api";
import { handleErrorKey } from "@/inject/App";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const invalidCredentials = ref<boolean>(false);
const password = ref<string>("");
const passwordRef = ref<InstanceType<typeof PasswordInput> | null>(null);
const remember = ref<boolean>(false);
const username = ref<string>("");

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    invalidCredentials.value = false;
    const session = await signIn({ uniqueName: username.value, password: password.value, isPersistent: remember.value });
    account.signIn(session);
    const redirect: string | undefined = route.query.redirect?.toString();
    router.push(redirect ?? { name: "Profile" });
  } catch (e: unknown) {
    const { data, status } = e as ApiError;
    if (status === 400 && (data as ErrorDetail)?.errorCode === "InvalidCredentials") {
      invalidCredentials.value = true;
      password.value = "";
      passwordRef.value?.focus();
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <h1>{{ t("users.signIn.title") }}</h1>
    <app-alert dismissible variant="warning" v-model="invalidCredentials">
      <strong>{{ t("users.signIn.failed") }}</strong> {{ t("users.signIn.invalidCredentials") }}
    </app-alert>
    <form @submit.prevent="onSubmit">
      <unique-name-input label="users.name.user.label" placeholder="users.usernameOrEmailPlaceholder" required v-model="username" />
      <PasswordInput ref="passwordRef" required v-model="password" />
      <form-checkbox class="mb-3" id="remember" label="users.signIn.remember" v-model="remember" />
      <icon-submit class="me-2" :disabled="isSubmitting" icon="fas fa-arrow-right-to-bracket" :loading="isSubmitting" text="users.signIn.submit" />
    </form>
  </main>
</template>
