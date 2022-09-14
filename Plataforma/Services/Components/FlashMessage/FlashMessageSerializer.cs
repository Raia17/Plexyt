using Plataforma.Services.Contracts.FlashMessage;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Plataforma.Services.Components.FlashMessage;

public class FlashMessageSerializer : IFlashMessageSerializer {

    public List<IFlashMessageModel> Deserialize(string data) {
        return JsonSerializer.Deserialize<List<FlashMessageModel>>(data)?.Cast<IFlashMessageModel>().ToList() ?? new List<IFlashMessageModel>();
    }

    public string Serialize(IList<IFlashMessageModel> messages) {
        return JsonSerializer.Serialize(messages);
    }
}