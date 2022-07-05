using System;

namespace LinkMobility.PSWin.Client.Model
{
    public class NetworkSpecification
    {
        public NetworkSpecification(short mcc, short mnc)
        {
            Mcc = mcc;
            Mnc = mnc;
        }

        public short Mcc { get; set; }
        public short Mnc { get; set; }

        public override string ToString()
        {
            return $"{Mcc:000}:{Mnc:00}";
        }

        public static implicit operator NetworkSpecification(string s)
        {
            var parts = s.Split(':');
            short mnc, mcc;

            if (parts.Length == 2 && short.TryParse(parts[0], out mnc) && short.TryParse(parts[1], out mcc))
                return new NetworkSpecification(mnc, mcc);
            else
                throw new ArgumentException("Not a valid network specification (mnc:mcc)");
        }
    }
}
