<template>
  <form-textarea :id="id" :label="label" :placeholder="placeholder" :rows="rows" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'EffectField',
  props: {
    id: {
      type: String,
      default: 'effect'
    },
    label: {
      type: String,
      default: 'effect'
    },
    placeholder: {
      type: String,
      default: ''
    },
    rows: {
      type: Number,
      default: 10
    },
    value: {}
  }
}
</script>
