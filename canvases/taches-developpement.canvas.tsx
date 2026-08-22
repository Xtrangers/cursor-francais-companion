import {
  Button,
  Callout,
  Grid,
  H1,
  H2,
  Pill,
  Row,
  Stack,
  Stat,
  Table,
  Text,
  UsageBar,
  useCanvasState,
} from "cursor/canvas";

type Status = "todo" | "doing" | "done";
type PhaseId = "p0" | "p1" | "p2" | "p3" | "p4" | "p5" | "p6" | "p7" | "p8";

type Task = {
  id: string;
  phase: PhaseId;
  title: string;
  summary: string;
  steps: string;
};

const PHASES: { id: PhaseId; label: string; goal: string; jalon: boolean }[] = [
  {
    id: "p0",
    label: "0 · Outillage",
    goal: "Solution .NET 10 WPF qui compile à vide.",
    jalon: false,
  },
  {
    id: "p1",
    label: "1 · Validation UIA",
    goal: "Savoir ce que Cursor expose vraiment.",
    jalon: true,
  },
  {
    id: "p2",
    label: "2 · Shell WPF",
    goal: "Menu à 4 modules, Traducteur seul actif.",
    jalon: true,
  },
  {
    id: "p3",
    label: "3 · Dictionnaire",
    goal: "150–200 termes, exclusions, journal.",
    jalon: false,
  },
  {
    id: "p4",
    label: "4 · Automation",
    goal: "Rectangles UIA stables, jamais sur le code.",
    jalon: false,
  },
  {
    id: "p5",
    label: "5 · Overlay",
    goal: "Labels FR click-through collés à Cursor.",
    jalon: false,
  },
  {
    id: "p6",
    label: "6 · OCR",
    goal: "Secours local, après le MVP 1.",
    jalon: false,
  },
  {
    id: "p7",
    label: "7 · Livraison",
    goal: "Zip portable, QA, disclaimer.",
    jalon: true,
  },
  {
    id: "p8",
    label: "8 · Modules",
    goal: "Skills, Projets, Agents — après le traducteur.",
    jalon: true,
  },
];

