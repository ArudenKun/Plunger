using Humanizer;
using Hydro;
using JetBrains.Annotations;
using Plunger.Core.Case;

namespace Plunger.Pages.Shared.Toasts;

[PublicAPI]
public class ToastHost : HydroComponent
{
    public static readonly TimeSpan DefaultToastDuration = TimeSpan.FromSeconds(5);

    public List<Toast> ToastsList { get; set; } = [];

    public ToastHost()
    {
        Subscribe<ShowToast>(Handle);
        Subscribe<UnhandledHydroError>(Handle);
    }

    private void Handle(ShowToast data)
    {
        Client.ExecuteJs(
            $$"""
            iziToast.{{data.Type.Humanize().ToLower()}}({
                title: '{{data.Header}}',
                message: '{{data.Message}}',
                position: '{{data.Placement.Humanize().ToCamelCase()}}',
                timeout: '{{data.Duration.TotalMilliseconds}}'
            });
            """
        );
    }

    private void Handle(UnhandledHydroError data) =>
        ToastsList.Add(
            new Toast(
                Id: CreateGuid(),
                Header: data.Data as string ?? "Error",
                Message: data.Message
                    ?? "An unhandled hydro error occurred, please try again later.",
                Type: ToastType.Error,
                Placement: ToastPlacement.TopRight,
                Duration: DefaultToastDuration
            )
        );

    public void Close(string id) => ToastsList.RemoveAll(t => t.Id == id);

    private static string CreateGuid() => Guid.NewGuid().ToString("N");
}
