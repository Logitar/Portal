<template>
  <form-field
    :id="id"
    :label="label"
    :placeholder="placeholder"
    :ref="id"
    :required="required"
    :rules="allRules"
    type="password"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: 'password'
    },
    label: {
      type: String,
      default: 'user.password.label'
    },
    placeholder: {
      type: String,
      default: 'user.password.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    rules: {
      type: Object,
      default: null
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
      const rules = { ...this.rules }
      if (this.validate && this.settings) {
        const { requireDigit, requireLowercase, requireNonAlphanumeric, requireUppercase, requiredLength, requiredUniqueChars } = this.settings
        rules.min = requiredLength
        rules.require_digit = requireDigit
        rules.require_lowercase = requireLowercase
        rules.require_non_alphanumeric = requireNonAlphanumeric
        rules.require_uppercase = requireUppercase
        rules.required_unique_chars = requiredUniqueChars
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
