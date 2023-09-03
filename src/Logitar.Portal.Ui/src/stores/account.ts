import { ref } from "vue";
import { defineStore } from "pinia";
import type { User } from "@/types/users";

export const useAccountStore = defineStore(
  "account",
  () => {
    const authenticated = ref<User>();

    function signIn(user: User): void {
      authenticated.value = user;
    }
    function signOut(): void {
      authenticated.value = undefined;
    }

    return { authenticated, signIn, signOut };
  },
  {
    persist: true,
  }
);
