import {
  BarChart,
  Button,
  Callout,
  Card,
  CardBody,
  CardHeader,
  Code,
  Divider,
  Grid,
  H1,
  H2,
  H3,
  Link,
  Pill,
  Row,
  Stack,
  Stat,
  Table,
  Text,
  useCanvasAction,
  useCanvasState,
  useHostTheme,
} from "cursor/canvas";

type TabId =
  | "ui"
  | "stack"
  | "mcp"
  | "code"
  | "tasks-a"
  | "tasks-b"
  | "tests";

const TABS: { id: TabId; label: string }[] = [
  { id: "ui", label: "Fonctionnement UI" },
  { id: "stack", label: "Technologies" },
  { id: "mcp", label: "MCP et plugins" },
  { id: "code", label: "Code et solution" },
  { id: "tasks-a", label: "Tâches 0 à 3" },
  { id: "tasks-b", label: "Tâches 4 à 7" },
  { id: "tests", label: "Tests et livraison" },
];

export default function PlanImplementation() {
  const [tab, setTab] = useCanvasState<TabId>("plan-tab", "ui");

  return (
    <Stack gap={20}>
      <Stack gap={8}>
        <Row gap={8} align="center" wrap>
          <H1>Plan d’implémentation</H1>
          <Pill size="sm" active>
            Exécutable par l’agent
          </Pill>
        </Row>
        <Text tone="secondary">
          Plan pas-à-pas pour développer Cursor Français Companion : stack
          exacte, connecteurs MCP, type de code, fonctionnement de l’interface,
          et toutes les tâches en tableaux. Source : dossier produit +
          contraintes WinUI 3 overlay, 22 août 2026.
        </Text>
      </Stack>

      <Grid columns={4} gap={12}>
        <Stat value="72" label="Tâches planifiées" />
        <Stat value="8" label="Phases" tone="info" />
        <Stat value=".NET 10" label="Runtime LTS" tone="success" />
        <Stat value="Win32 D2D" label="Moteur overlay" tone="warning" />
      </Grid>

      <Row gap={6} wrap>
        {TABS.map((item) => (
          <span key={item.id}>
            <Pill active={tab === item.id} onClick={() => setTab(item.id)}>
              {item.label}
            </Pill>
          </span>
        ))}
      </Row>

      {tab === "ui" && <UiTab />}
      {tab === "stack" && <StackTab />}
      {tab === "mcp" && <McpTab />}
      {tab === "code" && <CodeTab />}
      {tab === "tasks-a" && <TasksATab />}
      {tab === "tasks-b" && <TasksBTab />}
      {tab === "tests" && <TestsTab />}
    </Stack>
  );
}

