import { createRouter, createWebHistory } from "vue-router";
import HomeView from "./views/HomeView.vue";
import { useAccountStore } from "@/stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      name: "Home",
      path: "/",
      component: HomeView,
      meta: { isPublic: true },
    },
    {
      name: "Configuration",
      path: "/configuration",
      // route level code-splitting
      // this generates a separate chunk (ConfigurationView.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/ConfigurationView.vue"),
    },
    // Users
    {
      name: "Profile",
      path: "/user/profile",
      component: () => import("./views/users/Profile.vue"),
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
      path: "/realms/:id",
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

router.beforeEach((to) => {
  const accountStore = useAccountStore();
  if (!to.meta.isPublic && !accountStore.authenticated) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;
