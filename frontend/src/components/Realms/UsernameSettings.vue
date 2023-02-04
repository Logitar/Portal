<template>
  <div>
    <h5 v-t="'realms.username.title'" />
    <b-row>
      <form-field
        class="col"
        id="allowedUsernameCharacters"
        label="realms.username.allowedCharacters.label"
        placeholder="realms.username.allowedCharacters.placeholder"
        :value="value.allowedCharacters"
        @input="onInput({ allowedCharacters: $event })"
      />
    </b-row>
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
