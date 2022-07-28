<template>
  <form-field
    :id="id"
    :label="label"
    :minLength="validate ? 8 : null"
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
    validate: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  computed: {
    allRules() {
      return {
        ...this.rules,
        password: this.validate
      }
    }
  },
  methods: {
    focus() {
      this.$refs[this.id].focus()
    }
  }
}
</script>
