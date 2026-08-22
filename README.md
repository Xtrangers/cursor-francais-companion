# Cursor Français Companion

**Projet communautaire — non affilié à Cursor / Anysphere.**

Application Windows 11 indépendante : overlay de traduction locale pour l’interface Cursor, sans plugin natif, sans injection et sans modification des fichiers internes.

> Traduire l’interface Cursor en français sans modifier Cursor, sans abonnement obligatoire et sans envoyer les conversations vers un service externe.

| | |
|---|---|
| Statut | Fiches produit + plan d’implémentation (code à venir) |
| Cible | Windows 11 |
| Runtime | .NET 10 LTS + C# 13 |
| Overlay | Win32 layered HWND + Direct2D (pas de XAML transparent) |
| Shell | .NET 10 WPF — menu modules (Traducteur actif) |
| Licence | MIT |

Cette page d’accueil reprend les deux fiches du projet. Les canvas Cursor (ouverts dans l’IDE) sont aussi versionnés :

- [canvases/cursor-francais-companion.canvas.tsx](canvases/cursor-francais-companion.canvas.tsx) — fiche produit
- [canvases/plan-implementation.canvas.tsx](canvases/plan-implementation.canvas.tsx) — plan d’exécution (75 tâches)
- [docs/disclaimer-fr.md](docs/disclaimer-fr.md) — mention légale
- [RULES.md](RULES.md) — loi du projet (42 règles, mot `rulesList`)
- [AGENT.md](AGENT.md) — contrat agent, stack, jalons

---

## 1. Décision

**Retenu : application compagnon Windows avec overlay ciblé Cursor.**

Un plugin Cursor officiel ajoute des règles, agents, commandes, compétences, hooks et MCP. Il ne peut pas redessiner l’interface native. Les packs de langue VS Code traduisent surtout l’héritage VS Code ; Agent, Chat et Composer restent souvent en anglais.

| Solution | Faisabilité | Résultat |
|---|---|---|
| Plugin Cursor classique | Faible | Ne traduit pas l’UI native |
| Pack de langue VS Code | Moyenne | Surtout l’héritage VS Code |
| Modification des fichiers internes | Possible mais risquée | Cassé après chaque update, crash |
| Injection JavaScript | Possible mais fragile | Sécurité, détection, instabilité |
| **Application Windows + overlay** | **Très bonne** | **Contrôlable, sans toucher Cursor** |
| Fork complet de Cursor | Très difficile | Maintenance trop lourde |
| OCR + fenêtre de traduction | Bonne pour un MVP | Simple, moins propre |
| **UI Automation + dictionnaire** | **Meilleure solution finale** | **Rapide, légère, plus précise** |

### À faire en premier

1. Détecter uniquement Cursor
2. Fenêtre de contrôle + toggle
3. Dictionnaire de 100 à 200 termes
4. Traduire via Windows UI Automation
5. Overlay transparent click-through
6. Ne jamais traduire le code
7. Journal des textes inconnus
8. Tester 100 %, 125 %, 150 %
9. OCR seulement après le prototype

### À ne pas faire

- Injection dans `Cursor.exe`
- Modifier les `.asar` ou ressources internes
- Traduire tout l’écran dès la v1
- Envoyer des captures vers une API sans accord
- Promettre une traduction parfaite
- S’appeler « Cursor Français officiel »

---

## 2. Comment l’interface fonctionne

Deux surfaces, **un seul processus**. Les clics restent ceux de Cursor.

```
┌─────────────────────────────────────────────┐
│  CursorFrancais.App.exe                     │
│                                             │
│  ┌──────────────┐  ┌─────────────────────┐  │
│  │ WPF + menu   │  │ Overlay Win32 + D2D │  │
│  │ Réglages     │  │ HWND layered        │  │
│  │ Dictionnaire │  │ Labels FR           │  │
│  │ Tray + hotkey│  │ Click-through       │  │
│  └──────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────┐
│  Cursor.exe  —  jamais modifié              │
└─────────────────────────────────────────────┘
```

**Pourquoi WPF pour le shell, pas pour l’overlay ?** WPF gère le menu modules (Traducteur, Skills, Projets, Agents) et le MVVM. L’overlay sur Cursor reste Win32 + Direct2D pour un click-through fiable. Au MVP, seul Traducteur est implémenté ; les autres entrées du menu affichent « Bientôt ».

