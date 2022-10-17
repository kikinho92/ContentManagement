using System;
using System.ComponentModel.DataAnnotations;

namespace Service.User.Data
{
    /// <summary>
    /// Data entity: information about the user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Internal identifier of the user.
        /// </summary>
        [StringLength(100), Key]
        public string Id { get; set; }

        /// <summary>
        /// Visible name of the user within the web app
        /// </summary>
        [StringLength(50), Required]
        public string Username { get; set; }

        /// <summary>
        /// Visible email of the user within the web app
        /// </summary>
        [StringLength(50), Required]
        public string Email { get; set; }
		
		/// <summary>
        /// User role in the web app (admin, university, ...)
        /// </summary>
        [StringLength(100)]
        public string RoleId { get; set; }
		
		/// <summary>
        /// Indicates the university to which the user belongs
        /// </summary>
        [StringLength(100)]
        public string GroupId { get; set; }

        /// <summary>
        /// Date the user was created
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; }


    }
}