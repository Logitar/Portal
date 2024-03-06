<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";

import type { SelectOption } from "@/types/components";
import type { Sender } from "@/types/senders";
import { formatSender } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { orderBy } from "@/helpers/arrayUtils";
import { searchSenders } from "@/api/senders";

const handleError = inject(handleErrorKey) as (e: unknown) => void;

withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    disabled: false,
    id: "sender",
    label: "senders.select.label",
    placeholder: "senders.select.placeholder",
    required: false,
  },
);

const senders = ref<Sender[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    senders.value.map(
      (sender) =>
        ({
          text: formatSender(sender),
          value: sender.id,
        }) as SelectOption,
    ),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "sender-selected", value: Sender | undefined): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedSender = id ? senders.value.find((sender) => sender.id === id) : undefined;
  emit("sender-selected", selectedSender);
}

onMounted(async () => {
  try {
    const search = await searchSenders({});
    senders.value = search.results;
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <form-select
    :disabled="disabled"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :no-state="!required"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
