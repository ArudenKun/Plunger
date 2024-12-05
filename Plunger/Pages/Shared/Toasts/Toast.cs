using AutoInterfaceAttributes;

namespace Plunger.Pages.Shared.Toasts;

[AutoInterface(Name = "IToastParameters")]
public record ShowToast(
    string Title,
    string Message,
    ToastType Type = ToastType.Success,
    ToastPlacement Placement = ToastPlacement.TopRight,
    TimeSpan Duration = default
) : IToastParameters;

public record Toast(
    string Id,
    string Title,
    string Message,
    ToastType Type,
    ToastPlacement Placement,
    TimeSpan Duration
) : ShowToast(Title, Message, Type, Placement, Duration);
