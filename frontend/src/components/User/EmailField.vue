<template>
  <form-field
    id="email"
    label="user.email.label"
    :maxLength="validate ? 256 : null"
    placeholder="user.email.placeholder"
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
