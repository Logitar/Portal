<template>
  <b-container>
    <h1 v-t="realm ? 'realms.editTitle' : 'realms.newTitle'" />
    <status-detail v-if="realm" :createdAt="new Date(realm.createdAt)" :updatedAt="realm.updatedAt ? new Date(realm.updatedAt) : null" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="realm" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <b-tabs content-class="mt-3">
          <b-tab :title="$t('realms.general')">
            <b-row>
              <name-field class="col" required v-model="name" />
              <alias-field class="col" v-if="realm" disabled :value="alias" />
              <alias-field class="col" v-else :name="name" required validate v-model="alias" />
            </b-row>
            <form-field
              id="url"
              label="realms.url.label"
              :maxLength="2048"
              placeholder="realms.url.placeholder"
              :rules="{ url: true }"
              type="url"
              v-model="url"
            >
              <b-input-group-append>
                <icon-button :disabled="!url" :href="url" icon="external-link-alt" target="_blank" variant="info" />
              </b-input-group-append>
            </form-field>
            <description-field :rows="15" v-model="description" />
          </b-tab>
          <b-tab :title="$t('realms.settings')">
            <b-form-group>
              <b-form-checkbox id="requireConfirmedAccount" v-model="requireConfirmedAccount">{{ $t('realms.requireConfirmedAccount') }}</b-form-checkbox>
            </b-form-group>
          </b-tab>
        </b-tabs>
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import AliasField from './AliasField.vue'
import { createRealm, updateRealm } from '@/api/realms'

export default {
  name: 'RealmEdit',
  components: {
    AliasField
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
      description: null,
      loading: false,
      name: null,
      realm: null,
      requireConfirmedAccount: false,
      url: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (this.alias ?? '') !== (this.realm?.alias ?? '') ||
        (this.name ?? '') !== (this.realm?.name ?? '') ||
        (this.url ?? '') !== (this.realm?.url ?? '') ||
        (this.description ?? '') !== (this.realm?.description ?? '') ||
        this.requireConfirmedAccount !== (this.realm?.requireConfirmedAccount ?? false)
      )
    },
    payload() {
      const payload = {
        name: this.name,
        description: this.description,
        requireConfirmedAccount: this.requireConfirmedAccount,
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
      this.description = realm.description
      this.name = realm.name
      this.requireConfirmedAccount = realm.requireConfirmedAccount
      this.url = realm.url
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
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
