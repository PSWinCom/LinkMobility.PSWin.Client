using LinkMobility.PSWin.Client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkMobility.PSWin.Client.Interfaces
{
    /// <summary>
    /// The interface for an implementation of a PSWin gateway API.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Send text messages to the API that this transport implements.
        /// </summary>
        /// <param name="messageBatch">The messages to send.</param>
        /// <param name="sessionData">
        ///     A free text field that can be used to tag the session/messages with customer specific data such as the application name, username, reference-id etc.
        ///     The maximum length is 200 characters.
        /// </param>
        /// <returns>A <see cref="MessageResult"/> for each message in <paramref name="messageBatch"/>, in the same order.</returns>
        Task<IEnumerable<MessageResult>> SendAsync(IEnumerable<Sms> messageBatch, string sessionData);
    }
}
