using CursorFrancais.Automation;
using FluentAssertions;
using Xunit;

namespace CursorFrancais.Automation.Tests;

public class AutomationPlaceholderTests
{
    [Fact]
    public void Role_est_renseigne()
    {
        AutomationPlaceholder.Role.Should().NotBeNullOrWhiteSpace();
    }
}
