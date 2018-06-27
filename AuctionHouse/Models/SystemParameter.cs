namespace AuctionHouse.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SystemParameter
    {
        public Guid ID { get; set; }

        public int RecentAuctions { get; set; }

        public int DefaultAuctionTime { get; set; }

        public decimal SilverPackage { get; set; }

        public decimal GoldPackage { get; set; }

        public decimal PlatinumPackage { get; set; }

        [Required]
        [StringLength(32)]
        public string Currency { get; set; }

        public decimal PriceRate { get; set; }
    }
}