function UiTab() {
  const theme = useHostTheme();

  return (
    <Stack gap={20}>
      <Callout tone="warning" title="Décision UI critique">
        L’écran de réglages est en WinUI 3. L’overlay de traduction n’est pas
        en XAML WinUI. WinUI compose via DirectComposition et ne gère pas
        proprement un HWND transparent click-through (WS_EX_LAYERED).
        L’overlay est une fenêtre Win32 layered, dessinée en Direct2D, avec
        les clics transmis à Cursor.
      </Callout>

      <H2>Deux surfaces, un seul processus</H2>
      <Grid columns={2} gap={12}>
        <Card>
          <CardHeader trailing={<Pill size="sm" active>WinUI 3</Pill>}>
            Fenêtre compagnon
          </CardHeader>
          <CardBody>
            <Stack gap={6}>
              <Text>
                Application Windows 11 classique : accueil, dictionnaire,
                paramètres, journal. NavigationView, ToggleSwitch, sliders.
              </Text>
              <Text size="small" tone="secondary">
                Reçoit les clics. Peut prendre le focus. Icône dans la zone
                de notification. Raccourci Ctrl + Alt + F.
              </Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader trailing={<Pill size="sm">Win32 + D2D</Pill>}>
            Overlay Cursor
          </CardHeader>
          <CardBody>
            <Stack gap={6}>
              <Text>
                Fenêtre sans bord, always-on-top, calée sur le HWND Cursor.
                Dessine uniquement les libellés traduits. Jamais de chrome
                Windows.
              </Text>
              <Text size="small" tone="secondary">
                WS_EX_LAYERED + WS_EX_TRANSPARENT + WS_EX_NOACTIVATE +
                WS_EX_TOOLWINDOW. Souris et clavier restent ceux de Cursor.
              </Text>
            </Stack>
          </CardBody>
        </Card>
      </Grid>

      <H2>Couches à l’écran</H2>
      <div
        style={{
          border: `1px solid ${theme.stroke.secondary}`,
          background: theme.bg.elevated,
          padding: 16,
        }}
      >
        <Stack gap={10}>
          <LayerRow
            label="1. Cursor.exe"
            detail="Fenêtre native inchangée. Boutons, menus, éditeur, Agent."
          />
          <LayerRow
            label="2. Overlay D2D"
            detail="Rectangles + texte FR au-dessus des libellés EN détectés. Click-through."
          />
          <LayerRow
            label="3. Badge statut"
            detail="Petite pastille « Traduction ON · 14 éléments », également click-through."
          />
          <LayerRow
            label="4. Fenêtre compagnon"
            detail="Hors de Cursor. Réglages, dictionnaire, journal. Visible à la demande."
          />
          <LayerRow
            label="5. Tray"
            detail="Clic droit : Activer, Désactiver, Ouvrir, Quitter."
          />
        </Stack>
      </div>

      <H2>Cycle de rendu (toutes les 150–250 ms si Cursor change)</H2>
      <Table
        headers={["Étape", "Module", "Entrée", "Sortie"]}
        striped
        stickyHeader
        rows={[
          [
            "1",
            "CursorLocator",
            "Processus Cursor.exe + chemin install",
            "HWND principal + PID",
          ],
          [
            "2",
            "WindowTracker",
            "WinEventHook move / resize / focus",
            "Rect écran + DPI + foreground",
          ],
          [
            "3",
            "UiaReader",
            "TreeWalker sur le HWND Cursor",
            "Liste {texte, bounding box, type}",
          ],
          [
            "4",
            "ElementFilter",
            "Type, classe, zone éditeur",
            "Éléments UI seulement, jamais le code",
          ],
          [
            "5",
            "DictionaryEngine",
            "Texte EN normalisé",
            "FR ou « inconnu » journalisé",
          ],
          [
            "6",
            "LabelLayout",
            "Boxes + DPI + mode bilingue",
            "Rectangles anti-chevauchement",
          ],
          [
            "7",
            "OverlayRenderer",
            "Liste de labels + opacité",
            "UpdateLayeredWindow (alpha)",
          ],
          [
            "8",
            "OcrFallback",
            "Zones sans Name UIA (MVP 2)",
            "Texte OCR local, image détruite",
          ],
        ]}
      />

      <H3>Règles de l’overlay</H3>
      <Table
        headers={["Règle", "Implémentation"]}
        rows={[
          [
            "Click-through total",
            "WS_EX_TRANSPARENT : aucun hit-test. Cursor reçoit tout.",
          ],
          [
            "Pas de vol de focus",
            "WS_EX_NOACTIVATE. Le raccourci est global, pas dans l’overlay.",
          ],
          [
            "Pas dans la barre des tâches",
            "WS_EX_TOOLWINDOW + owner = HWND compagnon.",
          ],
          [
            "Suit Cursor",
            "SetWindowPos sans activer, à chaque EVENT_OBJECT_LOCATIONCHANGE.",
          ],
          [
            "Masqué si Cursor inactif",
            "Si le premier plan n’est pas Cursor, l’overlay disparaît (réglage).",
          ],
          [
            "DPI par moniteur",
            "GetDpiForWindow(Cursor). Coordonnées physiques, pas DIP.",
          ],
          [
            "Thème",
            "Fond label sombre semi-transparent + texte clair, ou inverse.",
          ],
          [
            "Exclusion code",
            "Ignorer ControlType.Edit / Document et classes Monaco / editor.",
          ],
        ]}
      />

      <H2>Écrans WinUI à coder</H2>
      <Table
        headers={["Page XAML", "Contrôles", "État lié"]}
        striped
        rows={[
          [
            "HomePage.xaml",
            "InfoBar détection, ToggleSwitch, RadioButtons mode, CheckBoxes zones",
            "AppState.IsEnabled, TranslationMode, ZoneFlags",
          ],
          [
            "SettingsPage.xaml",
            "Slider opacité, Slider taille, CheckBox auto-hide, Hotkey recorder",
            "UiSettings",
          ],
          [
            "DictionaryPage.xaml",
            "ListView virtualisée, recherche, Ajouter / Importer / Exporter",
            "DictionaryStore",
          ],
          [
            "LogPage.xaml",
            "Liste des termes inconnus, compteur, bouton « proposer FR »",
            "UnknownTermLog",
          ],
          [
            "TrayFlyout",
            "Menu Flyout : Activer, Ouvrir, Quitter",
            "AppState",
          ],
        ]}
      />

      <H2>Ce que l’utilisateur voit</H2>
      <Grid columns={2} gap={16}>
        <Stack gap={8}>
          <H3>Mode français</H3>
          <Text>
            Le libellé anglais reste dans Cursor. Un cartouche FR est collé
            juste au-dessus ou à droite du contrôle, assez petit pour ne pas
            masquer le bouton. Le clic traverse le cartouche et actionne
            Cursor.
          </Text>
        </Stack>
        <Stack gap={8}>
          <H3>Mode bilingue</H3>
          <Text>
            Le cartouche affiche « Exécuter — Run ». Utile pour apprendre le
            vocabulaire. Même hit-test : aucun blocage.
          </Text>
        </Stack>
      </Grid>

      <StartBuildButton />
    </Stack>
  );
}

function LayerRow({ label, detail }: { label: string; detail: string }) {
  return (
    <Row gap={12} align="start">
      <Text weight="semibold" style={{ minWidth: 140 }}>
        {label}
      </Text>
      <Text tone="secondary">{detail}</Text>
    </Row>
  );
}

function StartBuildButton() {
  const dispatch = useCanvasAction();
  return (
    <Row>
      <Button
        variant="primary"
        onClick={() =>
          dispatch({
            type: "newComposerChat",
            userPrompt:
              "Exécute le plan d’implémentation de Cursor Français Companion. Commence par la phase 0 (outillage + solution .NET 10 / WinAppSDK 2.x) puis la phase 1 (validation UIA de Cursor). Overlay = Win32 layered + Direct2D, pas de XAML transparent. Pas d’injection, pas de modification des fichiers Cursor.",
          })
        }
      >
        Démarrer le développement
      </Button>
    </Row>
  );
}

