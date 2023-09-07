<script setup lang="ts">
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import AppModal from "@/components/shared/AppModal.vue";
import type { Session } from "@/types/sessions";
import type { ToastUtils } from "@/types/components";
import { handleErrorKey, toastsKey } from "@/inject/App";
import { signOut } from "@/api/sessions";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const toasts = inject(toastsKey) as ToastUtils;
const { d, t } = useI18n();

const props = defineProps<{
  session: Session;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof AppModal> | null>(null);

const isCurrentSession = computed<boolean>(() => props.session.id === account.currentSession?.id);

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

const emit = defineEmits<{
  (e: "signed-out", value: Session): void;
}>();
async function executeSignOut(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const session = await signOut(props.session.id);
      if (isCurrentSession.value) {
        account.signOut();
        router.push({ name: "SignIn" });
      } else {
        emit("signed-out", session);
        toasts.success("sessions.signOut.success");
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
      hide();
    }
  }
}
function onSignOut(): void {
  if (isCurrentSession.value) {
    show();
  } else {
    executeSignOut();
  }
}
</script>

<template>
  <span>
    <icon-button
      :disabled="!session.isActive"
      icon="fas fa-arrow-right-from-bracket"
      :loading="isLoading"
      text="sessions.signOut.submit"
      :variant="isCurrentSession ? 'danger' : 'warning'"
      @click="onSignOut"
    />
    <AppModal :id="`signOutSession_${props.session.id}`" ref="modalRef" title="sessions.signOut.title">
      <p>
        {{ t("sessions.signOut.confirm") }}
        <br />
        <span class="text-danger">{{ d(session.updatedOn, "medium") }}</span>
      </p>
      <template #footer>
        <icon-button icon="ban" text="actions.cancel" variant="secondary" @click="hide" />
        <icon-button
          :disabled="isLoading"
          icon="fas fa-arrow-right-from-bracket"
          :loading="isLoading"
          text="sessions.signOut.submit"
          variant="danger"
          @click="executeSignOut"
        />
      </template>
    </AppModal>
  </span>
</template>
