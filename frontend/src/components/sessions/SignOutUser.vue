<script setup lang="ts">
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import AppModal from "@/components/shared/AppModal.vue";
import type { ToastUtils } from "@/types/components";
import type { User } from "@/types/users";
import { formatUser } from "@/helpers/displayUtils";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { signOut } from "@/api/users";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean;
    user: User;
  }>(),
  {
    disabled: false,
  },
);

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof AppModal> | null>(null);

const isCurrentUser = computed<boolean>(() => props.user.id === account.authenticated?.id);

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

const emit = defineEmits<{
  (e: "signed-out", value: User): void;
}>();
async function onOk(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const user = await signOut(props.user.id);
      if (isCurrentUser.value) {
        account.signOut();
        router.push({ name: "SignIn" });
      } else {
        emit("signed-out", user);
        toasts.success("users.signOut.success");
      }
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
    <icon-button :disabled="disabled" icon="fas fa-arrow-right-from-bracket" :loading="isLoading" text="users.signOut.submit" variant="danger" @click="show" />
    <AppModal :id="`signOutUser_${user.id}`" ref="modalRef" title="users.signOut.title.modal">
      <p>
        {{ t("users.signOut.confirm") }}
        <br />
        <template v-if="isCurrentUser">{{ t("users.signOut.self") }}</template>
        <template v-else-if="user.realm">{{ t("users.signOut.other.realm") }}</template>
        <template v-else>{{ t("users.signOut.other.portal") }}</template>
        <br />
        <span class="text-danger">{{ formatUser(user) }}</span>
      </p>
      <template #footer>
        <icon-button icon="ban" text="actions.cancel" variant="secondary" @click="hide" />
        <icon-button
          :disabled="isLoading"
          icon="fas fa-arrow-right-from-bracket"
          :loading="isLoading"
          text="users.signOut.submit"
          variant="danger"
          @click="onOk"
        />
      </template>
    </AppModal>
  </span>
</template>