function StackTab() {
  return (
    <Stack gap={20}>
      <H2>Décisions de stack (août 2026)</H2>
      <Table
        headers={["Couche", "Choix", "Pourquoi", "À éviter"]}
        striped
        stickyHeader
        rowTone={["success", "success", "warning", "success", "info", "neutral"]}
        rows={[
          [
            "Runtime",
            ".NET 10 LTS (net10.0-windows10.0.22621.0)",
            "Support jusqu’au 14 nov. 2028. .NET 8 expire le 10 nov. 2026.",
            ".NET 8 / 9 en cible longue durée",
          ],
          [
            "Langage",
            "C# 13, nullable, file-scoped namespaces",
            "Idiomatique .NET 10, tests simples",
            "C++/WinRT sauf P/Invoke généré",
          ],
          [
            "UI réglages",
            "WinUI 3 + Windows App SDK 2.4 (canal Current)",
            "Look Windows 11. WinAppSDK 1.8 sort de maintenance le 9 sept. 2026.",
            "WPF/WinForms pour le shell, Electron",
          ],
          [
            "UI overlay",
            "Win32 layered HWND + Direct2D + DirectWrite",
            "Alpha par pixel et click-through fiables",
            "Fenêtre WinUI XAML transparente",
          ],
          [
            "Automation",
            "UI Automation COM (IUIAutomation) via CsWin32",
            "API Windows native, pas d’injection",
            "FlaUI au runtime (OK en tests)",
          ],
          [
            "OCR (MVP 2)",
            "Windows.Media.Ocr + Graphics Capture",
            "Local, fenêtre Cursor seulement",
            "API cloud, Tesseract distant",
          ],
        ]}
      />

      <H2>Paquets NuGet</H2>
      <Table
        headers={["Paquet", "Rôle", "Projet"]}
        striped
        rows={[
          [
            "Microsoft.WindowsAppSDK 2.4.x",
            "WinUI, fenêtres, packaging runtime",
            "App",
          ],
          [
            "Microsoft.Windows.SDK.BuildTools",
            "Build Win32 / MSIX",
            "App",
          ],
          [
            "Microsoft.Windows.CsWin32",
            "P/Invoke généré : HWND, hooks, D2D, UIA",
            "Overlay, Automation, Native",
          ],
          [
            "CommunityToolkit.WinUI / Mvvm",
            "ObservableObject, RelayCommand, tray helpers",
            "App",
          ],
          ["Microsoft.Data.Sqlite", "Dictionnaire et journal locaux", "Core"],
          ["Serilog + File sink", "Logs locaux, rotation, pas de PII", "Core"],
          [
            "xunit + FluentAssertions + Coverlet",
            "Tests unitaires dictionnaire / filtre",
            "Tests",
          ],
          [
            "FlaUI.UIA3 (tests seulement)",
            "Snapshots UIA hors process",
            "Tests",
          ],
        ]}
      />

      <H2>APIs Windows utilisées</H2>
      <Table
        headers={["API", "Usage"]}
        rows={[
          ["EnumWindows / GetWindowThreadProcessId", "Trouver le HWND Cursor"],
          ["QueryFullProcessImageName", "Valider le chemin d’installation"],
          ["SetWinEventHook", "Move, resize, focus, destruction"],
          ["DwmGetWindowAttribute", "Rect réel hors ombre DWM"],
          ["GetDpiForWindow", "DPI du moniteur de Cursor"],
          ["IUIAutomation / TreeWalker", "Name, BoundingRectangle, ControlType"],
          ["RegisterHotKey", "Ctrl + Alt + F"],
          ["Shell_NotifyIcon", "Icône tray"],
          ["CreateWindowEx + UpdateLayeredWindow", "Overlay alpha"],
          ["ID2D1Factory / IDWriteFactory", "Dessin des labels"],
          ["Windows.Media.Ocr (MVP 2)", "Lecture des zones sans UIA"],
          ["Windows.Graphics.Capture (MVP 2)", "Capture HWND Cursor uniquement"],
        ]}
      />

      <H2>Outillage machine (Rémi)</H2>
      <Table
        headers={["Outil", "Obligatoire", "Rôle"]}
        rowTone={["success", "success", "success", "info", "neutral"]}
        rows={[
          [".NET 10 SDK", "Oui", "Compiler la solution"],
          ["Windows 11 SDK (10.0.22621+)", "Oui", "En-têtes Win32 / UIA"],
          [
            "Charge de travail WinUI / Windows App SDK",
            "Oui",
            "Build WinUI. Visual Studio 2022/2026 ou Build Tools suffisent.",
          ],
          [
            "Extension C# dans Cursor (ms-dotnettools.csharp)",
            "Oui",
            "IntelliSense, omnisharp/roslyn",
          ],
          [
            "Visual Studio complet",
            "Non",
            "Utile pour le designer XAML, pas bloquant",
          ],
        ]}
      />

      <Callout tone="info" title="Electron écarté pour le compagnon">
        Cursor est lui-même Electron/VS Code. Un second runtime Chromium
        alourdit l’overlay, complique UI Automation et le click-through.
        Tout le compagnon reste du C# natif Windows.
      </Callout>
    </Stack>
  );
}

