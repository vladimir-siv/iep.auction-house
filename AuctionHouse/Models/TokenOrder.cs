namespace AuctionHouse.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TokenOrder
    {
        public Guid ID { get; set; }

        public Guid Buyer { get; set; }

        public decimal Amount { get; set; }

        [Required]
        [StringLength(32)]
        public string Currency { get; set; }

        public decimal PriceRate { get; set; }

        public bool? Status { get; set; }

        public virtual User User { get; set; }
    }
}
