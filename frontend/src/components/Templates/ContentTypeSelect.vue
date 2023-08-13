<template>
  <form-select
    id="contentType"
    label="templates.contentType.label"
    :options="options"
    placeholder="templates.contentType.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'ContentTypeSelect',
  props: {
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  computed: {
    options() {
      return this.orderBy(
        Object.entries(this.$i18n.t('templates.contentType.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  }
}
</script>
