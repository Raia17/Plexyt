namespace Plataforma.Models.Contracts;

public interface IActivable {
    public int Id { get; }
    public bool Active { get; set; }
}