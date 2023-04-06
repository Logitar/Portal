<template>
  <b-container>
    <h1 v-t="realm ? 'realms.editTitle' : 'realms.newTitle'" />
    <status-detail v-if="realm" :model="realm" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="realm" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <b-tabs content-class="mt-3">
          <b-tab :title="$t('realms.general')">
            <b-alert dismissible variant="warning" v-model="uniqueNameConflict">
              <strong v-t="'realms.uniqueName.conflict'" />
            </b-alert>
            <b-row>
              <name-field class="col" label="realms.displayName.label" placeholder="realms.displayName.placeholder" v-model="displayName" />
              <alias-field class="col" v-if="realm" disabled :value="uniqueName" />
              <alias-field class="col" v-else :name="displayName" ref="uniqueName" required validate v-model="uniqueName" />
            </b-row>
            <b-row>
              <locale-select class="col" label="realms.defaultLocale" v-model="defaultLocale" />
              <form-url class="col" id="url" label="realms.url.label" placeholder="realms.url.placeholder" validate v-model="url" />
            </b-row>
            <description-field :rows="15" v-model="description" />
          </b-tab>
          <b-tab :title="$t('realms.settings')">
            <b-form-group>
              <b-form-checkbox id="requireConfirmedAccount" v-model="requireConfirmedAccount">
                <span v-b-tooltip.hover :title="$t('realms.requireConfirmedAccount.help')">
                  {{ $t('realms.requireConfirmedAccount.label') }} <font-awesome-icon icon="info-circle" />
                </span>
              </b-form-checkbox>
            </b-form-group>
            <b-form-group>
              <b-form-checkbox id="requireUniqueEmail" v-model="requireUniqueEmail">
                <span v-b-tooltip.hover :title="$t('realms.requireUniqueEmail.help')">
                  {{ $t('realms.requireUniqueEmail.label') }} <font-awesome-icon icon="info-circle" />
                </span>
              </b-form-checkbox>
            </b-form-group>
            <username-settings v-model="usernameSettings" />
            <password-settings v-model="passwordSettings" />
            <!-- <div v-if="realm">
              <h5 v-t="'realms.passwordRecovery.title'" />
              <b-row>
                <sender-select
                  class="col"
                  id="passwordRecoverySenderId"
                  placeholder="realms.passwordRecovery.senderPlaceholder"
                  :realm="realm.id"
                  v-model="passwordRecoverySenderId"
                />
                <template-select class="col" id="passwordRecoveryTemplateId" :realm="realm.id" v-model="passwordRecoveryTemplateId" />
              </b-row>
            </div> -->
            <h5 v-t="'realms.jwt.title'" />
            <b-form-group>
              <form-field
                id="secret"
                label="realms.jwt.secret.label"
                :minLength="256 / 8"
                :maxLength="512 / 8"
                placeholder="realms.jwt.secret.placeholder"
                v-model="secret"
              />
              <!-- TODO(fpion): warning -->
            </b-form-group>
          </b-tab>
        </b-tabs>
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
// TODO(fpion): Password Recovery

import Vue from 'vue'
import AliasField from './AliasField.vue'
import PasswordSettings from './PasswordSettings.vue'
// import SenderSelect from '@/components/Senders/SenderSelect.vue'
// import TemplateSelect from '@/components/Templates/TemplateSelect.vue'
import UsernameSettings from './UsernameSettings.vue'
import { createRealm, updateRealm } from '@/api/realms'