function McpTab() {
  return (
    <Stack gap={20}>
      <Callout tone="info" title="Principe">
        Il n’existe pas de MCP WinUI, UI Automation ou Direct2D. Le
        développement Windows se fait en local (dotnet, CsWin32, tests). Les
        MCP servent à bootstrapper le repo, versionner, et éventuellement
        maquetter le shell.
      </Callout>

      <H2>MCP déjà disponibles dans ce workspace</H2>
      <Table
        headers={["Serveur MCP", "Statut plan", "Usage concret pour ce projet"]}
        striped
        rowTone={[
          "success",
          "success",
          "info",
          "neutral",
          "neutral",
          "danger",
          "danger",
        ]}
        rows={[
          [
            "cursor-app-control",
            "Requis — phase 0",
            "create_project + move_agent_to_root pour poser la solution C# dans le workspace.",
          ],
          [
            "plugin-github-github",
            "Requis — phase 0 et 7",
            "create_repository, branches, commits via gh, releases, issues des termes inconnus.",
          ],
          [
            "plugin-figma-figma",
            "Optionnel — phase 2",
            "Maquettes Home / Settings / Dictionary avant XAML. Pas obligatoire pour le MVP.",
          ],
          [
            "plugin-notion-workspace-notion",
            "Optionnel",
            "Doc produit si tu veux une copie Notion. Le canvas reste la source.",
          ],
          [
            "plugin-canva-canva",
            "Plus tard",
            "Visuels de lancement. Hors MVP.",
          ],
          [
            "plugin-playwright-playwright",
            "Ne pas utiliser",
            "Navigateur web. Inutile pour WinUI / overlay HWND.",
          ],
          [
            "plugin-datadog-datadog",
            "Ne pas utiliser au MVP",
            "Pas de télémétrie produit avant consentement et anonymisation.",
          ],
        ]}
      />

      <H2>Connecteurs MCP à ne pas inventer</H2>
      <Table
        headers={["Besoin", "Remplaçant réel"]}
        rows={[
          [
            "Lire l’UI de Cursor",
            "Module C# UiaReader + dump JSON local, pas un MCP.",
          ],
          [
            "Dessiner l’overlay",
            "OverlayRenderer Direct2D dans le process compagnon.",
          ],
          [
            "Traduire",
            "SQLite local. Aucun MCP de traduction. Aucune API cloud par défaut.",
          ],
          [
            "Tester l’UI Windows",
            "xUnit + FlaUI en projet de tests, lancés par dotnet test.",
          ],
        ]}
      />

      <H2>Plugins et artefacts Cursor à créer</H2>
      <Table
        headers={["Artefact", "Type", "Quand", "Contenu"]}
        striped
        rows={[
          [
            ".cursor/rules/no-injection.mdc",
            "Rule always-on",
            "Phase 0",
            "Interdit injection, .asar, hooks réseau, décompilation.",
          ],
          [
            ".cursor/rules/overlay-win32.mdc",
            "Rule glob Overlay/**",
            "Phase 0",
            "Overlay = Win32+D2D, pas de Window WinUI transparente.",
          ],
          [
            ".cursor/rules/csharp-winui.mdc",
            "Rule glob **/*.{cs,xaml}",
            "Phase 0",
            "Nullable, MVVM, pas de logique dans code-behind.",
          ],
          [
            "AGENTS.md",
            "Contexte repo",
            "Phase 0",
            "Architecture, commandes dotnet, garde-fous.",
          ],
          [
            "Skill dictionnaire",
            "Skill agent",
            "Phase 3",
            "Ajouter un terme EN→FR + test + entrée SQLite.",
          ],
          [
            "Plugin Cursor officiel",
            "Marketplace plugin",
            "Après MVP 1",
            "Uniquement dictionnaire / commandes. Pas l’overlay.",
          ],
        ]}
      />

      <H2>Extensions éditeur à installer dans Cursor</H2>
      <Table
        headers={["Extension", "ID", "Pourquoi"]}
        rows={[
          [
            "C#",
            "ms-dotnettools.csharp",
            "Langage, diagnostics, go-to-def",
          ],
          [
            "IntelliCode C# (optionnel)",
            "ms-dotnettools.vscodeintellicode-csharp",
            "Complétion",
          ],
          [
            "EditorConfig",
            "EditorConfig.EditorConfig",
            "Aligner le formatage",
          ],
          [
            "XAML (si disponible)",
            "selon marketplace Cursor",
            "Coloration XAML. Le designer reste Visual Studio.",
          ],
        ]}
      />

      <H2>Ce que j’utiliserai à chaque phase</H2>
      <Table
        headers={["Phase", "MCP / plugin", "Hors MCP"]}
        striped
        rows={[
          [
            "0 Bootstrap",
            "cursor-app-control, GitHub, rules Cursor",
            "dotnet new, SDK .NET 10, WinAppSDK",
          ],
          [
            "1 Validation",
            "Aucun MCP UI",
            "Probe C# console UIA + notes DPI",
          ],
          [
            "2 Shell WinUI",
            "Figma optionnel",
            "XAML + CommunityToolkit.Mvvm",
          ],
          [
            "3 Dictionnaire",
            "Skill interne plus tard",
            "SQLite + JSON seed",
          ],
          [
            "4–5 Overlay",
            "Aucun",
            "CsWin32, D2D, WinEventHook",
          ],
          [
            "6–7 Qualité / ship",
            "GitHub Releases",
            "dotnet test, MSIX, disclaimer",
          ],
        ]}
      />
    </Stack>
  );
}

