using System;

namespace LinkMobility.PSWin.Client.Model
{
    /**
     * <summary>
     * Representation of an SMS message that can be sent to the LinkMobility Gateway XML API.
     * </summary>
     * <see cref=""/>
     * <seealso cref="LinkMobility.GatewayClient.Message"/>
     */
    public class Sms
    {
        public Sms(string senderNumber, string receiverNumber, string text)
        {
            SenderNumber = senderNumber;
            ReceiverNumber = receiverNumber;
            Text = text;
        }

        /** 
         * <summary>
         * <para>This is the sender of the message.</para>
         * </summary>
         * <value>
         * <list type="bullet">
         * <item>
         * <description>4 or 5 digit short code (digits only)</description>
         * </item>
         * <item>
         * <description>a valid msIsdn including countrycode without leading zeros or + sign</description>
         * </item>
         * <item>
         * <description>an up to 11 characters long alpha numeric string only containing international characters and space</description>
         * </item>
         * </list>
         * </value>
        */
        public string SenderNumber { get; set; }

        /// <summary>
        /// The receiver of the message. Must be a valid msIsdn including countrycode without leading zeros or + sign.
        /// </summary>
        public string ReceiverNumber { get; set; }

        /// <summary>
        /// Contents of the message. For MMS this will be used as subject. Characters in this
        /// text should exist in the Latin1 (ISO-8859-1) characterset, any other characters may
        /// not be viewed properly on the receivers handset.
        /// </summary>
        public string Text { get; set; }

        public string ServiceCode { get; set; }
        public int? AgeLimit { get; set; }
        public NetworkSpecification Network { get; set; }
        public bool IsFlashMessage { get; set; }
        public Replace? Replace { get; set; }
        public string ShortCode { get; set; }
        public int Tariff { get; set; }
        public string CpaTag { get; set; }
        public bool RequestReceipt { get; set; }
        public TimeSpan? TimeToLive { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public MessageType? Type { get; set; }
    }

    public enum MessageType
    {
        Text = 1,
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

