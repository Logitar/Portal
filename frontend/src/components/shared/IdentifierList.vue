<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { CustomIdentifier } from "@/types/customIdentifiers";

const { t } = useI18n();

defineProps<{
  identifiers: CustomIdentifier[];
}>();
</script>

<template>
  <div>
    <table v-if="identifiers.length" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("identifiers.key.label") }}</th>
          <th scope="col">{{ t("identifiers.value.label") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="identifier in identifiers" :key="identifier.key">
          <td>{{ identifier.key }}</td>
          <td>{{ identifier.value }}</td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("identifiers.empty") }}</p>
  </div>
</template>
