<template>
  <form-field
    :disabled="disabled"
    :id="id"
    :label="label"
    :maxLength="validate ? 256 : null"
    :placeholder="placeholder"
    :required="required"
    :rules="{ email: validate }"
    type="email"
    :value="value"
    @input="$emit('input', $event)"
  >
    <b-input-group-append v-if="confirmed">
      <b-input-group-text class="bg-info text-white">{{ $t('user.email.confirmed') }}</b-input-group-text>
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
  }
}
</script>
