<template>
  <form-field
    :disabled="disabled"
    id="username"
    :label="label"
    :maxLength="validate ? 256 : null"
    :placeholder="placeholder"
    :required="required"
    :rules="{ username: validate }"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'UsernameField',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    label: {
      type: String,
      default: 'user.username.label'
    },
    placeholder: {
      type: String,
      default: 'user.username.placeholder'
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