function CodeTab() {
  return (
    <Stack gap={20}>
      <H2>Type de code, fichier par fichier</H2>
      <Table
        headers={["Type", "Extensions", "Où", "Interdit"]}
        striped
        rows={[
          [
            "C# bibliothèques",
            ".cs",
            "Core, Automation, Overlay, Native",
            "Code-behind gonflé, static god-objects",
          ],
          [
            "C# WinUI views",
            ".xaml.cs",
            "App/Views",
            "Logique métier, UIA, D2D",
          ],
          [
            "XAML WinUI",
            ".xaml",
            "App/Views, App/Styles",
            "Overlay, Canvas plein écran sur Cursor",
          ],
          [
            "CsWin32 native methods",
            "NativeMethods.txt + generated",
            "Native",
            "P/Invoke manuscrits dupliqués",
          ],
          [
            "SQL / seed",
            ".sql, .json",
            "Core/Data",
            "Envoyer le seed vers une API",
          ],
          [
            "Tests",
            "*Tests.cs",
            "tests/",
            "Tests qui injectent dans Cursor.exe",
          ],
          [
            "CI",
            ".yml",
            ".github/workflows",
            "Secrets dans le YAML",
          ],
        ]}
      />

      <H2>Solution</H2>
      <Text>
        Un seul exe : <Code>CursorFrancais.App</Code>. Les autres projets sont
        des class libraries référencées par l’app et les tests.
      </Text>
      <Table
        headers={["Projet", "TFM", "Responsabilité"]}
        striped
        rows={[
          [
            "src/CursorFrancais.App",
            "net10.0-windows10.0.22621.0",
            "WinUI, tray, hotkey, cycle de vie, composition DI",
          ],
          [
            "src/CursorFrancais.Core",
            "net10.0",
            "Modèles, dictionnaire, settings, journal, exclusions",
          ],
          [
            "src/CursorFrancais.Automation",
            "net10.0-windows",
            "Locator Cursor, UIA, filtres, snapshots",
          ],
          [
            "src/CursorFrancais.Overlay",
            "net10.0-windows",
            "HWND layered, D2D, layout labels, suivi fenêtre",
          ],
          [
            "src/CursorFrancais.Native",
            "net10.0-windows",
            "CsWin32 : hooks, DPI, DWM, notify icon",
          ],
          [
            "src/CursorFrancais.Ocr",
            "net10.0-windows10.0.22621.0",
            "MVP 2 seulement. Capture + OCR + purge",
          ],
          [
            "tests/CursorFrancais.Core.Tests",
            "net10.0",
            "Matching, exclusions, import/export",
          ],
          [
            "tests/CursorFrancais.Automation.Tests",
            "net10.0-windows",
            "Filtres UIA, fixtures JSON de dumps",
          ],
        ]}
      />

      <H2>Types C# centraux</H2>
      <Table
        headers={["Type", "Champs clés", "Projet"]}
        rows={[
          [
            "record CursorTarget",
            "ProcessId, MainHwnd, ExePath, Version, IsTrustedPath",
            "Automation",
          ],
          [
            "record UiElementHit",
            "AutomationId, Name, ControlType, ScreenRect, Dpi, Zone",
            "Automation",
          ],
          [
            "record TranslationLabel",
            "Source, Translated, Mode, RectDip, Opacity, IsBilingual",
            "Overlay",
          ],
          [
            "record DictionaryEntry",
            "En, Fr, Category, KeepEnglish, VersionHint",
            "Core",
          ],
          [
            "class AppState : ObservableObject",
            "IsEnabled, Mode, Zones, CursorStatus, UnknownCount",
            "App",
          ],
          [
            "enum TranslationMode",
            "FrenchOnly, Bilingual, Simplified",
            "Core",
          ],
          [
            "enum UiZone",
            "Menus, Buttons, Settings, AgentChat, Notifications, AiOutput",
            "Core",
          ],
        ]}
      />

      <H2>Arborescence cible</H2>
      <Table
        headers={["Chemin", "Contenu"]}
        striped
        rows={[
          ["CursorFrancais.slnx", "Solution .NET 10"],
          ["src/CursorFrancais.App/App.xaml", "Application WinUI, ressources thème"],
          ["src/CursorFrancais.App/MainWindow.xaml", "NavigationView"],
          ["src/CursorFrancais.App/Views/HomePage.xaml", "Toggle + statut"],
          ["src/CursorFrancais.App/ViewModels/*", "MVVM"],
          ["src/CursorFrancais.App/Services/TranslationHost.cs", "Orchestre le loop"],
          ["src/CursorFrancais.Core/Dictionary/*", "Store SQLite + matcher"],
          ["src/CursorFrancais.Core/Data/seed-fr.json", "150–200 termes"],
          ["src/CursorFrancais.Automation/CursorLocator.cs", "Process + path check"],
          ["src/CursorFrancais.Automation/UiaReader.cs", "Dump contrôles"],
          ["src/CursorFrancais.Overlay/OverlayWindow.cs", "CreateWindowEx layered"],
          ["src/CursorFrancais.Overlay/D2DRenderer.cs", "Direct2D / DirectWrite"],
          ["src/CursorFrancais.Native/NativeMethods.txt", "Liste APIs CsWin32"],
          ["assets/dictionary/", "Glossaires versionnés"],
          [".cursor/rules/", "Garde-fous agent"],
          ["docs/disclaimer-fr.md", "Non affilié à Cursor"],
        ]}
      />

      <H2>Pattern d’orchestration</H2>
      <Text>
        <Code>TranslationHost</Code> tourne sur un <Code>periodic Timer</Code>{" "}
        (et se réveille aussi sur WinEvent). Il n’ouvre aucun thread UI D2D
        hors du thread overlay. WinUI reste sur le dispatcher XAML. Les deux
        threads communiquent par <Code>Channel&lt;OverlayFrame&gt;</Code>.
      </Text>
      <Table
        headers={["Thread", "Travail"]}
        rows={[
          ["UI (WinUI)", "Pages, tray, binding AppState"],
          ["Automation", "UIA + locator. Timeout 40 ms, abandon si Cursor occupé"],
          ["Overlay", "Message loop Win32 + Present D2D"],
          ["IO", "SQLite et logs, jamais sur UIA"],
        ]}
      />
    </Stack>
  );
}

