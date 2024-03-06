<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";

import type { Realm } from "@/types/realms";
import type { SelectOption } from "@/types/components";
import { formatRealm } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { orderBy } from "@/helpers/arrayUtils";
import { searchRealms } from "@/api/realms";

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
    id: "realm",
    label: "realms.select.label",
    placeholder: "realms.select.placeholder",
    required: false,
  },
);

const realms = ref<Realm[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    realms.value.map(
      (realm) =>
        ({
          text: formatRealm(realm),
          value: realm.id,
        }) as SelectOption,
    ),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "realm-selected", value: Realm | undefined): void;
}>();

onMounted(async () => {
  try {
    const results = await searchRealms({});
    realms.value = results.items;
  } catch (e: unknown) {
    handleError(e);
  }
});

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedRealm = id ? realms.value.find((realm) => realm.id === id) : undefined;
  emit("realm-selected", selectedRealm);
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
