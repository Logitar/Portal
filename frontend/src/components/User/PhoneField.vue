<template>
  <form-field
    :id="id"
    :label="label"
    :maxLength="validate ? 50 : null"
    :placeholder="placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  >
    <b-input-group-append v-if="confirmed">
      <b-input-group-text class="bg-info text-white">{{ $t('user.phone.confirmed') }}</b-input-group-text>
    </b-input-group-append>
  </form-field>
</template>

<script>
export default {
  name: 'PhoneField',
  props: {
    confirmed: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'phone'
    },
    label: {
      type: String,
      default: 'user.phone.label'
    },
    placeholder: {
      type: String,
      default: 'user.phone.placeholder'
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
