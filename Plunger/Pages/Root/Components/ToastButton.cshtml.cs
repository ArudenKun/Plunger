using Hydro;
using Plunger.Pages.Shared.Toasts;

namespace Plunger.Pages.Root.Components;

public class ToastButton : HydroComponent
{
    public void ShowToast()
    {
        var rand = Random.Shared.Next(0, 2);
        DispatchGlobal(new ShowToast("Test Header", "This is a toast message", (ToastType)rand));
    }
}
