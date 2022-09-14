using Plataforma.Services.Contracts.FlashMessage;

namespace Plataforma.Services.Components.FlashMessage;

public class FlashMessageModel : IFlashMessageModel {
    public bool IsHtml { get; set; }

    public string Title { get; set; }

    public string SubTitle { get; set; }

    public string Message { get; set; }

    public string ButtonText { get; set; }

    public FlashMessageType Type { get; set; }

    public FlashMessageModel() {
        IsHtml = false;
        Type = FlashMessageType.Success;
    }
}