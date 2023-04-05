<template>
  <b-container>
    <h1 v-t="sender ? 'senders.editTitle' : 'senders.newTitle'" />
    <status-detail v-if="sender" :model="sender" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="sender" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <b-row>
          <realm-select class="col" :disabled="Boolean(sender)" v-model="realmId" />
          <provider-select class="col" :disabled="Boolean(sender)" :required="!sender" v-model="selectedProvider" />
        </b-row>
        <b-row>
          <email-field
            class="col"
            id="emailAddress"
            label="senders.emailAddress.label"
            placeholder="senders.emailAddress.placeholder"
            required
            validate
            v-model="emailAddress"
          />
          <name-field class="col" id="displayName" label="senders.displayName.label" placeholder="senders.displayName.placeholder" v-model="displayName" />
        </b-row>
        <send-grid-settings v-if="selectedProvider === 'SendGrid'" v-model="settings" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import EmailField from '@/components/User/EmailField.vue'
import ProviderSelect from './ProviderSelect.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import SendGridSettings from './SendGridSettings.vue'
import { createSender, updateSender } from '@/api/senders'

export default {
  name: 'SenderEdit',
  components: {
    EmailField,
    ProviderSelect,
    RealmSelect,
    SendGridSettings
  },
  props: {
    json: {
      type: String,
      default: ''
    },
    provider: {
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
      displayName: null,
      emailAddress: null,
      loading: false,
      providerSettings: null,
      realmId: null,
      selectedProvider: null,
      sender: null,
      settings: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.sender && this.realmId) ||
        (!this.sender && this.selectedProvider) ||
        (this.emailAddress ?? '') !== (this.sender?.emailAddress ?? '') ||
        (this.displayName ?? '') !== (this.sender?.displayName ?? '') ||
        JSON.stringify(this.settings ?? {}) !== JSON.stringify(this.providerSettings ?? {})
      )
    },
    payload() {
      const payload = {
        displayName: this.displayName,
        emailAddress: this.emailAddress,
        settings: Object.entries(this.settings ?? {}).map(([key, value]) => ({ key, value }))
      }
      if (!this.sender) {
        payload.provider = this.selectedProvider
        payload.realm = this.realmId
      }
      return payload
    }
  },
  methods: {
    indexSettings(settings) {
      return Object.fromEntries(settings.map(({ key, value }) => [key, value]))
    },
    setModel(sender) {
      this.sender = sender
      this.displayName = sender.displayName
      this.emailAddress = sender.emailAddress
      this.providerSettings = this.indexSettings(sender.settings)
      this.realmId = sender.realm?.id ?? null
      this.selectedProvider = sender.provider
      this.settings = this.indexSettings(sender.settings)
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            if (this.sender) {
              const { data } = await updateSender(this.sender.id, this.payload)
              this.setModel(data)
              this.toast('success', 'senders.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createSender(this.payload)
              window.location.replace(`/senders/${data.id}?status=created`)
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
  created() {
    if (this.json) {
      this.setModel(JSON.parse(this.json))
    }
    if (this.provider) {
      this.selectedProvider = this.provider
    }
    if (this.realm) {
      this.realmId = this.realm
    }
    if (this.status === 'created') {
      this.toast('success', 'senders.created')
    }
  }
}
</script>
