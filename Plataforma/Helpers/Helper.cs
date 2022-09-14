using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;

namespace Plataforma.Helpers;

public static class Helper {
    public static ViewDataDictionary GetNewViewDataDictionary(IDictionary<string, object> viewData) {
        var viewDataDictionary = new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        foreach (var (key, value) in viewData) {
            viewDataDictionary[key] = value;
        }
        return viewDataDictionary;
    }
}