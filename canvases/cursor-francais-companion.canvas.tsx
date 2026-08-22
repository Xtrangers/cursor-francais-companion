import type { ReactNode } from "react";
import {
  BarChart,
  Button,
  Callout,
  Card,
  CardBody,
  CardHeader,
  Code,
  CollapsibleSection,
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
  Swatch,
  Table,
  Text,
  TodoListCard,
  UsageBar,
  useCanvasAction,
  useCanvasState,
  useHostTheme,
} from "cursor/canvas";

type TabId =
  | "decision"
  | "architecture"
  | "produit"
  | "features"
  | "plan"
  | "mockups"
  | "risques";

const TABS: { id: TabId; label: string }[] = [
  { id: "decision", label: "Décision" },
  { id: "architecture", label: "Architecture" },
  { id: "produit", label: "Produit" },
  { id: "features", label: "Fonctionnalités" },
  { id: "plan", label: "MVP et plan" },
  { id: "mockups", label: "Maquettes" },
  { id: "risques", label: "Risques" },
];

export default function CursorFrancaisCompanion() {
  const [tab, setTab] = useCanvasState<TabId>("tab", "decision");

  return (
    <Stack gap={20}>
      <Stack gap={8}>
        <Row gap={8} align="center" wrap>
          <H1>Cursor Français Companion</H1>
          <Pill size="sm">Projet communautaire</Pill>
        </Row>
        <Text tone="secondary">
          Application Windows 11 indépendante : overlay de traduction locale
          pour l’interface Cursor, sans plugin natif, sans injection et sans
          modification des fichiers internes.
        </Text>
      </Stack>

      <Row gap={6} wrap>
        {TABS.map((item) => (
          <span key={item.id}>
            <Pill
              active={tab === item.id}
              onClick={() => setTab(item.id)}
            >
              {item.label}
            </Pill>
          </span>
        ))}
      </Row>

      {tab === "decision" && <DecisionTab />}
      {tab === "architecture" && <ArchitectureTab />}
      {tab === "produit" && <ProduitTab />}
      {tab === "features" && <FeaturesTab />}
      {tab === "plan" && <PlanTab />}
      {tab === "mockups" && <MockupsTab />}
      {tab === "risques" && <RisquesTab />}
    </Stack>
  );
}

