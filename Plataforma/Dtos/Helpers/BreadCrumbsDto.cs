using System.Collections.Generic;

namespace Plataforma.Dtos.Helpers;

public class BreadCrumbsDto {
    public List<BreadCrumbDto> BreadCrumbs { get; set; }
    public BreadCrumbsDto(params BreadCrumbDto[] breadCrumbs) {
        BreadCrumbs = new();
        foreach (var breadCrumbDto in breadCrumbs) {
            BreadCrumbs.Add(breadCrumbDto);
        }
    }
    //public BreadCrumbsDto(params Tuple<string, string>[] items) {
    //    BreadCrumbs = new();
    //    foreach (var item in items) {
    //        BreadCrumbs.Add(new BreadCrumbDto(item.Item1, item.Item2));
    //    }
    //}
}
public class BreadCrumbDto {
    public string Name { get; set; }
    public string Url { get; set; }
    public BreadCrumbDto(string name, string url) {
        Url = url;
        Name = name;
    }
}