using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CSSmall.Models
    {
        public class Booking
        {
            [Key]
            public int BookingID { get; set; }

            [Required]
            public string CustomerName { get; set; }

            [Required]
            public int GuestID { get; set; }

            [Required]
            public int RoomID { get; set; }

            [Required]
            public DateTime StartDate { get; set; }

            [Required]
            public DateTime EndDate { get; set; }

            [Required]
            public int Adults { get; set; }

            [Required]
            public int Children { get; set; }

            [Required]
            public decimal TotalSum { get; set; }
        }
    }

