using System;

namespace Plataforma.Models.Contracts;

public interface IClosable {
    public int Id { get; }
    public bool Closed { get; set; }
    public DateTime? ClosedTime { get; set; }
    public string ClosedUser { get; set; }
    public int? ClosedUserId { get; set; }
}
