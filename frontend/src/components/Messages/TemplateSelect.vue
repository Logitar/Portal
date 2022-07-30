<template>
  <form-select
    id="template"
    label="messages.template.label"
    :options="options"
    placeholder="messages.template.placeholder"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
import { getTemplates } from '@/api/templates'

export default {
  name: 'TemplateSelect',
  props: {
    realmId: {},
    value: {}
  },
  data() {
    return {
      templates: []
    }
  },
  computed: {
    options() {
      return this.templates.map(({ id, displayName, key }) => ({
        text: displayName ? `${displayName} (${key})` : key,
        value: id
      }))
    }
  },
  methods: {
    async refresh(realmId) {
      try {
        const { data } = await getTemplates({
          realmId,
          sort: 'Key',
          desc: false
        })
        this.templates = data.items
      } catch (e) {
        this.handleError(e)
      }
    }
  },
  watch: {
    realmId: {
      immediate: true,
      handler(realmId) {
        this.refresh(realmId)
      }
    }
  }
}
</script>
