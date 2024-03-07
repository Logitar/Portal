<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";

import type { Role } from "@/types/roles";
import type { SelectOption } from "@/types/components";
import { formatRole } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { listRoles } from "@/api/roles";
import { orderBy } from "@/helpers/arrayUtils";

const handleError = inject(handleErrorKey) as (e: unknown) => void;

const props = withDefaults(
  defineProps<{
    disabled?: boolean;
    disabledWhenEmpty?: boolean;
    exclude?: Role[];
    id?: string;
    label?: string;
    modelValue?: string;
    noLabel?: boolean;
    placeholder?: string;
    required?: boolean;
  }>(),
  {
    disabled: false,
    disabledWhenEmpty: false,
    id: "role",
    label: "roles.select.label",
    noLabel: false,
    placeholder: "roles.select.placeholder",
    required: false,
  },
);

const roles = ref<Role[]>([]);

const excludedIds = computed<Set<string>>(() => new Set(props.exclude?.map(({ id }) => id ?? [])));
const options = computed<SelectOption[]>(() =>
  orderBy(
    roles.value
      .filter(({ id }) => !excludedIds.value.has(id))
      .map(
        (role) =>
          ({
            text: formatRole(role),
            value: role.id,
          }) as SelectOption,
      ),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "role-selected", value: Role | undefined): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedRole = id ? roles.value.find((role) => role.id === id) : undefined;
  emit("role-selected", selectedRole);
}

onMounted(async () => {
  try {
    roles.value = await listRoles();
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <form-select
    :disabled="disabled || (disabledWhenEmpty && !options.length)"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :no-label="noLabel"
    :no-state="!required"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </form-select>
</template>
