using MailKit.Net.Smtp;                         
using MimeKit;

namespace Fruitkh.Utilities.EmailHandler.Models;

public class Message
{
    public string Subject { get; set; }
    public string Content { get; set; }
    public List<MailboxAddress> To { get; set; }
    public Message(List<string> to, string subject, string content)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress(x)));
        Subject = subject;
        Content = content;
    }
}
