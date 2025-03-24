using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


    namespace CSSmall.Models
    {
        public class Room
        {
            [Key]
            public int RoomID { get; set; }

            [Required]
            public string RoomNumber { get; set; }

            [Required]
            public string RoomType { get; set; }

            [Required]
            public bool IsVacant { get; set; }

            [Required]
            public decimal Price { get; set; }

            [Required]
            public bool NeedCleaning { get; set; }
        }
    }