| Couche | Rôle |
|---|---|
| 1. Cursor.exe | Fenêtre native inchangée |
| 2. Overlay D2D | Cartouches FR au-dessus des libellés EN. Click-through |
| 3. Badge statut | « Traduction ON · N éléments », aussi click-through |
| 4. Fenêtre WPF | Menu modules + contenu (Traducteur seul actif au MVP) |
| 5. Tray | Activer, désactiver, ouvrir, quitter |

### Cycle de rendu (150–250 ms si Cursor change)

| Étape | Module | Entrée | Sortie |
|---|---|---|---|
| 1 | CursorLocator | `Cursor.exe` + chemin install | HWND + PID |
| 2 | WindowTracker | WinEventHook move / resize / focus | Rect écran + DPI + foreground |
| 3 | UiaReader | TreeWalker sur le HWND Cursor | `{texte, box, type}` |
| 4 | ElementFilter | Type, classe, zone éditeur | UI seulement, jamais le code |
| 5 | DictionaryEngine | Texte EN normalisé | FR ou « inconnu » journalisé |
| 6 | LabelLayout | Boxes + DPI + mode | Rectangles anti-chevauchement |
| 7 | OverlayRenderer | Labels + opacité | `UpdateLayeredWindow` (alpha) |
| 8 | OcrFallback | Zones sans Name UIA (MVP 2) | Texte OCR local, image détruite |

### Règles overlay

| Règle | Implémentation |
|---|---|
| Click-through total | `WS_EX_TRANSPARENT` |
| Pas de vol de focus | `WS_EX_NOACTIVATE` |
| Hors barre des tâches | `WS_EX_TOOLWINDOW` |
| Suit Cursor | `SetWindowPos` sans activer |
| Masqué si Cursor inactif | Option auto-hide |
| DPI par moniteur | `GetDpiForWindow` |
| Exclusion code | Ignorer Edit / Document / Monaco |

Raccourci global : `Ctrl + Alt + F`.

---

## 3. Architecture produit (3 niveaux)

| Niveau | Moteur | Rôle |
|---|---|---|
| 1 | Dictionnaire local EN→FR | Libellés connus, hors ligne, instantané |
| 2 | Windows UI Automation | Boutons, menus, onglets, titres exposés |
| 3 | OCR Windows (secours) | Zones custom sans accessibilité — après le MVP 1 |

**Flux utilisateur :** démarrer Cursor → démarrer le compagnon → détection automatique → activer « Traduction française » → labels FR en overlay → désactivation à tout moment.

### Modes

| Mode | Comportement | Par défaut |
|---|---|---|
| Automatique | Traduit les éléments détectés | Libellés UI |
| Intelligent | Boutons, menus, titres — jamais le code | Recommandé |
| Manuel | Zone sélectionnée | Optionnel |
| Bilingue | `Exécuter — Run` | Apprentissage |

Exclusions par défaut : code, noms de fichiers, commandes, saisie utilisateur, réponses IA.

---

## 4. Stack technique (août 2026)

| Couche | Choix | Pourquoi | À éviter |
|---|---|---|---|
| Runtime | **.NET 10 LTS** (`net10.0-windows10.0.22621.0`) | Support jusqu’au 14 nov. 2028. .NET 8 expire le 10 nov. 2026 | .NET 8 / 9 en cible longue |
| Langage | **C# 13**, nullable | Idiomatique .NET 10 | C++/WinRT sauf P/Invoke généré |
| UI shell | **WPF (.NET 10, UseWPF=true)** | Menu extensible, MVVM, pas de WinAppSDK | WinUI 3, Electron |
| UI overlay | **Win32 + Direct2D + DirectWrite** | Alpha et click-through fiables | Overlay WPF sur Cursor |
| Automation | **IUIAutomation via CsWin32** | API Windows, pas d’injection | FlaUI au runtime (OK en tests) |
| OCR (MVP 2) | **Windows.Media.Ocr + Graphics Capture** | Local, HWND Cursor seulement | API cloud |
| Données | **SQLite** + JSON seed | Hors ligne | Envoi réseau par défaut |

### Solution cible

