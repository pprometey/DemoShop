using Newtonsoft.Json;

namespace DemoShop.Core.Infrastructure
{
    public class AlertMessage
    {
        public AlertStatus Status { get; set; }
        public string Message { get; set; }

        public AlertMessage(AlertStatus status = 0, string message = "")
        {
            this.Status = status;
            this.Message = message;
        }

        public override string ToString()
        {
            return Message;
        }

        public string SerializeObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AlertMessage DeserializeObject(string seralizeObject)
        {
            if (!string.IsNullOrEmpty(seralizeObject))
            {
                return JsonConvert.DeserializeObject<AlertMessage>(seralizeObject);
            }
            return null;
        }
    }
}