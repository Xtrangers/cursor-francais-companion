# AGENT.md — Cursor Français Companion

À lire en entier avant la première action, avec `RULES.md` (règle 34).

## Identité

- Propriétaire : Rémi (GitHub `Xtrangers`).
- Projet communautaire, **non affilié** à Cursor / Anysphere.
- Repo : `https://github.com/Xtrangers/cursor-francais-companion`
- Branche principale : `main` (pas `master`). Les agents secondaires travaillent chacun sur leur branche (règle 35).

## Mission

Application Windows 11 indépendante : overlay de traduction française de l'interface Cursor, plus tard gestion des skills, projets et agents. Un seul exe. Pas d'injection, pas de modification des fichiers Cursor, pas d'interception réseau.

## Stack figée

- Runtime : .NET 10 LTS, C# 13, `net10.0-windows`, `UseWPF=true`.
- Shell : WPF + menu modules (`ICompanionModule`).
- Overlay : Win32 layered HWND + Direct2D + DirectWrite (jamais un overlay WPF sur Cursor).
- Automation : UI Automation via CsWin32.
- Données : SQLite local, dictionnaire hors ligne.
- Budget : 0 € de dépendance payante sans accord (règles 38–39).

## Menu WPF

| Module | MVP 1 | Plus tard |
|---|---|---|
| Traducteur | Actif | Overlay, dictionnaire, UIA, OCR |
| Skills | Menu + écran Bientôt | Phase 8 |
| Projets | Menu + écran Bientôt | Phase 8 |
| Agents | Menu + écran Bientôt | Phase 8 |

Ne pas implémenter Skills / Projets / Agents avant le jalon traducteur.

## Fiches

Référence produit et plan : canvas Cursor + `README.md` GitHub. Ne les relire que pour republier ou trancher un périmètre (règle 5).

- `canvases/cursor-francais-companion.canvas.tsx`
- `canvases/plan-implementation.canvas.tsx`

## Jalons — s'arrêter et attendre Rémi

1. Fin de phase 1 (dump UIA + go / no-go).
2. Fin de phase 2 (shell WPF + menu à 4 entrées, Traducteur seul actif).
3. MVP 1 (overlay click-through + zip portable, P5 + P7-03/04).
4. Avant d'ouvrir la phase 8 (Skills / Projets / Agents).
5. Toute mise en ligne ou release GitHub.

## Interdits durables

- Injection JS / DLL dans `Cursor.exe`.
- Patch des `.asar` ou ressources internes.
- Envoi de captures ou du code utilisateur vers une API sans accord.
- Playwright / serveur web tant qu'il n'y a pas de surface web (règles 16–17 alors dormantes, pas abrogées).
- Force push, amend d'un commit déjà poussé (règle 32).
- Push sans demande de Rémi (règle 30).
- Modifier `RULES.md` si on n'est pas le propriétaire, sauf ordre explicite (règle 40). Un agent secondaire ne touche jamais `RULES.md`, `AGENT.md`, `docs/MEMOIRE.md` (règle 37).

## Langue

UI et messages : français. Pas d'anglais visible sauf usage courant (règle 29). Termes conservés : Agent, Composer, Cursor, Skill.

## Mémoire et journal

- Corrections de Rémi : `docs/MEMOIRE.md` (règle 11).
- Versions et jalons : `docs/JOURNAL.md` (règle 6).
