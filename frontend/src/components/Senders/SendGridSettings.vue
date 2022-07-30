<template>
  <div>
    <h3 v-t="'senders.sendGrid.title'" />
    <form-field
      id="apiKey"
      label="senders.sendGrid.apiKey.label"
      :maxLength="100"
      placeholder="senders.sendGrid.apiKey.placeholder"
      required
      v-model="apiKey"
    />
  </div>
</template>

<script>
export default {
  name: 'SendGridSettings',
  props: {
    value: {}
  },
  data() {
    return {
      apiKey: null
    }
  },
  computed: {
    settings() {
      return {
        ApiKey: this.apiKey
      }
    }
  },
  methods: {
    setModel(settings) {
      this.apiKey = settings.ApiKey
    }
  },
  watch: {
    settings: {
      deep: true,
      immediate: true,
      handler(settings) {
        this.$emit('input', settings)
      }
    },
    value: {
      deep: true,
      immediate: true,
      handler(value) {
        if (value) {
          this.setModel(value)
        }
      }
    }
  }
}
</script>
