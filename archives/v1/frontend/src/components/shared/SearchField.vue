<template>
  <form-field id="search" label="search.label" placeholder="search.placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'SearchField',
  props: {
    value: {}
  }
}
</script>