function TasksATab() {
  return (
    <Stack gap={20}>
      <Text>
        Phases 0 à 3 = fondation. Chaque ligne est une tâche agent
        exécutable, dans l’ordre. Ne pas sauter la phase 1 : sans dump UIA
        réel, l’overlay est de la spéculation.
      </Text>
      <BarChart
        categories={["0 Outillage", "1 Validation", "2 Shell", "3 Dictionnaire"]}
        series={[{ name: "Tâches", data: [8, 8, 9, 9], tone: "info" }]}
        height={180}
        showValues
      />

      <H2>Phase 0 — Outillage et solution</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Outils"]}
        striped
        stickyHeader
        rowTone={Array(8).fill("info") as Array<"info">}
        rows={[
          [
            "P0-01",
            "Vérifier .NET 10 SDK, Windows 11 SDK, charge WinAppSDK",
            "Compte-rendu versions (dotnet --info)",
            "Shell local",
          ],
          [
            "P0-02",
            "Installer l’extension C# dans Cursor si absente",
            "OmniSharp / Roslyn actif",
            "Marketplace Cursor",
          ],
          [
            "P0-03",
            "Créer la solution et les 7 projets listés",
            "CursorFrancais.slnx compile à vide",
            "dotnet new, cursor-app-control",
          ],
          [
            "P0-04",
            "Ajouter NuGet : WinAppSDK 2.4, CsWin32, Sqlite, Toolkit, Serilog, xunit",
            "restore OK",
            "dotnet add package",
          ],
          [
            "P0-05",
            "Écrire AGENTS.md + 3 rules Cursor (no-injection, overlay-win32, csharp-winui)",
            "Fichiers .cursor/rules",
            "create-rule",
          ],
          [
            "P0-06",
            "EditorConfig + Directory.Build.props (nullable, TreatWarningsAsErrors progressif)",
            "Build reproductible",
            "C#",
          ],
          [
            "P0-07",
            "Git init / remote GitHub si demandé",
            "Repo versionné, .gitignore dotnet",
            "GitHub MCP, gh",
          ],
          [
            "P0-08",
            "CI minimale : dotnet build + test sur windows-latest",
            ".github/workflows/ci.yml",
            "GitHub MCP",
          ],
        ]}
      />

      <H2>Phase 1 — Validation Cursor (preuve)</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Critère de fin"]}
        striped
        stickyHeader
        rows={[
          [
            "P1-01",
            "Écrire CursorLocator : process, chemin, éditeur, version fichier",
            "Locator + tests chemin fictif",
            "Refuse VS Code / autres Electron",
          ],
          [
            "P1-02",
            "Outil console DumpCursorUi : JSON des contrôles visibles",
            "tools/DumpCursorUi",
            "Fichier dump-YYYYMMDD.json",
          ],
          [
            "P1-03",
            "Lancer le dump sur Cursor ouvert (Agent, Chat, Composer, Settings)",
            "4 dumps annotés",
            "Liste des Name UIA réellement exposés",
          ],
          [
            "P1-04",
            "Mesurer les bounding boxes à 100 %, 125 %, 150 %",
            "Tableau DPI dans docs/uia-findings.md",
            "Écart documenté",
          ],
          [
            "P1-05",
            "Tester thème sombre / clair Cursor",
            "Notes contraste",
            "Décision couleurs overlay",
          ],
          [
            "P1-06",
            "Tester 2 fenêtres Cursor + écran secondaire",
            "Comportement multi-HWND",
            "Stratégie : overlay par fenêtre",
          ],
          [
            "P1-07",
            "Classer chaque contrôle : traduisible / protégé / inaccessible",
            "Matrice dans docs/uia-findings.md",
            "Base du filtre MVP 1",
          ],
          [
            "P1-08",
            "Go / no-go overlay UIA",
            "Décision écrite",
            "Si UIA &lt; 30 % des boutons, avancer OCR plus tôt",
          ],
        ]}
      />

      <H2>Phase 2 — Shell WinUI (sans overlay)</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Critère de fin"]}
        striped
        stickyHeader
        rows={[
          [
            "P2-01",
            "MainWindow + NavigationView (Accueil, Dictionnaire, Paramètres, Journal)",
            "XAML navigable",
            "4 pages s’ouvrent",
          ],
          [
            "P2-02",
            "HomePage : statut Cursor, Toggle Traduction, modes, zones",
            "Bindé à AppState",
            "Toggle persiste",
          ],
          [
            "P2-03",
            "SettingsPage : opacité, taille, auto-hide, hotkey, confidentialité",
            "UiSettings JSON local",
            "Reload conserve les valeurs",
          ],
          [
            "P2-04",
            "Tray : icône, flyout Activer / Ouvrir / Quitter",
            "NotifyIcon",
            "Fermer la fenêtre laisse le tray",
          ],
          [
            "P2-05",
            "Hotkey global Ctrl + Alt + F (configurable)",
            "RegisterHotKey",
            "Toggle même si Cursor a le focus",
          ],
          [
            "P2-06",
            "Démarrage auto Windows (option, désactivé par défaut)",
            "Raccourci Startup ou Task",
            "Réversible depuis Settings",
          ],
          [
            "P2-07",
            "Thème sombre Windows 11, typo compacte",
            "ResourceDictionary",
            "Pas de logo Cursor copié",
          ],
          [
            "P2-08",
            "Bandeau « Projet communautaire non affilié »",
            "InfoBar permanente Accueil",
            "Toujours visible",
          ],
          [
            "P2-09",
            "Maquettes Figma optionnelles si tu le demandes",
            "Fichier Figma",
            "Sinon XAML direct",
          ],
        ]}
      />

      <H2>Phase 3 — Moteur de dictionnaire</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Critère de fin"]}
        striped
        stickyHeader
        rows={[
          [
            "P3-01",
            "Schéma SQLite : entries, unknown_terms, meta",
            "Migrator v1",
            "Fichier %LocalAppData%/CursorFrancais/dict.db",
          ],
          [
            "P3-02",
            "Seed 150–200 termes Cursor (menus, Agent, Chat, Composer)",
            "seed-fr.json",
            "Apply/Reject/Run/Settings/… couverts",
          ],
          [
            "P3-03",
            "Normalizer : casse, & accélérateurs, ellipses, espaces",
            "TextNormalizer",
            "« New Chat... » → « New Chat »",
          ],
          [
            "P3-04",
            "Matcher exact puis normalisé ; keep-english (Agent, Composer)",
            "DictionaryEngine",
            "Tests unitaires ≥ 30 cas",
          ],
          [
            "P3-05",
            "Exclusions : chemins, extensions, extraits code, IDs modèles",
            "ExclusionRules",
            "gpt-4o, C:\\proj\\a.cs, npm test non traduits",
          ],
          [
            "P3-06",
            "Journal unknown_terms (texte, zone, count, dernière vue)",
            "UnknownTermLog",
            "Visible dans LogPage",
          ],
          [
            "P3-07",
            "CRUD dictionnaire + import/export JSON",
            "DictionaryPage fonctionnelle",
            "Round-trip fichier",
          ],
          [
            "P3-08",
            "Catégories : Menus, Buttons, Agent, Settings, System",
            "Champ Category",
            "Filtre ListView",
          ],
          [
            "P3-09",
            "Skill Cursor « ajouter un terme »",
            "SKILL.md interne",
            "Agent peut ajouter EN→FR + test",
          ],
        ]}
      />
    </Stack>
  );
}

