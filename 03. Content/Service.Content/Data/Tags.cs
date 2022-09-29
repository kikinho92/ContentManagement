using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Content.Data
{
    /// <summary>
    /// Data entity: information about Tags.
    /// </summary>
    public class Tags
    {
    /// <summary>
        /// Internal identifier of the tag.
        /// </summary>
        [StringLength(100), Key]
        public string Id { get; set; }

        /// <summary>
        /// Visible name of the tag.
        /// </summary>
        [StringLength(100), Required]
        public string Name { get; set; }
    }
}