using Hydro;
using Plunger.Pages.Shared.Toasts;

namespace Plunger.Pages.Root.Components;

public class ToastButton : HydroComponent
{
    public void ShowToast()
    {
        var type = Random.Shared.Next(0, 2);
        var placement = Random.Shared.Next(0, 6);
        var duration = Random.Shared.Next(1, 10);
        DispatchGlobal(
            new ShowToast(
                "Test Header",
                "This is a toast message",
                (ToastType)type,
                (ToastPlacement)placement,
                TimeSpan.FromSeconds(duration)
            )
        );
    }
}
