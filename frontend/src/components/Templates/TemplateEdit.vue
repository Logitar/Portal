<template>
  <b-container>
    <h1 v-t="template ? 'templates.editTitle' : 'templates.newTitle'" />
    <status-detail v-if="template" :createdAt="new Date(template.createdAt)" :updatedAt="template.updatedAt ? new Date(template.updatedAt) : null" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="!!template" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <b-tabs content-class="mt-3">
          <b-tab :title="$t('templates.template')">
            <b-row v-if="!!template">
              <realm-select v-if="!!realmId" class="col" disabled :value="realmId" />
              <p v-else class="col" v-t="'templates.noRealm'" />
              <key-field class="col" disabled :value="key" />
            </b-row>
            <b-row v-else>
              <realm-select class="col" v-model="realmId" />
              <key-field class="col" required validate v-model="key" />
            </b-row>
            <form-textarea id="contents" label="templates.contents.label" placeholder="templates.contents.placeholder" required :rows="20" v-model="contents" />
          </b-tab>
          <b-tab :title="$t('templates.metadata')">
            <name-field id="displayName" label="templates.displayName.label" placeholder="templates.displayName.placeholder" v-model="displayName" />
            <description-field :rows="20" v-model="description" />
          </b-tab>
        </b-tabs>
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import KeyField from './KeyField.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { createTemplate, updateTemplate } from '@/api/templates'

export default {
  name: 'TemplateEdit',
  components: {
    KeyField,
    RealmSelect
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
      contents: null,
      description: null,
      displayName: null,
      key: null,
      loading: false,
      realmId: null,
      template: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.template && !!this.realmId) ||
        (!this.template && !!this.key) ||
        (this.contents ?? '') !== (this.template?.contents ?? '') ||
        (this.displayName ?? '') !== (this.template?.displayName ?? '') ||
        (this.description ?? '') !== (this.template?.description ?? '')
      )
    },
    payload() {
      const payload = {
        contents: this.contents,
        displayName: this.displayName,
        description: this.description
      }
      if (!this.template) {
        payload.key = this.key
        payload.realm = this.realmId
      }
      return payload
    }
  },
  methods: {
    setModel(template) {
      this.template = template
      this.contents = template.contents
      this.description = template.description
      this.displayName = template.displayName
      this.key = template.key
      this.realmId = template.realm?.id
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            if (this.template) {
              const { data } = await updateTemplate(this.template.id, this.payload)
              this.setModel(data)
              this.toast('success', 'templates.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createTemplate(this.payload)
              window.location.replace(`/templates/${data.id}?status=created`)
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
      this.toast('success', 'templates.created')
    }
  }
}
</script>
