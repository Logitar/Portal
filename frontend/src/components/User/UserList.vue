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
            <th scope="col" v-t="'user.username.label'" />
            <th scope="col" v-t="'user.fullName'" />
            <th scope="col" v-t="'user.email.label'" />
            <th scope="col" v-t="'user.phone.label'" />
            <th scope="col" v-t="'user.passwordChangedAt'" />
            <th scope="col" v-t="'user.signedInAt'" />
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>
              <b-link :href="`/users/${user.id}`"><user-avatar :user="user" /> {{ user.username }}</b-link>
            </td>
            <td v-text="user.fullName || '—'" />
            <td>
              {{ user.email || '—' }}
              <b-badge v-if="user.isEmailConfirmed" variant="info">{{ $t('user.email.confirmed') }}</b-badge>
            </td>
            <td>
              {{ user.phoneNumber || '—' }}
              <b-badge v-if="user.isPhoneNumberConfirmed" variant="info">{{ $t('user.phone.confirmed') }}</b-badge>
            </td>
            <td>{{ user.passwordChangedAt ? $d(new Date(user.passwordChangedAt), 'medium') : '—' }}</td>
            <td>{{ user.signedInAt ? $d(new Date(user.signedInAt), 'medium') : '—' }}</td>
            <td>{{ $d(new Date(user.updatedAt), 'medium') }}</td>
            <td>
              <icon-button class="mx-1" v-if="user.isDisabled" icon="unlock" :loading="loading" text="actions.enable" variant="warning" @click="enable(user)" />
              <icon-button class="mx-1" v-else icon="lock" :loading="loading" text="actions.disable" variant="warning" @click="disable(user)" />
              <icon-button class="mx-1" icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`delete_${user.id}`" />
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
import UserAvatar from './UserAvatar.vue'
import { deleteUser, disableUser, enableUser, getUsers } from '@/api/users'
import { getQueryString } from '@/helpers/queryUtils'

export default {
  name: 'UserList',
  components: {
    RealmSelect,
    UserAvatar
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
    async disable({ id }) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await disableUser(id)
          refresh = true
          this.toast('success', 'user.disabled.success')
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (refresh) {
          await this.refresh()
        }
      }
    },
    async enable({ id }) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await enableUser(id)
          refresh = true
          this.toast('success', 'user.enabled')
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (refresh) {
          await this.refresh()
        }
      }
    },
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
