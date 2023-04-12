<template>
  <form-field
    id="firstName"
    label="user.firstName.label"
    :maxLength="validate ? 128 : null"
    placeholder="user.firstName.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'FirstNameField',
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
