<template>
  <b-container>
    <h1 v-t="'tokens.title'" />
    <b-tabs content-class="mt-3">
      <create-token-tab />
      <validate-token-tab />
    </b-tabs>
  </b-container>
</template>

<script>
import CreateTokenTab from './CreateTokenTab.vue'
import ValidateTokenTab from './ValidateTokenTab.vue'

export default {
  name: 'Tokens',
  components: {
    CreateTokenTab,
    ValidateTokenTab
  }
}
</script>
