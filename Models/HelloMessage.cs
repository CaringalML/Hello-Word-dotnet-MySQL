namespace HelloWorldApi.Models
{
    public class HelloMessage
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Message { get; set; } = "Hello World";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}