namespace LinkMobility.PSWin.Client.Model
{
    public class CpaGas
    {
        public CpaGas(int tariff)
        {
            Tariff = tariff;
        }

        public CpaGas(int tariff, string serviceCode, int? ageLimit, string cpaTag)
        {
            Tariff = tariff;
            ServiceCode = serviceCode;
            AgeRestriction = ageLimit;
            Tag = cpaTag;
        }

        /// <summary>
        /// Required.
        /// Specifies the amount to charge the end-user in units of Norwegian ører (1/100th NOK).
        /// For example, to charge the end-user NOK 5,- you specify 500 as the value.
        /// Tariff values up to 50000 (NOK 500,-) can be used.
        /// </summary>
        public int Tariff { get; set; }

        /// <summary>
        /// Specifies the type of Goods or Service that the transaction is related to.
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// You may specify an age restriction when sending Premium SMS messages.
        /// The value will be matched against the age of the subscriber.
        /// The subscriber must then be at least the given age in order to receive the message.
        /// The operators usually accept 16 and 18.
        /// The Gateway will pick the most appropriate age limit it the operator cannot accept the given age limit.
        /// 
        /// Examples:
        ///     AgeLimit is given as 17. The operator only accepts 0, 16 or 18. Gateway sends 16 to operator.
        ///     AgeLimit is given as 22. The operator only accepts 0, 16 or 18. Gateway sends 18 to operator.
        ///     
        /// It is recommended that you only use 16 or 18. 
        /// </summary>
        public int? AgeRestriction { get; set; }

        /// <summary>
        /// Premium SMS messages may have an additional description associated with them.
        /// This value will be shown on the subscribers phone bill to help identify the service purchased.
        /// Please note that not all operators support this property, and there may be restrictions the format.
        /// Pelase consult PSWinCom technical support if you intend to use this feature. Others should set it to null.
        /// </summary>
        public string Tag { get; set; }
    }
}
