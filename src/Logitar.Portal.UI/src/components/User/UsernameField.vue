<template>
  <form-field
    :disabled="disabled"
    :id="id"
    :label="label"
    :maxLength="validate ? 256 : null"
    :placeholder="placeholder"
    :ref="id"
    :required="required"
    :rules="allRules"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  name: 'UsernameField',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'username'
    },
    label: {
      type: String,
      default: 'user.username.label'
    },
    placeholder: {
      type: String,
      default: 'user.username.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    settings: {
      type: Object,
      default: null
    },
    validate: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  computed: {
    allRules() {
      const rules = {}
      if (this.validate && this.settings) {
        const { allowedCharacters } = this.settings
        rules.username = allowedCharacters
      }
      return rules
    }
  },
  methods: {
    focus() {
      this.$refs[this.id].focus()
    }
  }
}
</script>
