import { spawn } from "node:child_process";
import { fileURLToPath } from "node:url";

// Set this before dotnet watch starts. Legacy NuGet assets are virtual wwwroot
// files; Vite handles CSS/JS updates while watch keeps C#/Razor hot reload.
const child = spawn(
  "dotnet",
  ["watch", "--project", "Purchasing.Mvc", "run", "--launch-profile", "http-cli", ...process.argv.slice(2)],
  {
    cwd: fileURLToPath(new URL("../", import.meta.url)),
    stdio: "inherit",
    env: { ...process.env, DOTNET_WATCH_SUPPRESS_STATIC_FILE_HANDLING: "true" },
  },
);

child.on("error", (error) => {
  console.error(`Could not start dotnet watch: ${error.message}`);
  process.exitCode = 1;
});
child.on("exit", (code) => { process.exitCode = code ?? 1; });
// Forward explicit termination and wait for .NET to finish its cleanup.
for (const signal of ["SIGINT", "SIGTERM"]) {
  process.on(signal, () => child.kill(signal));
}