function TasksBTab() {
  return (
    <Stack gap={20}>
      <Text>
        Phases 4 à 7 = cœur produit puis durcissement. L’overlay (phase 5)
        commence seulement si la phase 4 fournit des rectangles stables.
      </Text>
      <BarChart
        categories={["4 UIA", "5 Overlay", "6 OCR", "7 Qualité"]}
        series={[{ name: "Tâches", data: [8, 10, 6, 6], tone: "info" }]}
        height={180}
        showValues
      />

      <H2>Phase 4 — Détection et UI Automation</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Critère de fin"]}
        striped
        stickyHeader
        rows={[
          [
            "P4-01",
            "WindowTracker : hook move/resize/focus/destroy",
            "Events CursorBoundsChanged",
            "Latence visuelle &lt; 100 ms au drag",
          ],
          [
            "P4-02",
            "UiaReader.ReadVisible(hwnd) avec timeout",
            "IReadOnlyList&lt;UiElementHit&gt;",
            "Pas de freeze UI si Cursor lent",
          ],
          [
            "P4-03",
            "ElementFilter : types autorisés + zones code interdites",
            "FilterResult",
            "0 label sur l’éditeur Monaco",
          ],
          [
            "P4-04",
            "Mapper DPI : UIA rect → pixels overlay",
            "CoordinateMapper",
            "Alignement ±2 px à 150 %",
          ],
          [
            "P4-05",
            "Cache + dirty-check (hash des Name+Rect)",
            "FrameDiffer",
            "Pas de redraw si rien n’a changé",
          ],
          [
            "P4-06",
            "Support N fenêtres Cursor = N overlays",
            "OverlayHost collection",
            "Fermer une fenêtre détruit son overlay",
          ],
          [
            "P4-07",
            "Zone Agent/Chat : heuristique titre / AutomationId",
            "ZoneClassifier",
            "Respect du checkbox « Agent et Chat »",
          ],
          [
            "P4-08",
            "Tests sur dumps JSON figés (pas besoin de Cursor ouvert)",
            "Automation.Tests",
            "CI verte sans GUI Cursor",
          ],
        ]}
      />

      <H2>Phase 5 — Overlay graphique</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Critère de fin"]}
        striped
        stickyHeader
        rowTone={[
          "warning",
          "warning",
          "warning",
          "warning",
          "info",
          "info",
          "info",
          "neutral",
          "neutral",
          "success",
        ]}
        rows={[
          [
            "P5-01",
            "Créer HWND layered click-through (CreateWindowEx)",
            "OverlayWindow.cs",
            "Clics atteignent Cursor à 100 %",
          ],
          [
            "P5-02",
            "Init Direct2D + DirectWrite, render target BGRA",
            "D2DRenderer.cs",
            "Texte net à 100/125/150 %",
          ],
          [
            "P5-03",
            "UpdateLayeredWindow à chaque frame dirty",
            "Present()",
            "Pas de fond noir opaque",
          ],
          [
            "P5-04",
            "Dessiner label : pill fond + texte FR, marge 2–4 px",
            "LabelPainter",
            "Lisible sur thème sombre Cursor",
          ],
          [
            "P5-05",
            "Layout anti-collision (décaler à droite / au-dessus)",
            "LabelLayout",
            "Deux labels ne se recouvrent pas",
          ],
          [
            "P5-06",
            "Sync géométrie avec WindowTracker",
            "SetWindowPos no-activate",
            "Overlay = rect Cursor, y compris maximisé",
          ],
          [
            "P5-07",
            "Hide si Cursor n’est plus foreground (setting)",
            "ForegroundPolicy",
            "Alt-Tab vers autre app masque l’overlay",
          ],
          [
            "P5-08",
            "Mode bilingue : « FR — EN » dans le pill",
            "TranslationLabel.IsBilingual",
            "Réglage HomePage",
          ],
          [
            "P5-09",
            "Opacité et taille depuis UiSettings",
            "Re-render immédiat",
            "Slider 50–100 % visible",
          ],
          [
            "P5-10",
            "Badge « Traduction ON · N éléments »",
            "Coin overlay",
            "Click-through aussi",
          ],
        ]}
      />

      <H2>Phase 6 — OCR de secours (après MVP 1 validé)</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Critère de fin"]}
        striped
        rows={[
          [
            "P6-01",
            "Activer projet CursorFrancais.Ocr",
            "Csproj décommenté",
            "Réf App conditionnelle",
          ],
          [
            "P6-02",
            "Graphics Capture du HWND Cursor uniquement",
            "CursorFrameGrabber",
            "Aucune autre fenêtre dans le bitmap",
          ],
          [
            "P6-03",
            "Windows.Media.Ocr sur ROI sans Name UIA",
            "OcrEngineHost",
            "Texte + box",
          ],
          [
            "P6-04",
            "Purge immédiate du bitmap (IDisposable, 0 fichier disque)",
            "using scope",
            "Setting « ne jamais enregistrer » honoré",
          ],
          [
            "P6-05",
            "Governor CPU : max 2 OCR/s, stop si CPU overlay &gt; seuil",
            "OcrGovernor",
            "Désactivable",
          ],
          [
            "P6-06",
            "Ne jamais OCR la zone éditeur / terminal",
            "Reuse ExclusionRules",
            "Test de ROI",
          ],
        ]}
      />

      <H2>Phase 7 — Qualité, packaging, distribution</H2>
      <Table
        headers={["ID", "Tâche", "Livrable", "Critère de fin"]}
        striped
        rows={[
          [
            "P7-01",
            "Matrice manuelle : start/stop, update Cursor, DPI, multi-écran, fullscreen, modal",
            "docs/qa-matrix.md cochée",
            "Aucun crash, overlay jamais bloquant",
          ],
          [
            "P7-02",
            "Budget perf : idle &lt; 1 % CPU, &lt; 80 Mo RAM hors OCR",
            "Mesure Process",
            "Documenté",
          ],
          [
            "P7-03",
            "Portable zip + installeur MSIX unsigned d’abord",
            "artifacts/",
            "Lance sans admin",
          ],
          [
            "P7-04",
            "Disclaimer FR + LICENSE + NOTICE non affilié",
            "docs + écran Accueil",
            "Texte visible au premier lancement",
          ],
          [
            "P7-05",
            "GitHub Release + notes de version + versions Cursor testées",
            "Release MCP GitHub",
            "Tag v0.1.0",
          ],
          [
            "P7-06",
            "Signature Authenticode si certificat dispo",
            "binaires signés",
            "Sinon documenter le warning SmartScreen",
          ],
        ]}
      />
    </Stack>
  );
}

