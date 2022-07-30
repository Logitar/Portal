<template>
  <form-select
    id="status"
    label="messages.status.label"
    :options="options"
    placeholder="messages.status.placeholder"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'StatusSelect',
  props: {
    value: {}
  },
  computed: {
    options() {
      return this.orderBy(
        Object.entries(this.$i18n.t('messages.status.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  }
}
</script>
