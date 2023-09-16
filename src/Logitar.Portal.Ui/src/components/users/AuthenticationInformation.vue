<script setup lang="ts">
import { computed, inject, ref, watchEffect } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import PasswordInput from "./PasswordInput.vue";
import type { ApiError, ErrorDetail } from "@/types/api";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { User, UpdateUserPayload, UserUpdatedEvent } from "@/types/users";
import { handleErrorKey } from "@/inject/App";
import { saveProfile } from "@/api/account";
import { updateUser } from "@/api/users";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { d, t } = useI18n();

const props = withDefaults(
  defineProps<{
    isProfile?: boolean;
    passwordSettings?: PasswordSettings;
    uniqueNameSettings?: UniqueNameSettings;
    user: User;
  }>(),
  {
    isProfile: false,
    passwordSettings: () => ({
      requiredLength: 8,
      requiredUniqueChars: 8,
      requireNonAlphanumeric: true,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      strategy: "PBKDF2",
    }),
    uniqueNameSettings: () => ({ allowedCharacters: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+" }),
  }
);

const confirm = ref<string>("");
const current = ref<string>("");
const currentRef = ref<InstanceType<typeof PasswordInput> | null>(null);
const invalidCredentials = ref<boolean>(false);
const password = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(
  () => uniqueName.value !== props.user.uniqueName || Boolean(current.value) || Boolean(password.value) || Boolean(confirm.value)
);
const isPasswordRequired = computed<boolean>(() => Boolean(current.value) || Boolean(password.value) || Boolean(confirm.value));

watchEffect(() => {
  const user = props.user;
  uniqueName.value = user.uniqueName; // TODO(fpion): doesn't seem to work after updating the user (#301)
});

function reset(): void {
  current.value = "";
  password.value = "";
  confirm.value = "";
}

const emit = defineEmits<{
  (e: "user-updated", event: UserUpdatedEvent): void;
}>();
const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async (_, { resetForm }) => {
  invalidCredentials.value = false;
  uniqueNameAlreadyUsed.value = false;
  try {
    if (props.isProfile) {
      const payload: UpdateUserPayload = {
        password: {
          currentPassword: current.value,
          newPassword: password.value,
        },
      };
      const user = await saveProfile(payload);
      emit("user-updated", { user, toast: "users.profile.passwordChanged" });
    } else {
      const payload: UpdateUserPayload = {
        uniqueName: uniqueName.value !== props.user.uniqueName ? uniqueName.value : undefined,
        password: password.value ? { newPassword: password.value } : undefined,
      };
      const user = await updateUser(props.user.id, payload);
      emit("user-updated", { user, toast: payload.uniqueName ? undefined : "users.password.changed" });
    }
    resetForm();
  } catch (e: unknown) {
    reset();
    currentRef.value?.focus();
    const { data, status } = e as ApiError;
    if (status === 400 && (data as ErrorDetail)?.errorCode === "InvalidCredentials") {
      invalidCredentials.value = true;
    } else if (status === 409 && (data as ErrorDetail)?.errorCode === "UniqueNameAlreadyUsed") {
      uniqueNameAlreadyUsed.value = true;
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <form @submit.prevent="onSubmit">
    <div v-if="!isProfile" class="mb-3">
      <icon-submit :disabled="!hasChanges || isSubmitting" icon="fas fa-floppy-disk" :loading="isSubmitting" text="actions.save" />
    </div>
    <app-alert dismissible variant="warning" v-model="uniqueNameAlreadyUsed">
      <strong>{{ t("users.name.user.alreadyUsed.lead") }}</strong> {{ t("users.name.user.alreadyUsed.help") }}
    </app-alert>
    <unique-name-input
      :disabled="isProfile"
      label="users.name.user.label"
      placeholder="users.name.user.placeholder"
      required
      :settings="uniqueNameSettings"
      validate
      v-model="uniqueName"
    />
    <template v-if="!isProfile || user.hasPassword">
      <h5>{{ t("users.password.label") }}</h5>
      <app-alert dismissible variant="warning" v-model="invalidCredentials">
        <strong>{{ t("users.password.changeFailed") }}</strong> {{ t("users.password.invalidCredentials") }}
      </app-alert>
      <p v-if="user.passwordChangedOn">{{ t("users.password.changedOn", { date: d(user.passwordChangedOn, "medium") }) }}</p>
      <PasswordInput
        v-if="isProfile"
        id="current"
        label="users.password.current.label"
        placeholder="users.password.current.placeholder"
        ref="currentRef"
        :required="isPasswordRequired"
        v-model="current"
      />
      <p v-else>
        <i class="text-warning"><font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("users.password.warning") }}</i>
      </p>
      <div class="row">
        <PasswordInput
          class="col-lg-6"
          label="users.password.new.label"
          placeholder="users.password.new.placeholder"
          :required="isPasswordRequired"
          :settings="passwordSettings"
          :validate="isPasswordRequired"
          v-model="password"
        />
        <PasswordInput
          class="col-lg-6"
          :confirm="{ value: password, label: 'users.password.new.label' }"
          id="confirm"
          label="users.password.confirm.label"
          placeholder="users.password.confirm.placeholder"
          :required="isPasswordRequired"
          :validate="isPasswordRequired"
          v-model="confirm"
        />
      </div>
      <div class="mb-3">
        <icon-submit v-if="isProfile" :disabled="!hasChanges || isSubmitting" icon="fas fa-key" :loading="isSubmitting" text="users.password.submit" />
      </div>
    </template>
  </form>
</template>
