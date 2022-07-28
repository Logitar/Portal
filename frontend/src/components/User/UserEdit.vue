<template>
  <b-container>
    <h1 v-t="user ? 'user.editTitle' : 'user.newTitle'" />
    <template v-if="user">
      <status-detail :createdAt="new Date(user.createdAt)" :updatedAt="user.updatedAt ? new Date(user.updatedAt) : null" />
      <p v-if="user.signedInAt">{{ $t('user.signedInAt') }} {{ $d(new Date(user.signedInAt), 'medium') }}</p>
    </template>
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="!!user" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <realm-select v-if="user && user.realm" disabled :value="realmId" />
        <p v-else-if="!!user" v-t="'user.noRealm'" />
        <realm-select v-else v-model="realmId" />
        <h3 v-t="'user.information.authentication'" />
        <username-field :disabled="!!user" placeholder="user.create.usernamePlaceholder" :required="!user" :validate="!user" v-model="username" />
        <b-row v-if="!user">
          <password-field class="col" placeholder="user.create.passwordPlaceholder" required validate v-model="password" />
          <password-field
            class="col"
            id="confirm"
            label="user.password.confirm.label"
            placeholder="user.password.confirm.placeholder"
            required
            :rules="{ confirmed: 'password' }"
            v-model="passwordConfirmation"
          />
        </b-row>
        <template v-else-if="user.passwordChangedAt">
          <h5 v-t="'user.password.label'" />
          <p>{{ $t('user.password.changedAt') }} {{ $d(new Date(user.passwordChangedAt), 'medium') }}</p>
        </template>
        <h3 v-t="'user.information.personal'" />
        <b-row>
          <email-field class="col" validate v-model="email" />
        </b-row>
        <b-row>
          <first-name-field class="col" validate v-model="firstName" />
          <last-name-field class="col" validate v-model="lastName" />
        </b-row>
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import EmailField from './EmailField.vue'
import FirstNameField from './FirstNameField.vue'
import LastNameField from './LastNameField.vue'
import PasswordField from './PasswordField.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import UsernameField from './UsernameField.vue'
import { createUser, updateUser } from '@/api/users'

export default {
  name: 'UserEdit',
  components: {
    EmailField,
    FirstNameField,
    LastNameField,
    PasswordField,
    RealmSelect,
    UsernameField
  },
  props: {
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
      email: null,
      firstName: null,
      lastName: null,
      loading: false,
      middleName: null,
      password: null,
      passwordConfirmation: null,
      phoneNumber: null,
      picture: null,
      realmId: null,
      user: null,
      username: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.user && !!this.username) ||
        (this.email ?? '') !== (this.user?.email ?? '') ||
        (this.firstName ?? '') !== (this.user?.firstName ?? '') ||
        (this.lastName ?? '') !== (this.user?.lastName ?? '')
      )
    },
    payload() {
      const payload = {
        email: this.email,
        phoneNumber: this.phoneNumber,
        firstName: this.firstName,
        lastName: this.lastName,
        middleName: this.middleName,
        locale: this.$i18n.locale,
        picture: this.picture
      }
      if (!this.user) {
        payload.realmId = this.realmId
        payload.username = this.username
        payload.password = this.password
      }
      return payload
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
      this.phoneNumber = user.phoneNumber
      this.picture = user.picture
      this.realmId = user.realm?.id ?? null
      this.username = user.username
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
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
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  async created() {
    if (this.json) {
      this.setModel(JSON.parse(this.json))
    }
    if (this.realm) {
      this.realmId = this.realm
    }
    if (this.status === 'created') {
      this.toast('success', 'user.created')
    }
  }
}
</script>
