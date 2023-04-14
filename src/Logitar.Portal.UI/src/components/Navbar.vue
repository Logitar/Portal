<template>
  <div>
    <b-navbar toggleable="lg" type="dark" variant="dark">
      <b-navbar-brand href="/">
        <img src="@/assets/logo.png" alt="Portal Logo" height="32" />
        Portal
        <b-badge v-if="environment" variant="warning">{{ environment }}</b-badge>
      </b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav>
          <b-nav-item v-if="environment === 'development'" href="/swagger" target="_blank"><font-awesome-icon icon="vial" /> Swagger</b-nav-item>
          <template v-if="currentUser.isAuthenticated">
            <b-nav-item href="/realms">
              <font-awesome-icon icon="chess-rook" />
              {{ $t('realms.title') }}
            </b-nav-item>
            <!-- <b-nav-item href="/api-keys">
              <font-awesome-icon icon="key" />
              {{ $t('apiKeys.title') }}
            </b-nav-item> -->
            <b-nav-item href="/users">
              <font-awesome-icon icon="users" />
              {{ $t('user.title') }}
            </b-nav-item>
            <b-nav-item href="/sessions">
              <font-awesome-icon icon="user-clock" />
              {{ $t('user.session.title') }}
            </b-nav-item>
            <b-nav-item href="/tokens">
              <font-awesome-icon icon="id-card" />
              {{ $t('tokens.title') }}
            </b-nav-item>
            <b-nav-item href="/senders">
              <font-awesome-icon icon="paper-plane" />
              {{ $t('senders.title') }}
            </b-nav-item>
            <b-nav-item href="/templates">
              <font-awesome-icon icon="at" />
              {{ $t('templates.title') }}
            </b-nav-item>
            <b-nav-item href="/dictionaries">
              <font-awesome-icon icon="language" />
              {{ $t('dictionaries.title') }}
            </b-nav-item>
            <b-nav-item href="/messages">
              <font-awesome-icon icon="envelope" />
              {{ $t('messages.title') }}
            </b-nav-item>
          </template>
        </b-navbar-nav>

        <b-navbar-nav class="ml-auto">
          <!-- <b-nav-form>
            <b-input-group>
              <b-form-input size="sm" :placeholder="$t('actions.search')" />
              <b-input-group-append>
                <icon-button class="my-2 my-sm-0" icon="search" size="sm" />
              </b-input-group-append>
            </b-input-group>
          </b-nav-form> -->

          <!-- <b-nav-item-dropdown v-if="otherLocales.length" :text="localeName" right>
            <b-dropdown-item v-for="locale in otherLocales" :key="locale.value" :active="locale.value === $i18n.locale" @click="translate(locale.value)">
              {{ locale.text }}
            </b-dropdown-item>
          </b-nav-item-dropdown> -->

          <template v-if="currentUser.isAuthenticated">
            <b-nav-item-dropdown right>
              <template #button-content>
                <user-avatar :user="currentUser" :size="24" />
              </template>
              <b-dropdown-item href="/user/profile">
                <font-awesome-icon icon="user" />
                {{ currentUser.fullName || currentUser.username }}
              </b-dropdown-item>
              <b-dropdown-item href="/user/sign-out">
                <font-awesome-icon icon="sign-out-alt" />
                {{ $t('user.signOut') }}
              </b-dropdown-item>
            </b-nav-item-dropdown>
          </template>
          <template v-else>
            <b-nav-item href="/user/sign-in">
              <font-awesome-icon icon="sign-in-alt" />
              {{ $t('user.signIn.title') }}
            </b-nav-item>
          </template>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
  </div>
</template>

<script>
import UserAvatar from '@/components/User/UserAvatar.vue'

export default {
  name: 'Navbar',
  components: {
    UserAvatar
  },
  props: {
    user: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      currentUser: null
    }
  },
  computed: {
    environment() {
      return process.env.VUE_APP_ENV
    }
  },
  methods: {
    setModel(currentUser) {
      this.currentUser = currentUser
    }
  },
  created() {
    this.setModel(JSON.parse(this.user))
  }
}
</script>
