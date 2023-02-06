<template>
  <form-field
    id="lastName"
    label="user.lastName.label"
    :maxLength="validate ? 127 : null"
    placeholder="user.lastName.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'LastNameField',
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
