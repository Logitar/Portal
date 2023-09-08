<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import RoleSelect from "./RoleSelect.vue";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";

const { t } = useI18n();

withDefaults(
  defineProps<{
    loading?: boolean;
    realm?: Realm;
    roles: Role[];
  }>(),
  {
    loading: false,
  }
);

const selectedRole = ref<Role>();

const emit = defineEmits<{
  (e: "role-added", value: Role): void;
  (e: "role-removed", value: Role): void;
}>();

function addRole(): void {
  if (selectedRole.value) {
    emit("role-added", selectedRole.value);
    selectedRole.value = undefined;
  }
}
function removeRole(role: Role): void {
  emit("role-removed", role);
}
</script>

<template>
  <div>
    <RoleSelect
      disabled-when-empty
      :exclude="roles"
      :model-value="selectedRole?.id"
      no-label
      placeholder="roles.manage.add"
      :realm="realm"
      @role-selected="selectedRole = $event"
    >
      <template #append>
        <icon-button :disabled="!selectedRole || loading" icon="fas fa-plus" :loading="loading" text="actions.add" variant="success" @click="addRole" />
      </template>
    </RoleSelect>
    <table v-if="roles.length" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("roles.sort.options.UniqueName") }}</th>
          <th scope="col">{{ t("roles.sort.options.DisplayName") }}</th>
          <th scope="col">{{ t("roles.sort.options.UpdatedOn") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="role in roles" :key="role.id">
          <td>
            <RouterLink :to="{ name: 'RoleEdit', params: { id: role.id } }" target="_blank">
              {{ role.uniqueName }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
            </RouterLink>
          </td>
          <td>{{ role.displayName ?? "—" }}</td>
          <td><status-block :actor="role.updatedBy" :date="role.updatedOn" /></td>
          <td>
            <icon-button :disabled="loading" icon="times" :loading="loading" text="actions.remove" variant="danger" @click="removeRole(role)" />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("roles.manage.empty") }}</p>
  </div>
</template>
