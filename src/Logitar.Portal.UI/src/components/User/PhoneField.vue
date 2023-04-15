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
    <b-input-group-append v-if="verified">
      <b-input-group-text class="bg-info text-white"><font-awesome-icon icon="check" />&nbsp;{{ $t('user.phone.verified') }}</b-input-group-text>
    </b-input-group-append>
  </form-field>
</template>

<script>
export default {
  name: 'PhoneField',
  props: {
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
    value: {},
    verified: {
      type: Boolean,
      default: false
    }
  }
}
</script>
