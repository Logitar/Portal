<template>
  <form-select
    :disabled="disabled"
    id="provider"
    label="senders.provider.label"
    :options="options"
    placeholder="senders.provider.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'ProviderSelect',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  computed: {
    options() {
      return this.orderBy(
        Object.entries(this.$i18n.t('senders.provider.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  }
}
</script>