function DecisionTab() {
  return (
    <Stack gap={20}>
      <Callout tone="success" title="Décision retenue">
        Application compagnon Windows avec overlay ciblé Cursor. C’est la
        solution la plus réaliste : plus simple qu’une modification interne,
        moins fragile qu’une injection JavaScript, plus propre qu’un
        remplacement de fichiers, et développable par étapes.
      </Callout>

      <Grid columns={4} gap={12}>
        <Stat value="Overlay Win" label="Solution retenue" tone="success" />
        <Stat value="Faible" label="Plugin Cursor classique" tone="danger" />
        <Stat value="Moyenne" label="Pack langue VS Code" tone="warning" />
        <Stat value="Élevé" label="Risque UI Cursor" tone="warning" />
      </Grid>

      <Stack gap={8}>
        <H2>Pourquoi pas un plugin Cursor</H2>
        <Text>
          Les plugins officiels ajoutent des règles, agents, commandes,
          compétences, hooks et serveurs MCP. Ils ne sont pas conçus pour
          modifier l’interface graphique native de l’application Windows.
          Cursor repose en partie sur VS Code : un pack de langue peut
          traduire l’héritage VS Code, mais les zones Agent, Chat et Composer
          restent souvent en anglais.
        </Text>
        <Row gap={16} wrap>
          <Link href="https://cursor.com/docs/plugins">
            Capacités officielles des plugins
          </Link>
          <Link href="https://forum.cursor.com">
            Forum Cursor — packs de langue
          </Link>
        </Row>
      </Stack>

      <Stack gap={8}>
        <H2>Comparaison des approches</H2>
        <Text size="small" tone="tertiary">
          Score de faisabilité / propreté (0–10). Source : analyse produit
          interne, 22 août 2026.
        </Text>
        <BarChart
          horizontal
          height={280}
          categories={[
            "UI Automation + dictionnaire",
            "Overlay Windows ciblé",
            "OCR + fenêtre de traduction",
            "Pack de langue VS Code",
            "Injection JavaScript",
            "Modification fichiers internes",
            "Plugin Cursor classique",
            "Fork complet de Cursor",
          ]}
          series={[
            {
              name: "Faisabilité et contrôle",
              data: [10, 9, 7, 5, 3, 3, 2, 1],
            },
          ]}
          yMax={10}
          showValues
        />
      </Stack>

      <Table
        headers={["Solution", "Faisabilité", "Résultat"]}
        striped
        rowTone={[
          "danger",
          "warning",
          "warning",
          "warning",
          "success",
          "danger",
          "info",
          "success",
        ]}
        rows={[
          [
            "Plugin Cursor classique",
            "Faible",
            "Ne peut pas traduire toute l’interface native",
          ],
          [
            "Pack de langue VS Code",
            "Moyenne",
            "Traduit seulement une partie héritée de VS Code",
          ],
          [
            "Modification des fichiers internes",
            "Possible mais risquée",
            "Cassé après les mises à jour, risque de crash",
          ],
          [
            "Injection JavaScript",
            "Possible mais fragile",
            "Risque de sécurité, de détection et d’instabilité",
          ],
          [
            "Application Windows avec overlay",
            "Très bonne",
            "Solution la plus réaliste et contrôlable",
          ],
          [
            "Fork complet de Cursor",
            "Très difficile",
            "Coûteux, maintenance très lourde",
          ],
          [
            "OCR + fenêtre de traduction",
            "Bonne pour un MVP",
            "Simple à développer, mais moins propre",
          ],
          [
            "UI Automation + dictionnaire",
            "Meilleure solution finale",
            "Rapide, légère et plus précise",
          ],
        ]}
      />

      <Grid columns={2} gap={16}>
        <Stack gap={8}>
          <H3>À faire en premier</H3>
          <Text>Détecter uniquement Cursor.</Text>
          <Text>Afficher une petite fenêtre de contrôle.</Text>
          <Text>Bouton traduction activée / désactivée.</Text>
          <Text>Dictionnaire de 100 à 200 termes.</Text>
          <Text>Traduire via Windows UI Automation.</Text>
          <Text>Ajouter l’overlay transparent.</Text>
          <Text>Ne jamais traduire le code.</Text>
          <Text>Journal des textes inconnus.</Text>
          <Text>Tester 100 %, 125 % et 150 %.</Text>
          <Text>OCR uniquement après le prototype.</Text>
        </Stack>
        <Stack gap={8}>
          <H3>À ne pas faire</H3>
          <Text>Injection dans le processus Cursor.</Text>
          <Text>Modifier les fichiers .asar ou internes.</Text>
          <Text>Traduire tout l’écran dès le départ.</Text>
          <Text>Envoyer des captures vers une API sans accord.</Text>
          <Text>Promettre une traduction parfaite en v1.</Text>
          <Text>Utiliser le nom « Cursor Français officiel ».</Text>
          <Text>Intercepter le réseau de Cursor.</Text>
          <Text>Décompiler l’application.</Text>
          <Text>Traduire les réponses IA par défaut.</Text>
        </Stack>
      </Grid>
    </Stack>
  );
}

