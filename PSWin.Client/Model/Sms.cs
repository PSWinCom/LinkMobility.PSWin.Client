using System;

namespace LinkMobility.PSWin.Client.Model
{
    /// <summary>
    /// An SMS message that is sent to the PSWin gateway.
    /// </summary>
    public class Sms
    {
        public Sms(string senderNumber, string receiverNumber, string text)
        {
            SenderNumber = senderNumber;
            ReceiverNumber = receiverNumber;
            Text = text;
        }

        /// <summary>
        /// The message sender. Can be one of:
        /// <list type="bullet">
        ///     <item><description>
        ///         Four- or five digit short code (digits only)
        ///     </description></item>
        ///     <item><description>
        ///         A valid MSISDN. No spaces or international call prefix.
        ///     </description></item>
        ///     <item><description>
        ///         A string up to 11 characters long, containing only spaces and the following characters: A-Z, a-z, 0-9 and <![CDATA[!”#%&’()*+-./><;]]>
        ///     </description></item>
        /// </list>
        /// </summary>
        public string SenderNumber { get; set; }

        /// <summary>
        /// The receiver of the message.
        /// Must be a valid MSISDN. No spaces or international call prefix.
        /// </summary>
        public string ReceiverNumber { get; set; }

        /// <summary>
        /// The message text.
        /// For plain text messages, the message length should not exceed 160 characters unless you would like to use concatenated SMS messages.
        /// Text messages exceeding 160 characters will be split up into a maximum of 16 SMS messages, each of 134 characters.
        /// Thus, the maximum length is 16*134=2144 characters. This is done automatically by the SMS Gateway.
        /// Text messages of more than 2144 characters will be truncated.
        /// Please note that only characters defined in the GSM-7 basic character set is allowed for messages of <see cref="Type"/> Text.
        public string Text { get; set; }

        /// <summary>
        /// Send a message as a Flash (a.k.a. Class 0) message.
        /// </summary>
        public bool IsFlashMessage { get; set; }

        /// <summary>
        /// Indicates a set of messages that can replace each other.
        /// This parameter can be used to specify that the message should replace a previous message with the same set-number in the Inbox of the handset.
        /// </summary>
        public Replace? Replace { get; set; }

        /// <summary>
        /// The PSWinCom Gateway supports billing using mobile phones in Norway.
        /// As opposed to traditional CPA/Premium SMS which can only be used to bill mobile content, CPA GAS can only be used to bill goods and services.
        /// </summary>
        public Payment Payment { get; set; }

        /// <summary>
        /// Include the gateway reference of the message in the response.
        /// The same reference is provided by the delivery report.
        /// </summary>
        public bool RequestReceipt { get; set; }

        /// <summary>
        /// Specifies the number of minutes this message will be valid.
        /// The time is counted from the moment the message has been received and stored on PSWinCom Gateway.
        /// After the time has elapsed, the message will not be sent to the operator.
        /// </summary>
        public TimeSpan? TimeToLive { get; set; }

        /// <summary>
        /// A date and time (CET) when the Gateway should try to deliver the message.
        /// If this parameter is present the message will be considered to be a deferred message that will be queued for future delivery instead of immediately being forwarded to operator.
        /// Maximum delay of message is currently one week (7 days).
        /// The Gateway account must be provisioned to use this feature.
        /// </summary>
        public DateTime? DeliveryTime { get; set; }

        /// <summary>
        /// The type of message content.
        /// </summary>
        public MessageType? Type { get; set; }
    }

    public enum MessageType
    {
        /// <summary>
        /// Default. Text message must consists of characters from the GSM-7 base character set.
        /// </summary>
        Text = 1,

        /// <summary>
        /// Text message must consist of characters from the Unicode character set.
        /// Each character counts as two against the character count of the message.
        /// </summary>
        Unicode = 9,
    }

    public enum Replace
    {
        Set1 = 1,
        Set2 = 2,
        Set3 = 3,
        Set4 = 4,
        Set5 = 5,
        Set6 = 6,
        Set7 = 7
    }
}

