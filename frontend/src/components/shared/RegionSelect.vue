<template>
  <form-select
    :disabled="disabled"
    id="region"
    label="region.label"
    :options="options"
    placeholder="region.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  >
    <template #before><slot name="before" /></template>
    <template #after><slot name="after" /></template>
    <slot />
  </form-select>
</template>

<script>
export default {
  name: 'RegionSelect',
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
        Object.entries(this.$i18n.t('region.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  }
}
</script>
