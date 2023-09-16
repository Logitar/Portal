<script setup lang="ts">
import { inject, onMounted, onUnmounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { v4 as uuidv4 } from "uuid";

import { bindItemKey, unbindItemKey } from "@/inject/AppAccordion";

const bindItem: ((id: string, setParentId: (value?: string) => void) => void) | undefined = inject(bindItemKey);
const unbindItem: ((id: string) => void) | undefined = inject(unbindItemKey);
const { t } = useI18n();

withDefaults(
  defineProps<{
    active?: boolean;
    title: string;
  }>(),
  {
    active: false,
  }
);

const id = ref<string>(uuidv4());
const parent = ref<string>();

function setParentId(value?: string): void {
  parent.value = value ? `#${value}` : undefined;
}
onMounted(() => {
  if (bindItem) {
    bindItem(id.value, setParentId);
  }
});
onUnmounted(() => {
  if (unbindItem) {
    unbindItem(id.value);
  }
});
</script>

<template>
  <div class="accordion-item">
    <h2 class="accordion-header" :id="`heading_${id}`">
      <button
        :class="{ 'accordion-button': true, collapsed: !active }"
        type="button"
        data-bs-toggle="collapse"
        :data-bs-target="`#collapse_${id}`"
        aria-expanded="true"
        :aria-controls="`collapse_${id}`"
      >
        {{ t(title) }}
      </button>
    </h2>
    <div
      :id="`collapse_${id}`"
      :class="{ 'accordion-collapse': true, collapse: true, show: active }"
      :aria-labelledby="`heading_${id}`"
      :data-bs-parent="parent"
    >
      <div class="accordion-body">
        <slot></slot>
      </div>
    </div>
  </div>
</template>
