namespace Roomie.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Reservation
    {
        [Key]
        public int ResId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string GuestFName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string GuestLName { get; set; }

        [Display(Name = "Number of Guests")]
        public int GuestNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        [Display(Name = "Check in Date")]
        public DateTime DateCheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        [Display(Name = "Check out Date")]
        public DateTime DateCheckOut { get; set; }

        [Display(Name = "Comments")]
        public string GuestComments { get; set; }

        [Display(Name = "Staff Comments")]
        public string StaffComments { get; set; }

        //public int RoomType { get; set; }

        public int? RoomNumber { get; set; }

        public double TotalPrice { get; set; }

        //public virtual UserProfile UserProfile { get; set; }

        public virtual Room Room { get; set; }

        //public virtual RoomType RoomType1 { get; set; }
    }
}