| Projet | Responsabilité |
|---|---|
| `src/CursorFrancais.App` | WPF, menu modules, tray, hotkey |
| `src/CursorFrancais.Core` | Dictionnaire, settings, exclusions |
| `src/CursorFrancais.Automation` | Locator Cursor, UIA, filtres |
| `src/CursorFrancais.Overlay` | HWND layered, D2D, layout |
| `src/CursorFrancais.Native` | CsWin32 |
| `src/CursorFrancais.Ocr` | MVP 2 seulement |
| `tests/CursorFrancais.Core.Tests` | Matcher / exclusions |
| `tests/CursorFrancais.Automation.Tests` | Snapshots JSON |

---

## 5. MCP, plugins et outillage Cursor

Il n’existe **pas** de MCP WPF / UI Automation / Direct2D. Le compagnon se développe en C# local.

| Serveur MCP | Statut | Usage |
|---|---|---|
| `cursor-app-control` | Requis — phase 0 | Créer / ancrer le projet |
| `plugin-github-github` | Requis — phase 0 et 7 | Repo, CI, releases |
| `plugin-figma-figma` | Optionnel — phase 2 | Maquettes du shell |
| `plugin-notion-workspace-notion` | Optionnel | Copie doc |
| `plugin-playwright-playwright` | Ne pas utiliser | Navigateur, pas du desktop |
| `plugin-datadog-datadog` | Pas au MVP | Pas de télémétrie sans consentement |

Extensions éditeur : `ms-dotnettools.csharp`.  
Rules à créer en phase 0 : `no-injection`, `overlay-win32`, `csharp-wpf`.

Un **plugin Cursor officiel** pourra plus tard porter le dictionnaire et des commandes. L’overlay Windows restera obligatoire pour modifier visuellement l’UI.

---

## 6. MVP

| Version | Objectif |
|---|---|
| **MVP 1** | 60–80 % des textes fixes : détection, toggle, dictionnaire, menus / boutons / sidebar, mode sombre, bilingue, exclusion du code, journal, zip portable |
| **MVP 2** | OCR de secours, panneaux Agent, glossaire perso, multi-fenêtres, correction |
| **Avancé** | Profils par version, FR pro / simplifié, ES / DE, Store éventuel |

**Go / no-go :** dump UI Automation réel de Cursor (phase 1). Si trop peu de `Name` exposés sur Agent/Chat, le MVP 1 reste valable sur les libellés fixes ; l’OCR avance plus tôt.

---

## Menu WPF (décision du 22 août 2026)

Le compagnon n’est plus une fenêtre « traducteur seul ». C’est un **socle à modules** :

| Entrée | MVP 1 | Plus tard |
|---|---|---|
| **Traducteur** | Actif | Overlay, dictionnaire, UIA, OCR |
| **Skills** | Menu + écran Bientôt | Gérer les skills Cursor de l’utilisateur |
| **Projets** | Menu + écran Bientôt | Profils de workspaces / presets |
| **Agents** | Menu + écran Bientôt | Liste et presets d’agents |

Contrat C# : `ICompanionModule` (`Id`, `Title`, `IsAvailable`, `CreateView()`). Ajouter une fonction = une classe + une vue, sans recoder `MainWindow`.

## 7. Plan d’exécution — 75 tâches

Ordre agent : **0 → 1 (dump UIA) → 2+3 → 4 → 5 → livrable portable → 6 OCR plus tard**.

### Phase 0 — Outillage et solution (8)

| ID | Tâche | Livrable |
|---|---|---|
| P0-01 | Vérifier .NET 10 SDK, Windows 11 SDK, templates WPF | `dotnet --info` |
| P0-02 | Installer l’extension C# dans Cursor | Roslyn actif |
| P0-03 | Créer la solution et les 7 projets | `CursorFrancais.slnx` compile |
| P0-04 | NuGet : CommunityToolkit.Mvvm, NotifyIcon.Wpf, CsWin32, Sqlite, Serilog, xunit | restore OK |
| P0-05 | `AGENTS.md` + 3 rules Cursor | `.cursor/rules` |
| P0-06 | EditorConfig + `Directory.Build.props` | Build reproductible |
| P0-07 | Git / remote GitHub | Repo versionné |
| P0-08 | CI `dotnet build` + `test` sur `windows-latest` | `.github/workflows/ci.yml` |

### Phase 1 — Validation Cursor (8)

