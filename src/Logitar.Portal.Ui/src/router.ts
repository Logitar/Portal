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
    // Account
    {
      name: "Profile",
      path: "/user/profile",
      component: () => import("./views/account/ProfileView.vue"),
    },
    {
      name: "SignIn",
      path: "/user/sign-in",
      component: () => import("./views/account/SignIn.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/user/sign-out",
      component: () => import("./views/account/SignOut.vue"),
    },
    // API keys
    {
      name: "ApiKeyList",
      path: "/api-keys",
      component: () => import("./views/apiKeys/ApiKeyList.vue"),
    },
    {
      name: "ApiKeyEdit",
      path: "/api-keys/:id",
      component: () => import("./views/apiKeys/ApiKeyEdit.vue"),
    },
    {
      name: "CreateApiKey",
      path: "/create-api-key",
      component: () => import("./views/apiKeys/ApiKeyEdit.vue"),
    },
    // Dictionaries
    {
      name: "DictionaryList",
      path: "/dictionaries",
      component: () => import("./views/dictionaries/DictionaryList.vue"),
    },
    {
      name: "DictionaryEdit",
      path: "/dictionaries/:id",
      component: () => import("./views/dictionaries/DictionaryEdit.vue"),
    },
    {
      name: "CreateDictionary",
      path: "/create-dictionary",
      component: () => import("./views/dictionaries/DictionaryEdit.vue"),
    },
    // Messages
    {
      name: "MessageList",
      path: "/messages",
      component: () => import("./views/messages/MessageList.vue"),
    },
    {
      name: "MessageView",
      path: "/messages/:id",
      component: () => import("./views/messages/MessageView.vue"),
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
    // Roles
    {
      name: "RoleList",
      path: "/roles",
      component: () => import("./views/roles/RoleList.vue"),
    },
    {
      name: "RoleEdit",
      path: "/roles/:id",
      component: () => import("./views/roles/RoleEdit.vue"),
    },
    {
      name: "CreateRole",
      path: "/create-role",
      component: () => import("./views/roles/RoleEdit.vue"),
    },
    // Senders
    {
      name: "SenderList",
      path: "/senders",
      component: () => import("./views/senders/SenderList.vue"),
    },
    {
      name: "SenderEdit",
      path: "/senders/:id",
      component: () => import("./views/senders/SenderEdit.vue"),
    },
    {
      name: "CreateSender",
      path: "/create-sender",
      component: () => import("./views/senders/SenderEdit.vue"),
    },
    // Sessions
    {
      name: "SessionList",
      path: "/sessions",
      component: () => import("./views/sessions/SessionList.vue"),
    },
    {
      name: "SessionEdit",
      path: "/sessions/:id",
      component: () => import("./views/sessions/SessionEdit.vue"),
    },
    // Templates
    {
      name: "TemplateList",
      path: "/templates",
      component: () => import("./views/templates/TemplateList.vue"),
    },
    {
      name: "TemplateEdit",
      path: "/templates/:id",
      component: () => import("./views/templates/TemplateEdit.vue"),
    },
    {
      name: "CreateTemplate",
      path: "/create-template",
      component: () => import("./views/templates/TemplateEdit.vue"),
    },
    // Tokens
    {
      name: "Tokens",
      path: "/tokens",
      component: () => import("./views/TokenView.vue"),
    },
    // Users
    {
      name: "UserList",
      path: "/users",
      component: () => import("./views/users/UserList.vue"),
    },
    {
      name: "UserEdit",
      path: "/users/:id",
      component: () => import("./views/users/UserEdit.vue"),
    },
    {
      name: "CreateUser",
      path: "/create-user",
      component: () => import("./views/users/UserEdit.vue"),
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
