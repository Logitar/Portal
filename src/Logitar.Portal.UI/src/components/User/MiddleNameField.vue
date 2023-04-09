<template>
  <form-field
    id="middleName"
    label="user.middleName.label"
    :maxLength="validate ? 128 : null"
    placeholder="user.middleName.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'MiddleNameField',
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
