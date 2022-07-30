import Vue from 'vue'
import { library } from '@fortawesome/fontawesome-svg-core'
import {
  faAt,
  faBan,
  faChessRook,
  faClipboard,
  faCog,
  faDice,
  faEnvelope,
  faExternalLinkAlt,
  faHome,
  faIdCard,
  faKey,
  faPaperPlane,
  faPlus,
  faSave,
  faSignInAlt,
  faSignOutAlt,
  faStar,
  faSyncAlt,
  faTimes,
  faTrashAlt,
  faUser,
  faUserAltSlash,
  faUsers,
  faVial
} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

library.add(
  faAt,
  faBan,
  faChessRook,
  faClipboard,
  faCog,
  faDice,
  faEnvelope,
  faExternalLinkAlt,
  faHome,
  faIdCard,
  faKey,
  faPaperPlane,
  faPlus,
  faSave,
  faSignInAlt,
  faSignOutAlt,
  faStar,
  faSyncAlt,
  faTimes,
  faTrashAlt,
  faUser,
  faUserAltSlash,
  faUsers,
  faVial
)

Vue.component('font-awesome-icon', FontAwesomeIcon)
