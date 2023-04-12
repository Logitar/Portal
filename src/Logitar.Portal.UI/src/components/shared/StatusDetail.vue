<template>
  <p>
    <status-info :actor="model && model.createdBy" :date="new Date(this.model ? this.model.createdOn : this.createdOn)" dateFormat="statusDetail.createdOn" />
    <template v-if="(model && model.updatedOn) || updatedOn">
      <br />
      <status-info :actor="model && model.updatedBy" :date="new Date(this.model ? this.model.updatedOn : this.updatedOn)" dateFormat="statusDetail.updatedOn" />
    </template>
  </p>
</template>

<script>
export default {
  name: 'StatusDetail',
  props: {
    createdOn: {
      type: Date,
      default: null
    },
    model: {
      type: Object,
      default: null
    },
    updatedOn: {
      type: Date,
      default: null
    }
  }
}
</script>
