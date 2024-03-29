<script setup lang="ts">
import { computed, inject, onMounted, onUnmounted, onUpdated, ref } from "vue";
import { nanoid } from "nanoid";

import type { TabOptions } from "@/types/components";
import { bindTabKey, unbindTabKey } from "@/inject/AppTabs";

const bindTab: ((tab: TabOptions) => void) | undefined = inject(bindTabKey);
const unbindTab: ((tab: TabOptions) => void) | undefined = inject(unbindTabKey);

const props = withDefaults(
  defineProps<{
    active?: boolean;
    disabled?: boolean;
    title: string;
  }>(),
  {
    active: false,
    disabled: false,
  },
);

const id = ref<string>(nanoid());

const classes = computed<string[]>(() => {
  const classes = ["tab-pane", "fade"];
  if (props.active) {
    classes.push("show");
    classes.push("active");
  }
  return classes;
});

const options = computed<TabOptions>(() => ({ active: props.active, disabled: props.disabled, id: id.value, title: props.title }));
onMounted(() => {
  if (bindTab) {
    bindTab(options.value);
  }
});
onUnmounted(() => {
  if (unbindTab) {
    unbindTab(options.value);
  }
});
onUpdated(() => {
  if (bindTab) {
    bindTab(options.value);
  }
});
</script>

<template>
  <div :class="classes" :id="`tab_${id}_pane`" role="tabpanel" :aria-labelledby="`tab_${id}_head`" tabindex="0">
    <slot></slot>
  </div>
</template>
