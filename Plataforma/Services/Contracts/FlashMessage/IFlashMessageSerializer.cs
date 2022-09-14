using System.Collections.Generic;

namespace Plataforma.Services.Contracts.FlashMessage;

public interface IFlashMessageSerializer {
    List<IFlashMessageModel> Deserialize(string data);
    string Serialize(IList<IFlashMessageModel> messages);
}