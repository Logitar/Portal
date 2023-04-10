<template>
  <form-field
    class="col"
    id="issuer"
    label="tokens.issuer.label"
    :maxLength="100"
    placeholder="tokens.issuer.placeholder"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'IssuerField',
  props: {
    value: {}
  }
}
</script>
