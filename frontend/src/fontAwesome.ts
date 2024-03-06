import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faAt,
  faBan,
  faCheck,
  faChessRook,
  faChevronLeft,
  faCircleInfo,
  faClipboard,
  faCog,
  faEdit,
  faEnvelope,
  faEye,
  faEyeSlash,
  faFloppyDisk,
  faHistory,
  faHome,
  faIdCard,
  faKey,
  faLanguage,
  faLock,
  faLockOpen,
  faPaperPlane,
  faPhone,
  faPlus,
  faRobot,
  faRotate,
  faSearch,
  faStar,
  faTimes,
  faTrash,
  faTriangleExclamation,
  faUser,
  faUserClock,
  faUserGroup,
  faUserSlash,
  faUsers,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faAt,
  faBan,
  faCheck,
  faChessRook,
  faChevronLeft,
  faCircleInfo,
  faClipboard,
  faCog,
  faEdit,
  faEnvelope,
  faEye,
  faEyeSlash,
  faFloppyDisk,
  faHistory,
  faHome,
  faIdCard,
  faKey,
  faLanguage,
  faLock,
  faLockOpen,
  faPaperPlane,
  faPhone,
  faPlus,
  faRobot,
  faRotate,
  faSearch,
  faStar,
  faTimes,
  faTrash,
  faTriangleExclamation,
  faUser,
  faUserClock,
  faUserGroup,
  faUserSlash,
  faUsers,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