function TestsTab() {
  return (
    <Stack gap={20}>
      <H2>Pyramide de tests</H2>
      <Table
        headers={["Niveau", "Quoi", "Outil", "Quand"]}
        striped
        rowTone={["success", "info", "warning", "neutral"]}
        rows={[
          [
            "Unitaire",
            "Normalizer, matcher, exclusions, settings JSON",
            "xUnit",
            "Chaque PR, dès P3",
          ],
          [
            "Snapshot UIA",
            "Filtres sur dumps JSON figés",
            "xUnit + fixtures",
            "Dès P4-08, en CI",
          ],
          [
            "Intégration machine",
            "Locator + overlay click-through sur Cursor réel",
            "Script manuel / FlaUI",
            "P5 et P7, pas en CI publique",
          ],
          [
            "Perf",
            "CPU/RAM, fréquence de frames",
            "dotnet-counters",
            "P7-02",
          ],
        ]}
      />

      <H2>Matrice QA manuelle (P7-01)</H2>
      <Table
        headers={["Scénario", "Attendu"]}
        striped
        stickyHeader
        rows={[
          ["Cursor fermé au lancement du compagnon", "Statut « non détecté », overlay off"],
          ["Ouvrir Cursor ensuite", "Détection &lt; 2 s, overlay si activé"],
          ["Fermer Cursor", "Overlay détruit, pas d’exception"],
          ["Resize / maximize / snap Windows", "Labels recollés, pas de fantômes"],
          ["DPI 100 / 125 / 150", "Texte net, décalage ≤ 2 px"],
          ["Thème Cursor sombre et clair", "Contraste labels OK"],
          ["Deux fenêtres Cursor", "Deux overlays indépendants"],
          ["Écran secondaire", "Overlay sur le bon moniteur"],
          ["Plein écran Zen / distraction-free", "Pas de labels orphelins"],
          ["Menu déroulant / palette commande", "Soit traduit, soit ignoré, jamais bloqué"],
          ["Clic sur un bouton recouvert d’un label", "Le bouton Cursor s’active"],
          ["Raccourcis Cursor (Ctrl+K, etc.)", "Inchangés"],
          ["Ctrl+Alt+F", "Toggle immédiat"],
          ["Focus autre application", "Overlay masqué si option on"],
          ["Sleep / reprise", "Reprise propre, re-hook"],
          ["Mise à jour Cursor", "Pas de crash ; profil « unknown version »"],
        ]}
      />

      <H2>Ordre d’exécution pour moi (agent)</H2>
      <Table
        headers={["Ordre", "Action", "Stop si"]}
        rows={[
          [
            "1",
            "Phase 0 complète jusqu’à sln qui build",
            "SDK .NET 10 ou WinAppSDK manquant — te le dire",
          ],
          [
            "2",
            "Phase 1 dump UIA avec Cursor ouvert chez toi",
            "Tu n’as pas Cursor lancé — te demander de l’ouvrir",
          ],
          [
            "3",
            "Phases 2 + 3 en parallèle une fois P1-08 = go",
            "UIA trop pauvre — basculer OCR plus tôt, te le proposer",
          ],
          [
            "4",
            "Phase 4 puis 5 (overlay)",
            "Click-through échoue — ne pas « bricoler » en injectant Cursor",
          ],
          [
            "5",
            "P7 partiel (portable) = MVP 1 shippable",
            "—",
          ],
          [
            "6",
            "Phase 6 OCR seulement après usage réel du MVP 1",
            "CPU / privacy non validés",
          ],
        ]}
      />

      <H2>Hors périmètre jusqu’au MVP 1</H2>
      <Table
        headers={["Interdit", "Raison"]}
        rowTone={["danger", "danger", "danger", "danger", "warning", "warning"]}
        rows={[
          ["Injection JS / DLL dans Cursor.exe", "Instable, antivirus, ToS"],
          ["Patch des .asar / ressources", "Cassé à chaque update"],
          ["MCP Playwright pour « cliquer Cursor »", "Mauvais outil, UI desktop"],
          ["Envoi de captures vers un LLM", "Confidentialité"],
          ["Plugin marketplace comme véhicule d’overlay", "Les plugins ne redessinent pas l’UI native"],
          ["Promettre 100 % de l’UI", "UIA partielle + UI dynamique"],
        ]}
      />

      <Callout tone="success" title="MVP 1 = fin de P5 + P7-03/04">
        Détection Cursor, toggle, 150 termes, labels UIA en overlay
        click-through, journal des inconnus, portable zip, disclaimer. OCR,
        multi-langue et Store = plus tard.
      </Callout>

      <StartBuildButton />
    </Stack>
  );
}