function ArchitectureTab() {
  const theme = useHostTheme();

  return (
    <Stack gap={20}>
      <Callout tone="info" title="Architecture hybride à trois niveaux">
        Dictionnaire local d’abord, UI Automation Windows ensuite, OCR local
        uniquement en secours. Aucune donnée utilisateur n’est envoyée par
        défaut.
      </Callout>

      <Grid columns={3} gap={12}>
        <Card>
          <CardHeader trailing={<Pill size="sm" active>Niveau 1</Pill>}>
            Dictionnaire
          </CardHeader>
          <CardBody>
            <Stack gap={8}>
              <Text>
                Traduction instantanée des libellés connus : Agent, Chat,
                Composer, Apply, Accept, Reject, Run, Settings, Models,
                Context, New chat, Ask, Debug, Plan, Privacy, Usage.
              </Text>
              <Text size="small" tone="secondary">
                Gratuit, hors ligne, vocabulaire cohérent, parfait pour les
                éléments répétitifs.
              </Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader trailing={<Pill size="sm" active>Niveau 2</Pill>}>
            UI Automation
          </CardHeader>
          <CardBody>
            <Stack gap={8}>
              <Text>
                Lecture des contrôles accessibles : boutons, menus, onglets,
                champs, titres, panneaux, éléments sélectionnés.
              </Text>
              <Text size="small" tone="secondary">
                Préférable à l’OCR dès que Cursor expose correctement ses
                informations d’accessibilité.
              </Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader trailing={<Pill size="sm">Niveau 3</Pill>}>
            OCR de secours
          </CardHeader>
          <CardBody>
            <Stack gap={8}>
              <Text>
                Capture ciblée de la fenêtre Cursor uniquement. Analyse des
                boutons personnalisés, panneaux Agent et libellés non exposés.
              </Text>
              <Text size="small" tone="secondary">
                Local, sans conservation d’image, limité en fréquence, coupé
                si la CPU monte trop.
              </Text>
            </Stack>
          </CardBody>
        </Card>
      </Grid>

      <Stack gap={8}>
        <H2>Flux utilisateur</H2>
        <Table
          headers={["Étape", "Action"]}
          rows={[
            ["1", "L’utilisateur démarre Cursor"],
            ["2", "Il démarre Cursor Français Companion"],
            ["3", "Le logiciel détecte automatiquement la fenêtre Cursor"],
            ["4", "Il vérifie que le chemin correspond bien à Cursor"],
            ["5", "L’utilisateur active Traduction française"],
            ["6", "L’application analyse l’interface visible"],
            ["7", "Les textes connus sont associés au dictionnaire local"],
            ["8", "Les traductions apparaissent dans une couche transparente"],
            ["9", "Les clics restent transmis à Cursor"],
            ["10", "La traduction peut être désactivée à tout moment"],
          ]}
        />
      </Stack>

      <Stack gap={8}>
        <H2>Couche visuelle</H2>
        <Text>
          L’application ne remplace pas l’interface de Cursor. Elle ajoute une
          couche transparente au-dessus. Les boutons, positions, clics et
          raccourcis d’origine restent ceux de Cursor.
        </Text>
        <div
          style={{
            border: `1px solid ${theme.stroke.secondary}`,
            background: theme.bg.elevated,
            padding: 16,
          }}
        >
          <Stack gap={10}>
            <Row justify="space-between">
              <Text weight="semibold">Cursor</Text>
              <Text size="small" tone="tertiary">
                couche de traduction indépendante
              </Text>
            </Row>
            <Grid columns={2} gap={10}>
              <div
                style={{
                  border: `1px solid ${theme.stroke.tertiary}`,
                  padding: 10,
                }}
              >
                <Stack gap={4}>
                  <Text size="small" tone="tertiary">
                    Barre latérale
                  </Text>
                  <Text>Fichiers</Text>
                  <Text>Paramètres</Text>
                  <Text>Extensions</Text>
                </Stack>
              </div>
              <div
                style={{
                  border: `1px solid ${theme.stroke.tertiary}`,
                  padding: 10,
                }}
              >
                <Stack gap={4}>
                  <Text size="small" tone="tertiary">
                    Agent / Conversation
                  </Text>
                  <Text>Discussion avec l’agent</Text>
                  <Text>Ajouter un fichier</Text>
                  <Text>Exécuter</Text>
                </Stack>
              </div>
            </Grid>
          </Stack>
        </div>
      </Stack>

      <Stack gap={8}>
        <H2>Pourquoi C# / WPF plutôt qu’Electron</H2>
        <Text>
          Cursor lui-même est une application desktop de la famille VS Code /
          Electron. Le compagnon, lui, doit parler nativement à Windows :
          menu modules, overlays, raccourcis globaux, UI Automation. C# /
          .NET 10 + WPF pour le shell, Win32 + Direct2D pour l’overlay.
          Plus tard : Skills, Projets, Agents dans le même menu.
        </Text>
        <Table
          headers={["Besoin", "Outil conseillé"]}
          striped
          rows={[
            ["Développement", "Cursor"],
            ["Assistance IA", "Grok 4.6 via Cursor"],
            ["Application Windows", "C# / .NET 8 LTS"],
            ["Interface native", "WPF (.NET 10) + menu modules"],
            ["Automation Windows", "UI Automation"],
            ["OCR", "Windows OCR"],
            ["Base locale", "SQLite ou fichier local"],
            ["Tests automatisés", "WinAppDriver ou UI Automation"],
            ["Packaging", "MSIX"],
            ["Versioning", "GitHub"],
            ["Distribution", "GitHub Releases ou Microsoft Store"],
            ["Erreurs", "Sentry optionnel, anonymisé"],
            ["Documentation", "Markdown"],
            ["Maquettes", "Figma ou Penpot"],
          ]}
        />
      </Stack>

      <Callout tone="warning" title="Règle d’usage de l’IA">
        Cursor et Grok 4.6 servent à concevoir, coder, tester et documenter.
        Ils ne doivent pas recevoir automatiquement les captures d’écran ou
        le contenu de l’utilisateur sans consentement clair.
      </Callout>
    </Stack>
  );
}

