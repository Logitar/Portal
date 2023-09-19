import VueI18nPlugin from "@intlify/unplugin-vue-i18n/vite";
import vue from "@vitejs/plugin-vue";
import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import { resolve, dirname } from "node:path";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    VueI18nPlugin({
      include: resolve(dirname(fileURLToPath(import.meta.url)), "./i18n/**"),
      jitCompilation: true,
    }),
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  build: {
    outDir: "../Logitar.Portal.Web/wwwroot",
    emptyOutDir: true,
    rollupOptions: {
      output: {
        entryFileNames: `assets/[name].js`,
        chunkFileNames: `assets/[name].js`,
        assetFileNames: `assets/[name].[ext]`,
      },
    },
  },
  server: {
    port: 7745,
  },
});
