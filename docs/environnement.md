# Environnement vérifié — phase 0

Date : 2026-08-22. Commandes réelles, pas une supposition.

| Élément | Valeur |
|---|---|
| OS | Windows 11 25H2, build 26200, x64 |
| SDK utilisé | .NET **10.0.302** (stable), pin `global.json` + `rollForward: latestPatch` |
| SDK présent aussi | 10.0.400-preview — ne pas l'utiliser |
| Runtime WPF | Microsoft.WindowsDesktop.App 10.0.10 |
| Template WPF | `dotnet new wpf` : build OK, 0 erreur |
| Windows SDK | 10.0.26100.0 |
| Visual Studio | 18 Insiders (designer optionnel) |
| Cursor | 3.16.17, `Cursor.exe` sous `%LocalAppData%\Programs\cursor` |

Probe WPF : `dotnet new wpf` dans un dossier temp, `dotnet build` réussi.
