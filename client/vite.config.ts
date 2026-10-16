import { globSync } from "node:fs";
import { resolve } from "node:path";
import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

const clientRoot = fileURLToPath(new URL(".", import.meta.url));

// The existing Project and IIS Express profiles use different HTTP/HTTPS ports.
const backendOrigins = process.env.FRONTEND_BACKEND_URL
  ? [process.env.FRONTEND_BACKEND_URL]
  : [
      "http://localhost:44395",
      "https://localhost:44396",
      "http://localhost:50212",
      "https://localhost:44395",
    ];
const assetOrigin = process.env.FRONTEND_ASSET_URL ?? "http://localhost:5178";

export default defineConfig({
  plugins: [
    {
      name: "razor-tailwind-refresh",
      enforce: "pre",
      hotUpdate({ file, timestamp }) {
        if (!file.endsWith(".cshtml")) return;
        // Tailwind otherwise requests a full reload for a Razor source file,
        // before dotnet watch has compiled it. Refresh only the stylesheet here;
        // dotnet watch owns the document reload after applying Razor changes.
        const styles = this.environment.moduleGraph.getModulesByFile(
          fileURLToPath(new URL("./src/styles.css", import.meta.url)),
        );
        for (const module of styles ?? []) {
          this.environment.moduleGraph.invalidateModule(
            module,
            new Set(),
            timestamp,
            true,
          );
        }
        return [...(styles ?? [])];
      },
    },
    react(),
    tailwindcss(),
  ],
  base: "./",
  publicDir: false,
  server: {
    host: "localhost",
    port: Number(new URL(assetOrigin).port || 80),
    strictPort: true,
    origin: assetOrigin,
    cors: { origin: backendOrigins },
    watch: { ignored: ["**/bin/**", "**/obj/**"] },
  },
  build: {
    outDir: "../Purchasing.Mvc/wwwroot/modern",
    emptyOutDir: true,
    manifest: true,
    modulePreload: { polyfill: false },
    rollupOptions: {
      input: [
        resolve(clientRoot, "src/styles.css"),
        // Each page script is an entry; imported helpers belong in components/.
        ...globSync("src/pages/**/*.{ts,tsx}", {
          cwd: clientRoot,
          exclude: ["**/*.d.ts"],
        })
          .sort()
          .map((file) => resolve(clientRoot, file)),
      ],
    },
  },
});
