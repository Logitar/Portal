<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import type { Realm } from "@/types/realms";
import type { SelectOption } from "@/types/components";
import type { User } from "@/types/users";
import { handleErrorKey } from "@/inject/App";
import { orderBy } from "@/helpers/arrayUtils";
import { searchUsers } from "@/api/users";

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
    id: "user",
    label: "users.select.label",
    placeholder: "users.select.placeholder",
    required: false,
  }
);

const users = ref<User[]>([]);
const options = computed<SelectOption[]>(() =>
  orderBy(
    users.value.map(
      ({ id, uniqueName, fullName }) =>
        ({
          text: fullName ? `${fullName} (${uniqueName})` : uniqueName,
          value: id,
        } as SelectOption)
    ),
    "text"
  )
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "user-selected", value: User | undefined): void;
}>();

watch(
  () => props.realm,
  async (realm) => {
    try {
      const search = await searchUsers({
        realm: realm?.id,
      });
      users.value = search.results;
    } catch (e: unknown) {
      handleError(e);
    }
  },
  { immediate: true }
);

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedUser = id ? users.value.find((user) => user.id === id) : undefined;
  emit("user-selected", selectedUser);
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
