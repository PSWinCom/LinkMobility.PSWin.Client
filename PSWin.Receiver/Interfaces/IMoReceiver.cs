using LinkMobility.PSWin.Receiver.Model;
using System.Threading.Tasks;

namespace LinkMobility.PSWin.Receiver.Interfaces
{
    public interface IMoReceiver
    {
        Task ReceiveAsync(MoMessage moMessage);
    }
}
