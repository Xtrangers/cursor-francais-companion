# Matrice QA manuelle (P7-01)

Cocher chez Rémi avant une release. Overlay **jamais bloquant** : un clic doit atteindre Cursor.

| # | Scénario | Attendu | Fait |
|---|---|---|---|
| 1 | Cursor fermé au lancement du compagnon | Statut « non détecté », overlay off | ☐ |
| 2 | Ouvrir Cursor ensuite | Détection &lt; 2 s, overlay si activé | ☐ |
| 3 | Fermer Cursor | Overlay détruit, pas d’exception | ☐ |
| 4 | Resize / maximize / snap Windows | Labels recollés, pas de fantômes | ☐ |
| 5 | DPI 100 / 125 / 150 | Texte net, décalage ≤ 2 px | ☐ |
| 6 | Thème Cursor sombre et clair | Contraste labels OK | ☐ |
| 7 | Deux fenêtres Cursor | Deux overlays indépendants | ☐ |
| 8 | Écran secondaire | Overlay sur le bon moniteur | ☐ |
| 9 | Plein écran Zen | Pas de labels orphelins | ☐ |
| 10 | Menu déroulant / palette | Soit traduit, soit ignoré, jamais bloqué | ☐ |
| 11 | Clic sur un bouton sous un label | Le bouton Cursor s’active | ☐ |
| 12 | Raccourcis Cursor (Ctrl+K, etc.) | Inchangés | ☐ |
| 13 | Ctrl+Alt+F | Toggle immédiat | ☐ |
| 14 | Focus autre application | Overlay masqué si l’option est on | ☐ |
| 15 | Sleep / reprise | Reprise propre | ☐ |
| 16 | Mise à jour Cursor | Pas de crash | ☐ |
| 17 | Menu Skills / Projets / Agents | Écran Bientôt, pas de crash | ☐ |
| 18 | Retour Traducteur | État du toggle conservé | ☐ |

Cursor testé pour le dump UIA : **3.16.17**.