function ProduitTab() {
  return (
    <Stack gap={20}>
      <Grid columns={3} gap={12}>
        <Stat value="Local" label="Traduction par défaut" tone="success" />
        <Stat value="Cursor only" label="Périmètre d’action" tone="info" />
        <Stat value="Non affilié" label="Positionnement légal" />
      </Grid>

      <Stack gap={8}>
        <H2>Promesse</H2>
        <Text>
          Traduire l’interface Cursor en français sans modifier Cursor, sans
          abonnement obligatoire et sans envoyer les conversations vers un
          service externe.
        </Text>
      </Stack>

      <Stack gap={8}>
        <H2>Public cible</H2>
        <Table
          headers={["Audience", "Besoin principal"]}
          rows={[
            ["Développeurs français", "Interface Agent / Chat lisible"],
            ["Étudiants et débutants", "Vocabulaire Cursor moins opaque"],
            ["Formateurs et écoles", "Support pédagogique francophone"],
            ["Entreprises francophones", "Adoption interne plus simple"],
            ["Utilisateurs non anglophones", "Accès aux panneaux propriétaires"],
          ]}
        />
        <Text size="small" tone="tertiary">
          Besoin ciblé : les packs de langue VS Code ne couvrent pas
          nécessairement les zones propres à Cursor (forum.cursor.com).
        </Text>
      </Stack>

      <Stack gap={8}>
        <H2>Ce qui rend le produit propre à Cursor</H2>
        <Text>
          Ce n’est pas un traducteur d’écran généraliste. Il doit rester
          strictement limité à Cursor.
        </Text>
        <Table
          headers={["Mesure", "Règle"]}
          striped
          rows={[
            ["Processus", "Vérifier le nom du processus Cursor"],
            ["Installation", "Vérifier l’éditeur ou le chemin d’installation"],
            ["Périmètre", "Ne jamais traduire les autres logiciels"],
            ["Vocabulaire", "Dictionnaire conçu pour Cursor"],
            ["Zones", "Reconnaître Agent, Chat et Composer"],
            ["Versions", "Profil de compatibilité par version"],
            ["Identité", "S’inspirer de Cursor sans copier le logo"],
            ["Mention", "Projet communautaire non affilié à Cursor"],
          ]}
        />
      </Stack>

      <Stack gap={8}>
        <H2>Formulation à donner à l’agent</H2>
        <Text>
          Développer une application Windows 11 indépendante, spécialisée
          uniquement dans l’application Cursor AI Desktop. L’application doit
          détecter la fenêtre Cursor, récupérer les éléments d’interface
          accessibles avec Windows UI Automation, traduire les textes anglais
          vers le français à l’aide d’un dictionnaire local spécialisé, puis
          afficher les traductions dans une couche transparente non
          intrusive. Le code de l’utilisateur, les noms de fichiers, les
          commandes et les conversations doivent être protégés par défaut.
          L’application ne doit pas injecter de code dans Cursor, modifier
          ses fichiers internes ni intercepter son réseau. L’OCR local doit
          servir uniquement de solution de secours. L’application doit
          proposer un mode français, un mode bilingue, une activation
          globale, un raccourci clavier, une liste d’exclusion et une
          interface de configuration moderne.
        </Text>
        <AskToBuildButton />
      </Stack>
    </Stack>
  );
}

function AskToBuildButton() {
  const dispatch = useCanvasAction();
  return (
    <Row>
      <Button
        variant="primary"
        onClick={() =>
          dispatch({
            type: "newComposerChat",
            userPrompt:
              "Développer le MVP de Cursor Français Companion : application Windows 11 C# / WPF avec menu (Traducteur actif, Skills/Projets/Agents bientôt), dictionnaire local, UI Automation, overlay Win32, pas d’injection ni de modification des fichiers Cursor.",
          })
        }
      >
        Ouvrir un chat de développement
      </Button>
    </Row>
  );
}

