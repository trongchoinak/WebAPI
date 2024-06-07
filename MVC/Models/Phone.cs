namespace MVC.Models
{
    public class Phone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Manufacturer { get; set; }
        public string? ImageBase64 { get; set; }
    }
}
