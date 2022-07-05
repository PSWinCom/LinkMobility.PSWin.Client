namespace LinkMobility.PSWin.Receiver.Model
{
    public class DrMessage
    {
        public DrMessage(string reference, string receiver, DeliveryState state, string deliverytime)
        {
            Reference = reference;
            Receiver = receiver;
            State = state;
            Deliverytime = deliverytime;
        }

        public string Reference { get; }
        public string Receiver { get; }
        public DeliveryState State { get; }
        public string Deliverytime { get; }

        public bool IsDelivered => State == DeliveryState.DELIVRD;
    }

    public enum DeliveryState
    {
        UNKNOWN,  // No information of delivery status available.
        DELIVRD,  // Message was successfully delivered to destination.
        EXPIRED,  // Message validity period has expired.
        DELETED,  // Message has been deleted.
        UNDELIV,  // The SMS was undeliverable (not a valid number or no available route to destination).
        REJECTD,  // Message was rejected.
        FAILED,  // The SMS failed to be delivered because no operator accepted the message or due to internal Gateway error.
        NULL,  // No delivery report received from operator. Unknown delivery status.

        // The following status codes will apply specially for Premium messages
        BARRED,  // The receiver number is barred/blocked/not in use. Do not retry message, and remove number from any subscriber list.
        BARREDA,  // The receiver could not receive the message because his/her age is below the specified AgeLimit.
        ZEROBAL,  // The receiver has an empty prepaid account.
    }
}