function FeaturesTab() {
  return (
    <Stack gap={20}>
      <H2>Périmètre fonctionnel</H2>
      <CollapsibleSection
        title="Détection de Cursor"
        count={6}
        leading={<Swatch color="blue" />}
        defaultOpen
      >
        <Stack gap={4}>
          <Text>Détection automatique du processus et de la fenêtre principale.</Text>
          <Text>Vérification du chemin d’installation.</Text>
          <Text>Support de plusieurs fenêtres Cursor.</Text>
          <Text>Sélection manuelle si besoin.</Text>
          <Text>Fonctionnement uniquement avec Cursor.</Text>
        </Stack>
      </CollapsibleSection>
      <CollapsibleSection
        title="Traduction de l’interface"
        count={9}
        leading={<Swatch color="green" />}
      >
        <Stack gap={4}>
          <Text>Menus, boutons, panneaux, onglets, paramètres.</Text>
          <Text>Agent, Chat, Composer, notifications.</Text>
          <Text>Fenêtres de confirmation, écrans d’accueil, statuts.</Text>
          <Text>Éléments hérités de VS Code lorsque visibles.</Text>
        </Stack>
      </CollapsibleSection>
      <CollapsibleSection
        title="Dictionnaire français spécialisé"
        count={6}
        leading={<Swatch color="purple" />}
      >
        <Stack gap={4}>
          <Text>Vocabulaire informatique cohérent, variantes pro ou simples.</Text>
          <Text>Glossaire personnalisable.</Text>
          <Text>Conservation de certains termes techniques en anglais.</Text>
          <Text>Protection des noms de modèles, fichiers et commandes.</Text>
        </Stack>
      </CollapsibleSection>
      <CollapsibleSection
        title="Contrôle utilisateur"
        count={10}
        leading={<Swatch color="orange" />}
      >
        <Stack gap={4}>
          <Text>Activation globale, raccourci Ctrl + Alt + F.</Text>
          <Text>Opacité, taille, couleur, mode bilingue.</Text>
          <Text>Exclusion de zone, réinitialisation, démarrage auto.</Text>
          <Text>Icône dans la zone de notification.</Text>
        </Stack>
      </CollapsibleSection>
      <CollapsibleSection
        title="Confidentialité"
        count={6}
        leading={<Swatch color="yellow" />}
      >
        <Stack gap={4}>
          <Text>Dictionnaire local par défaut, OCR local autant que possible.</Text>
          <Text>Aucune capture permanente, exclusion des zones sensibles.</Text>
          <Text>Aucun enregistrement des conversations, mode hors ligne.</Text>
        </Stack>
      </CollapsibleSection>

      <Divider />

      <H2>Modes de traduction</H2>
      <Table
        headers={["Mode", "Comportement", "Par défaut"]}
        rowTone={["success", "info", "neutral", "neutral"]}
        rows={[
          [
            "Automatique",
            "Traduit les éléments détectés",
            "Oui pour les libellés UI",
          ],
          [
            "Intelligent",
            "Boutons, menus, titres, panneaux — jamais le code",
            "Recommandé",
          ],
          [
            "Manuel",
            "L’utilisateur sélectionne une zone à traduire",
            "Optionnel",
          ],
          [
            "Bilingue",
            "Agent / Agent ou Exécuter — Run",
            "Utile pour apprendre",
          ],
        ]}
      />

      <Callout tone="neutral" title="Mode intelligent — exclusions">
        Ne pas traduire le code, les noms de fichiers, les commandes, les
        messages saisis par l’utilisateur, ni les réponses de l’IA sauf
        activation spécifique.
      </Callout>

      <H2>Complexité de réalisation</H2>
      <Text size="small" tone="tertiary">
        Répartition indicative du périmètre. Source : dossier produit, 22
        août 2026.
      </Text>
      <UsageBar
        total={100}
        topLeftLabel="Effort relatif du produit"
        topRightLabel="Simple 35 · Tests 25 · Difficile 25 · À éviter 15"
        segments={[
          { id: "simple", value: 35, color: "green" },
          { id: "tests", value: 25, color: "blue" },
          { id: "hard", value: 25, color: "orange" },
          { id: "avoid", value: 15, color: "red" },
        ]}
      />
      <Grid columns={2} gap={16}>
        <Stack gap={8}>
          <H3>Simple</H3>
          <Text>Fenêtre de réglages, détection Cursor, marche/arrêt.</Text>
          <Text>Dictionnaire, icône système, mode bilingue.</Text>
          <Text>Mise à jour vocabulaire, lancement auto, raccourcis.</Text>
          <H3>Réalisable avec tests</H3>
          <Text>UI Automation, coordonnées, clics à travers l’overlay.</Text>
          <Text>Resize, thème sombre/clair, DPI 100/125/150.</Text>
        </Stack>
        <Stack gap={8}>
          <H3>Difficile</H3>
          <Text>UI dynamique, panneaux temps réel, décalage visuel.</Text>
          <Text>Zoom Cursor, multi-écrans, compatibilité après update.</Text>
          <H3>À éviter au départ</H3>
          <Text>Injection, .asar, interception réseau, décompilation.</Text>
          <Text>Traduction complète des réponses IA.</Text>
        </Stack>
      </Grid>
    </Stack>
  );
}

