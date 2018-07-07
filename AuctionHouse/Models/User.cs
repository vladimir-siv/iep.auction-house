namespace AuctionHouse.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
		public static User Dummy { get; } = new User
		{
			ID = Guid.Empty,
			FirstName = "Unknown",
			LastName = "User",
			Email = "???",
			Password = "???",
			Balance = 0
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Bids = new HashSet<Bid>();
            TokenOrders = new HashSet<TokenOrder>();
        }

        public Guid ID { get; set; }

        [Required(ErrorMessage = "First name required")]
        [StringLength(64, MinimumLength = 1, ErrorMessage = "First name cannot be empty or too long.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name required")]
        [StringLength(64, MinimumLength = 1, ErrorMessage = "Last name cannot be empty or too long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[StringLength(64, MinimumLength = 5, ErrorMessage = "Email address must be at least 5 characters long but not too long.")]
		public string Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        [StringLength(64, MinimumLength = 3, ErrorMessage = "Password has to be at least 3 characters long but cannot be too long.")]
        public string Password { get; set; }

        public decimal Balance { get; set; }

        public virtual Administrator Administrator { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bids { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TokenOrder> TokenOrders { get; set; }
    }
}
