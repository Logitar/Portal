<template>
  <p>
    {{ $t('statusDetail.createdAt', { date: $d(createdAt, 'medium') }) }}
    <template v-if="updatedAt">
      <br />
      {{ $t('statusDetail.updatedAt', { date: $d(updatedAt, 'medium') }) }}
    </template>
  </p>
</template>

<script>
export default {
  name: 'StatusDetail',
  props: {
    createdAt: {
      type: Date,
      required: true
    },
    updatedAt: {
      type: Date,
      default: null
    }
  }
}
</script>
