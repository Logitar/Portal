<template>
  <form-field
    :disabled="disabled"
    :id="id"
    :label="label"
    :maxLength="validate ? 100 : null"
    :placeholder="placeholder"
    :ref="id"
    :required="required"
    :rules="{ identifier: validate }"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'KeyField',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'key'
    },
    label: {
      type: String,
      default: 'templates.key.label'
    },
    placeholder: {
      type: String,
      default: 'templates.key.placeholder'
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
  },
  methods: {
    focus() {
      this.$refs[this.id].focus()
    }
  }
}
</script>
