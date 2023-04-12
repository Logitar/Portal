<template>
  <p>
    <status-info :actor="model && model.createdBy" :date="new Date(this.model ? this.model.createdAt : this.createdAt)" dateFormat="statusDetail.createdAt" />
    <template v-if="(model && model.updatedAt) || updatedAt">
      <br />
      <status-info :actor="model && model.updatedBy" :date="new Date(this.model ? this.model.updatedAt : this.updatedAt)" dateFormat="statusDetail.updatedAt" />
    </template>
  </p>
</template>

<script>
export default {
  name: 'StatusDetail',
  props: {
    createdAt: {
      type: Date,
      default: null
    },
    model: {
      type: Object,
      default: null
    },
    updatedAt: {
      type: Date,
      default: null
    }
  }
}
</script>
