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
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.IO;
using Roomie.Utils;
using Microsoft.AspNet.Identity;

namespace Roomie.Controllers
{
    public class ReservationsController : Controller
    {
        private DBModel db = new DBModel();
        private EmailSender es = new EmailSender();

        // GET: Reservations
        public ActionResult Index()
        {
            //var reservations = db.Reservations.Include(r => r.Room).Include(r => r.RoomType1).Include(r => r.UserProfile);
            //return View(reservations.ToList());
            return View(db.Reservations.ToList());
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // GET: Reservations/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var users = db.AspNetUsers.Where(s => s.Id == userId).ToList();
                AspNetUser user = users[0];

                ViewBag.ResUser = userId;

                if (users.Single().IsStaff)
                {
                    ViewBag.isStaff = true;
                }

                ViewBag.RoomNumber = new SelectList(db.Rooms, "RoomNum", "RoomNum");
                //ViewBag.RoomType = new SelectList(db.RoomTypes, "TypeId", "TypeName");

                return View();
            }
            else
            {
                return View("Index");
            }         
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ResId,GuestFName,GuestLName,GuestNumber,DateCheckIn,DateCheckOut,GuestComments,StaffComments,RoomNumber,TotalPrice")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.TotalPrice = (reservation.DateCheckOut - reservation.DateCheckIn).TotalDays * 100; //db.RoomTypes.Find(reservation.RoomType).Price;

                db.Reservations.Add(reservation);
                db.SaveChanges();

                SendConfirmation(reservation);

                return RedirectToAction("Index");
            }

            ViewBag.RoomNumber = new SelectList(db.Rooms, "RoomNum", "RoomNum", reservation.RoomNumber);
            //ViewBag.RoomType = new SelectList(db.RoomTypes, "TypeId", "TypeName", reservation.RoomType);

            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomNumber = new SelectList(db.Rooms, "RoomNum", "RoomNum", reservation.RoomNumber);
            //ViewBag.RoomType = new SelectList(db.RoomTypes, "TypeId", "TypeName", reservation.RoomType);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ResId,ResUser,GuestFName,GuestLName,GuestNumber,DateCheckIn,DateCheckOut,GuestComments,StaffComments,RoomType,RoomNumber,TotalPrice")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoomNumber = new SelectList(db.Rooms, "RoomNum", "RoomNum", reservation.RoomNumber);
            //ViewBag.RoomType = new SelectList(db.RoomTypes, "TypeId", "TypeName", reservation.RoomType);

            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void SendConfirmation(Reservation res)
        {
            string checkin = res.DateCheckIn.ToString();
            string checkout = res.DateCheckOut.ToString();
            string newline = System.Environment.NewLine;

            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("jcir0001@student.monash.edu", "Roomie");
            var subject = "Reservation Confirmation";
            var to = new EmailAddress("j.cirocco@hotmail.com", "Roomie");
            var plainTextContent = "";
            var htmlContent = "<strong>" + res.ResId + "</strong>" + newline +
                "<p>Your reservation id is: " + res.ResId + newline
                + "Check In: " + checkin + newline
                + "Check Out: " + checkout + newline
                + "Price: " + res.TotalPrice + "</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var bytes = System.IO.File.ReadAllBytes("C:/Users/jciro/Documents/Confirmation.pdf");
            var file = Convert.ToBase64String(bytes);
            msg.AddAttachment("Confirmation.pdf", file);

            var response = client.SendEmailAsync(msg);
        }

    }
}
