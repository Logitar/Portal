<template>
  <b-container>
    <h1 v-t="user ? 'user.editTitle' : 'user.newTitle'" />
    <template v-if="user">
      <status-detail :model="user" />
      <p v-if="user.signedInOn">
        {{ $t('user.signedInOn') }} {{ $d(new Date(user.signedInOn), 'medium') }}
        <br />
        <b-link :href="viewSessionsUrl">{{ $t('user.session.view') }}</b-link>
      </p>
      <p v-if="user.isDisabled" class="text-danger"><status-info :actor="user.disabledBy" :date="new Date(user.disabledOn)" dateFormat="user.disabledOn" /></p>
    </template>
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <template v-if="user">
            <icon-submit class="mx-1" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
            <toggle-status :disabled="user.id === current" :user="user" @updated="setModel" />
          </template>
          <icon-submit class="mx-1" v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <realm-select :disabled="Boolean(user)" v-model="realmId" />
        <h3 v-t="'user.information.authentication'" />
        <b-alert dismissible variant="warning" v-model="usernameConflict">
          <strong v-t="'user.username.conflict'" />
        </b-alert>
        <username-field
          :disabled="Boolean(user)"
          placeholder="user.create.usernamePlaceholder"
          :realm="selectedRealm"
          ref="username"
          :required="!user"
          :validate="!user"
          v-model="username"
        />
        <template v-if="!user || user.passwordChangedOn">
          <h5 v-if="user" v-t="'user.password.label'" />
          <p v-if="user && user.passwordChangedOn">{{ $t('user.password.changedOn') }} {{ $d(new Date(user.passwordChangedOn), 'medium') }}</p>
          <p v-if="user && (password || passwordConfirmation)" class="text-warning">
            <font-awesome-icon icon="exclamation-triangle" /> <i v-t="'user.password.warning'" />
          </p>
          <b-row>
            <password-field
              v-if="user"
              class="col"
              label="user.password.new.label"
              placeholder="user.password.new.placeholder"
              :realm="selectedRealm"
              validate
              v-model="password"
            />
            <password-field v-else class="col" placeholder="user.create.passwordPlaceholder" :realm="selectedRealm" required validate v-model="password" />
            <password-field
              class="col"
              id="confirm"
              label="user.password.confirm.label"
              placeholder="user.password.confirm.placeholder"
              :required="!user || Boolean(password)"
              :rules="{ confirmed: 'password' }"
              v-model="passwordConfirmation"
            />
          </b-row>
        </template>
        <h3 v-t="'user.information.personal'" />
        <b-alert dismissible variant="warning" v-model="emailConflict">
          <strong v-t="'user.email.conflict.header'" />
          <template v-if="selectedRealm">{{ ` ${$t('user.email.conflict.detail', { name: selectedRealm.name })}` }}</template>
        </b-alert>
        <p v-if="user && (user.isEmailConfirmed || user.isPhoneNumberConfirmed)" class="text-warning">
          <font-awesome-icon icon="exclamation-triangle" /> <i v-t="'user.confirmed.warning'" />
        </p>
        <b-row>
          <email-field class="col" :confirmed="user && user.isEmailConfirmed" ref="email" validate v-model="email" />
          <phone-field class="col" :confirmed="user && user.isPhoneNumberConfirmed" validate v-model="phoneNumber" />
        </b-row>
        <b-row>
          <first-name-field class="col" validate v-model="firstName" />
          <last-name-field class="col" validate v-model="lastName" />
        </b-row>
        <b-row>
          <locale-select class="col" v-model="locale" />
          <picture-field class="col" validate v-model="picture" />
        </b-row>
        <h3 v-t="'user.externalProviders.title'" />
        <table class="table table-striped" v-if="user && user.externalProviders && user.externalProviders.length">
          <thead>
            <tr>
              <th scope="col" v-t="'user.externalProviders.name'" />
              <th scope="col" v-t="'user.externalProviders.addedOn'" />
            </tr>
          </thead>
          <tbody>
            <tr v-for="externalProvider in user.externalProviders" :key="externalProvider.id">
              <td>
                {{ externalProvider.displayName || externalProvider.key }}
                <font-awesome-icon v-if="externalProvider.key === 'Google'" :icon="['fab', 'google']" />
              </td>
              <td>{{ $d(new Date(externalProvider.addedOn), 'medium') }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else v-t="'user.externalProviders.empty'" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import EmailField from './EmailField.vue'
import FirstNameField from './FirstNameField.vue'
import LastNameField from './LastNameField.vue'
import PasswordField from './PasswordField.vue'
import PhoneField from './PhoneField.vue'
import PictureField from './PictureField.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import ToggleStatus from './ToggleStatus.vue'
import UsernameField from './UsernameField.vue'
import { createUser, updateUser } from '@/api/users'
import { getQueryString } from '@/helpers/queryUtils'
import { getRealm } from '@/api/realms'

export default {
  name: 'UserEdit',
  components: {
    EmailField,
    FirstNameField,
    LastNameField,
    PasswordField,
    PhoneField,
    PictureField,
    RealmSelect,
    ToggleStatus,
    UsernameField
  },
  props: {
    current: {
      type: String,
      default: ''
    },
    json: {
      type: String,
      default: ''
    },
    realm: {
      type: String,
      default: ''
    },
    status: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      currentUser: null,
      email: null,
      emailConflict: null,
      firstName: null,
      lastName: null,
      locale: null,
      loading: false,
      middleName: null,
      password: null,
      passwordConfirmation: null,
      phoneNumber: null,
      picture: null,
      realmId: null,
      selectedRealm: null,
      user: null,
      username: null,
      usernameConflict: false
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.user && this.realmId) ||
        (!this.user && this.username) ||
        this.password ||
        this.passwordConfirmation ||
        (this.email ?? '') !== (this.user?.email ?? '') ||
        (this.phoneNumber ?? '') !== (this.user?.phoneNumber ?? '') ||
        (this.firstName ?? '') !== (this.user?.firstName ?? '') ||
        (this.lastName ?? '') !== (this.user?.lastName ?? '') ||
        (this.locale ?? '') !== (this.user?.locale ?? '') ||
        (this.picture ?? '') !== (this.user?.picture ?? '')
      )
    },
    payload() {
      const payload = {
        password: this.password || null,
        email: this.email || null,
        phoneNumber: this.phoneNumber || null,
        firstName: this.firstName,
        lastName: this.lastName,
        middleName: this.middleName,
        locale: this.locale,
        picture: this.picture || null
      }
      if (!this.user) {
        payload.realm = this.realmId
        payload.username = this.username
      }
      return payload
    },
    viewSessionsUrl() {
      return '/sessions' + getQueryString({ realm: this.user.realm?.id ?? null, user: this.user.id })
    }
  },
  methods: {
    setModel(user) {
      this.user = user
      this.email = user.email
      this.firstName = user.firstName
      this.lastName = user.lastName
      this.locale = user.locale
      this.middleName = user.middleName
      this.password = null
      this.passwordConfirmation = null
      this.phoneNumber = user.phoneNumber
      this.picture = user.picture
      this.realmId = user.realm?.id ?? null
      this.username = user.username
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        this.emailConflict = false
        this.usernameConflict = false
        try {
          if (await this.$refs.form.validate()) {
            if (this.user) {
              const { data } = await updateUser(this.user.id, this.payload)
              this.setModel(data)
              this.toast('success', 'user.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createUser(this.payload)
              window.location.replace(`/users/${data.id}?status=created`)
            }
          }
        } catch (e) {
          const { data, status } = e
          if (status === 409) {
            if (data?.field?.toLowerCase() === 'username') {
              this.usernameConflict = true
              this.$refs.username.focus()
              return
            } else if (data?.field?.toLowerCase() === 'email') {
              this.emailConflict = true
              this.$refs.email.focus()
              return
            }
          }
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  created() {
    if (this.json) {
      this.setModel(JSON.parse(this.json))
    }
    if (this.realm) {
      this.realmId = this.realm
    }
    if (this.status === 'created') {
      this.toast('success', 'user.created')
    }
  },
  watch: {
    async realmId(id) {
      if (id) {
        try {
          const { data } = await getRealm(id)
          this.selectedRealm = data
        } catch (e) {
          this.handleError(e)
        }
      } else {
        this.selectedRealm = null
      }
    }
  }
}
</script>
