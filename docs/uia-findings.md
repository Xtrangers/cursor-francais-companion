# Constat UI Automation Cursor

Date : 2026-08-22. Cursor **3.16.17**. Outil : `DumpCursorUi`.
Dump brut local (non versionné, titres personnels) : `docs/dumps/dump-20260822-035957.json`.

## Fenêtres observées (P1-03, P1-06)

| Fenêtre | Classe | Éléments UIA | Boutons |
|---|---|---|---|
| Cursor Agents (seule fenêtre top-level) | `Chrome_WidgetWin_1` | 468 | 98 |

- Chat, Composer et Settings **n’ont pas de HWND séparés** dans ce dump : ce sont des vues dans la même fenêtre Electron.
- Une seule fenêtre top-level. Pas d’écran secondaire observé dans ce run.
- Relancer `DumpCursorUi` après avoir ouvert Settings / Composer si on veut un second échantillon.

## DPI (P1-04)

| Échelle Windows | DPI | Observé |
|---|---|---|
| 100 % | 96 | Non mesuré (machine à 150 %) |
| 125 % | 120 | Non mesuré |
| **150 %** | **144** | **Oui** — fenêtre 3862×2110, contenu 3840×2088 |

Le cadre `-11,-11` est l’ombre DWM. L’overlay devra utiliser le rect étendu ou `DwmGetWindowAttribute`.

## Thème (P1-05)

UIA ne donne pas le thème clair/sombre. Rien dans les `Name` ne l’indique. À régler visuellement en phase 5 (labels contrastés sur fond sombre par défaut, option inverse).

## Types (extrait)

DataItem 137, Button 98, Text 95, Group 59, ListItem 31, Pane 11, List 9, Image 5, TabItem 5, Table 4. Éditeurs marqués : 5.

## Noms chrome utiles (traduisibles)

File, Edit, View, Help, Hide Sidebar, Hide Apps, Go Back, Go Forward, New Chat, Search, Automations, Customize, Customize Sidebar, Open Workspace, Account menu, Chat actions, Command, Copy message, Fork chat, Enter Full Screen, Context, Add agents, context, tools.

Chrome Windows déjà en français (ne pas retraduire) : Réduire, Restaurer, Fermer.

## À protéger (ne pas traduire)

- Titres de conversations et noms de workspaces (ils apparaissent comme `Button`).
- Noms de branches (`Branch main…`).
- Compteurs (`Context 89%`, `Editing 8 files…`).
- Contenu de la fiche Canvas interne (boutons d’étapes du plan).
- Zones `EstEditeur` (Document / Edit / monaco).

## Matrice (P1-07)

| Classe | Décision MVP 1 |
|---|---|
| Boutons menu File/Edit/View/Help | Traduisible |
| New Chat, Search, Automations, Sidebar | Traduisible |
| Account / Chat actions / Copy message | Traduisible si dans le dictionnaire |
| Réduire / Fermer (déjà FR) | Ignorer |
| Titres de chats, repos, branches | Protégé |
| Edit / Document / monaco | Protégé (code) |
| DataItem sans nom stable | Inaccessible / ignorer |
| Panneaux Agent internes dynamiques | Partiel : Name présent souvent, filtrer la longueur |

## Go / no-go (P1-08) — jalon

**Go overlay UIA.** Plus de 30 % des boutons ont un `Name` (98 boutons sur 468 nœuds). Pas besoin d’avancer l’OCR avant le MVP 1.

Conditions :

1. Filtrer titres longs, workspaces, branches, compteurs.
2. Un overlay par HWND (ici un seul).
3. Tester 100 % et 125 % chez Rémi (machine actuelle = 150 %).
4. Recaler le rect DWM (décalage -11 px).

Attente validation Rémi avant phase 2.
