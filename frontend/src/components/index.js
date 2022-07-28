import Navbar from './Navbar.vue'

export default {
  ApiKeyEdit: () => import(/* webpackChunkName: "apiKeyEdit" */ './ApiKeys/ApiKeyEdit.vue'),
  ApiKeyList: () => import(/* webpackChunkName: "apiKeyList" */ './ApiKeys/ApiKeyList.vue'),
  Home: () => import(/* webpackChunkName: "home" */ './Home.vue'),
  Navbar,
  Profile: () => import(/* webpackChunkName: "profile" */ './Account/Profile.vue'),
  RealmEdit: () => import(/* webpackChunkName: "realmEdit" */ './Realms/RealmEdit.vue'),
  RealmList: () => import(/* webpackChunkName: "realmList" */ './Realms/RealmList.vue'),
  SignIn: () => import(/* webpackChunkName: "signIn" */ './Account/SignIn.vue'),
  TemplateEdit: () => import(/* webpackChunkName: "templateEdit" */ './Templates/TemplateEdit.vue'),
  TemplateList: () => import(/* webpackChunkName: "templateList" */ './Templates/TemplateList.vue'),
  UserEdit: () => import(/* webpackChunkName: "userEdit" */ './User/UserEdit.vue'),
  UserList: () => import(/* webpackChunkName: "userList" */ './User/UserList.vue')
}
