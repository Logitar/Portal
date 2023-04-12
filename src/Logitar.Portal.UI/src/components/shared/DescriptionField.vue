<template>
  <form-textarea
    :disabled="disabled"
    :id="id"
    :label="label"
    :maxLength="maxLength"
    :placeholder="placeholder"
    :rows="rows"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'DescriptionField',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'description'
    },
    label: {
      type: String,
      default: 'description.label'
    },
    maxLength: {
      type: Number,
      default: 1000
    },
    placeholder: {
      type: String,
      default: 'description.placeholder'
    },
    rows: {
      type: Number,
      default: 25
    },
    value: {}
  }
}
</script>