function PlanTab() {
  return (
    <Stack gap={20}>
      <H2>Couverture prévue</H2>
      <Text size="small" tone="tertiary">
        Objectif de traduction des textes fixes visibles. Source : plan MVP,
        22 août 2026.
      </Text>
      <BarChart
        categories={["MVP 1", "MVP 2", "Version avancée"]}
        series={[
          {
            name: "Textes fixes couverts (%)",
            data: [70, 88, 96],
            tone: "info",
          },
        ]}
        valueSuffix=" %"
        yMax={100}
        height={200}
        showValues
      />

      <Grid columns={3} gap={12}>
        <Card>
          <CardHeader trailing={<Pill size="sm" active>Maintenant</Pill>}>
            MVP 1
          </CardHeader>
          <CardBody>
            <Stack gap={6}>
              <Text>
                60 à 80 % des textes fixes : détection, dictionnaire, boutons
                connus, menus, barre latérale, mode sombre, bilingue,
                exclusion du code, journal des inconnus.
              </Text>
              <Text size="small" tone="secondary">
                Pas encore : conversations, dynamiques parfaits, fenêtres
                externes, garantie toutes versions.
              </Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader>MVP 2</CardHeader>
          <CardBody>
            <Text>
              OCR de secours, panneaux Agent, zones intelligentes, glossaire
              personnalisable, multi-fenêtres, mise à jour distante du
              dictionnaire, écran de correction.
            </Text>
          </CardBody>
        </Card>
        <Card>
          <CardHeader>Avancé</CardHeader>
          <CardBody>
            <Text>
              Traduction contextuelle, profils par version, français pro /
              simplifié, ES / DE, publication éventuelle en boutique.
            </Text>
          </CardBody>
        </Card>
      </Grid>

      <H2>Phases de développement</H2>
      <TodoListCard
        defaultExpanded
        todos={[
          {
            id: "p1",
            content:
              "Phase 1 — Validation : UI Automation, DPI, thème, multi-fenêtres, resize",
            status: "pending",
          },
          {
            id: "p2",
            content:
              "Phase 2 — Prototype visuel : fenêtre, tray, overlay, focus Cursor",
            status: "pending",
          },
          {
            id: "p3",
            content:
              "Phase 3 — Moteur local : dictionnaire, matching, protection du code",
            status: "pending",
          },
          {
            id: "p4",
            content:
              "Phase 4 — UI Automation : contrôles, coordonnées, suivi des mouvements",
            status: "pending",
          },
          {
            id: "p5",
            content:
              "Phase 5 — OCR de secours : fenêtre Cursor seule, pas d’image persistante",
            status: "pending",
          },
          {
            id: "p6",
            content:
              "Phase 6 — Stabilité : zoom, écrans, plein écran, RAM, focus, reprise",
            status: "pending",
          },
          {
            id: "p7",
            content:
              "Phase 7 — Distribution : portable, installeur, GitHub, disclaimer",
            status: "pending",
          },
        ]}
      />

      <Callout tone="info" title="Plugin plus tard, pas maintenant">
        Un plugin Cursor pourra éventuellement fournir le dictionnaire, les
        commandes de gestion, les paramètres et la documentation. L’application
        Windows restera nécessaire pour modifier visuellement l’interface.
      </Callout>
    </Stack>
  );
}

