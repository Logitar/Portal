<template>
  <form-field
    :disabled="disabled"
    id="key"
    label="templates.key.label"
    :maxLength="validate ? 100 : null"
    placeholder="templates.key.placeholder"
    :required="required"
    :rules="{ identifier: validate }"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'KeyField',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    required: {
      type: Boolean,
      default: false
    },
    validate: {
      type: Boolean,
      default: false
    },
    value: {}
  }
}
</script>
