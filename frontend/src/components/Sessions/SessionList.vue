<template>
  <b-container>
    <h1 v-t="'user.session.title'" />
    <div class="my-2">
      <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <template v-if="selectedUser">
        <icon-button :disabled="sessions.every(s => !s.isActive)" icon="sign-out-alt" text="user.session.signOutAll" variant="danger" v-b-modal.signOutAll />
        <sign-out-all-modal :isCurrent="isCurrent" :loading="loading" :user="selectedUser" @ok="signOutAll" />
      </template>
    </div>
    <b-row>
      <form-select
        class="col"
        id="isActive"
        label="user.session.active.label"
        :options="yesNoOptions"
        placeholder="user.session.active.placeholder"
        v-model="isActive"
      />
      <realm-select class="col" v-model="selectedRealm" />
      <user-select class="col" :realm="selectedRealm" v-model="userId" />
    </b-row>
    <b-row>
      <form-select
        class="col"
        id="isPersistent"
        label="user.session.persistent.label"
        :options="yesNoOptions"
        placeholder="user.session.persistent.placeholder"
        v-model="isPersistent"
      />
      <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
      <count-select class="col" v-model="count" />
    </b-row>
    <template v-if="sessions.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'updated'" />
            <th scope="col" v-t="'user.session.user'" />
            <th scope="col" v-t="'user.session.signedOutOn'" />
            <th scope="col" v-t="'user.session.persistent.label'" />
            <th scope="col" v-t="'user.session.ipAddress'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="sessionItem in sessions" :key="sessionItem.id">
            <td>
              <b-link :href="`/sessions/${sessionItem.id}`">{{ $d(new Date(sessionItem.updatedOn || sessionItem.createdOn), 'medium') }}</b-link>
            </td>
            <td>
              <b-link :href="`/users/${sessionItem.user.id}`" target="_blank" class="mx-1"><user-avatar :user="sessionItem.user" /></b-link>
              <b-link :href="`/users/${sessionItem.user.id}`" target="_blank"
                >{{ sessionItem.user.fullName || sessionItem.user.username }} <font-awesome-icon icon="external-link-alt"
              /></b-link>
            </td>
            <td>
              <status-cell v-if="sessionItem.signedOutOn" :actor="sessionItem.signedOutBy" :date="sessionItem.signedOutOn" />
              <b-badge v-else-if="sessionItem.isActive" variant="info">{{ $t('user.session.active.label') }}</b-badge>
            </td>
            <td v-text="$t(sessionItem.isPersistent ? 'yes' : 'no')" />
            <td v-text="sessionItem.ipAddress" />
            <td>
              <icon-button
                v-if="sessionItem.id !== session"
                :disabled="loading || !sessionItem.isActive"
                icon="sign-out-alt"
                :loading="loading"
                text="user.session.signOut"
                variant="warning"
                @click="signOut(sessionItem.id, $event)"
              />
              <template v-else>
                <icon-button icon="sign-out-alt" text="user.session.signOut" variant="danger" v-b-modal.signOut />
                <sign-out-modal :loading="loading" :session="sessionItem" @ok="signOut(null, $event)" />
              </template>
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="table" />
    </template>
    <p v-else v-t="'user.session.empty'" />
  </b-container>
</template>

<script>
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import SignOutAllModal from '@/components/Sessions/SignOutAllModal.vue'
import SignOutModal from '@/components/Sessions/SignOutModal.vue'
import UserAvatar from '@/components/User/UserAvatar.vue'
import UserSelect from '@/components/User/UserSelect.vue'
import { getSessions, signOut, signOutAll } from '@/api/sessions'
import { getUser } from '@/api/users'

export default {
  name: 'SessionList',
  components: {
    RealmSelect,
    SignOutAllModal,
    SignOutModal,
    UserAvatar,
    UserSelect
  },
  props: {
    realm: {
      type: String,
      default: ''
    },
    session: {
      type: String,
      required: true
    },
    user: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      count: 10,
      desc: true,
      isActive: null,
      isPersistent: null,
      loading: false,
      page: 1,
      selectedRealm: null,
      selectedUser: null,
      sort: 'UpdatedOn',
      sessions: [],
      total: 0,
      userId: null
    }
  },
  computed: {
    isCurrent() {
      return this.sessions.some(({ id }) => id === this.session)
    },
    params() {
      return {
        isActive: this.isActive,
        isPersistent: this.isPersistent,
        realm: this.selectedRealm,
        userId: this.userId,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('user.session.sort.options')).map(([value, text]) => ({ text, value })),
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
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getSessions(params ?? this.params)
          this.sessions = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    },
    async signOut(id, callback) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await signOut(id ?? this.session)
          if (id) {
            refresh = true
            this.toast('success', 'user.session.success')
            if (typeof callback === 'function') {
              callback()
            }
          } else {
            window.location.replace('/user/sign-in')
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
    },
    async signOutAll(callback) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await signOutAll(this.userId)
          if (this.isCurrent) {
            window.location.replace('/user/sign-in')
          } else {
            refresh = true
            this.toast('success', 'user.session.successAll')
            if (typeof callback === 'function') {
              callback()
            }
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
    }
  },
  created() {
    if (this.realm) {
      this.selectedRealm = this.realm
    }
    if (this.user) {
      this.userId = this.user
    }
    this.refresh()
  },
  watch: {
    params: {
      deep: true,
      async handler(newValue, oldValue) {
        if (newValue?.realm !== oldValue?.realm) {
          this.userId = null
          await this.refresh()
          return
        }

        if (
          newValue?.index &&
          oldValue &&
          (newValue.isActive !== oldValue.isActive ||
            newValue.isPersistent !== oldValue.isPersistent ||
            newValue.realm !== oldValue.realm ||
            newValue.userId !== oldValue.userId ||
            newValue.count !== oldValue.count)
        ) {
          this.page = 1
          await this.refresh()
          return
        }

        await this.refresh(newValue)
      }
    },
    async userId(id) {
      if (id) {
        try {
          const { data } = await getUser(id)
          this.selectedUser = data
        } catch (e) {
          this.handleError(e)
        }
      } else {
        this.selectedUser = null
      }
    }
  }
}
</script>
