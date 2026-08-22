# Règles du projet Cursor Français Companion

> **Ce fichier fait loi.** Tout agent travaillant sur ce projet — principal ou secondaire, Claude Code ou Cursor — lit ce fichier en entier avant sa première action, s'y conforme à la lettre, et ne l'oublie jamais.
>
> **Souveraineté** : seul le propriétaire du projet (Rémi) modifie ce fichier. Désobéir à une règle exige son ordre explicite, et doit être annoncé en citant la règle écartée.

Les 42 règles ci-dessous sont les premières règles du projet. Elles ne s'oublient jamais.

Sur le mot `rulesList`, afficher la liste complète (règle 1).

---

## Commande

**1.** Sur le mot `rulesList`, afficher la liste complète.

## Avant de commencer

**2.** Annoncer mon plan : ce que je fais, quels fichiers je touche.

**3.** Lire un fichier avant de le modifier — jamais à l'aveugle.

**4.** Avant la première modification : `git status`, version, état réel de la base. Ne jamais se fier au résumé précédent.

**5.** Les fiches sont la référence mais coûtent cher : ne les récupérer que pour republier ou trancher un périmètre.

**6.** Consigner chaque version dans `docs/JOURNAL.md`. Reporter dans les fiches dès que : 5 lignes, jalon, mise en ligne, ou votre demande.

**7.** S'arrêter aux jalons et attendre votre validation.

## Quand quelque chose ne va pas

**8.** Signaler mes erreurs sans attendre, même anciennes, même invisibles pour vous.

**9.** Pas de correctif à l'aveugle : cause réelle, explication, puis correction.

**10.** Diagnostiquer une capture d'écran avant de corriger : reproduire, identifier, expliquer.

**11.** Noter chaque correction de votre part dans `docs/MEMOIRE.md`, pour ne jamais la refaire.

## La vérification du travail

**12.** Regarder tout changement visible avant de le déclarer fini.

**13.** Aucun test désactivé, ignoré ni assoupli pour faire passer une livraison.

**14.** Chaque bogue corrigé reçoit son test — qui échoue d'abord.

**15.** Vérifier avant d'affirmer : une commande, jamais une supposition.

**16.** Vérification visuelle par le serveur intégré, jamais par un serveur en tâche de fond shell.

**17.** Playwright avant chaque jalon ou mise en ligne — pas à chaque commit. (Dormant tant qu'il n'existe pas de surface web ; dès qu'une UI web existe, cette règle s'applique.)

**18.** Accessibilité à zéro violation : tout élément interactif porte son libellé.

**19.** En fin de tâche, une passe groupée : typecheck, lint, test, build. Aucun avertissement lint ajouté.

## L'argent et les documents légaux

**20.** Tout calcul d'argent est testé, au centime près.

**21.** Ne jamais toucher un document émis — sauf la réinitialisation prévue.

## Secrets et sécurité

**22.** Ne jamais utiliser un jeton collé en discussion : le signaler, demander sa révocation.

**23.** Aucun secret dans le code ni dans les commits.

## Qualité du code

**24.** Chercher l'existant avant d'écrire. Recréer un outil qui existe est une faute.

**25.** Tout écran nouveau gère ses trois états : chargement, vide, erreur avec réessaie.

**26.** Tout module de logique pure reçoit son fichier de tests co-localisé.

**27.** Écrire les fichiers avec les outils dédiés — jamais de texte accentué via le shell.

## Les messages à l'écran

**28.** Un message d'erreur dit ce qui a échoué et quelle est la prochaine action.

**29.** Pas d'anglais visible, sauf usage courant français.

## Rythme et livraisons

**30.** Commiter librement une tâche finie. Ne jamais pousser sans votre demande.

**31.** Avant une tâche versionnée, lire `git log` et réserver son numéro de version.

**32.** Ne jamais réécrire l'historique git : pas de force push, pas d'amend sur un commit poussé.

## Travail multi-agents

**33. Paralléliser ce qui est séparable.**
Avant d'engager une tâche, dire si elle se découpe en parts indépendantes — des morceaux qui ne se lisent ni ne se modifient l'un l'autre, et dont chacun peut être jugé bon ou mauvais isolément.
Si oui, et s'il y a au moins trois parts : un agent par part, plus un agent superviseur qui vérifie la cohérence de l'ensemble, corrige les écarts et contrôle la direction. Le superviseur ne se justifie que là — il lui faut plusieurs travaux à confronter.
Si non — enquête, diagnostic, correction ciblée, tout travail où chaque étape dépend de la précédente — le faire seul, et le dire. Découper une chaîne de raisonnement produit des agents qui redécouvrent le même contexte et un superviseur qui recolle des morceaux dont aucun n'a vu l'ensemble.
Annoncer le choix avant de commencer, en une phrase : « trois parts indépendantes, je lance trois agents » ou « une seule chaîne, je le fais seul ». Rémi peut toujours en décider autrement.

**34.** Tout agent lit `RULES.md` et `AGENT.md` en entier avant sa première action.

**35.** Isoler les agents : une branche chacun, jamais master, périmètres disjoints.

**36.** Avant d'intégrer : relire le diff, vérifier version et horodatages, relancer la suite complète.

**37.** Un agent secondaire ne modifie jamais `RULES.md`, `AGENT.md` ni `docs/MEMOIRE.md`.

## Budget 0 €

**38.** Aucune dépendance payante sans votre accord.

**39.** Respecter les paliers gratuits : pas de requêtes multipliées, gros modules à la demande.

## Souveraineté des règles

**40.** Seul vous (Rémi) modifiez ce fichier. Désobéir exige votre ordre explicite, et s'annonce en citant la règle écartée.

**41.** Ces règles ne s'oublient jamais. Relire après tout résumé, et avant toute tâche touchant l'argent, la base ou les documents émis.

**42.** Ne pas relire un fichier déjà lu, ni relancer une vérification encore valable. Économiser le contexte est une règle.
