<template>
  <form-field
    class="col"
    id="secret"
    label="tokens.secret.label"
    :minLength="32"
    :maxLength="64"
    placeholder="tokens.secret.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'SecretField',
  props: {
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  }
}
</script>