| ID | Tâche | Critère de fin |
|---|---|---|
| P1-01 | `CursorLocator` (process, chemin, version) | Refuse VS Code / autres Electron |
| P1-02 | Console `DumpCursorUi` → JSON | Fichier dump horodaté |
| P1-03 | Dumps Agent, Chat, Composer, Settings | Liste des `Name` UIA réels |
| P1-04 | Mesures 100 / 125 / 150 % | Écart DPI documenté |
| P1-05 | Thème sombre / clair | Décision couleurs overlay |
| P1-06 | 2 fenêtres + écran secondaire | Overlay par HWND |
| P1-07 | Matrice traduisible / protégé / inaccessible | Base du filtre MVP 1 |
| P1-08 | Go / no-go overlay UIA | Si UIA < 30 % des boutons → OCR plus tôt |

### Phase 2 — Shell WPF + menu modules (14)

| ID | Tâche | Critère de fin |
|---|---|---|
| P2-01 | `MainWindow` WPF : menu gauche + ContentControl | Chrome unique |
| P2-02 | `ICompanionModule` + `ModuleRegistry` + `ShellViewModel` | Changer de module change la vue |
| P2-03 | Menu : Traducteur, Skills, Projets, Agents | 4 entrées cliquables |
| P2-04 | Traducteur actif ; les 3 autres → écran « Bientôt » | Pas de crash |
| P2-05 | Accueil Traducteur : statut, toggle, modes, zones | Toggle persisté |
| P2-06 | Sous-pages : Réglages, Dictionnaire, Journal | Menu gauche reste sur Traducteur |
| P2-07 | Settings : opacité, taille, auto-hide, hotkey | JSON local |
| P2-08 | Tray | Fermer la fenêtre laisse le tray |
| P2-09 | Hotkey `Ctrl + Alt + F` | Toggle même si Cursor a le focus |
| P2-10 | Démarrage auto (off par défaut) | Réversible |
| P2-11 | Thème sombre WPF | Pas de logo Cursor copié |
| P2-12 | Bandeau « non affilié » | Visible quel que soit le module |
| P2-13 | Mémoriser le dernier module | Prêt pour la phase 8 |
| P2-14 | Maquettes Figma optionnelles | Sinon XAML WPF direct |

### Phase 3 — Dictionnaire (9)

| ID | Tâche | Critère de fin |
|---|---|---|
| P3-01 | Schéma SQLite | `%LocalAppData%/CursorFrancais/dict.db` |
| P3-02 | Seed 150–200 termes | Apply / Reject / Run / Settings… |
| P3-03 | Normalizer (`New Chat...` → `New Chat`) | Tests |
| P3-04 | Matcher + keep-english (Agent, Composer) | ≥ 30 cas unitaires |
| P3-05 | Exclusions chemins, code, IDs modèles | `gpt-4o`, `C:\proj\a.cs` non traduits |
| P3-06 | Journal `unknown_terms` | Visible dans Journal |
| P3-07 | CRUD + import/export JSON | Round-trip |
| P3-08 | Catégories | Filtre ListView |
| P3-09 | Skill Cursor « ajouter un terme » | Agent peut ajouter EN→FR + test |

### Phase 4 — UI Automation (8)

| ID | Tâche | Critère de fin |
|---|---|---|
| P4-01 | WindowTracker (move / resize / focus) | Latence &lt; 100 ms au drag |
| P4-02 | `UiaReader.ReadVisible` + timeout | Pas de freeze |
| P4-03 | ElementFilter | 0 label sur Monaco |
| P4-04 | CoordinateMapper DPI | Alignement ±2 px à 150 % |
| P4-05 | Cache + dirty-check | Pas de redraw inutile |
| P4-06 | N fenêtres Cursor = N overlays | Destroy propre |
| P4-07 | ZoneClassifier Agent/Chat | Checkbox respectée |
| P4-08 | Tests sur dumps JSON figés | CI sans Cursor ouvert |

### Phase 5 — Overlay graphique (10)

| ID | Tâche | Critère de fin |
|---|---|---|
| P5-01 | HWND layered click-through | Clics = Cursor à 100 % |
| P5-02 | Direct2D + DirectWrite | Texte net 100/125/150 % |
| P5-03 | `UpdateLayeredWindow` | Pas de fond noir opaque |
| P5-04 | LabelPainter (pill + texte) | Lisible sur thème sombre |
| P5-05 | LabelLayout anti-collision | Pas de recouvrement |
| P5-06 | Sync géométrie Cursor | Y compris maximisé |
| P5-07 | Hide si Cursor n’est plus foreground | Alt-Tab masque |
| P5-08 | Mode bilingue | `FR — EN` |
| P5-09 | Opacité / taille depuis settings | Slider immédiat |
| P5-10 | Badge « Traduction ON · N » | Click-through |

