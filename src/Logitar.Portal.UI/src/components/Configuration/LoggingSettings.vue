<template>
  <form-select
    id="extent"
    label="configuration.logging.extent.label"
    :options="options"
    placeholder="configuration.logging.extent.placeholder"
    required
    :value="value.extent"
    @input="onInput({ extent: $event })"
  >
    <template #after>
      <b-form-checkbox :disabled="value.extent === 'None'" id="onlyErrors" :checked="value.onlyErrors" @input="onInput({ onlyErrors: $event })">
        {{ $t('configuration.logging.extent.onlyErrors') }}
      </b-form-checkbox>
    </template>
  </form-select>
</template>

<script>
export default {
  name: 'LoggingSettings',
  props: {
    value: {}
  },
  computed: {
    options() {
      return this.orderBy(
        Object.entries(this.$i18n.t('configuration.logging.extent.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  },
  methods: {
    onInput(changes) {
      const value = { ...this.value }
      for (const [key, change] of Object.entries(changes)) {
        value[key] = change
      }
      if (value.extent === 'None') {
        value.onlyErrors = false
      }
      this.$emit('input', value)
    }
  }
}
</script>
