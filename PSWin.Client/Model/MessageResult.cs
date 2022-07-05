namespace LinkMobility.PSWin.Client.Model
{
    public class MessageResult
    {
        public MessageResult(string gatewayReference, bool isStatusOk, string statusText, Sms message)
        {
            this.GatewayReference = gatewayReference;
            this.IsStatusOk = isStatusOk;
            this.StatusText = statusText;
            this.Message = message;
        }

        public string GatewayReference { get; }
        public bool IsStatusOk { get; }
        public string StatusText { get; }
        public Sms Message { get; }
    }
}
