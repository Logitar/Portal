<template>
  <form-field
    class="col"
    id="purpose"
    label="tokens.purpose.label"
    :maxLength="100"
    placeholder="tokens.purpose.placeholder"
    :rules="{ purpose: true }"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'PurposeField',
  props: {
    value: {}
  }
}
</script>