export default {
  name: 'RealmEdit',
  components: {
    AliasField,
    PasswordSettings,
    // SenderSelect,
    // TemplateSelect,
    UsernameSettings
  },
  props: {
    json: {
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
      uniqueName: null,
      uniqueNameConflict: false,
      defaultLocale: null,
      description: null,
      displayName: null,
      loading: false,
      passwordRecoverySenderId: null,
      passwordRecoveryTemplateId: null,
      passwordSettings: {
        requireDigit: true,
        requireLowercase: true,
        requireNonAlphanumeric: false,
        requireUppercase: true,
        requiredLength: 6,
        requiredUniqueChars: 1
      },
      realm: null,
      requireConfirmedAccount: false,
      requireUniqueEmail: false,
      secret: null,
      url: null,
      usernameSettings: {
        allowedCharacters: 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'
      }
    }
  },
  computed: {
    hasChanges() {
      return (
        (this.defaultLocale !== this.realm?.defaultLocale ?? null) ||
        (this.description ?? '') !== (this.realm?.description ?? '') ||
        (this.displayName ?? '') !== (this.realm?.displayName ?? '') ||
        this.passwordRecoverySenderId !== this.realm?.passwordRecoverySenderId ||
        this.passwordRecoveryTemplateId !== this.realm?.passwordRecoveryTemplateId ||
        this.passwordSettings.requireDigit !== (this.realm?.passwordSettings?.requireDigit ?? false) ||
        this.passwordSettings.requireLowercase !== (this.realm?.passwordSettings?.requireLowercase ?? false) ||
        this.passwordSettings.requireNonAlphanumeric !== (this.realm?.passwordSettings?.requireNonAlphanumeric ?? false) ||
        this.passwordSettings.requireUppercase !== (this.realm?.passwordSettings?.requireUppercase ?? false) ||
        (this.passwordSettings.requiredLength ?? 0) !== (this.realm?.passwordSettings?.requiredLength ?? 0) ||
        (this.passwordSettings.requiredUniqueChars ?? 0) !== (this.realm?.passwordSettings?.requiredUniqueChars ?? 0) ||
        this.requireConfirmedAccount !== (this.realm?.requireConfirmedAccount ?? false) ||
        this.requireUniqueEmail !== (this.realm?.requireUniqueEmail ?? false) ||
        (this.secret ?? '') !== (this.realm?.secret ?? '') ||
        (this.uniqueName ?? '') !== (this.realm?.uniqueName ?? '') ||
        (this.url ?? '') !== (this.realm?.url ?? '') ||
        (this.usernameSettings.allowedCharacters ?? '') !== (this.realm?.usernameSettings.allowedCharacters ?? '')
      )
    },
    payload() {
      const payload = {
        claimMappings: null, // TODO(fpion): implement
        customAttributes: null, // TODO(fpion): implement
        defaultLocale: this.defaultLocale,
        description: this.description,
        displayName: this.displayName,
        passwordRecoverySenderId: this.passwordRecoverySenderId,
        passwordRecoveryTemplateId: this.passwordRecoveryTemplateId,
        passwordSettings: { ...this.passwordSettings },
        requireConfirmedAccount: this.requireConfirmedAccount,
        requireUniqueEmail: this.requireUniqueEmail,
        secret: this.secret,
        url: this.url,
        usernameSettings: { ...this.usernameSettings }
      }
      if (!this.realm) {
        payload.uniqueName = this.uniqueName
      }
      return payload
    }
  },
  methods: {
    setModel(realm) {
      this.realm = realm
      this.defaultLocale = realm.defaultLocale
      this.description = realm.description
      this.displayName = realm.displayName
      this.passwordRecoverySenderId = realm.passwordRecoverySenderId
      this.passwordRecoveryTemplateId = realm.passwordRecoveryTemplateId
      Vue.set(this.passwordSettings, 'requireDigit', realm.passwordSettings?.requireDigit ?? false)
      Vue.set(this.passwordSettings, 'requireLowercase', realm.passwordSettings?.requireLowercase ?? false)
      Vue.set(this.passwordSettings, 'requireNonAlphanumeric', realm.passwordSettings?.requireNonAlphanumeric ?? false)
      Vue.set(this.passwordSettings, 'requireUppercase', realm.passwordSettings?.requireUppercase ?? false)
      Vue.set(this.passwordSettings, 'requiredLength', realm.passwordSettings?.requiredLength ?? 0)
      Vue.set(this.passwordSettings, 'requiredUniqueChars', realm.passwordSettings?.requiredUniqueChars ?? 0)
      this.requireConfirmedAccount = realm.requireConfirmedAccount
      this.requireUniqueEmail = realm.requireUniqueEmail
      this.secret = realm.secret
      this.uniqueName = realm.uniqueName
      this.url = realm.url
      Vue.set(this.usernameSettings, 'allowedCharacters', realm.usernameSettings?.allowedCharacters ?? null)
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        this.uniqueNameConflict = false
        try {
          if (await this.$refs.form.validate()) {
            if (this.realm) {
              const { data } = await updateRealm(this.realm.id, this.payload)
              this.setModel(data)
              this.toast('success', 'realms.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createRealm(this.payload)
              window.location.replace(`/realms/${data.id}?status=created`)
            }
          }
        } catch (e) {
          const { data, status } = e
          if (status === 409 && data?.code === 'UniqueNameAlreadyUsed') {
            this.uniqueNameConflict = true
            this.$refs.uniqueName.customize()
            Vue.nextTick(() => this.$refs.uniqueName.focus())
            return
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
    if (this.status === 'created') {
      this.toast('success', 'realms.created')
    }
  }
}
</script>
