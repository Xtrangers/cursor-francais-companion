using CursorFrancais.Core;

namespace CursorFrancais.Core.Tests;

public class CoreInfoTests
{
    public void NomProduit_est_renseigne()
    {
        if (string.IsNullOrWhiteSpace(CoreInfo.NomProduit))
        {
            throw new InvalidOperationException("Le nom du produit est vide.");
        }
    }
}
