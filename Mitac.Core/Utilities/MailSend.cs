using Mitac.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Mitac.Core.Utilities
{
    public class MailSend
    {
        public static void SendMailToDeveloper(string sendMailMessage,string receiver)
        {
            string body = "<p>Dear sir</p>";
            body += sendMailMessage;
            body += @"<br><br>~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~<br>
                    Notice:
                    ◎ Notification - only, please do not reply to sender.<br>
                    ◎ According to information security, please do not forward to others.<br>
                    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~<br>
                    MiTAC MIS Group thanks for your cooperation. <br>";

            string[] ReceiveAddresses = receiver.Split(';');
            string sendAddress = "MiSFCS@MIC.COM.TW";//寄件人邮箱
            string userPassword = string.Empty;//登陆密码
            foreach (string ReceiveAddress in ReceiveAddresses)
            {
                MailMessage mail = new MailMessage(sendAddress, ReceiveAddress);
                string mSmtp = "em.mitac-mkl.com.cn";
                SmtpClient client = new SmtpClient(mSmtp);
                try
                {
                    mail.Subject = "合同發起異常提醒";
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;
                    client.Credentials = new System.Net.NetworkCredential(sendAddress, userPassword);//用户名和密码
                    client.Send(mail);
                }
                catch (SmtpException ex)
                {
                }
                finally
                {
                    mail.Dispose();
                }
            }
        }

    }
}