### Phase 6 — OCR de secours, après MVP 1 (6)

| ID | Tâche | Critère de fin |
|---|---|---|
| P6-01 | Activer `CursorFrancais.Ocr` | Réf conditionnelle |
| P6-02 | Graphics Capture du HWND Cursor seul | Aucune autre fenêtre |
| P6-03 | `Windows.Media.Ocr` sur ROI sans UIA | Texte + box |
| P6-04 | Purge bitmap immédiate | 0 fichier disque |
| P6-05 | Governor CPU (max 2 OCR/s) | Désactivable |
| P6-06 | Jamais OCR éditeur / terminal | Test de ROI |

### Phase 7 — Qualité et livraison (6)

| ID | Tâche | Critère de fin |
|---|---|---|
| P7-01 | Matrice QA manuelle | Overlay jamais bloquant |
| P7-02 | Idle &lt; 1 % CPU, &lt; 80 Mo RAM hors OCR | Mesure documentée |
| P7-03 | Zip portable + MSIX unsigned | Lance sans admin |
| P7-04 | Disclaimer + LICENSE | Visible au 1er lancement |
| P7-05 | GitHub Release + versions Cursor testées | Tag `v0.1.0` |
| P7-06 | Signature Authenticode si certificat | Sinon warning SmartScreen documenté |

### Phase 8 — Modules futurs, après le MVP traducteur (6)

| ID | Tâche | Critère de fin |
|---|---|---|
| P8-01 | Skills : lister / activer / éditer les skills locaux | Sans injection dans Cursor |
| P8-02 | Projets : profils de workspaces | Dernier dossier mémorisé |
| P8-03 | Agents : liste et presets | Orchestration locale, pas de fork |
| P8-04 | Isolation des settings par module | Un module cassé n’arrête pas le traducteur |
| P8-05 | Badges menu Bientôt → Actif | Entrées déjà là depuis P2 |
| P8-06 | Ne pas commencer P8 avant P5 + P7-03 | Traducteur d’abord |

**MVP 1 shippable = fin de P5 + P7-03/04.** Skills / Projets / Agents = phase 8.

---

## 8. Matrice QA (P7-01)

| Scénario | Attendu |
|---|---|
| Cursor fermé au lancement | Statut « non détecté », overlay off |
| Ouvrir Cursor ensuite | Détection &lt; 2 s |
| Fermer Cursor | Overlay détruit, pas d’exception |
| Resize / maximize / snap | Labels recollés |
| DPI 100 / 125 / 150 | Texte net, décalage ≤ 2 px |
| Thème sombre et clair | Contraste OK |
| Deux fenêtres Cursor | Deux overlays |
| Écran secondaire | Bon moniteur |
| Clic sur un bouton sous un label | Le bouton Cursor s’active |
| Raccourcis Cursor | Inchangés |
| `Ctrl + Alt + F` | Toggle immédiat |
| Mise à jour Cursor | Pas de crash ; profil « unknown version » |

---

## 9. Risques

| Risque | Niveau | Réponse |
|---|---|---|
| Cursor change son UI | Élevé | Profils par version |
| Overlay mal positionné | Élevé | Recalcul + réglage manuel |
| Textes illisibles (pas d’UIA) | Élevé | OCR de secours |
| Faux positifs sur le code | Élevé | Zones protégées |
| Consommation CPU | Moyen | Cache + throttle |
| Antivirus | Moyen | Pas d’injection, signature |
| Conditions d’utilisation Cursor | Moyen / élevé | Ne pas modifier ni injecter |
| Cursor ajoute le français officiel | Moyen | Pivoter en assistant multilingue |

---

## 10. Hors périmètre jusqu’au MVP 1

- Injection JS / DLL dans `Cursor.exe`
- Patch des `.asar`
- Playwright pour « cliquer Cursor »
- Envoi de captures vers un LLM
- Promettre 100 % de l’UI

---

## Sources

- [Plugins Cursor](https://cursor.com/docs/plugins) — règles, skills, agents, MCP ; pas l’UI native
- [Forum Cursor](https://forum.cursor.com) — packs de langue vs composants propriétaires
- [WPF / .NET](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Support .NET](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)

---

Cursor Français Companion — projet communautaire, licence MIT. Voir [docs/disclaimer-fr.md](docs/disclaimer-fr.md).
