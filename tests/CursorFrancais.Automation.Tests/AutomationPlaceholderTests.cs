using CursorFrancais.Automation;

namespace CursorFrancais.Automation.Tests;

public class AutomationPlaceholderTests
{
    public void Role_est_renseigne()
    {
        if (string.IsNullOrWhiteSpace(AutomationPlaceholder.Role))
        {
            throw new InvalidOperationException("Le rôle automation est vide.");
        }
    }
}
