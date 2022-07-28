import Vue from 'vue'
import { library } from '@fortawesome/fontawesome-svg-core'
import {
  faBan,
  faChessRook,
  faClipboard,
  faCog,
  faDice,
  faHome,
  faIdCard,
  faKey,
  faPlus,
  faSave,
  faSignInAlt,
  faSignOutAlt,
  faSyncAlt,
  faTimes,
  faTrashAlt,
  faUser,
  faUsers
} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

library.add(
  faBan,
  faChessRook,
  faClipboard,
  faCog,
  faDice,
  faHome,
  faIdCard,
  faKey,
  faPlus,
  faSave,
  faSignInAlt,
  faSignOutAlt,
  faSyncAlt,
  faTimes,
  faTrashAlt,
  faUser,
  faUsers
)

Vue.component('font-awesome-icon', FontAwesomeIcon)