function MockupsTab() {
  const theme = useHostTheme();
  const [screen, setScreen] = useCanvasState<
    "main" | "settings" | "dict" | "overlay"
  >("mockup", "main");

  return (
    <Stack gap={16}>
      <Row gap={6} wrap>
        <Pill active={screen === "main"} onClick={() => setScreen("main")}>
          Écran principal
        </Pill>
        <Pill
          active={screen === "settings"}
          onClick={() => setScreen("settings")}
        >
          Paramètres
        </Pill>
        <Pill active={screen === "dict"} onClick={() => setScreen("dict")}>
          Dictionnaire
        </Pill>
        <Pill
          active={screen === "overlay"}
          onClick={() => setScreen("overlay")}
        >
          Overlay Cursor
        </Pill>
      </Row>

      {screen === "main" && (
        <Wireframe title="Cursor Français Companion">
          <Stack gap={12}>
            <Row gap={8} align="center">
              <StatusDot color={theme.accent.primary} />
              <Text weight="semibold">Cursor détecté</Text>
            </Row>
            <Text size="small" tone="secondary">
              Version détectée : compatible
            </Text>
            <Divider />
            <Text size="small" tone="tertiary">
              Traduction française
            </Text>
            <Row>
              <Pill active>ACTIVÉE</Pill>
            </Row>
            <H3>Mode</H3>
            <Stack gap={4}>
              <Text>Français uniquement</Text>
              <Text tone="secondary">Anglais + Français</Text>
              <Text tone="secondary">Français simplifié</Text>
            </Stack>
            <H3>Zones traduites</H3>
            <Stack gap={4}>
              <Text>Menus · Boutons · Paramètres · Agent et Chat</Text>
              <Text tone="secondary">
                Messages IA et notifications : désactivés
              </Text>
            </Stack>
          </Stack>
        </Wireframe>
      )}

      {screen === "settings" && (
        <Wireframe title="Paramètres">
          <Stack gap={12}>
            <H3>Apparence</H3>
            <Row justify="space-between">
              <Text>Opacité de la traduction</Text>
              <Text weight="semibold">85 %</Text>
            </Row>
            <Row justify="space-between">
              <Text>Taille du texte</Text>
              <Text weight="semibold">100 %</Text>
            </Row>
            <Text>Adapter automatiquement les positions</Text>
            <Text>Masquer les traductions quand Cursor n’est pas actif</Text>
            <Divider />
            <Row justify="space-between">
              <Text>Raccourci global</Text>
              <Code>Ctrl + Alt + F</Code>
            </Row>
            <Divider />
            <H3>Confidentialité</H3>
            <Text>OCR local uniquement</Text>
            <Text>Ne jamais enregistrer les captures</Text>
            <Text>Ne jamais traduire le code</Text>
            <Row gap={8}>
              <Button variant="primary">Enregistrer</Button>
              <Button variant="ghost">Réinitialiser</Button>
            </Row>
          </Stack>
        </Wireframe>
      )}

      {screen === "dict" && (
        <Stack gap={12}>
          <H2>Dictionnaire Cursor</H2>
          <Table
            headers={["Terme anglais", "Traduction française", "Note"]}
            striped
            rows={[
              ["Agent", "Agent", "Conservé"],
              ["Composer", "Composer", "Conservé"],
              ["Apply", "Appliquer", ""],
              ["Reject", "Refuser", ""],
              ["Run", "Exécuter", ""],
              ["Review", "Réviser", ""],
              ["Settings", "Paramètres", ""],
              ["Add context", "Ajouter un contexte", ""],
              ["New chat", "Nouvelle discussion", ""],
              ["Ask", "Demander", ""],
              ["Debug", "Déboguer", ""],
              ["Plan", "Planifier", ""],
              ["Privacy", "Confidentialité", ""],
              ["Usage", "Utilisation", ""],
              ["Extensions", "Extensions", "Conservé"],
              ["Plugins", "Extensions", "Selon glossaire"],
            ]}
          />
          <Row gap={8}>
            <Button variant="secondary">Ajouter un terme</Button>
            <Button variant="ghost">Importer</Button>
            <Button variant="ghost">Exporter</Button>
          </Row>
        </Stack>
      )}

      {screen === "overlay" && (
        <Wireframe title="Cursor — couche de traduction">
          <Stack gap={12}>
            <Grid columns={2} gap={16}>
              <Stack gap={6}>
                <Row justify="space-between">
                  <Text tone="tertiary">Agent</Text>
                  <Text weight="semibold">Agent</Text>
                </Row>
                <Row justify="space-between">
                  <Text tone="tertiary">Settings</Text>
                  <Text weight="semibold">Paramètres</Text>
                </Row>
                <Row justify="space-between">
                  <Text tone="tertiary">Add context</Text>
                  <Text weight="semibold">Ajouter un contexte</Text>
                </Row>
                <Row justify="space-between">
                  <Text tone="tertiary">Run</Text>
                  <Text weight="semibold">Exécuter</Text>
                </Row>
                <Row justify="space-between">
                  <Text tone="tertiary">Apply</Text>
                  <Text weight="semibold">Appliquer</Text>
                </Row>
              </Stack>
              <div
                style={{
                  border: `1px solid ${theme.stroke.secondary}`,
                  padding: 12,
                  alignSelf: "start",
                }}
              >
                <Stack gap={4}>
                  <Text weight="semibold">Traduction ON</Text>
                  <Text size="small" tone="secondary">
                    14 éléments
                  </Text>
                </Stack>
              </div>
            </Grid>
          </Stack>
        </Wireframe>
      )}
    </Stack>
  );
}

