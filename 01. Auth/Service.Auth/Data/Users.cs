using System.ComponentModel.DataAnnotations;

namespace Service.Auth.Data
{
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
        /// Email
        /// </summary>
        [StringLength(100)]
        public string Email { get; set; }
    }
}