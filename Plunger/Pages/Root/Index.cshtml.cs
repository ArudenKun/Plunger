using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Plunger.Pages.Root;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void ShowToast()
    {
        var rand = Random.Shared.Next(0, 5);
        // DispatchGlobal(new ShowToast("Testing", Placement: (ToastPlacement)rand));
    }
}
