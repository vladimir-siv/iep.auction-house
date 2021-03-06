namespace AuctionHouse.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Auction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auction()
        {
            Bids = new HashSet<Bid>();
        }

        public Guid ID { get; set; }

        [Required]
        [StringLength(64)]
        public string Title { get; set; }

        public int AuctionTime { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? OpenedOn { get; set; }

        public DateTime? CompletedOn { get; set; }

        public decimal StartingPrice { get; set; }

        [Required]
        [StringLength(32)]
        public string Currency { get; set; }

        public decimal PriceRate { get; set; }

        public Guid Holder { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bids { get; set; }

		public Bid LastBid { get { return Bids.Best((bid1, bid2) => bid1.BidOn > bid2.BidOn ? bid1 : bid2); } }
    }
}
