using System;

namespace Plataforma.Dtos.Abstract
{
    public class PersonDto
    {
        public string Name { get; set; }
        public int? Nif { get; set; }
        public DateTime Birthday { get; set; } = DateTime.Now;
        public string Email { get; set; }
        public int? Telephone { get; set; }
        public int? Mobilephone { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}
