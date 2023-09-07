<script setup lang="ts">
import { useI18n } from "vue-i18n";
import type { Identifier } from "@/types/identifier";

const { t } = useI18n();

defineProps<{
  identifiers: Identifier[];
}>();
</script>

<template>
  <div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("identifiers.key.label") }}</th>
          <th scope="col">{{ t("identifiers.value.label") }}</th>
          <th scope="col">{{ t("identifiers.updatedOn") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="identifier in identifiers" :key="identifier.id">
          <td>{{ identifier.key }}</td>
          <td>{{ identifier.value }}</td>
          <td>
            <status-block :actor="identifier.updatedBy" :date="identifier.updatedOn" />
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
