# Frontend assets and gradual page migration

Away Status is the first page using the modern Razor layout. Other pages still use
`_Layout.cshtml` and their existing assets. The page remains a normal MVC form;
React is available for future components but is not loaded by Away Status.

## Develop

Install the .NET SDK from `global.json` and Node.js 22.18 or newer. Node and npm
must be on your PATH, including the PATH used by Visual Studio. Restart Visual
Studio after installing Node.

From the repository root:

```sh
npm start
```

This runs `dotnet watch` with the `http-cli` profile at `http://localhost:44395`.
Purchasing starts Vite at `http://localhost:5178`. Visit `/User/AwayStatus` using
your normal development login and configuration. The Debug build runs `npm ci`
on first checkout and when `client/package.json` or its lockfile changes. There
is no root npm install step and no separate frontend terminal.

In Visual Studio, select Purchasing.Mvc as the startup project and use its
Purchasing.Mvc or IIS Express launch profile, then press F5 or Ctrl+F5. Existing
HTTPS and HTTP application ports are preserved. The client source appears under
`Client` in Solution Explorer. The development VS Code launch also enables Vite.

### Development server ownership

`Infrastructure/Vite` in Purchasing.Mvc owns the development server. It runs only
when the environment is `Development` and `Vite:UseDevServer` is true. The tracked
development profiles set that flag. It starts Node directly, waits until Vite is
listening before starting the application, and forwards frontend output to the
application log. Node runs Vite in process and watches its connection to .NET, so
an abrupt debugger stop also closes the server.

An occupied Vite port or startup timeout fails application startup with a useful
error. The launcher does not stop or adopt an existing server on that port.
Stop the other application or choose another port using `Vite:DevServerUrl`.
The frontend configuration accepts the existing localhost development origins.
Set `FRONTEND_BACKEND_URL` to the application origin when running on a different
local port.

`npm start` sets `DOTNET_WATCH_SUPPRESS_STATIC_FILE_HANDLING=true` before running
.NET's watcher. Legacy NuGet assets are not physical files under `wwwroot`; Vite
handles CSS/JS updates. C# and Razor hot reload and browser refresh remain enabled.

### Run with built assets

To run without Vite, build the frontend and omit the development launch profile:

```sh
npm --prefix client ci
npm --prefix client run build
dotnet run --project Purchasing.Mvc --no-launch-profile -- --environment Development --urls http://localhost:44395
```

Alternatively set `Vite:UseDevServer=false` in a local IDE launch configuration.
The application then uses the same manifest and files as a published build. A
plain `dotnet run` that selects the standard development profile starts Vite.
Production ignores development-server settings and never starts Node.
`npm --prefix client run dev` is an alias for root `npm start`.

CSS updates in place. Razor updates are owned by `dotnet watch`, which reloads the
page after compilation. A small Vite hook prevents Tailwind's Razor-source watcher
from reloading the page before .NET has applied the changes. Check both behaviors
when upgrading Vite or Tailwind.

## Share styles and markup

`client/src/styles.css` is the common CSS entry. It contains the Tailwind import,
daisyUI configuration, and the `purchasing` light theme. Change shared colors,
radii, and component defaults there. It scans all Razor views for class names;
scanning a view does not load CSS into that view. Use complete class names in Razor
and React so Tailwind can discover them.

`_ModernLayout.cshtml` loads that CSS independently:

```cshtml
<vite-entry src="src/styles.css" />
```

The layout uses `_ModernNavigation` and an alternate ServiceMessages view. It
preserves the existing navigation destinations and role rules, order search,
account links, status messages, and footer. The default service-message rendering
and legacy layout are unchanged.

To convert another page in a later change:

1. Set `Layout = "_ModernLayout"` in that page.
2. Rewrite its markup and partials with shared daisyUI components and Tailwind.
3. Preserve its controller, form field names, authorization, validation, and links.
4. Add a page entry to `client/vite.config.ts` only if it needs browser behavior.
5. Load that entry in the page's `Scripts` section using `vite-entry`.

The modern layout renders the page h1 from `ViewBag.Title`, optional subtitle, and
optional `SubNav`. It does not load jQuery or legacy CSS. Old widgets need explicit
migration; loading both global stylesheets is not a compatibility strategy.

## Calendar and forms

Away Status uses a native `type="date"` field as its submitted form control.
The reusable `_Calendar` partial adds a Cally picker, styled by daisyUI, only after
`client/src/components/date-picker.ts` has loaded and registered the components.
If JavaScript or the bundle fails, the native input and submit buttons still work.
Dates stay `YYYY-MM-DD` strings in the browser; there is no UTC conversion.

Return from absence is a separate normal POST with an empty date. The narrow
`AwayStatusViewModel` prevents unrelated User validation from interfering and
rejects malformed dates before persistence. Blank, past, current, and future dates
retain the existing server semantics. SQL remains responsible for `IsAway`.

## Add React when a page needs it

React, React DOM, TypeScript, and Vite's React plugin are configured. Create a TSX
entry, mount into an explicitly owned element with `createRoot`, and register the
entry in `vite.config.ts`. Load it with:

```cshtml
@section Scripts {
    <vite-entry src="src/pages/example.tsx" react="true" />
}
```

The explicit React flag installs the Fast Refresh preamble in development. Plain
JavaScript and CSS entries do not need that runtime. Production tags are resolved
from the build manifest; React is included only when an entry imports it. The
shared theme works for React `className` and Razor `class` alike.

## Build and publish

`dotnet publish Purchasing.Mvc -c Release` runs `npm ci` and the TypeScript/Vite
build, then copies `wwwroot/modern`, including `.vite/manifest.json`, into the
publish output. Node is a build dependency only. Azure Pipelines selects Node 22;
the deployment path is unchanged. Existing legacy bundling still runs.

Vite output is excluded from .NET's earlier static-asset discovery so repeated
publishes do not copy stale hashed filenames. The helper emits standard CSS links
and module scripts and includes CSS from imported chunks. Shared assets are
emitted once per page. Built imports are relative, and helper links respect
`Request.PathBase`.

## Verify

```sh
npm --prefix client run build
dotnet test Purchasing.Tests --filter 'FullyQualifiedName~AwayStatus|FullyQualifiedName~ViteEntryTagHelper|FullyQualifiedName~ViteDevelopmentServer'
dotnet publish Purchasing.Mvc -c Release
```

The focused MVC tests use an isolated host with real Razor, model binding,
authorization filters and antiforgery, plus in-memory repositories. They do not
connect to a database or CAS. Use test data for browser checks that submit forms.
