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
    realm: {
      type: Object,
      default: null
    },
    required: {
      type: Boolean,
      default: false
    },
    rules: {
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
      if (this.validate) {
        const passwordSettings = this.realm
          ? this.realm.passwordSettings
          : {
              requireDigit: true,
              requireLowercase: true,
              requireNonAlphanumeric: true,
              requireUppercase: true,
              requiredLength: 8,
              requiredUniqueChars: 8
            }
        if (passwordSettings) {
          const { requireDigit, requireLowercase, requireNonAlphanumeric, requireUppercase, requiredLength, requiredUniqueChars } = passwordSettings
          rules.min = requiredLength
          rules.require_digit = requireDigit
          rules.require_lowercase = requireLowercase
          rules.require_non_alphanumeric = requireNonAlphanumeric
          rules.require_uppercase = requireUppercase
          rules.required_unique_chars = requiredUniqueChars
        }
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
