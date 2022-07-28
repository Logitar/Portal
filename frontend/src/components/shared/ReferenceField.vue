<template>
  <form-field
    id="reference"
    label="reference.label"
    :maxLength="2048"
    placeholder="reference.placeholder"
    :rules="{ url: true }"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'ReferenceField',
  props: {
    value: {}
  }
}
</script>
