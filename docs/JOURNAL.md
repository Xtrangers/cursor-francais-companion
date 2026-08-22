# Journal des versions

Chaque entrée : date UTC, hash git, ce qui change. Règle 6.

## 2026-08-22 — Phase 3 dictionnaire

- SQLite local, seed ≥ 150 termes, normalizer, matcher, exclusions.
- CRUD + import/export, journal des inconnus, skill `ajouter-terme`.

## 2026-08-22 — Phase 2 shell WPF

- Menu 4 modules, Traducteur seul actif, écran Bientôt.
- Accueil / Réglages / Dictionnaire / Journal, tray, hotkey, démarrage auto.
- Bandeau non affilié + disclaimer premier lancement.
- P2-14 : XAML direct, pas de Figma.

## 2026-08-22 — P1-03 à P1-08 constat UIA

- Dump réel Cursor 3.16.17 : 468 éléments, 98 boutons, DPI 150 %.
- Go overlay UIA. Jalon : attendre Rémi avant phase 2.
- JSON brut non commité (titres de chats).

## 2026-08-22 — P1-02 dump

- Outil `tools/DumpCursorUi`. JSON brut ignoré par git (données personnelles).

## 2026-08-22 — P1-01 locator

- `CursorPathRules` refuse VS Code / Code.exe.
- `CursorLocator` liste les processus Cursor fiables + fenêtres UIA.

## 2026-08-22 — P0-08 CI

- `.github/workflows/ci.yml` : restore, build, test sur windows-latest, SDK 10.0.302.
- Phase 0 terminée.

## 2026-08-22 — P0-07 gitignore

- TestResults, nupkg, `.vscode` sauf `extensions.json`.

## 2026-08-22 — P0-06 build props

- `Directory.Build.props` (nullable, C# 13) et `.editorconfig`.
- Build toujours 0 avertissement.

## 2026-08-22 — P0-05 rules techniques

- `.cursor/rules/no-injection.mdc`, `overlay-win32.mdc`, `csharp-wpf.mdc`.

## 2026-08-22 — P0-04 NuGet

- Mvvm, NotifyIcon, DI, Sqlite, Serilog, CsWin32, xunit.
- `dotnet test` : 2 réussites, 0 échec.

## 2026-08-22 — P0-03 solution

- `CursorFrancais.slnx` : App, Core, Automation, Overlay, Native, Ocr, 2 projets de tests.
- `dotnet build` : 0 erreur, 0 avertissement.

## 2026-08-22 — P0-02 extension C#

- Installé `anysphere.csharp` (marketplace Cursor).
- Recommandation dans `.vscode/extensions.json`.

## 2026-08-22 — P0-01 outillage

- Pin SDK 10.0.302 via `global.json`.
- Versions consignées dans `docs/environnement.md`.

## 2026-08-22 — fiche tâches

- Ajout de `canvases/taches-developpement.canvas.tsx` : 75 tâches, 9 étapes, sous-étapes, statuts cliquables.
- Pas de jalon. Pas de push.

## 2026-08-22 — règles projet

- Commit : `083e2d2` — loi du projet (`RULES.md`, `AGENT.md`, `.cursor/rules/00-loi-projet.mdc`, `docs/MEMOIRE.md`).
- Base précédente : `0a28f5a` (shell WPF + menu modules dans le plan).
- Pas de jalon produit. Pas de push (règle 30).
