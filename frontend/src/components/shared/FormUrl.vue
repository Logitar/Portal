<template>
  <form-field
    :disabled="disabled"
    :id="id"
    :label="label"
    :maxLength="validate ? 2048 : null"
    :placeholder="placeholder"
    :required="required"
    :rules="{ url: validate }"
    type="url"
    :value="value"
    @input="$emit('input', $event)"
  >
    <b-input-group-append>
      <icon-button :disabled="!value" :href="value" icon="external-link-alt" target="_blank" variant="info" />
    </b-input-group-append>
  </form-field>
</template>

<script>
export default {
  name: 'FormUrl',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'url'
    },
    label: {
      type: String,
      default: ''
    },
    placeholder: {
      type: String,
      default: ''
    },
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
