<script setup lang="ts">
import { computed, inject, ref, watch } from "vue";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { SelectOption } from "@/types/components";
import { handleErrorKey } from "@/inject/App";
import { orderBy } from "@/helpers/arrayUtils";
import { searchRoles } from "@/api/roles";

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
    realm?: Realm;
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
  }
);

const roles = ref<Role[]>([]);

const excludedIds = computed<Set<string>>(() => new Set(props.exclude?.map(({ id }) => id ?? [])));
const options = computed<SelectOption[]>(() =>
  orderBy(
    roles.value
      .filter(({ id }) => !excludedIds.value.has(id))
      .map(
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
  (e: "role-selected", value: Role | undefined): void;
}>();

watch(
  () => props.realm,
  async (realm) => {
    try {
      const search = await searchRoles({
        realm: realm?.id,
      });
      roles.value = search.results;
    } catch (e: unknown) {
      handleError(e);
    }
  },
  { immediate: true }
);

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedRole = id ? roles.value.find((role) => role.id === id) : undefined;
  emit("role-selected", selectedRole);
}
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
