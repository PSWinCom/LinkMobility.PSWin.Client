namespace LinkMobility.PSWin.Client.Model
{
    /// <summary>
    /// The response from sending a message to the PSWin gateway.
    /// </summary>
    public class MessageResult
    {
        /// <summary>
        /// Initialize all the fields of the response.
        /// </summary>
        public MessageResult(string gatewayReference, bool isStatusOk, string statusText, Sms message)
        {
            this.GatewayReference = gatewayReference;
            this.IsStatusOk = isStatusOk;
            this.StatusText = statusText;
            this.Message = message;
        }

        /// <summary>
        /// The unique ID assigned to the message by the gateway,
        /// if references are enabled on the account.
        /// Otherwise null.
        /// The same reference is included in delivery reports.
        /// </summary>
        public string GatewayReference { get; }

        /// <summary>
        /// The message passed initial checks and was sent further down the pipeline.
        /// Does not indicate whether the message actually was or could be delivered.
        /// This information is available through delivery reports.
        /// </summary>
        public bool IsStatusOk { get; }

        /// <summary>
        /// When <see cref="IsStatusOk"/> is false,
        /// this contains a short description of the problem.
        /// </summary>
        public string StatusText { get; }

        /// <summary>
        /// The message that this result belongs to.
        /// </summary>
        public Sms Message { get; }
    }
}
