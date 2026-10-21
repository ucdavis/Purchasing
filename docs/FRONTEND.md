# Frontend assets and gradual page migration

Away Status and Order Review have both legacy and modern Razor views. Configuration
selects the modern view for allowed users; the default is the legacy interface.
Other pages still use `_Layout.cshtml` and their existing assets. Both interfaces
use the same controllers and form handling. React is available for future
components but is not loaded by these pages.

## Select the interface

The checked-in `appsettings.json` defaults are:

```json
"ModernUi": {
  "Enabled": false,
  "AllowedLogins": []
}
```

Both settings must permit the modern interface. When `Enabled` is true, an
authenticated user's login must match an entry in `AllowedLogins`, ignoring case.
Disabled, empty, or unlisted means legacy. These settings only select views;
existing authorization still controls access to each action.

To preview the modern Away Status view locally, run from the repository root,
replacing `your-login-id` with the login used to sign in:

```sh
dotnet user-secrets set "ModernUi:Enabled" "true" --project Purchasing.Mvc
dotnet user-secrets set "ModernUi:AllowedLogins:0" "your-login-id" --project Purchasing.Mvc
```

To switch everyone back to the legacy interface, set `ModernUi:Enabled` to false.
For the local override:

```sh
dotnet user-secrets set "ModernUi:Enabled" "false" --project Purchasing.Mvc
```

Deployment environment variables use `ModernUi__Enabled` and indexed entries such
as `ModernUi__AllowedLogins__0`. Restart the application after changing deployment
configuration. Keep personal login IDs out of tracked settings.

`Infrastructure/ModernUi/ModernUiViewLocationExpander.cs` selects a view under
`Views/Modern/{Controller}/{Action}.cshtml` when the user is allowed and that view
exists. Otherwise MVC uses the original view and layout. Controllers continue to
return `View(model)`, including when validation fails on POST. There are no separate
routes, controller actions, or user-preference records. This convention applies to
normal controller main views. Explicit view paths and shared partials are not
automatically replaced.

## Develop

Install the .NET SDK from `global.json` and Node.js 22.18 or newer. Node and npm
must be on your PATH, including the PATH used by Visual Studio. Restart Visual
Studio after installing Node.

From the repository root:

```sh
npm start
```

This runs `dotnet watch` with the `http-cli` profile at `http://localhost:44395`.
Purchasing starts Vite at `http://localhost:5178`. Enable the modern interface for
your login as described above, then visit `/User/AwayStatus`. Debug and Release builds run
`npm ci` on first checkout and when `client/package.json` or its lockfile changes.
There is no root npm install step and no separate frontend terminal.

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

After requesting forced termination, the host waits up to five seconds for its
Node process to exit before completing shutdown. If that wait expires, shutdown
raises a timeout error.

An occupied Vite port or startup timeout fails application startup with a useful
error. The launcher does not stop or adopt an existing server on that port.
Stop the other application or choose another port using `Vite:DevServerUrl`.
The frontend configuration accepts the existing localhost development origins.
Set `FRONTEND_BACKEND_URL` to the application origin when running on a different
local port.

`npm start` sets `DOTNET_WATCH_SUPPRESS_STATIC_FILE_HANDLING=true` before running
.NET's watcher. Legacy NuGet assets are not physical files under `wwwroot`; Vite
handles modern CSS/JS updates. Refresh the browser manually after editing legacy
CSS or JavaScript. C# and Razor hot reload and browser refresh remain enabled.

### Run with built assets

To run without Vite, build the frontend and omit the development launch profile:

```sh
npm --prefix client ci
npm --prefix client run build
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Purchasing.Mvc --no-launch-profile --urls http://localhost:44395
```

Alternatively set `Vite:UseDevServer=false` in a local IDE launch configuration.
The application then uses the same manifest and files as a published build. A
plain `dotnet run` that selects the standard development profile starts Vite.
Production ignores development-server settings and never starts Node.
`npm --prefix client run dev` is an alias for root `npm start`.

Modern CSS updates in place. Razor updates are owned by `dotnet watch`, which
reloads the page after compilation. A small Vite hook prevents Tailwind's
Razor-source watcher from reloading the page before .NET has applied the changes.
Check both behaviors when upgrading Vite or Tailwind.

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

1. Keep its original view at `Views/{Controller}/{Action}.cshtml`.
2. Add its alternative at `Views/Modern/{Controller}/{Action}.cshtml` and set
   `Layout = "_ModernLayout"` there.
3. Rewrite the alternative's markup with shared daisyUI components and Tailwind.
   Preserve its view model, controller, form field names, authorization, validation,
   and links. Keep markup specific to the new interface in separate partials.
4. Create a `.ts` or `.tsx` entry under `client/src/pages/` only if new browser
   behavior needs one, and load it in the page's `Scripts` section using `vite-entry`.

The new view is available to allowed users automatically. Unconverted pages keep
their original layout even for those users. A styles-only conversion can remain
Razor and retain its existing JavaScript, provided the modern layout or view loads
the libraries and widget styles it needs. TypeScript and React are optional.

Vite discovers page entries under `src/pages/`, including subfolders. There is no
per-page configuration list. Every `.ts` and `.tsx` file there is an entry, except
TypeScript declarations (`.d.ts`). Keep shared components and helpers under
`src/components/` and import them from page entries. The shared CSS entry stays
explicit in `vite.config.ts` and loads independently of page scripts.

The modern layout renders the page h1 from `ViewBag.Title`, optional subtitle, and
optional `SubNav`. It does not load jQuery or legacy CSS. Include any retained
widget's dependencies explicitly and check them against the shared theme. Avoid
loading the complete Bootstrap and Tailwind stylesheets together.

Order Review adds a request summary, section links, responsive cards and tables
under `Views/Modern/Order/Review.cshtml`. It shares the existing `_Review*` partials,
permissions, forms and review handlers. Its optional `Styles` section loads only
the retained widget styles before the modern theme. Its `Scripts` section loads
the required jQuery widgets explicitly, without the legacy global startup or site
stylesheet. `SkipNavigation` disables the legacy floating navigation for this view;
the original view keeps its existing navigation. Review styles are scoped to
`.modern-order-review` and `.modern-order-review-dialog` in the shared CSS entry.

To preview it, enable the same Modern UI settings and open an existing order's
Review link. Check requester, approver and purchaser actions, completed orders,
account warnings, notes, attachments, favorites, print variants and a narrow
viewport. Unlisted users continue to see `Views/Order/Review.cshtml`.

## Calendar and forms

The modern Away Status view uses a native `type="date"` field as its submitted form control.
The reusable `_Calendar` partial adds a Cally picker, styled by daisyUI, only after
`client/src/components/date-picker.ts` has loaded and registered the components.
If JavaScript or the bundle fails, the native input and submit buttons still work.
Dates stay `YYYY-MM-DD` strings in the browser; there is no UTC conversion. The
legacy view retains its jQuery date picker and existing layout.

The modern view returns from absence with a separate normal POST containing an
empty date. The legacy view keeps its existing jQuery behavior, clearing the date
before submitting the same form. Both views use the narrow `AwayStatusViewModel`,
which prevents unrelated User validation from interfering and
rejects malformed dates before persistence. Blank, past, current, and future dates
retain the existing server semantics. SQL remains responsible for `IsAway`.

## Add React when a page needs it

React, React DOM, TypeScript, and Vite's React plugin are configured. Create a TSX
entry under `client/src/pages/` and mount into an explicitly owned element with
`createRoot`. Vite discovers the entry automatically. Load it with:

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
