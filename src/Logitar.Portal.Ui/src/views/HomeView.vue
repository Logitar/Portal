<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import PasswordInput from "@/components/users/PasswordInput.vue";
import PersonNameInput from "@/components/users/PersonNameInput.vue";
import UsernameInput from "@/components/users/UsernameInput.vue";
import { handleErrorKey } from "@/inject/App";
import { initializeConfiguration, isConfigurationInitialized } from "@/api/configuration";
import { useAccountStore } from "@/stores/account";

const { locale, t } = useI18n();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();

const account = useAccountStore();

const confirm = ref<string>("");
const emailAddress = ref<string>("");
const firstName = ref<string>("");
const hasLoaded = ref<boolean>(false);
const lastName = ref<string>("");
const password = ref<string>("");
const username = ref<string>("");

function onEmailAddressInput(value: string): void {
  emailAddress.value = value;
  username.value = value;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const user = await initializeConfiguration({
      locale: locale.value,
      user: {
        uniqueName: username.value,
        password: password.value,
        emailAddress: emailAddress.value,
        firstName: firstName.value,
        lastName: lastName.value,
      },
    });
    account.signIn(user);
    router.push({ name: "Configuration", query: { status: "initialized" } });
  } catch (e: unknown) {
    handleError(e);
  }
});

onMounted(async () => {
  try {
    const result = await isConfigurationInitialized();
    if (result.isInitialized) {
      router.push({ name: "SignIn" });
    } else {
      hasLoaded.value = true;
    }
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <div v-show="hasLoaded">
      <h1>{{ t("configuration.initialization.title") }}</h1>
      <h3>{{ t("configuration.initialization.user.label") }}</h3>
      <p>
        <font-awesome-icon icon="info-circle" /> <i>{{ t("configuration.initialization.user.help") }}</i>
      </p>
      <form @submit.prevent="onSubmit">
        <div class="row">
          <EmailAddressInput class="col-lg-6" :model-value="emailAddress" required validate @update:model-value="onEmailAddressInput" />
          <UsernameInput class="col-lg-6" required validate v-model="username" />
        </div>
        <div class="row">
          <PersonNameInput class="col-lg-6" required type="first" validate v-model="firstName" />
          <PersonNameInput class="col-lg-6" required type="last" validate v-model="lastName" />
        </div>
        <div class="row">
          <PasswordInput class="col-lg-6" required validate v-model="password" />
          <PasswordInput
            class="col-lg-6"
            :confirm="{ value: password, label: 'users.password.label' }"
            id="confirm"
            label="users.password.confirm.label"
            placeholder="users.password.confirm.placeholder"
            required
            validate
            v-model="confirm"
          />
        </div>
        <icon-submit class="me-2" :disabled="isSubmitting" icon="fas fa-cog" :loading="isSubmitting" text="configuration.initialization.initialize" />
      </form>
    </div>
  </main>
</template>
