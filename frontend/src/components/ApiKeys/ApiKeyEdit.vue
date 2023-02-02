<template>
  <b-container>
    <h1 v-t="apiKey ? 'apiKeys.editTitle' : 'apiKeys.newTitle'" />
    <b-alert variant="warning" dismissible v-model="showString">
      <strong v-t="'apiKeys.string.heading'" />
      <br />
      <b-input-group>
        <b-form-input readonly ref="input" :value="xApiKey" @focus="$event.target.select()" />
        <template #append>
          <icon-button icon="clipboard" text="apiKeys.clipboard" variant="warning" @click="copyToClipboard()" />
        </template>
      </b-input-group>
      {{ $t('apiKeys.string.warning') }}
    </b-alert>
    <template v-if="apiKey">
      <status-detail :model="apiKey" />
      <p v-if="apiKey.expiresOn" :class="{ 'text-danger': isExpired }">
        <template v-if="isExpired">{{ $t('apiKeys.expiredOn') }}</template>
        <template v-else>{{ $t('apiKeys.expiresOn') }}</template>
        {{ $d(new Date(apiKey.expiresOn), 'medium') }}
      </p>
      <p v-else v-t="'apiKeys.neverExpires'" />
    </template>
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div v-if="!apiKey || !apiKey.isExpired" class="my-2">
          <icon-submit v-if="apiKey" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <form-datetime v-if="!apiKey" id="expiresOn" label="apiKeys.expiresOn" :minDate="new Date()" validate v-model="expiresOn" />
        <name-field :disabled="isExpired" id="title" label="apiKeys.title.label" placeholder="apiKeys.title.placeholder" required v-model="title" />
        <description-field :disabled="isExpired" :rows="15" v-model="description" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import { createApiKey, updateApiKey } from '@/api/keys'

export default {
  name: 'ApiKeyEdit',
  props: {
    json: {
      type: String,
      default: ''
    },
    status: {
      type: String,
      default: ''
    },
    xApiKey: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      apiKey: null,
      description: null,
      expiresOn: null,
      loading: false,
      showString: false,
      title: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.apiKey && this.expiresOn) || (this.title ?? '') !== (this.apiKey?.title ?? '') || (this.description ?? '') !== (this.apiKey?.description ?? '')
      )
    },
    isExpired() {
      return this.apiKey && this.apiKey.expiresOn <= new Date().toISOString()
    },
    payload() {
      const payload = {
        title: this.title,
        description: this.description
      }
      if (!this.apiKey) {
        payload.expiresOn = this.expiresOn
      }
      return payload
    }
  },
  methods: {
    copyToClipboard() {
      this.$refs.input.focus()
      navigator.clipboard.writeText(this.xApiKey)
    },
    setModel(apiKey) {
      this.apiKey = apiKey
      this.description = apiKey.description
      this.expiresOn = apiKey.expiresOn
      this.title = apiKey.title
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            if (this.apiKey) {
              const { data } = await updateApiKey(this.apiKey.id, this.payload)
              this.setModel(data)
              this.toast('success', 'apiKeys.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createApiKey(this.payload)
              window.location.replace(`/api-keys/${data.id}?status=created&x-api-key=${encodeURIComponent(data.xApiKey)}`)
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
    if (this.status === 'created') {
      this.toast('success', 'apiKeys.created')
    }
    if (this.xApiKey) {
      this.showString = true
    }
  }
}
</script>
