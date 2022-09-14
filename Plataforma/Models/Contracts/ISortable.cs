namespace Plataforma.Models.Contracts;

public interface ISortable {
    public int Id { get; }
    public int Order { get; set; }
}