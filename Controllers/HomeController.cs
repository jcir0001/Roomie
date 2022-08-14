using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Roomie.Models;

namespace Roomie.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private DBModel db = new DBModel();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var users = db.AspNetUsers.Where(s => s.Id == userId).ToList();
                AspNetUser user = users[0];

                if (users.Single().IsStaff)
                {
                    //Get daily summary params
                    ViewBag.Occupancy = db.getTodayOccupancy() + "%";
                    ViewBag.Revenue = "$" + db.getRevenue();
                    ViewBag.PendingCheckout = db.getTodaysCheckoutsPending().Count;
                    ViewBag.PendingArrivals = db.getTodaysReservationsPending().Count;
                    ViewBag.InHouse = db.getInHouseReservations().Count;

                    //Get rooms management params
                    ViewBag.RoomsDirty = db.getNumberDirtyDirtyRooms();
                    ViewBag.RoomsOOO = db.getNumberOOORooms();
                    ViewBag.RoomsReady = db.getNumberReadyRooms();
                    ViewBag.RoomsOccupied = db.getNumberOccupiedRooms();

                    int departurecount = db.getTodaysCheckoutsDeparted().Count + db.getTodaysCheckoutsPending().Count;
                    int inhousecount = db.getInHouseReservations().Count;

                    //Get res management params
                    ViewBag.DeparturesCount = departurecount;
                    ViewBag.ArrivalsCount = db.getAllTodayArrivals();
                    ViewBag.Inhouse = inhousecount;

                    return View("IndexStaff");
                }
                else
                {
                    return View("IndexGuest");
                }
            }

        return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ResManage()
        {
            var arrivals = db.getTodaysReservationsPending();
            var departures = db.getTodaysCheckoutsPending();
            var inhouse = db.getInHouseReservations();

            ViewBag.text = arrivals.Count.ToString() + inhouse.Count.ToString() + departures.Count.ToString();

            ViewBag.arrivals = arrivals;
            ViewBag.departures = departures;
            ViewBag.inhouse = inhouse;

            return View();
        }



        // GET: Home/RoomManage
        public ActionResult RoomManage()
        {
            if (User.Identity.IsAuthenticated)
            {
                var rooms = db.Rooms.ToList();

                var userId = User.Identity.GetUserId();
                var users = db.AspNetUsers.Where(s => s.Id == userId).ToList();
                AspNetUser user = users[0];

                if (users.Single().IsStaff)
                {
                    //Get daily summary params
                    ViewBag.Occupancy = db.getTodayOccupancy() + "%";
                    ViewBag.Revenue = "$" + db.getRevenue();
                    ViewBag.PendingCheckout = db.getTodaysCheckoutsPending().Count;
                    ViewBag.PendingArrivals = db.getTodaysReservationsPending().Count;
                    ViewBag.InHouse = db.getInHouseReservations().Count;

                    //Get rooms management params
                    ViewBag.RoomsDirty = db.getNumberDirtyDirtyRooms();
                    ViewBag.RoomsOOO = db.getNumberOOORooms();
                    ViewBag.RoomsReady = db.getNumberReadyRooms();
                    ViewBag.RoomsOccupied = db.getNumberOccupiedRooms();

                    int departurecount = db.getTodaysCheckoutsDeparted().Count + db.getTodaysCheckoutsPending().Count;
                    int inhousecount = db.getInHouseReservations().Count;

                    //Get res management params
                    ViewBag.DeparturesCount = departurecount;
                    ViewBag.ArrivalsCount = db.getAllTodayArrivals();
                    ViewBag.Inhouse = inhousecount;

                    return View();
                }
            }

            return View("Index");
        }



    }
}