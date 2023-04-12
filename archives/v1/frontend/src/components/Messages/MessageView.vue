<template>
  <b-container>
    <h1 v-t="'messages.viewTitle'" />
    <b-tabs content-class="mt-3">
      <status-tab :message="message" />
      <content-tab :message="message" />
      <recipient-tab :message="message" />
    </b-tabs>
  </b-container>
</template>

<script>
import ContentTab from './MessageView/ContentTab.vue'
import RecipientTab from './MessageView/RecipientTab.vue'
import StatusTab from './MessageView/StatusTab.vue'

export default {
  name: 'MessageView',
  components: {
    ContentTab,
    StatusTab,
    RecipientTab
  },
  props: {
    json: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      message: null
    }
  },
  created() {
    this.message = JSON.parse(this.json)
  }
}
</script>
