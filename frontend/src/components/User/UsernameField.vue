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
  },
  methods: {
    focus() {
      this.$refs[this.id].focus()
    }
  }
}
</script>
