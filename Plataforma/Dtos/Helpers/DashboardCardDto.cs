namespace Plataforma.Dtos.Helpers
{
    public class DashboardCardDto
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Count { get; set; }
        public string Controller { get; set; } = "Home";
        public string Action { get; set; } = "Create";
    }
}
