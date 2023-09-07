import { ref } from "vue";
import { defineStore } from "pinia";
import type { Session } from "@/types/sessions";
import type { User } from "@/types/users";

export const useAccountStore = defineStore(
  "account",
  () => {
    const authenticated = ref<User>();
    const currentSession = ref<Session>();

    function setUser(user: User): void {
      authenticated.value = user;
    }
    function signIn(session: Session): void {
      currentSession.value = session;
      setUser(session.user);
    }
    function signOut(): void {
      authenticated.value = undefined;
    }

    return { authenticated, currentSession, setUser, signIn, signOut };
  },
  {
    persist: true,
  }
);
