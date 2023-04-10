<template>
  <form-field
    class="col"
    id="secret"
    label="tokens.secret.label"
    :minLength="32"
    :maxLength="64"
    placeholder="tokens.secret.placeholder"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'SecretField',
  props: {
    value: {}
  }
}
</script>
