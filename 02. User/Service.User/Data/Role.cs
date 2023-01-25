using System;
using System.ComponentModel.DataAnnotations;

namespace Service.User.Data
{
    /// <summary>
    /// Data entity: information about roles.
    /// </summary>
    public class Role
    {
    /// <summary>
        /// Internal identifier of the role.
        /// </summary>
        [StringLength(100), Key]
        public string Id { get; set; }

        /// <summary>
        /// Visible name of the role.
        /// </summary>
        [StringLength(100), Required]
        public string Name { get; set; }
    }
}