<template>
  <form-field
    :id="id"
    :label="label"
    :maxLength="validate ? 256 : null"
    :placeholder="placeholder"
    :required="required"
    :rules="{ email: validate }"
    type="email"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'EmailField',
  props: {
    id: {
      type: String,
      default: 'email'
    },
    label: {
      type: String,
      default: 'user.email.label'
    },
    placeholder: {
      type: String,
      default: 'user.email.placeholder'
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
