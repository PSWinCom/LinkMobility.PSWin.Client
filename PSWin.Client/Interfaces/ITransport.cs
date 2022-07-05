using LinkMobility.PSWin.Client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkMobility.PSWin.Client.Interfaces
{
    public interface ITransport
    {
        Task<IEnumerable<MessageResult>> SendAsync(IEnumerable<Sms> messageBatch, string sessionData);
    }
}
