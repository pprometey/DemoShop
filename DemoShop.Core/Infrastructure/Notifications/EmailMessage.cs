namespace DemoShop.Core.Infrastructure
{
    public class EmailMessage
    {
        public string ToName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
    }
}