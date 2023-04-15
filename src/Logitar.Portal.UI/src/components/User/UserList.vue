<template>
  <div>
    <b-container>
      <h1 v-t="'user.title'" />
      <div class="my-2">
        <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
        <icon-button class="mx-1" :href="createUrl" icon="plus" text="actions.create" variant="success" />
      </div>
      <b-row>
        <search-field class="col" v-model="search" />
        <realm-select class="col" v-model="realm" />
        <form-select
          class="col"
          id="isConfirmed"
          label="user.confirmed.label"
          :options="yesNoOptions"
          placeholder="user.confirmed.placeholder"
          v-model="isConfirmed"
        />
      </b-row>
      <b-row>
        <form-select
          class="col"
          id="isDisabled"
          label="user.disabled.label"
          :options="yesNoOptions"
          placeholder="user.disabled.placeholder"
          v-model="isDisabled"
        />
        <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
        <count-select class="col" v-model="count" />
      </b-row>
      <p v-if="!users.length" v-t="'user.empty'" />
    </b-container>
    <b-container v-if="users.length" fluid>
      <table id="table" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'user.sort.options.Username'" />
            <th scope="col" v-t="'user.sort.options.FullName'" />
            <th scope="col" v-t="'user.sort.options.EmailAddress'" />
            <th scope="col" v-t="'user.sort.options.PhoneE164Formatted'" />
            <th scope="col" v-t="'user.sort.options.PasswordChangedOn'" />
            <th scope="col" v-t="'user.sort.options.SignedInOn'" />
            <th scope="col" v-t="'user.sort.options.UpdatedOn'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>
              <b-link :href="`/users/${user.id}`" class="mx-1"><user-avatar :user="user" /></b-link>
              <b-link :href="`/users/${user.id}`">{{ user.username }}</b-link>
            </td>
            <td v-text="user.fullName || '—'" />
            <td>
              {{ user.email?.address || '—' }}
              <b-badge v-if="user.email?.isVerified" variant="info">{{ $t('user.email.verified') }}</b-badge>
            </td>
            <td>
              {{ user.phone?.e164Formatted || '—' }}
              <b-badge v-if="user.phone?.isVerified" variant="info">{{ $t('user.phone.verified') }}</b-badge>
            </td>
            <td>{{ user.passwordChangedOn ? $d(new Date(user.passwordChangedOn), 'medium') : '—' }}</td>
            <td>{{ user.signedInOn ? $d(new Date(user.signedInOn), 'medium') : '—' }}</td>
            <td><status-cell :actor="user.updatedBy" :date="user.updatedOn" /></td>
            <td>
              <toggle-status :disabled="user.id === current" :user="user" @updated="refresh()" />
              <icon-button
                class="mx-1"
                :disabled="user.id === current"
                icon="trash-alt"
                text="actions.delete"
                variant="danger"
                v-b-modal="`delete_${user.id}`"
              />
              <delete-modal
                confirm="user.delete.confirm"
                :displayName="user.fullName ? `${user.fullName} (${user.username})` : user.username"
                :id="`delete_${user.id}`"
                :loading="loading"
                title="user.delete.title"
                @ok="onDelete(user, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="table" />
    </b-container>
  </div>
</template>

<script>
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import ToggleStatus from './ToggleStatus.vue'
import UserAvatar from './UserAvatar.vue'
import { deleteUser, getUsers } from '@/api/users'
import { getQueryString } from '@/helpers/queryUtils'

export default {
  name: 'UserList',
  components: {
    RealmSelect,
    ToggleStatus,
    UserAvatar
  },
  props: {
    current: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      count: 10,
      desc: false,
      isConfirmed: null,
      isDisabled: null,
      loading: false,
      page: 1,
      realm: null,
      search: null,
      sort: 'Username',
      total: 0,
      users: []
    }
  },
  computed: {
    createUrl() {
      return '/create-user' + getQueryString({ realm: this.realm })
    },
    params() {
      return {
        isConfirmed: this.isConfirmed,
        isDisabled: this.isDisabled,
        realm: this.realm,
        search: this.search,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('user.sort.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    },
    yesNoOptions() {
      return [
        { text: this.$i18n.t('yes'), value: 'true' },
        { text: this.$i18n.t('no'), value: 'false' }
      ]
    }
  },
  methods: {
    async onDelete({ id }, callback = null) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await deleteUser(id)
          refresh = true
          this.toast('success', 'user.delete.success')
          if (typeof callback === 'function') {
            callback()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (refresh) {
          await this.refresh()
        }
      }
      if (callback) {
        callback()
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getUsers(params ?? this.params)
          this.users = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(newValue, oldValue) {
        if (
          newValue?.index &&
          oldValue &&
          (newValue.isConfirmed !== oldValue.isConfirmed ||
            newValue.isDisabled !== oldValue.isDisabled ||
            newValue.realm !== oldValue.realm ||
            newValue.search !== oldValue.search ||
            newValue.count !== oldValue.count)
        ) {
          this.page = 1
          await this.refresh()
        } else {
          await this.refresh(newValue)
        }
      }
    }
  }
}
</script>
