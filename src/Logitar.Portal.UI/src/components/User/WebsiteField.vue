<template>
  <form-url :id="id" :label="label" :placeholder="placeholder" :required="required" :validate="validate" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'WebsiteField',
  props: {
    id: {
      type: String,
      default: 'website'
    },
    label: {
      type: String,
      default: 'user.website.label'
    },
    placeholder: {
      type: String,
      default: 'user.website.placeholder'
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
