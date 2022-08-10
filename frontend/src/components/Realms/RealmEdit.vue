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
            <b-alert dismissible variant="warning" v-model="aliasConflict">
              <strong v-t="'realms.alias.conflict'" />
            </b-alert>
            <b-row>
              <name-field class="col" required v-model="name" />
              <alias-field class="col" v-if="realm" disabled :value="alias" />
              <alias-field class="col" v-else :name="name" ref="alias" required validate v-model="alias" />
            </b-row>
            <form-url id="url" label="realms.url.label" placeholder="realms.url.placeholder" validate v-model="url" />
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
            <form-field
              id="allowedUsernameCharacters"
              label="realms.allowedUsernameCharacters.label"
              placeholder="realms.allowedUsernameCharacters.placeholder"
              v-model="allowedUsernameCharacters"
            />
            <password-settings v-model="passwordSettings" />
            <div v-if="realm">
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
            </div>
            <h5 v-t="'realms.googleAuth.title'" />
            <form-field
              id="googleClientId"
              label="realms.googleAuth.id.label"
              :maxLength="256"
              placeholder="realms.googleAuth.id.placeholder"
              v-model="googleClientId"
            />
          </b-tab>
        </b-tabs>
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import Vue from 'vue'
import AliasField from './AliasField.vue'
import PasswordSettings from './PasswordSettings.vue'
import SenderSelect from '@/components/Senders/SenderSelect.vue'
import TemplateSelect from '@/components/Templates/TemplateSelect.vue'
import { createRealm, updateRealm } from '@/api/realms'

export default {
  name: 'RealmEdit',
  components: {
    AliasField,
    PasswordSettings,
    SenderSelect,
    TemplateSelect
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
      alias: null,
      aliasConflict: false,
      allowedUsernameCharacters: null,
      description: null,
      googleClientId: null,
      loading: false,
      name: null,
      passwordRecoverySenderId: null,
      passwordRecoveryTemplateId: null,
      passwordSettings: {
        requireDigit: false,
        requireLowercase: false,
        requireNonAlphanumeric: false,
        requireUppercase: false,
        requiredLength: 1,
        requiredUniqueChars: 1
      },
      realm: null,
      requireConfirmedAccount: false,
      requireUniqueEmail: false,
      url: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (this.alias ?? '') !== (this.realm?.alias ?? '') ||
        (this.allowedUsernameCharacters ?? '') !== (this.realm?.allowedUsernameCharacters ?? '') ||
        (this.description ?? '') !== (this.realm?.description ?? '') ||
        (this.googleClientId ?? '') !== (this.realm?.googleClientId ?? '') ||
        (this.name ?? '') !== (this.realm?.name ?? '') ||
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
        (this.url ?? '') !== (this.realm?.url ?? '')
      )
    },
    payload() {
      const payload = {
        allowedUsernameCharacters: this.allowedUsernameCharacters,
        description: this.description,
        googleClientId: this.googleClientId,
        name: this.name,
        passwordRecoverySenderId: this.passwordRecoverySenderId,
        passwordRecoveryTemplateId: this.passwordRecoveryTemplateId,
        passwordSettings: { ...this.passwordSettings },
        requireConfirmedAccount: this.requireConfirmedAccount,
        requireUniqueEmail: this.requireUniqueEmail,
        url: this.url
      }
      if (!this.realm) {
        payload.alias = this.alias
      }
      return payload
    }
  },
  methods: {
    browse() {},
    setModel(realm) {
      this.realm = realm
      this.alias = realm.alias
      this.allowedUsernameCharacters = realm.allowedUsernameCharacters
      this.description = realm.description
      this.googleClientId = realm.googleClientId
      this.name = realm.name
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
      this.url = realm.url
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        this.aliasConflict = false
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
          if (status === 409 && data?.field?.toLowerCase() === 'alias') {
            this.aliasConflict = true
            this.$refs.alias.customize()
            Vue.nextTick(() => this.$refs.alias.focus())
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
