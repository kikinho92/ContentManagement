using System;
using System.ComponentModel.DataAnnotations;

namespace Service.User.Data
{
    /// <summary>
    /// Data entity: information about roles.
    /// </summary>
    public class Groups
    {
    /// <summary>
        /// Internal identifier of the group.
        /// </summary>
        [StringLength(100), Key]
        public string Id { get; set; }

        /// <summary>
        /// Visible name of the group.
        /// </summary>
        [StringLength(100), Required]
        public string Name { get; set; }
    }
}