<template>
  <form-select
    :disabled="disabled"
    id="type"
    label="type.label"
    :options="options"
    placeholder="type.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'TypeSelect',
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
        Object.entries(this.$i18n.t('type.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  }
}
</script>
