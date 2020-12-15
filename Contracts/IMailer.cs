using System.Threading.Tasks;
namespace MailTest.Contracts
{
    public interface IMailer
    {
         Task SendEmailAsnyc(string email,string subject,string body);
    }
}