function Wireframe({
  title,
  children,
}: {
  title: string;
  children: ReactNode;
}) {
  const theme = useHostTheme();
  return (
    <div
      style={{
        border: `1px solid ${theme.stroke.primary}`,
        background: theme.bg.elevated,
      }}
    >
      <div
        style={{
          borderBottom: `1px solid ${theme.stroke.tertiary}`,
          padding: "10px 14px",
          background: theme.fill.tertiary,
        }}
      >
        <Row justify="space-between" align="center">
          <Text weight="semibold">{title}</Text>
          <Text size="small" tone="quaternary">
            — □ ×
          </Text>
        </Row>
      </div>
      <div style={{ padding: 16 }}>{children}</div>
    </div>
  );
}

function StatusDot({ color }: { color: string }) {
  return (
    <span
      style={{
        width: 8,
        height: 8,
        borderRadius: 9999,
        background: color,
        display: "inline-block",
      }}
    />
  );
}

function RisquesTab() {
  return (
    <Stack gap={20}>
      <H2>Risques principaux</H2>
      <Table
        headers={["Risque", "Niveau", "Réponse"]}
        striped
        stickyHeader
        rowTone={[
          "danger",
          "danger",
          "danger",
          "danger",
          "warning",
          "warning",
          "warning",
          "warning",
          "danger",
          "warning",
        ]}
        rows={[
          [
            "Cursor change son interface",
            "Élevé",
            "Profils de compatibilité par version",
          ],
          [
            "Overlay mal positionné",
            "Élevé",
            "Recalcul régulier et réglage manuel",
          ],
          ["Textes impossibles à lire", "Élevé", "OCR de secours"],
          [
            "Faux positifs sur le code",
            "Élevé",
            "Zones protégées et exclusions",
          ],
          [
            "Consommation excessive",
            "Moyen",
            "Analyse limitée et mise en cache",
          ],
          [
            "Blocage par antivirus",
            "Moyen",
            "Éviter l’injection et signer l’application",
          ],
          [
            "Conditions d’utilisation Cursor",
            "Moyen / élevé",
            "Ne pas modifier les fichiers ni injecter de code",
          ],
          [
            "Traductions incorrectes",
            "Moyen",
            "Dictionnaire validé et corrections utilisateur",
          ],
          [
            "Données sensibles dans OCR",
            "Élevé",
            "Traitement local, aucune conservation",
          ],
          [
            "Cursor ajoute officiellement le français",
            "Moyen",
            "Transformer le produit en assistant multilingue",
          ],
        ]}
      />

      <H2>Modèle économique possible</H2>
      <Grid columns={3} gap={12}>
        <Card>
          <CardHeader>Gratuit</CardHeader>
          <CardBody>
            <Stack gap={6}>
              <Text>Menus principaux, dictionnaire local.</Text>
              <Text>Mode français et bilingue.</Text>
              <Text>Un seul ordinateur.</Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader>Pro</CardHeader>
          <CardBody>
            <Stack gap={6}>
              <Text>OCR avancé, corrections automatiques.</Text>
              <Text>Mises à jour rapides, profils par version.</Text>
              <Text>Plusieurs langues et multi-écrans.</Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader>Licence équipe</CardHeader>
          <CardBody>
            <Text>
              Écoles, centres de formation, entreprises, DSI, organismes
              publics.
            </Text>
          </CardBody>
        </Card>
      </Grid>

      <Callout tone="warning" title="Risque commercial">
        Le marché est limité à la communauté Cursor francophone. Le MVP doit
        rester léger, gratuit ou peu cher, et mesurer d’abord le nombre
        d’utilisateurs, les versions Cursor, les zones demandées, les termes
        inconnus et la stabilité — avant d’investir dans un produit coûteux.
      </Callout>

      <H2>Sources</H2>
      <Text>
        Capacités actuelles des plugins Cursor : règles, compétences, agents,
        commandes, hooks et MCP — pas la modification de l’UI native.{" "}
        <Link href="https://cursor.com/docs/plugins">cursor.com/docs/plugins</Link>
      </Text>
      <Text>
        Packs de langue : surtout l’interface héritée de VS Code ; plusieurs
        composants Cursor restent en anglais.{" "}
        <Link href="https://forum.cursor.com">forum.cursor.com</Link>
      </Text>
    </Stack>
  );
}
