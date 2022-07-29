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
        <realm-select class="col" v-model="realmId" />
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
              <b-link :href="`/users/${user.id}`">
                <img v-if="user.picture" :src="user.picture" :alt="`${user.username}'s avatar`" class="rounded-circle" width="24" height="24" />
                <v-gravatar v-else-if="user.email" class="rounded-circle" :email="user.email" :size="24" />
                {{ user.username }}
              </b-link>
            </td>
            <td v-text="user.fullName || '—'" />
            <td>
              {{ user.email || '—' }}
              <b-badge v-if="user.emailConfirmed" variant="info">{{ $t('user.email.confirmed') }}</b-badge>
            </td>
            <td>
              {{ user.phoneNumber || '—' }}
              <b-badge v-if="user.phoneNumberConfirmed" variant="info">{{ $t('user.phone.confirmed') }}</b-badge>
            </td>
            <td>{{ user.passwordChangedAt ? $d(new Date(user.passwordChangedAt), 'medium') : '—' }}</td>
            <td>{{ user.signedInAt ? $d(new Date(user.signedInAt), 'medium') : '—' }}</td>
            <td>{{ $d(new Date(user.updatedAt || user.createdAt), 'medium') }}</td>
            <td>
              <icon-button icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`delete_${user.id}`" />
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
import { deleteUser, getUsers } from '@/api/users'
import { getQueryString } from '@/helpers/queryUtils'

export default {
  name: 'UserList',
  components: {
    RealmSelect
  },
  data() {
    return {
      count: 10,
      desc: false,
      loading: false,
      page: 1,
      realmId: null,
      search: null,
      sort: 'Username',
      total: 0,
      users: []
    }
  },
  computed: {
    createUrl() {
      return '/create-user' + getQueryString({ realm: this.realmId })
    },
    params() {
      return {
        realmId: this.realmId,
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
          (newValue.realmId !== oldValue.realmId || newValue.search !== oldValue.search || newValue.count !== oldValue.count)
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
