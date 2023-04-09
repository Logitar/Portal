<template>
  <form-url :id="id" :label="label" :placeholder="placeholder" :required="required" :validate="validate" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'ProfileField',
  props: {
    id: {
      type: String,
      default: 'profile'
    },
    label: {
      type: String,
      default: 'user.profile.label'
    },
    placeholder: {
      type: String,
      default: 'user.profile.placeholder'
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
