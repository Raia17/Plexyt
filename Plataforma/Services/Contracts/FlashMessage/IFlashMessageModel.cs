namespace Plataforma.Services.Contracts.FlashMessage;

public interface IFlashMessageModel {
    public bool IsHtml { get; set; }

    public string Title { get; set; }

    public string SubTitle { get; set; }

    public string Message { get; set; }

    public string ButtonText { get; set; }

    public FlashMessageType Type { get; set; }
}