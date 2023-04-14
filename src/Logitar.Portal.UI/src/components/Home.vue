<template>
  <b-container>
    <h1 v-t="'configuration.initialization.title'" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <h3 v-t="'configuration.initialization.user.label'" />
        <p><font-awesome-icon icon="info-circle" /> <i v-t="'configuration.initialization.user.help'" /></p>
        <b-row>
          <email-field class="col" required validate v-model="user.emailAddress" />
          <username-field class="col" required :settings="usernameSettings" validate v-model="user.username" />
        </b-row>
        <b-row>
          <first-name-field class="col" required validate v-model="user.firstName" />
          <last-name-field class="col" required validate v-model="user.lastName" />
        </b-row>
        <b-row>
          <password-field class="col" required :settings="passwordSettings" validate v-model="user.password" />
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
        <icon-submit :disabled="loading" icon="cog" :loading="loading" text="configuration.initialization.initialize" variant="primary" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import EmailField from './User/EmailField.vue'
import FirstNameField from './User/FirstNameField.vue'
import LastNameField from './User/LastNameField.vue'
import PasswordField from './User/PasswordField.vue'
import UsernameField from './User/UsernameField.vue'
import { initialize } from '@/api/configurations'

export default {
  name: 'Home',
  components: {
    EmailField,
    FirstNameField,
    LastNameField,
    PasswordField,
    UsernameField
  },
  data() {
    return {
      loading: false,
      loggingSettings: {
        extent: 'ActivityOnly',
        onlyErrors: false
      },
      passwordConfirmation: null,
      passwordSettings: {
        requiredLength: 6,
        requiredUniqueChars: 1,
        requireNonAlphanumeric: false,
        requireLowercase: true,
        requireUppercase: true,
        requireDigit: true
      },
      user: {
        emailAddress: null,
        firstName: null,
        lastName: null,
        password: null,
        username: null
      },
      usernameSettings: {
        allowedCharacters: 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'
      }
    }
  },
  computed: {
    payload() {
      return {
        defaultLocale: this.$i18n.locale,
        secret: null,
        usernameSettings: { ...this.usernameSettings },
        passwordSettings: { ...this.passwordSettings },
        loggingSettings: { ...this.loggingSettings },
        user: { ...this.user }
      }
    }
  },
  methods: {
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            await initialize(this.payload)
            window.location.replace('/user/profile')
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    'user.emailAddress': {
      handler(emailAddress) {
        this.user.username = emailAddress
      }
    }
  }
}
</script>
