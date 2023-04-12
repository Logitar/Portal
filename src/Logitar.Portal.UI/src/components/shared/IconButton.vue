<template>
  <b-button :disabled="disabled" :href="href" :size="size" :target="target" :type="type" :variant="variant" @click="$emit('click', $event)">
    <b-spinner v-if="loading" small />
    <font-awesome-icon v-else :icon="icon" />
    {{ $t(text) }}
  </b-button>
</template>

<script>
export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    href: {
      type: String,
      default: null
    },
    icon: {
      type: String,
      required: true
    },
    loading: {
      type: Boolean,
      default: false
    },
    size: {
      type: String,
      default: ''
    },
    target: {
      type: String,
      default: ''
    },
    text: {
      type: String,
      default: ''
    },
    type: {
      type: String,
      default: 'button'
    },
    variant: {
      type: String,
      default: ''
    }
  }
}
</script>
