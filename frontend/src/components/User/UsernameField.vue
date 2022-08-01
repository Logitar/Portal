<template>
  <form-field
    :disabled="disabled"
    id="username"
    :label="label"
    :maxLength="validate ? 256 : null"
    :placeholder="placeholder"
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
    label: {
      type: String,
      default: 'user.username.label'
    },
    placeholder: {
      type: String,
      default: 'user.username.placeholder'
    },
    realm: {
      type: Object,
      default: null
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
  computed: {
    allRules() {
      const rules = {}
      if (this.validate) {
        if (this.realm?.allowedUsernameCharacters) {
          rules.username = this.realm.allowedUsernameCharacters
        } else if (!this.realm) {
          rules.username = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'
        }
      }
      return rules
    }
  }
}
</script>
