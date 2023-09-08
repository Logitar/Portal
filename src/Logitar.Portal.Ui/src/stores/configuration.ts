import { defineStore } from "pinia";
import { ref } from "vue";

import type { Configuration } from "@/types/configuration";

export const useConfigurationStore = defineStore("configuration", () => {
  const configuration = ref<Configuration>();
  const isInitialized = ref<boolean>();
  const toast = ref<string>();

  function initialize(_configuration: Configuration): void {
    configuration.value = _configuration;
    isInitialized.value = true;
    toast.value = "configuration.initialized";
  }

  return { configuration, initialize, isInitialized, toast };
});
