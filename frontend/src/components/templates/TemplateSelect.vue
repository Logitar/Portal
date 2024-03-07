<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";

import type { SelectOption } from "@/types/components";
import type { Template } from "@/types/templates";
import { formatTemplate } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { orderBy } from "@/helpers/arrayUtils";
import { searchTemplates } from "@/api/templates";

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
    id: "template",
    label: "templates.select.label",
    placeholder: "templates.select.placeholder",
    required: false,
  },
);

const templates = ref<Template[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    templates.value.map(
      (template) =>
        ({
          text: formatTemplate(template),
          value: template.id,
        }) as SelectOption,
    ),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "template-selected", value: Template | undefined): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedTemplate = id ? templates.value.find((template) => template.id === id) : undefined;
  emit("template-selected", selectedTemplate);
}

onMounted(async () => {
  try {
    const results = await searchTemplates({});
    templates.value = results.items;
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
