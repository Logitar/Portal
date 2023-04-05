<template>
  <form-select id="count" label="count" :options="options" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'CountSelect',
  props: {
    desc: {
      type: Boolean,
      default: false
    },
    options: {
      type: Array,
      default: () => [10, 25, 50, 100]
    },
    value: {}
  }
}
</script>
