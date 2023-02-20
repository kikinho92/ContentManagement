using System;
using System.ComponentModel.DataAnnotations;

namespace Scrapy.uc3m.Data
{
    /// <summary>
    /// Data entity: information about relation between tags and contents.
    /// </summary>
    public class ContentTag
    {
    /// <summary>
        /// Internal identifier of content.
        /// </summary>
        [StringLength(100), Key]
        public string IdContent { get; set; }

        /// <summary>
        /// Visible name of the tag.
        /// </summary>
        [StringLength(100), Required]
        public string IdTag { get; set; }
    }
}