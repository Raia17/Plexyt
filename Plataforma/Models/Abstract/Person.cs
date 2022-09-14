namespace Plataforma.Models.Abstract
{
    public abstract class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Nif { get; set; }
        public string Email { get; set; }
        public int Telephone { get; set; }
        public int Mobilephone { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}
