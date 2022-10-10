using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Auth.Data
{
    public class UserSession
    {
        [StringLength(100), Key]
        public string Id { get; set; }

        /// <summary>
        /// Internal identifier of the user.
        /// </summary>
        [StringLength(100), Required]
        public string UserId { get; set; }

        /// <summary>
        /// Public unique identifier of the user that opened the session
        /// </summary>
        [StringLength(100), Required]
        public string UserEmail { get; set; }

        /// <summary>
        /// Public unique identifier of the user role
        /// </summary>
        [StringLength(100), Required]
        public string UserRole { get; set; }

        /// <summary>
        /// Token generated during session initialization to be required when the atuhentication JWT token
        /// expires and it is desired to refresh it.
        /// </summary>
        [StringLength(100), Required]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Time-stamp when the session was open by the user
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// Time-stamp when the JWT token was refresed most recently
        /// </summary>
        public DateTime? LastRefresh { get; set; }

        /// <summary>
        /// Time-stamp when the user logged out
        /// </summary>
        public DateTime? CloseTime { get; set; }
    }
}