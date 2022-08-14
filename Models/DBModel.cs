using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;

namespace Roomie.Models
{
    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomType> RoomTypes { get; set; }
        
        //public virtual DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasMany(e => e.Reservations)
                .WithOptional(e => e.Room)
                .HasForeignKey(e => e.RoomNumber);


            modelBuilder.Entity<RoomType>()
                .HasMany(e => e.Rooms)
                .WithRequired(e => e.RoomType)
                .HasForeignKey(e => e.Type)
                .WillCascadeOnDelete(false);
            /*
            modelBuilder.Entity<RoomType>()
                .HasMany(e => e.Reservations)
                .WithRequired(e => e.RoomType1)
                .HasForeignKey(e => e.RoomType)
                .WillCascadeOnDelete(false);
           
            
            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.Reservations)
                .WithRequired(e => e.UserProfile)
                .HasForeignKey(e => e.ResUser)
                .WillCascadeOnDelete(false);
            */

        }

        public int getNumberOOORooms()
        {
            var roomCount = Rooms.Where(s => s.IsOOO == true).ToList();
            return roomCount.Count;
        }

        public int getNumberOccupiedRooms()
        {
            var roomCount = Rooms.Where(s => s.IsOccupied == true && s.IsOOO == false).ToList();
            return roomCount.Count;
        }

        public int getNumberDirtyDirtyRooms()
        {
            var roomCount = Rooms.Where(s => s.IsOccupied == false && s.IsDirty == true).ToList();
            return roomCount.Count;
        }

        public int getNumberReadyRooms()
        {
            return Rooms.Where(r => r.IsDirty != false && r.IsOccupied != false).ToList().Count;
        }

        public float getTodayOccupancy()
        {
            var totalRooms = Rooms.Where(r => r.IsOOO != true).ToList();
            var inHouse = Reservations.Where(r => r.DateCheckIn < DateTime.Today && r.DateCheckOut > DateTime.Today).ToList();
            var arrivals = Reservations.Where(r => r.DateCheckIn == DateTime.Today).ToList();
            float occupancy = 0;

            if (totalRooms.Count != 0)
            {
                occupancy = ((inHouse.Count + arrivals.Count) / totalRooms.Count) * 100;
            }

            return occupancy;
        }

        public double getRevenue()
        {
            double revenue = 0;

            foreach (Reservation r in getInHouseReservations())
            {
                revenue += (r.TotalPrice / getResRoomNights(r));
            }

            foreach (Reservation r in getAllTodayArrivals())
            {
                revenue += (r.TotalPrice / getResRoomNights(r));
            }

            return revenue;

        }

        public int getResRoomNights(Reservation res)
        {
            return (res.DateCheckOut - res.DateCheckIn).Days;
        }

        public List<Reservation> getAllTodayArrivals()
        {
            return Reservations.Where(r => r.DateCheckIn == DateTime.Today).ToList();
        }

        public List<Reservation> getTodaysReservationsPending()
        {
            return Reservations.Where(r => r.DateCheckIn == DateTime.Today && r.Room == null).ToList();
        }

        public List<Reservation> getTodaysReservationsArrived()
        {
            return Reservations.Where(r => r.DateCheckIn == DateTime.Today && r.Room != null).ToList();
        }

        public List<Reservation> getTodaysCheckoutsPending()
        {
            return Reservations.Where(r => r.DateCheckOut == DateTime.Today && r.Room != null).ToList();
        }
        public List<Reservation> getTodaysCheckoutsDeparted()
        {
            return Reservations.Where(r => r.DateCheckOut == DateTime.Today && r.Room == null).ToList();
        }

        public List<Reservation> getInHouseReservations()
        {
            return Reservations.Where(r => r.DateCheckIn  < DateTime.Today && r.DateCheckOut > DateTime.Today && r.Room != null).ToList();
        }

    }
}
