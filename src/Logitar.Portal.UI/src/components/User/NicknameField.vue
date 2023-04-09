<template>
  <form-field
    id="nickname"
    label="user.nickname.label"
    :maxLength="validate ? 128 : null"
    placeholder="user.nickname.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'NicknameField',
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
