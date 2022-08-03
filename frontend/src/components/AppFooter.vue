<template>
  <footer class="bg-dark">
    <b-container fluid>
      <b-link class="text-light" href="/">
        <img src="@/assets/logo.png" alt="Portal Logo" height="32" />
        &copy; Portal 2022
      </b-link>
    </b-container>
  </footer>
</template>

<script>
export default {
  name: 'AppFooter'
}
</script>

<style scoped>
footer {
  position: absolute;
  bottom: 0;
  width: 100%;
  height: 60px;
  line-height: 60px;
  text-align: center;
}
</style>
