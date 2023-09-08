<script setup lang="ts">
import { inject } from "vue";
import { useI18n } from "vue-i18n";

import CreateToken from "@/components/tokens/CreateToken.vue";
import ValidateToken from "@/components/tokens/ValidateToken.vue";
import { handleErrorKey } from "@/inject/App";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

function onError(e: unknown): void {
  handleError(e);
}
</script>

<template>
  <main class="container">
    <h1>{{ t("tokens.title") }}</h1>
    <app-tabs>
      <app-tab active title="actions.create"><CreateToken @error="onError" /></app-tab>
      <app-tab title="actions.validate"><ValidateToken @error="onError" /></app-tab>
    </app-tabs>
  </main>
</template>
