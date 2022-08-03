import AppFooter from './AppFooter.vue'
import Navbar from './Navbar.vue'

export default {
  ApiKeyEdit: () => import(/* webpackChunkName: "apiKeyEdit" */ './ApiKeys/ApiKeyEdit.vue'),
  ApiKeyList: () => import(/* webpackChunkName: "apiKeyList" */ './ApiKeys/ApiKeyList.vue'),
  AppFooter,
  Home: () => import(/* webpackChunkName: "home" */ './Home.vue'),
  MessageList: () => import(/* webpackChunkName: "messageList" */ './Messages/MessageList.vue'),
  MessageView: () => import(/* webpackChunkName: "messageView" */ './Messages/MessageView.vue'),
  Navbar,
  Profile: () => import(/* webpackChunkName: "profile" */ './Account/Profile.vue'),
  RealmEdit: () => import(/* webpackChunkName: "realmEdit" */ './Realms/RealmEdit.vue'),
  RealmList: () => import(/* webpackChunkName: "realmList" */ './Realms/RealmList.vue'),
  SenderEdit: () => import(/* webpackChunkName: "senderEdit" */ './Senders/SenderEdit.vue'),
  SenderList: () => import(/* webpackChunkName: "senderList" */ './Senders/SenderList.vue'),
  SignIn: () => import(/* webpackChunkName: "signIn" */ './Account/SignIn.vue'),
  TemplateEdit: () => import(/* webpackChunkName: "templateEdit" */ './Templates/TemplateEdit.vue'),
  TemplateList: () => import(/* webpackChunkName: "templateList" */ './Templates/TemplateList.vue'),
  Tokens: () => import(/* webpackChunkName: "tokens" */ './Tokens/Tokens.vue'),
  UserEdit: () => import(/* webpackChunkName: "userEdit" */ './User/UserEdit.vue'),
  UserList: () => import(/* webpackChunkName: "userList" */ './User/UserList.vue')
}