const TASKS: Task[] = [
  {
    id: "P0-01",
    phase: "p0",
    title: "Vérifier le SDK",
    summary: "Confirmer que la machine compile du WPF .NET 10.",
    steps: "1. dotnet --info  ·  2. dotnet new wpf -o _probe  ·  3. Noter SDK et Windows",
  },
  {
    id: "P0-02",
    phase: "p0",
    title: "Extension C#",
    summary: "Activer IntelliSense C# dans Cursor.",
    steps: "1. Vérifier ms-dotnettools.csharp  ·  2. Installer si absent  ·  3. Ouvrir un .cs",
  },
  {
    id: "P0-03",
    phase: "p0",
    title: "Créer la solution",
    summary: "Poser App, Core, Automation, Overlay, Native, Ocr, Tests.",
    steps: "1. slnx  ·  2. 7 csproj  ·  3. dotnet build à vide",
  },
  {
    id: "P0-04",
    phase: "p0",
    title: "Paquets NuGet",
    summary: "Ajouter uniquement les dépendances gratuites prévues.",
    steps: "1. Mvvm + NotifyIcon  ·  2. CsWin32 + Sqlite + Serilog  ·  3. xunit restore",
  },
  {
    id: "P0-05",
    phase: "p0",
    title: "Rules agent",
    summary: "AGENTS déjà là ; ajouter les 3 rules techniques.",
    steps: "1. overlay-win32  ·  2. csharp-wpf  ·  3. no-injection (rappel)",
  },
  {
    id: "P0-06",
    phase: "p0",
    title: "Build props",
    summary: "Nullable et formatage reproductibles.",
    steps: "1. EditorConfig  ·  2. Directory.Build.props  ·  3. Build sans warning nouveau",
  },
  {
    id: "P0-07",
    phase: "p0",
    title: "Git déjà en place",
    summary: "Repo existe. Vérifier ignore et branches.",
    steps: "1. Contrôler .gitignore  ·  2. Rester sur une branche de travail  ·  3. Pas de push",
  },
  {
    id: "P0-08",
    phase: "p0",
    title: "CI Windows",
    summary: "Build + tests sur windows-latest, sans secret.",
    steps: "1. workflow ci.yml  ·  2. build  ·  3. test",
  },
  {
    id: "P1-01",
    phase: "p1",
    title: "Locator Cursor",
    summary: "Trouver Cursor.exe et refuser les autres Electron.",
    steps: "1. Process + chemin  ·  2. Tests chemins fictifs  ·  3. Rejeter VS Code",
  },
  {
    id: "P1-02",
    phase: "p1",
    title: "Outil dump UIA",
    summary: "Console qui écrit les contrôles visibles en JSON.",
    steps: "1. tools/DumpCursorUi  ·  2. Name + Rect + Type  ·  3. Fichier horodaté",
  },
  {
    id: "P1-03",
    phase: "p1",
    title: "Dumps des 4 zones",
    summary: "Photographier Agent, Chat, Composer, Settings.",
    steps: "1. Ouvrir chaque zone  ·  2. Dump  ·  3. Annoter les Name utiles",
  },
  {
    id: "P1-04",
    phase: "p1",
    title: "Mesures DPI",
    summary: "Voir le décalage à 100, 125 et 150 %.",
    steps: "1. Trois échelles  ·  2. Tableau dans docs/uia-findings.md  ·  3. Écart noté",
  },
  {
    id: "P1-05",
    phase: "p1",
    title: "Thèmes Cursor",
    summary: "Décider les couleurs des labels sombre / clair.",
    steps: "1. Dump thème sombre  ·  2. Thème clair  ·  3. Contraste choisi",
  },
  {
    id: "P1-06",
    phase: "p1",
    title: "Multi-fenêtres",
    summary: "Deux Cursor et un écran secondaire.",
    steps: "1. Deux HWND  ·  2. Écran 2  ·  3. Règle : un overlay par fenêtre",
  },
  {
    id: "P1-07",
    phase: "p1",
    title: "Matrice des contrôles",
    summary: "Classer : traduisible, protégé, inaccessible.",
    steps: "1. Lire les dumps  ·  2. Remplir la matrice  ·  3. Base du filtre MVP",
  },
  {
    id: "P1-08",
    phase: "p1",
    title: "Go / no-go UIA",
    summary: "Décider si l’overlay UIA suffit. Jalon : attendre Rémi.",
    steps: "1. Compter les boutons exposés  ·  2. Si < 30 % → OCR plus tôt  ·  3. Stop jalon",
  },
  {
    id: "P2-01",
    phase: "p2",
    title: "Chrome MainWindow",
    summary: "Fenêtre WPF : menu gauche, contenu, statut.",
    steps: "1. XAML split  ·  2. Resize  ·  3. Thème sombre",
  },
  {
    id: "P2-02",
    phase: "p2",
    title: "Contrat modules",
    summary: "ICompanionModule + registry + ShellViewModel.",
    steps: "1. Interface  ·  2. DI  ·  3. SelectedModule change la vue",
  },
  {
    id: "P2-03",
    phase: "p2",
    title: "Quatre entrées menu",
    summary: "Traducteur, Skills, Projets, Agents visibles et cliquables.",
    steps: "1. ListBox bindée  ·  2. Ordre fixe  ·  3. Libellés FR",
  },
  {
    id: "P2-04",
    phase: "p2",
    title: "Écran Bientôt",
    summary: "Seul Traducteur est actif ; les autres expliquent la suite.",
    steps: "1. ComingSoonView  ·  2. IsAvailable  ·  3. Pas de crash au clic",
  },
  {
    id: "P2-05",
    phase: "p2",
    title: "Accueil Traducteur",
    summary: "Statut Cursor, toggle, modes, zones.",
    steps: "1. Bindings  ·  2. Persistance  ·  3. États vide / erreur / ok",
  },
  {
    id: "P2-06",
    phase: "p2",
    title: "Sous-pages Traducteur",
    summary: "Réglages, Dictionnaire, Journal dans le module.",
    steps: "1. Nav interne  ·  2. Menu gauche reste Traducteur  ·  3. Retour accueil",
  },
  {
    id: "P2-07",
    phase: "p2",
    title: "Réglages UI",
    summary: "Opacité, taille, auto-hide, hotkey, confidentialité.",
    steps: "1. Formulaire  ·  2. JSON local  ·  3. Reload conserve",
  },
  {
    id: "P2-08",
    phase: "p2",
    title: "Icône tray",
    summary: "Activer, ouvrir, quitter depuis la zone de notification.",
    steps: "1. NotifyIcon  ·  2. Fermer ≠ quitter  ·  3. Libellés FR",
  },
  {
    id: "P2-09",
    phase: "p2",
    title: "Raccourci global",
    summary: "Ctrl + Alt + F bascule la traduction.",
    steps: "1. RegisterHotKey  ·  2. Configurable  ·  3. Marche sous Cursor",
  },
  {
    id: "P2-10",
    phase: "p2",
    title: "Démarrage auto",
    summary: "Option off par défaut, réversible.",
    steps: "1. Case à cocher  ·  2. Raccourci Startup  ·  3. Retirer proprement",
  },
  {
    id: "P2-11",
    phase: "p2",
    title: "Thème WPF",
    summary: "Sombre, compact, sans copier le logo Cursor.",
    steps: "1. Resources  ·  2. Contraste  ·  3. Contrôles accessibles",
  },
  {
    id: "P2-12",
    phase: "p2",
    title: "Bandeau non affilié",
    summary: "Mention communautaire visible dans tout le shell.",
    steps: "1. Texte FR  ·  2. Tous les modules  ·  3. Premier lancement",
  },
  {
    id: "P2-13",
    phase: "p2",
    title: "Dernier module",
    summary: "Mémoriser l’entrée de menu (Traducteur au MVP).",
    steps: "1. LastModuleId  ·  2. Restore au boot  ·  3. Prêt phase 8",
  },
  {
    id: "P2-14",
    phase: "p2",
    title: "Maquettes optionnelles",
    summary: "Figma seulement si Rémi le demande.",
    steps: "1. Sinon XAML direct  ·  2. Ne pas bloquer  ·  3. —",
  },
  {
    id: "P3-01",
    phase: "p3",
    title: "Schéma SQLite",
    summary: "entries, unknown_terms, meta en local.",
    steps: "1. Migrator v1  ·  2. %LocalAppData%  ·  3. Tests migration",
  },
  {
    id: "P3-02",
    phase: "p3",
    title: "Seed 150–200 termes",
    summary: "Menus, Agent, Chat, Composer, Settings.",
    steps: "1. seed-fr.json  ·  2. Apply/Reject/Run  ·  3. Import au 1er lancement",
  },
  {
    id: "P3-03",
    phase: "p3",
    title: "Normalizer",
    summary: "Casse, points de suspension, accélérateurs.",
    steps: "1. New Chat... → New Chat  ·  2. Tests  ·  3. & ignoré",
  },
  {
    id: "P3-04",
    phase: "p3",
    title: "Matcher",
    summary: "Exact, puis normalisé. Agent/Composer restent EN.",
    steps: "1. Exact  ·  2. Normalisé  ·  3. ≥ 30 tests",
  },
  {
    id: "P3-05",
    phase: "p3",
    title: "Exclusions",
    summary: "Code, chemins, commandes, IDs de modèles.",
    steps: "1. Règles  ·  2. Cas gpt-4o / .cs / npm  ·  3. Tests",
  },
  {
    id: "P3-06",
    phase: "p3",
    title: "Journal inconnus",
    summary: "Compter les textes sans traduction.",
    steps: "1. Table unknown  ·  2. Vue Journal  ·  3. Compteur",
  },
  {
    id: "P3-07",
    phase: "p3",
    title: "CRUD dictionnaire",
    summary: "Ajouter, importer, exporter sans perte.",
    steps: "1. UI liste  ·  2. JSON round-trip  ·  3. États vide / erreur",
  },
  {
    id: "P3-08",
    phase: "p3",
    title: "Catégories",
    summary: "Filtrer Menus, Boutons, Agent, Settings.",
    steps: "1. Champ Category  ·  2. Filtre ListView  ·  3. Seed tagué",
  },
  {
    id: "P3-09",
    phase: "p3",
    title: "Skill ajouter un terme",
    summary: "L’agent peut ajouter EN→FR + test.",
    steps: "1. SKILL.md  ·  2. Exemple  ·  3. Pas d’API cloud",
  },
  {
    id: "P4-01",
    phase: "p4",
    title: "WindowTracker",
    summary: "Suivre move, resize, focus, destroy de Cursor.",
    steps: "1. WinEventHook  ·  2. Events  ·  3. Latence < 100 ms",
  },
  {
    id: "P4-02",
    phase: "p4",
    title: "UiaReader",
    summary: "Lire l’arbre visible avec timeout.",
    steps: "1. TreeWalker  ·  2. Timeout 40 ms  ·  3. Pas de freeze WPF",
  },
  {
    id: "P4-03",
    phase: "p4",
    title: "Filtre éléments",
    summary: "Zéro label sur l’éditeur / Monaco.",
    steps: "1. Types autorisés  ·  2. Zones code  ·  3. Tests dumps",
  },
  {
    id: "P4-04",
    phase: "p4",
    title: "Mapper DPI",
    summary: "UIA → pixels overlay, ±2 px à 150 %.",
    steps: "1. GetDpiForWindow  ·  2. Conversion  ·  3. Cas 100/125/150",
  },
  {
    id: "P4-05",
    phase: "p4",
    title: "Cache dirty",
    summary: "Ne redessiner que si Name ou Rect change.",
    steps: "1. Hash frame  ·  2. Skip identique  ·  3. Mesure CPU",
  },
  {
    id: "P4-06",
    phase: "p4",
    title: "N overlays",
    summary: "Une couche par fenêtre Cursor.",
    steps: "1. Collection  ·  2. Destroy à la fermeture  ·  3. Test 2 HWND",
  },
  {
    id: "P4-07",
    phase: "p4",
    title: "Zone Agent/Chat",
    summary: "Honorer la case « Agent et Chat ».",
    steps: "1. Heuristique titre  ·  2. AutomationId  ·  3. Filtre checkbox",
  },
  {
    id: "P4-08",
    phase: "p4",
    title: "Tests dumps figés",
    summary: "CI sans Cursor ouvert.",
    steps: "1. Fixtures JSON  ·  2. xunit  ·  3. CI verte",
  },
  {
    id: "P5-01",
    phase: "p5",
    title: "HWND layered",
    summary: "Fenêtre click-through au-dessus de Cursor.",
    steps: "1. CreateWindowEx  ·  2. WS_EX_TRANSPARENT  ·  3. Clic = Cursor",
  },
  {
    id: "P5-02",
    phase: "p5",
    title: "Direct2D",
    summary: "Texte net à tous les DPI.",
    steps: "1. Factory D2D/DWrite  ·  2. BGRA  ·  3. Contrôle 150 %",
  },
  {
    id: "P5-03",
    phase: "p5",
    title: "Present alpha",
    summary: "UpdateLayeredWindow sans fond noir.",
    steps: "1. Present  ·  2. Alpha  ·  3. Pas d’opaque",
  },
  {
    id: "P5-04",
    phase: "p5",
    title: "Pills FR",
    summary: "Fond + texte, lisible sur thème sombre.",
    steps: "1. LabelPainter  ·  2. Marge 2–4 px  ·  3. Contraste",
  },
  {
    id: "P5-05",
    phase: "p5",
    title: "Anti-collision",
    summary: "Deux labels ne se recouvrent pas.",
    steps: "1. Layout  ·  2. Décalage  ·  3. Cas menus denses",
  },
  {
    id: "P5-06",
    phase: "p5",
    title: "Suivi géométrie",
    summary: "L’overlay épouse Cursor, même maximisé.",
    steps: "1. SetWindowPos  ·  2. No-activate  ·  3. Snap Windows",
  },
  {
    id: "P5-07",
    phase: "p5",
    title: "Hide hors focus",
    summary: "Alt-Tab masque l’overlay si l’option est on.",
    steps: "1. ForegroundPolicy  ·  2. Setting  ·  3. Retour Cursor",
  },
  {
    id: "P5-08",
    phase: "p5",
    title: "Mode bilingue",
    summary: "Afficher « Exécuter — Run ».",
    steps: "1. Flag  ·  2. Rendu  ·  3. Case accueil",
  },
  {
    id: "P5-09",
    phase: "p5",
    title: "Opacité / taille",
    summary: "Les sliders WPF changent l’overlay tout de suite.",
    steps: "1. Bind settings  ·  2. Re-render  ·  3. 50–100 %",
  },
  {
    id: "P5-10",
    phase: "p5",
    title: "Badge ON",
    summary: "Pastille « Traduction ON · N », click-through.",
    steps: "1. Coin overlay  ·  2. Compteur  ·  3. Pas de hit-test",
  },
  {
    id: "P6-01",
    phase: "p6",
    title: "Activer le projet OCR",
    summary: "Après validation du MVP 1 seulement.",
    steps: "1. Csproj  ·  2. Réf App  ·  3. Flag compil",
  },
  {
    id: "P6-02",
    phase: "p6",
    title: "Capture HWND Cursor",
    summary: "Aucune autre fenêtre dans le bitmap.",
    steps: "1. Graphics Capture  ·  2. HWND seul  ·  3. Test",
  },
  {
    id: "P6-03",
    phase: "p6",
    title: "OCR local",
    summary: "Lire les ROI sans Name UIA.",
    steps: "1. Windows.Media.Ocr  ·  2. Texte + box  ·  3. Dictionnaire",
  },
  {
    id: "P6-04",
    phase: "p6",
    title: "Purge image",
    summary: "Zéro fichier disque, dispose immédiat.",
    steps: "1. using  ·  2. Pas de save  ·  3. Setting honoré",
  },
  {
    id: "P6-05",
    phase: "p6",
    title: "Governor CPU",
    summary: "Max 2 OCR/s, coupure si charge haute.",
    steps: "1. Throttle  ·  2. Seuil  ·  3. Off possible",
  },
  {
    id: "P6-06",
    phase: "p6",
    title: "Pas d’OCR éditeur",
    summary: "Réutiliser les exclusions du dictionnaire.",
    steps: "1. ROI filter  ·  2. Terminal exclu  ·  3. Test",
  },
  {
    id: "P7-01",
    phase: "p7",
    title: "Matrice QA",
    summary: "Parcourir tous les scénarios manuels.",
    steps: "1. docs/qa-matrix.md  ·  2. Cocher  ·  3. Overlay jamais bloquant",
  },
  {
    id: "P7-02",
    phase: "p7",
    title: "Budget perf",
    summary: "Idle < 1 % CPU, < 80 Mo hors OCR.",
    steps: "1. Mesure Process  ·  2. Noter  ·  3. Régler le throttle",
  },
  {
    id: "P7-03",
    phase: "p7",
    title: "Portable + MSIX",
    summary: "Lancer sans admin. Jalon livraison.",
    steps: "1. Zip  ·  2. MSIX unsigned  ·  3. Test machine propre",
  },
  {
    id: "P7-04",
    phase: "p7",
    title: "Disclaimer",
    summary: "Non affilié, visible au premier lancement.",
    steps: "1. docs  ·  2. Écran  ·  3. LICENSE",
  },
  {
    id: "P7-05",
    phase: "p7",
    title: "Release GitHub",
    summary: "Tag + notes + versions Cursor testées. Push seulement si demandé.",
    steps: "1. Notes FR  ·  2. Tag v0.1.0  ·  3. Attendre Rémi pour publier",
  },
  {
    id: "P7-06",
    phase: "p7",
    title: "Signature",
    summary: "Authenticode si certificat, sinon SmartScreen documenté.",
    steps: "1. Certificat ?  ·  2. Signer  ·  3. Sinon doc warning",
  },
  {
    id: "P8-01",
    phase: "p8",
    title: "Module Skills",
    summary: "Lister / activer les skills locaux, sans injection.",
    steps: "1. SkillsView  ·  2. IsAvailable true  ·  3. Fichiers user only",
  },
  {
    id: "P8-02",
    phase: "p8",
    title: "Module Projets",
    summary: "Profils de workspaces et dernier dossier.",
    steps: "1. ProjectsView  ·  2. Ouvrir dossier  ·  3. Mémoire chemin",
  },
  {
    id: "P8-03",
    phase: "p8",
    title: "Module Agents",
    summary: "Liste et presets, pas de fork Cursor.",
    steps: "1. AgentsView  ·  2. Presets  ·  3. Local / docs",
  },
  {
    id: "P8-04",
    phase: "p8",
    title: "Settings isolés",
    summary: "Un module cassé n’arrête pas le traducteur.",
    steps: "1. Store par module  ·  2. try/catch  ·  3. Test panne",
  },
  {
    id: "P8-05",
    phase: "p8",
    title: "Badges menu",
    summary: "Passer Bientôt → Actif sans recoder le chrome.",
    steps: "1. IsAvailable  ·  2. Badge  ·  3. Menu inchangé",
  },
  {
    id: "P8-06",
    phase: "p8",
    title: "Garde-fou ordre",
    summary: "Interdit avant P5 + P7-03. Jalon : attendre Rémi.",
    steps: "1. Vérifier MVP 1  ·  2. Validation  ·  3. Alors seulement P8-01",
  },
];

