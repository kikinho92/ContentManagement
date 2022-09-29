using System;
using System.ComponentModel.DataAnnotations;

namespace Service.User.Data
{
    /// <summary>
    /// Data entity: information about the user.
    /// </summary>
    public class Users
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
        /// Password of the user to able to access to the web app
        /// </summary>
        [StringLength(50)]
        public string Password { get; set; }
		
		/// <summary>
        /// User role in the web app (admin, university, ...)
        /// </summary>
        [StringLength(100), Key]
        public string RoleId { get; set; }
		
		/// <summary>
        /// Indicates the university to which the user belongs
        /// </summary>
        [StringLength(100), Key]
        public string GroupId { get; set; }

        /// <summary>
        /// Date the user was created
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Indicates if the user is active or was removed
        /// </summary>
        public bool Active { get; set; }


    }
}