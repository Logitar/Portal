<template>
  <b-tab :title="$t('messages.contents.label')">
    <p>
      {{ $t('messages.templateFormat') }}
      <b-link v-if="template" :href="`/templates/${template.id}`" target="_blank">
        {{ template.displayName ? `${template.displayName} (${template.key})` : template.key }} <font-awesome-icon icon="external-link-alt" />
      </b-link>
      <strong v-else>{{ message.templateDisplayName ? `${message.templateDisplayName} (${message.templateKey})` : message.templateKey }}</strong
      >.
    </p>
    <h3 v-text="message.subject" />
    <p class="text-warning"><i v-t="'messages.contents.warning'" /></p>
    <div class="accordion" role="tablist">
      <b-card no-body class="mb-1">
        <b-card-header header-tag="header" class="p-1" role="tab">
          <b-link class="nav-link" v-b-toggle.contents>
            {{ $t(`templates.contentType.options.${message.templateContentType}`) }} {{ $t('messages.contentFormat') }}
          </b-link>
        </b-card-header>
        <b-collapse id="contents" accordion="contents-accordion" role="tabpanel">
          <b-card-body>
            <p>
              <b-form-checkbox switch v-model="viewAsHtml">{{ $t('messages.contents.viewAsHtml') }}</b-form-checkbox>
            </p>
            <div v-if="viewAsHtml" v-html="message.body" />
            <div v-else v-text="message.body" />
          </b-card-body>
        </b-collapse>
      </b-card>
      <b-card no-body class="mb-1">
        <b-card-header header-tag="header" class="p-1" role="tab">
          <b-link class="nav-link" v-b-toggle.localization>{{ $t('messages.contents.localization') }}</b-link>
        </b-card-header>
        <b-collapse id="localization" accordion="contents-accordion" role="tabpanel">
          <b-card-body>
            <b-form-group>
              <b-form-checkbox :checked="message.ignoreUserLocale" disabled>{{ $t('messages.contents.ignoreUserLocale') }}</b-form-checkbox>
            </b-form-group>
            <locale-select v-if="message.locale" disabled :value="message.locale" />
            <p v-else v-t="'messages.contents.noLocale'" />
          </b-card-body>
        </b-collapse>
      </b-card>
      <b-card no-body class="mb-1">
        <b-card-header header-tag="header" class="p-1" role="tab">
          <b-link class="nav-link" v-b-toggle.variables>{{ $t('messages.contents.variables') }}</b-link>
        </b-card-header>
        <b-collapse id="variables" accordion="contents-accordion" role="tabpanel">
          <b-card-body>
            <json-viewer v-if="Object.keys(variables).length" :value="variables" :expand-depth="5" copyable boxed />
            <p v-else v-t="'messages.contents.noVariable'" />
          </b-card-body>
        </b-collapse>
      </b-card>
    </div>
  </b-tab>
</template>

<script>
import { getTemplate } from '@/api/templates'

export default {
  name: 'ContentTab',
  props: {
    message: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      template: null,
      viewAsHtml: false
    }
  },
  computed: {
    variables() {
      return Object.fromEntries(this.message.variables.map(({ key, value }) => [key, value]))
    }
  },
  async created() {
    this.viewAsHtml = this.message.templateContentType === 'text/html'
    try {
      const { data } = await getTemplate(this.message.templateId)
      this.template = data
    } catch (e) {
      if (e.status !== 404) {
        this.handleError(e)
      }
    }
  }
}
</script>
