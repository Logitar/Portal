import type { App } from "vue";
import JsonViewer from "vue3-json-viewer";

import "vue3-json-viewer/dist/index.css";

export default function (app: App) {
  app.use(JsonViewer);
}
