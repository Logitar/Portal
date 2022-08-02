<template>
  <b-container>
    <h1 v-t="template ? 'templates.editTitle' : 'templates.newTitle'" />
    <status-detail v-if="template" :createdAt="new Date(template.createdAt)" :updatedAt="template.updatedAt ? new Date(template.updatedAt) : null" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="template" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <b-tabs content-class="mt-3">
          <b-tab :title="$t('templates.template')">
            <p v-if="template && !realmId" v-t="'templates.noRealm'" />
            <b-row>
              <template v-if="template">
                <realm-select v-if="realmId" class="col" disabled :value="realmId" />
                <key-field class="col" disabled :value="key" />
              </template>
              <template v-else>
                <realm-select class="col" v-model="realmId" />
                <key-field class="col" required validate v-model="key" />
              </template>
              <content-type-select class="col" required v-model="contentType" />
            </b-row>
            <form-field id="subject" label="templates.subject.label" :maxLength="256" placeholder="templates.subject.placeholder" required v-model="subject" />
            <form-textarea id="contents" label="templates.contents.label" placeholder="templates.contents.placeholder" required :rows="20" v-model="contents" />
          </b-tab>
          <b-tab :title="$t('templates.metadata')">
            <name-field id="displayName" label="templates.displayName.label" placeholder="templates.displayName.placeholder" v-model="displayName" />
            <description-field :rows="15" v-model="description" />
          </b-tab>
          <b-tab v-if="template" :title="$t('templates.demo')">
            <p v-if="!hasEmail" class="text-danger" v-t="'templates.emailRequired'" />
            <p v-if="!defaultSender" class="text-danger" v-t="'templates.defaultSenderRequired'" />
            <demo-form v-else :sender="defaultSender" :template="template" />
          </b-tab>
        </b-tabs>
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import ContentTypeSelect from './ContentTypeSelect.vue'
import DemoForm from './DemoForm.vue'
import KeyField from './KeyField.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { createTemplate, updateTemplate } from '@/api/templates'

export default {
  name: 'TemplateEdit',
  components: {
    ContentTypeSelect,
    DemoForm,
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
    sender: {
      type: String,
      default: ''
    },
    status: {
      type: String,
      default: ''
    },
    user: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      contentType: null,
      contents: null,
      defaultSender: null,
      description: null,
      displayName: null,
      key: null,
      loading: false,
      realmId: null,
      subject: null,
      template: null,
      userSummary: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.template && this.realmId) ||
        (!this.template && this.key) ||
        (this.contentType ?? '') !== (this.template?.contentType ?? '') ||
        (this.subject ?? '') !== (this.template?.subject ?? '') ||
        (this.contents ?? '') !== (this.template?.contents ?? '') ||
        (this.displayName ?? '') !== (this.template?.displayName ?? '') ||
        (this.description ?? '') !== (this.template?.description ?? '')
      )
    },
    hasEmail() {
      return Boolean(this.userSummary.email)
    },
    payload() {
      const payload = {
        subject: this.subject,
        contentType: this.contentType,
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
      this.contentType = template.contentType
      this.contents = template.contents
      this.description = template.description
      this.displayName = template.displayName
      this.key = template.key
      this.realmId = template.realm?.id ?? null
      this.subject = template.subject
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
  created() {
    if (this.json) {
      this.setModel(JSON.parse(this.json))
    }
    if (this.realm) {
      this.realmId = this.realm
    }
    if (this.sender) {
      this.defaultSender = JSON.parse(this.sender)
    }
    if (this.status === 'created') {
      this.toast('success', 'templates.created')
    }
    if (this.user) {
      this.userSummary = JSON.parse(this.user)
    }
  }
}
</script>
