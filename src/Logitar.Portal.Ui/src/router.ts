import { createRouter, createWebHistory } from "vue-router";
import { isConfigurationInitialized } from "./api/configuration";
import { useAccountStore } from "@/stores/account";
import { useConfigurationStore } from "@/stores/configuration";

const router = createRouter({
  history: createWebHistory(import.meta.env.MODE === "development" ? import.meta.env.BASE_URL : "/app"),
  routes: [
    {
      name: "Dashboard",
      path: "/",
      // route level code-splitting
      // this generates a separate chunk (Dashboard.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/DashboardView.vue"),
    },
    // Configuration
    {
      name: "ConfigurationEdit",
      path: "/configuration",
      component: () => import("./views/configuration/ConfigurationEdit.vue"),
    },
    {
      name: "Setup",
      path: "/setup",
      component: () => import("./views/configuration/ConfigurationInit.vue"),
      meta: { isPublic: true },
    },
    // Users
    {
      name: "Profile",
      path: "/user/profile",
      component: () => import("./views/users/ProfileView.vue"),
    },
    {
      name: "SignIn",
      path: "/user/sign-in",
      component: () => import("./views/users/SignIn.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/user/sign-out",
      component: () => import("./views/users/SignOut.vue"),
    },
    // Realms
    {
      name: "RealmList",
      path: "/realms",
      component: () => import("./views/realms/RealmList.vue"),
    },
    {
      name: "RealmEdit",
      path: "/realms/:uniqueSlug",
      component: () => import("./views/realms/RealmEdit.vue"),
    },
    {
      name: "CreateRealm",
      path: "/create-realm",
      component: () => import("./views/realms/RealmEdit.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      component: () => import("./views/NotFound.vue"),
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const configuration = useConfigurationStore();
  if (typeof configuration.isInitialized === "undefined") {
    const result = await isConfigurationInitialized();
    configuration.isInitialized = result.isInitialized;
  }
  if (configuration.isInitialized === false && to.name !== "Setup") {
    return { name: "Setup" };
  } else if (configuration.isInitialized === true && to.name === "Setup") {
    return { name: "Dashboard" };
  }

  const account = useAccountStore();
  if (!to.meta.isPublic && !account.authenticated) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;
