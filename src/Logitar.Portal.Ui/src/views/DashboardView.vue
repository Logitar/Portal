<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <main class="container">
    <div class="row vh-100 text-center align-items-center">
      <div class="col jumbotron">
        <h1 class="display-4">{{ t("brand") }}</h1>
        <p class="lead">{{ t("dashboard.title") }}</p>
        <hr class="my-4" />
        <p>{{ t("dashboard.help") }}</p>
        <p class="lead">
          <RouterLink :to="{ name: 'RealmList' }" class="btn btn-primary btn-lg" role="button">
            <font-awesome-icon icon="fas fa-chess-rook" /> {{ t("dashboard.link") }}
          </RouterLink>
        </p>
      </div>
    </div>
  </main>
</template>

<style scoped>
@media (min-width: 576px) {
  .jumbotron {
    padding: 4rem 2rem;
  }
}

.jumbotron {
  padding: 2rem 1rem;
  margin-bottom: 2rem;
  background-color: #e9ecef;
  border-radius: 0.3rem;
  text-align: center;
}
</style>
