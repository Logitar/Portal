import { defineStore } from "pinia";
import { ref } from "vue";

import type { Realm } from "@/types/realms";
import type { Session } from "@/types/sessions";
import type { User } from "@/types/users";

export const useAccountStore = defineStore(
  "account",
  () => {
    const authenticated = ref<User>();
    const currentRealm = ref<Realm>();
    const currentSession = ref<Session>();

    function setRealm(realm: Realm): void {
      currentRealm.value = realm;
    }
    function setUser(user: User): void {
      authenticated.value = user;
    }
    function signIn(session: Session): void {
      currentSession.value = session;
      setUser(session.user);
    }
    function signOut(): void {
      authenticated.value = undefined;
      currentRealm.value = undefined;
      currentSession.value = undefined;
    }

    return { authenticated, currentRealm, currentSession, setRealm, setUser, signIn, signOut };
  },
  {
    persist: true,
  },
);
