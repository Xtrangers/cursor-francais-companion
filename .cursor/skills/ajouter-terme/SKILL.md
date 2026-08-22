# Ajouter un terme EN → FR

Utilise cette skill pour enrichir le dictionnaire local du compagnon. Aucune API cloud.

## Étapes

1. Lire `src/CursorFrancais.Core/Data/seed-fr.json`.
2. Ajouter un objet `{ "en": "...", "fr": "...", "category": "Menus|Buttons|Agent|Settings|System" }`.
3. Si le mot doit rester en anglais (Agent, Composer, Cursor, Skill, MCP), mettre `"keepEnglish": true`.
4. Ajouter un cas `[InlineData("EN", "FR")]` dans `tests/CursorFrancais.Core.Tests/DictionaryEngineTests.cs` — le test doit échouer si le terme manque.
5. Lancer `dotnet test CursorFrancais.slnx`.
6. Ne jamais envoyer le terme vers un service externe.

## Exemple

```json
{ "en": "Hide Sidebar", "fr": "Masquer la barre latérale", "category": "Buttons" }
```
