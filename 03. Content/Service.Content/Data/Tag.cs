using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Content.Data
{
    /// <summary>
    /// Data entity: information about Tags.
    /// </summary>
    public class Tag
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

        /// <summary>
        /// Owner user of the content
        /// </summary>
        [StringLength(100)]
        public string UserId { get; set; }

        /// <summary>
        /// Date the content was uploaded
        /// </summary>
        [Required]
        public DateTime UploadDate { get; set; }
    }
}