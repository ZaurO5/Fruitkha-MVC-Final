using Fruitkh.Utilities.EmailHandler.Models;

namespace Fruitkh.Utilities.EmailHandler.Abstract;

public interface IEmailService
{
    void SendMessage(Message message);
}
