<template>
  <form-field
    class="col"
    id="audience"
    label="tokens.audience.label"
    :maxLength="100"
    placeholder="tokens.audience.placeholder"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'AudienceField',
  props: {
    value: {}
  }
}
</script>
