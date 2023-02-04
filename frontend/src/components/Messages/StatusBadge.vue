<template>
  <b-badge :variant="variant">{{ $t(`messages.status.options.${status}`) }}</b-badge>
</template>

<script>
export default {
  name: 'StatusBadge',
  props: {
    message: {
      type: Object,
      required: true
    }
  },
  computed: {
    status() {
      return this.message.hasSucceeded ? 'Sent' : this.message.hasErrors ? 'Failed' : 'Unsent'
    },
    variant() {
      switch (this.status) {
        case 'Failed':
          return 'danger'
        case 'Sent':
          return 'success'
        case 'Unsent':
          return 'warning'
      }
      return null
    }
  }
}
</script>
