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
  faCog,
  faEye,
  faEyeSlash,
  faFloppyDisk,
  faHistory,
  faHome,
  faKey,
  faLock,
  faLockOpen,
  faPaperPlane,
  faPhone,
  faPlus,
  faRobot,
  faRotate,
  faSearch,
  faTimes,
  faTrash,
  faTriangleExclamation,
  faUser,
  faUserGroup,
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
  faCog,
  faEye,
  faEyeSlash,
  faFloppyDisk,
  faHistory,
  faHome,
  faKey,
  faLock,
  faLockOpen,
  faPaperPlane,
  faPhone,
  faPlus,
  faRobot,
  faRotate,
  faSearch,
  faTimes,
  faTrash,
  faTriangleExclamation,
  faUser,
  faUserGroup,
  faVial
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
