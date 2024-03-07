<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";

import type { SelectOption } from "@/types/components";
import type { User } from "@/types/users";
import { formatUser } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { listUsers } from "@/api/users";
import { orderBy } from "@/helpers/arrayUtils";

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
    id: "user",
    label: "users.select.label",
    placeholder: "users.select.placeholder",
    required: false,
  },
);

const users = ref<User[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    users.value.map(
      (user) =>
        ({
          text: formatUser(user),
          value: user.id,
        }) as SelectOption,
    ),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "user-selected", value: User | undefined): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedUser = id ? users.value.find((user) => user.id === id) : undefined;
  emit("user-selected", selectedUser);
}

onMounted(async () => {
  try {
    users.value = await listUsers();
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
