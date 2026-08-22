# Budget performance (P7-02)

Cibles MVP 1, hors OCR :

| Mesure | Seuil |
|---|---|
| CPU idle (Cursor ouvert, overlay on, UI stable) | &lt; 1 % |
| RAM processus hors OCR | &lt; 80 Mo |
| Lecture UIA | timeout 40 ms, pas de freeze WPF |
| OCR | max 2 / s, coupure si CPU overlay &gt; 30 % |

Le dirty-check (`FrameDiffer`) évite un redraw si Name+Rect n’ont pas changé. Timer overlay : 200 ms.

## Comment mesurer

```powershell
Get-Process CursorFrancais.App | Select-Object CPU, WorkingSet64, Id
```

Ou `dotnet-counters monitor --process-id <pid>`.

Les chiffres idle réels se relèvent après le premier lancement (disclaimer accepté) avec Cursor ouvert. Ne pas affirmer un pourcentage sans cette mesure.
