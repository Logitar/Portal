<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";

import type { Realm } from "@/types/realms";
import type { SelectOption } from "@/types/components";
import type { Template } from "@/types/templates";
import { handleErrorKey } from "@/inject/App";
import { orderBy } from "@/helpers/arrayUtils";
import { searchTemplates } from "@/api/templates";

const handleError = inject(handleErrorKey) as (e: unknown) => void;

const props = withDefaults(
  defineProps<{
    disabled?: boolean;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    realm?: Realm;
    required?: boolean;
  }>(),
  {
    disabled: false,
    id: "template",
    label: "templates.select.label",
    placeholder: "templates.select.placeholder",
    required: false,
  }
);

const templates = ref<Template[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    templates.value.map(
      ({ id, uniqueName, displayName }) =>
        ({
          text: displayName ? `${displayName} (${uniqueName})` : uniqueName,
          value: id,
        } as SelectOption)
    ),
    "text"
  )
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "template-selected", value: Template | undefined): void;
}>();

watch(
  () => props.realm,
  async (realm) => {
    try {
      const search = await searchTemplates({
        realm: realm?.id,
      });
      templates.value = search.results;
    } catch (e: unknown) {
      handleError(e);
    }
  },
  { immediate: true }
);

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedTemplate = id ? templates.value.find((template) => template.id === id) : undefined;
  emit("template-selected", selectedTemplate);
}
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
