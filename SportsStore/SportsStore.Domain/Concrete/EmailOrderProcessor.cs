using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using System.Net.Mail;
using System.Net;

namespace SportsStore.Domain.Concrete
{
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings _emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            _emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = _emailSettings.UseSSl;
                smtpClient.Host = _emailSettings.ServerName;
                smtpClient.Port = _emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);

                if(_emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = _emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                var body = new StringBuilder()
                    .AppendLine("Nowe zamówienie")
                    .AppendLine("---")
                    .AppendLine("Produkty:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (wartość: {2:c}", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Wartość całkowita: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Wysyłka dla:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1)
                    .AppendLine(shippingDetails.Line2 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Country)
                    .AppendLine("---")
                    .AppendFormat("Pakowanie prezentu: {0}", shippingDetails.GiftWrap ? "Tak" : "Nie");

                MailMessage mailMessage = new MailMessage(_emailSettings.MailFromAddress, _emailSettings.MailToAddress,
                "Otrzymano nowe zamówienie!", body.ToString());

                if(_emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }           
        }
    }

    public class EmailSettings
    {
        public string MailToAddress = "zamowienia@przyklad.pl";
        public string MailFromAddress = "sklepsportowy@przyklad.pl";
        public bool UseSSl = true;
        public string UserName = "UserSmtp";
        public string Password = "HasloSmtp";
        public string ServerName = "smtp.przyklad.pl";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\sports_store_emails";
    }
}
