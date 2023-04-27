<template>
  <b-container>
    <h1 v-t="'configuration.title'" />
    <status-detail v-if="configuration" :model="configuration" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
        </div>
        <locale-select label="configuration.defaultLocale" v-model="defaultLocale" />
        <logging-settings v-model="loggingSettings" />
        <username-settings v-model="usernameSettings" />
        <password-settings v-model="passwordSettings" />
        <h5 v-t="'jwt.title'" />
        <jwt-secret-field :oldValue="configuration?.secret" v-model="secret" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import Vue from 'vue'
import LoggingSettings from './LoggingSettings.vue'
import { updateConfiguration } from '@/api/configurations'

export default {
  name: 'ConfigurationEdit',
  components: {
    LoggingSettings
  },
  props: {
    json: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      configuration: null,
      defaultLocale: null,
      loading: false,
      loggingSettings: {
        extent: 'ActivityOnly',
        onlyErrors: false
      },
      passwordSettings: {
        requiredLength: 6,
        requiredUniqueChars: 1,
        requireNonAlphanumeric: false,
        requireLowercase: true,
        requireUppercase: true,
        requireDigit: true
      },
      secret: null,
      usernameSettings: {
        allowedCharacters: 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'
      }
    }
  },
  computed: {
    hasChanges() {
      return (
        (this.defaultLocale !== this.configuration?.defaultLocale ?? null) ||
        this.loggingSettings.extent !== (this.configuration?.loggingSettings.extent ?? 'None') ||
        this.loggingSettings.onlyErrors !== (this.configuration?.loggingSettings.onlyErrors ?? false) ||
        this.passwordSettings.requireDigit !== (this.configuration?.passwordSettings.requireDigit ?? false) ||
        this.passwordSettings.requireLowercase !== (this.configuration?.passwordSettings.requireLowercase ?? false) ||
        this.passwordSettings.requireNonAlphanumeric !== (this.configuration?.passwordSettings.requireNonAlphanumeric ?? false) ||
        this.passwordSettings.requireUppercase !== (this.configuration?.passwordSettings.requireUppercase ?? false) ||
        (this.passwordSettings.requiredLength ?? 0) !== (this.configuration?.passwordSettings.requiredLength ?? 0) ||
        (this.passwordSettings.requiredUniqueChars ?? 0) !== (this.configuration?.passwordSettings.requiredUniqueChars ?? 0) ||
        (this.secret ?? '') !== (this.configuration?.secret ?? '') ||
        (this.usernameSettings.allowedCharacters ?? '') !== (this.configuration?.usernameSettings.allowedCharacters ?? '')
      )
    },
    payload() {
      return {
        defaultLocale: this.defaultLocale,
        loggingSettings: { ...this.loggingSettings },
        passwordSettings: { ...this.passwordSettings },
        secret: this.secret,
        usernameSettings: { ...this.usernameSettings }
      }
    }
  },
  methods: {
    setModel(configuration) {
      this.configuration = configuration
      this.defaultLocale = configuration.defaultLocale
      this.secret = configuration.secret
      Vue.set(this.usernameSettings, 'allowedCharacters', configuration.usernameSettings.allowedCharacters ?? null)
      Vue.set(this.passwordSettings, 'requireDigit', configuration.passwordSettings.requireDigit ?? false)
      Vue.set(this.passwordSettings, 'requireLowercase', configuration.passwordSettings.requireLowercase ?? false)
      Vue.set(this.passwordSettings, 'requireNonAlphanumeric', configuration.passwordSettings.requireNonAlphanumeric ?? false)
      Vue.set(this.passwordSettings, 'requireUppercase', configuration.passwordSettings.requireUppercase ?? false)
      Vue.set(this.passwordSettings, 'requiredLength', configuration.passwordSettings.requiredLength ?? 0)
      Vue.set(this.passwordSettings, 'requiredUniqueChars', configuration.passwordSettings.requiredUniqueChars ?? 0)
      Vue.set(this.loggingSettings, 'extent', configuration.loggingSettings.extent ?? 'ActivityOnly')
      Vue.set(this.loggingSettings, 'onlyErrors', configuration.loggingSettings.onlyErrors ?? false)
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await updateConfiguration(this.payload)
            this.setModel(data)
            this.toast('success', 'configuration.updated')
            this.$refs.form.reset()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  created() {
    this.setModel(JSON.parse(this.json))
  }
}
</script>
