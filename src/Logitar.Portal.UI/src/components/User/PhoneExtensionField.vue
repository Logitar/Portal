<template>
  <form-field
    :id="id"
    :label="label"
    :maxLength="validate ? 16 : null"
    :placeholder="placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'PhoneExtensionField',
  props: {
    id: {
      type: String,
      default: 'phoneExtension'
    },
    label: {
      type: String,
      default: 'user.phone.extension.label'
    },
    placeholder: {
      type: String,
      default: 'user.phone.extension.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    validate: {
      type: Boolean,
      default: false
    },
    value: {},
    verified: {
      type: Boolean,
      default: false
    }
  }
}
</script>
