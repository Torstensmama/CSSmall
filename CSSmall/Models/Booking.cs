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
        public int GuestID { get; set; }  // Gäst-ID, kan kopplas till en framtida Guest-modell

        [Required]
        [ForeignKey("Room")]
        public int RoomID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public int AmountOfAdults { get; set; }

        [Required]
        public int AmountOfChildren { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalSum { get; set; }

        // Navigationsproperty
        public Room Room { get; set; }
    }
}
