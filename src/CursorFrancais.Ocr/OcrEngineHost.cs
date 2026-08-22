using Windows.Globalization;
using Windows.Media.Ocr;

namespace CursorFrancais.Ocr;

public static class OcrEngineHost
{
    public static async Task<IReadOnlyList<OcrHit>> LireAsync(CursorFrame cadre, CancellationToken annulation)
    {
        var moteur = OcrEngine.TryCreateFromLanguage(new Language("en"))
                     ?? OcrEngine.TryCreateFromUserProfileLanguages();
        if (moteur is null)
        {
            return [];
        }

        annulation.ThrowIfCancellationRequested();
        var resultat = await moteur.RecognizeAsync(cadre.Bitmap);
        var hits = new List<OcrHit>();
        foreach (var ligne in resultat.Lines)
        {
            var texte = ligne.Text?.Trim() ?? string.Empty;
            if (RoiFilter.EstInterdit(texte, estEditeur: false, classe: null))
            {
                continue;
            }

            var box = ligne.Words.Count == 0
                ? (0, 0, 0, 0)
                : (
                    (int)ligne.Words.Min(m => m.BoundingRect.X),
                    (int)ligne.Words.Min(m => m.BoundingRect.Y),
                    (int)(ligne.Words.Max(m => m.BoundingRect.X + m.BoundingRect.Width)
                          - ligne.Words.Min(m => m.BoundingRect.X)),
                    (int)(ligne.Words.Max(m => m.BoundingRect.Y + m.BoundingRect.Height)
                          - ligne.Words.Min(m => m.BoundingRect.Y)));
            hits.Add(new OcrHit(texte, box.Item1, box.Item2, box.Item3, box.Item4));
        }

        return hits;
    }
}
