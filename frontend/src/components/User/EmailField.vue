<template>
  <form-field
    :disabled="disabled"
    :id="id"
    :label="label"
    :maxLength="validate ? 255 : null"
    :placeholder="placeholder"
    :ref="id"
    :required="required"
    :rules="{ email: validate }"
    type="email"
    :value="value"
    @input="$emit('input', $event)"
  >
    <b-input-group-append v-if="confirmed">
      <b-input-group-text class="bg-info text-white"><font-awesome-icon icon="check" />&nbsp;{{ $t('user.email.confirmed') }}</b-input-group-text>
    </b-input-group-append>
  </form-field>
</template>

<script>
export default {
  name: 'EmailField',
  props: {
    confirmed: {
      type: Boolean,
      default: false
    },
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'email'
    },
    label: {
      type: String,
      default: 'user.email.label'
    },
    placeholder: {
      type: String,
      default: 'user.email.placeholder'
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
