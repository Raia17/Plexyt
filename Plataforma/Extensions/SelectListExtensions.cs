using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Plataforma.Extensions;

public static class SelectListExtensions {
    public static SelectList SetPlaceholder(this SelectList selectList, string text = "", string value = "") {
        if (selectList.FirstOrDefault()?.Value == "") return selectList;

        var list = selectList.ToList();
        list.Insert(0, new SelectListItem {
            Text = text,
            Value = value
        });
        return new SelectList(list, "Value", "Text", selectList.SelectedValue);
    }
}