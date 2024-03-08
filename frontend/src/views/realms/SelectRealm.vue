<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { Realm } from "@/types/realms";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const router = useRouter();
const { t } = useI18n();

const realm = ref<string>(account.currentRealm?.id ?? "");
const selectedRealm = ref<Realm | undefined>(account.currentRealm);

const isSameRealm = computed<boolean>(() => account.currentRealm?.id === selectedRealm.value?.id);

function submit(): void {
  account.setRealm(selectedRealm.value);
  if (selectedRealm.value) {
    router.push({ name: "RealmEdit", params: { uniqueSlug: selectedRealm.value.uniqueSlug } });
  } else {
    router.push({ name: "Dashboard" });
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("realms.current.title") }}</h1>
    <p>
      <font-awesome-icon icon="info-circle" /> <i>{{ t("realms.current.help") }}</i>
    </p>
    <form @submit.prevent="submit">
      <RealmSelect v-model="realm" @realm-selected="selectedRealm = $event" />
      <icon-submit :disabled="isSameRealm" icon="fas fa-chess-rook" text="realms.current.submit" />
    </form>
  </main>
</template>