const STATUS_LABEL: Record<Status, string> = {
  todo: "À faire",
  doing: "En cours",
  done: "Fait",
};

function nextStatus(s: Status): Status {
  if (s === "todo") return "doing";
  if (s === "doing") return "done";
  return "todo";
}

export default function TachesDeveloppement() {
  const [phase, setPhase] = useCanvasState<PhaseId | "all">("tasks-phase", "p0");
  const [statuses, setStatuses] = useCanvasState<Record<string, Status>>(
    "tasks-status",
    {},
  );

  const statusOf = (id: string): Status => statuses[id] ?? "todo";
  const visible = TASKS.filter((t) => phase === "all" || t.phase === phase);
  const done = TASKS.filter((t) => statusOf(t.id) === "done").length;
  const doing = TASKS.filter((t) => statusOf(t.id) === "doing").length;
  const todo = TASKS.length - done - doing;

  const setStatus = (id: string, s: Status) => {
    setStatuses((prev) => ({ ...prev, [id]: s }));
  };

  const current = PHASES.find((p) => p.id === phase);

  return (
    <Stack gap={20}>
      <Stack gap={8}>
        <Row gap={8} align="center" wrap>
          <H1>Fiche tâches</H1>
          <Pill size="sm" active>
            Suivi développement
          </Pill>
        </Row>
        <Text tone="secondary">
          75 tâches, 9 étapes. Clic sur le statut pour avancer : À faire → En
          cours → Fait. Les jalons exigent la validation de Rémi avant la
          suite. Source : plan du 22 août 2026.
        </Text>
      </Stack>

      <Grid columns={4} gap={12}>
        <Stat value={`${done}/${TASKS.length}`} label="Terminées" tone="success" />
        <Stat value={String(doing)} label="En cours" tone="info" />
        <Stat value={String(todo)} label="À faire" />
        <Stat value="9" label="Étapes" />
      </Grid>

      <UsageBar
        total={TASKS.length}
        topLeftLabel="Avancement global"
        topRightLabel={`${done} faites · ${doing} en cours · ${todo} restantes`}
        segments={[
          { id: "done", value: done, color: "green" },
          { id: "doing", value: doing, color: "blue" },
          { id: "todo", value: todo, color: "gray" },
        ]}
      />

      <Row gap={6} wrap>
        <span>
          <Pill active={phase === "all"} onClick={() => setPhase("all")}>
            Toutes
          </Pill>
        </span>
        {PHASES.map((p) => (
          <span key={p.id}>
            <Pill active={phase === p.id} onClick={() => setPhase(p.id)}>
              {p.label}
            </Pill>
          </span>
        ))}
      </Row>

      {current && (
        <Callout
          tone={current.jalon ? "warning" : "info"}
          title={
            current.jalon
              ? `${current.label} — jalon : s’arrêter et attendre Rémi`
              : current.label
          }
        >
          {current.goal}
        </Callout>
      )}

      {phase === "all" ? (
        PHASES.map((p) => (
          <div key={p.id}>
            <PhaseTable
              title={p.label}
              tasks={TASKS.filter((t) => t.phase === p.id)}
              statusOf={statusOf}
              setStatus={setStatus}
            />
          </div>
        ))
      ) : (
        <PhaseTable
          title={current ? current.label : "Étape"}
          tasks={visible}
          statusOf={statusOf}
          setStatus={setStatus}
        />
      )}

      <Row>
        <Button
          variant="ghost"
          onClick={() => setStatuses({})}
        >
          Réinitialiser les statuts
        </Button>
      </Row>
    </Stack>
  );
}

function PhaseTable({
  title,
  tasks,
  statusOf,
  setStatus,
}: {
  title: string;
  tasks: Task[];
  statusOf: (id: string) => Status;
  setStatus: (id: string, s: Status) => void;
}) {
  return (
    <Stack gap={8}>
      <H2>{title}</H2>
      <Table
        headers={["ID", "Tâche", "Résumé", "Sous-étapes", "Statut"]}
        striped
        stickyHeader
        rowTone={tasks.map((t) => {
          const s = statusOf(t.id);
          if (s === "done") return "success";
          if (s === "doing") return "info";
          return "neutral";
        })}
        rows={tasks.map((t) => {
          const s = statusOf(t.id);
          return [
            t.id,
            t.title,
            t.summary,
            t.steps,
            <span>
              <Pill
                size="sm"
                active={s !== "todo"}
                onClick={() => setStatus(t.id, nextStatus(s))}
              >
                {STATUS_LABEL[s]}
              </Pill>
            </span>,
          ];
        })}
      />
    </Stack>
  );
}
