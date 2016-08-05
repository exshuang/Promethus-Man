﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Web.Routing;
using System.Collections.Specialized;
using Prometheus.Models;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;

namespace Prometheus.Models
{
    public class EmailUtility
    {
        public static bool SendEmail(string title, List<string> tolist, string content)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("brad.qiu@finisar.com");
                foreach (var item in tolist)
                {
                    message.To.Add(item);
                }
                message.Subject = title;
                message.Body = content;

                SmtpClient client = new SmtpClient();
                client.Host = "wmail.finisar.com";
                client.EnableSsl = true;
                client.Timeout = 30000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("brad.qiu@finisar.com", "wangle@432");

                ServicePointManager.ServerCertificateValidationCallback
                    = delegate (object s, X509Certificate certificate, X509Chain chain
                    , SslPolicyErrors sslPolicyErrors) { return true; };

                new Thread(() => {
                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception ex)
                    { }
                }).Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }
    }
}