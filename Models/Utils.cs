// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Roomie.Models;

namespace Roomie.Utils
{
    public class EmailSender
    {
        private static void Main()
        {
            Execute().Wait();
        }

        private static void Main(string bookingId)
        {
            Execute(bookingId).Wait();
        }
        

        public static void Main(Reservation res)
        {
            Execute(res).Wait();
        }

        static async Task Execute()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("jcir0001@student.monash.edu", "Roomie");
            var subject = "Reservation Confirmation";
            var to = new EmailAddress("jcir0001@student.monash.edu", "Roomie");
            var plainTextContent = "your reservation is booked";
            var htmlContent = "<strong>" + "" + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        static async Task Execute(string bookingId)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("jcir0001@student.monash.edu", "Roomie");
            var subject = "Reservation Confirmation";
            var to = new EmailAddress("jcir0001@student.monash.edu", "Roomie");
            var plainTextContent = "your reservation is booked";
            var htmlContent = "<strong>" + bookingId + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public static async Task Execute(Reservation res)
        {
            string checkin = res.DateCheckIn.ToString();
            string checkout = res.DateCheckOut.ToString();

            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("jcir0001@student.monash.edu", "Roomie");
            var subject = "Reservation Confirmation";
            var to = new EmailAddress("jcir0001@student.monash.edu", "Roomie");
            var plainTextContent = "Your reservation id is: " + res.ResId + "/n" 
                + "Check In: " + checkin + "/n" 
                + "Check Out: " + checkout + "/n" 
                + "Price: " + res.TotalPrice;
            var htmlContent = "<strong>" + res.ResId + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
}