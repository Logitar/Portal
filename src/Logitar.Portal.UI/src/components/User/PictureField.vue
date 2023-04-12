<template>
  <form-url :id="id" :label="label" :placeholder="placeholder" :required="required" :validate="validate" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'PictureField',
  props: {
    id: {
      type: String,
      default: 'picture'
    },
    label: {
      type: String,
      default: 'user.picture.label'
    },
    placeholder: {
      type: String,
      default: 'user.picture.placeholder'
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
