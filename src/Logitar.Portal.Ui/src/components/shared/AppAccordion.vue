<script setup lang="ts">
import { computed, provide, ref, watch } from "vue";

import { bindItemKey, unbindItemKey } from "@/inject/AppAccordion";

const props = withDefaults(
  defineProps<{
    alwaysOpen?: boolean;
    flush?: boolean;
    id?: string;
  }>(),
  {
    alwaysOpen: false,
    flush: false,
    id: "accordion2",
  }
);

const items = ref<Map<string, (value?: string) => void>>(new Map<string, (value?: string) => void>());

const classes = computed<string[]>(() => {
  const classes = ["accordion"];
  if (props.flush) {
    classes.push("accordion-flush");
  }
  return classes;
});

function bindItem(id: string, setParentId: (value?: string) => void): void {
  items.value.set(id, setParentId);
  if (!props.alwaysOpen) {
    setParentId(props.id);
  }
}
function unbindItem(id: string): void {
  items.value.delete(id);
}
provide(bindItemKey, bindItem);
provide(unbindItemKey, unbindItem);

watch(
  () => props.alwaysOpen,
  (alwaysOpen) => {
    if (alwaysOpen) {
      for (const setParentId of items.value.values()) {
        setParentId(undefined);
      }
    } else {
      for (const setParentId of items.value.values()) {
        setParentId(props.id);
      }
    }
  }
);
watch(
  () => props.id,
  (id) => {
    if (!props.alwaysOpen) {
      for (const setParentId of items.value.values()) {
        setParentId(id);
      }
    }
  }
);

// TODO(fpion): not working when always-open is false (#335)
</script>

<template>
  <div :class="classes" :id="id">
    <slot></slot>
  </div>
</template>
