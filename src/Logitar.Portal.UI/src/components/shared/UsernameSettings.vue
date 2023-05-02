<template>
  <div>
    <h5 v-t="'username.title'" />
    <form-field
      id="allowedCharacters"
      label="username.allowedCharacters.label"
      placeholder="username.allowedCharacters.placeholder"
      :value="value.allowedCharacters"
      @input="onInput({ allowedCharacters: $event })"
    />
  </div>
</template>

<script>
export default {
  name: 'UsernameSettings',
  props: {
    value: {}
  },
  methods: {
    onInput(changes) {
      const value = { ...this.value }
      for (const [key, change] of Object.entries(changes)) {
        value[key] = change
      }
      this.$emit('input', value)
    }
  }
}
</script>
