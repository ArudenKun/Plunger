using Hydro;
using JetBrains.Annotations;

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
        // ToastsList.Add(
        //     new Toast(
        //         Id: CreateGuid(),
        //         Header: data.Header,
        //         Message: data.Message,
        //         Type: data.Type,
        //         Placement: data.Placement,
        //         Duration: data.Duration == default ? DefaultToastDuration : data.Duration
        //     )
        // );
        Client.ExecuteJs(
            $$"""
            iziModal.
            iziToast.info({
                title: '{{data.Header}}',
                message: '{{data.Message}}'
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
