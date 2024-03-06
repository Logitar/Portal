<script setup lang="ts">
import type { Aggregate } from "@/types/aggregate";

defineProps<{
  aggregate: Aggregate;
}>();
</script>

<template>
  <p>
    <status-info :actor="aggregate.createdBy" :date="aggregate.createdOn" format="status.createdOn" />
    <br />
    <status-info :actor="aggregate.updatedBy" :date="aggregate.updatedOn" format="status.updatedOn" />
  </p>
</template>
