<template>
  <form-field
    :disabled="disabled"
    :id="id"
    :label="label"
    :maxLength="maxLength"
    :placeholder="placeholder"
    required
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'NameField',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'name'
    },
    label: {
      type: String,
      default: 'name.label'
    },
    maxLength: {
      type: Number,
      default: 100
    },
    placeholder: {
      type: String,
      default: 'name.placeholder'
    },
    value: {}
  }
}
</script>
