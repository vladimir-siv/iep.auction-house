namespace AuctionHouse.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Bid
    {
        public Guid ID { get; set; }

        public Guid Bidder { get; set; }

        public Guid Auction { get; set; }

        public DateTime BidOn { get; set; }

        public decimal Amount { get; set; }

        public virtual Auction Auction1 { get; set; }

        public virtual User User { get; set; }
    }
}
