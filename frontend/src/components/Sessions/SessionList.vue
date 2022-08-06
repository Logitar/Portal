<template>
  <b-container>
    <h1 v-t="'user.session.title'" />
    <div class="my-2">
      <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <!-- TODO(fpion): Sign out all? -->
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
      <!-- TODO(fpion): UserSelect -->
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
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" v-t="'user.session.user'" />
            <th scope="col" v-t="'user.session.signedOutAt'" />
            <th scope="col" v-t="'user.session.persistent.label'" />
            <th scope="col" v-t="'user.session.ipAddress'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="sessionItem in sessions" :key="sessionItem.id">
            <td>
              <b-link :href="`/sessions/${sessionItem.id}`">{{ $d(new Date(sessionItem.updatedAt), 'medium') }}</b-link>
            </td>
            <td>
              <b-link :href="`/users/${sessionItem.user.id}`" target="_blank">
                <user-avatar :user="sessionItem.user" /> {{ sessionItem.user.fullName || sessionItem.user.username }}
                <font-awesome-icon icon="external-link-alt" />
              </b-link>
            </td>
            <td>
              <template v-if="sessionItem.signedOutAt">{{ $d(new Date(sessionItem.signedOutAt), 'medium') }}</template>
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
import SignOutModal from '@/components/Sessions/SignOutModal.vue'
import UserAvatar from '@/components/User/UserAvatar.vue'
import { getSessions, signOut } from '@/api/sessions'

export default {
  name: 'SessionList',
  components: {
    RealmSelect,
    SignOutModal,
    UserAvatar
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
      sort: 'UpdatedAt',
      sessions: [],
      total: 0,
      userId: null
    }
  },
  computed: {
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
        } else {
          await this.refresh(newValue)
        }
      }
    }
  }
}
</script>
