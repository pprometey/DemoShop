namespace DemoShop.Core.Infrastructure
{
    public class SMTPSenderOptions
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ReplyTo { get; set; }
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public int SecureSocketOptions { get; set; }
        public string SMTPLogin { get; set; }
        public string SMTPPassword { get; set; }
    }
}