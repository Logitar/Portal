<script setup lang="ts">
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import AppModal from "@/components/shared/AppModal.vue";
import type { ToastUtils } from "@/types/components";
import type { User } from "@/types/users";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { updateUser } from "@/api/users";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof AppModal> | null>(null);

const action = computed<string>(() => (props.user.isDisabled ? "enable" : "disable"));
const confirm = computed<string>(() => `users.${action.value}.confirm`);
const displayName = computed<string>(() => (props.user.fullName ? `${props.user.fullName} (${props.user.uniqueName})` : props.user.uniqueName));
const icon = computed<string>(() => `fas fa-${props.user.isDisabled ? "lock-open" : "lock"}`);
const isCurrentUser = computed<boolean>(() => props.user.id === account.authenticated?.id);
const text = computed<string>(() => `users.${action.value}.submit`);

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

const emit = defineEmits<{
  (e: "user-updated", value: User): void;
}>();
async function onOk(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const user = await updateUser(props.user.id, {
        isDisabled: !props.user.isDisabled,
      });
      toasts.success(`users.${action.value}.success`);
      emit("user-updated", user);
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
      hide();
    }
  }
}
</script>

<template>
  <span>
    <icon-button :disabled="isCurrentUser" :icon="icon" :text="text" variant="warning" @click="show" />
    <AppModal :id="`toggleUserStatus_${props.user.id}`" ref="modalRef" :title="`users.${action}.title`">
      <p>
        {{ t(confirm) }}
        <br />
        <span class="text-warning">{{ displayName }}</span>
      </p>
      <template #footer>
        <icon-button icon="ban" text="actions.cancel" variant="secondary" @click="hide" />
        <icon-button :disabled="isLoading" :icon="icon" :loading="isLoading" :text="text" variant="warning" @click="onOk" />
      </template>
    </AppModal>
  </span>
</template>
