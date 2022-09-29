using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Content.Data
{
    /// <summary>
    /// Data entity: information about the user.
    /// </summary>
    public class Contents
    {
        /// <summary>
        /// Internal identifier of the content.
        /// </summary>
        [StringLength(100), Key]
        public string Id { get; set; }

        /// <summary>
        /// Short description of the content
        /// </summary>
        [StringLength(50), Required]
        public string Title { get; set; }

        /// <summary>
        /// Link to conect with the aplication which bring up the content
        /// </summary>
        [StringLength(200)]
        public string Link { get; set; }
		
		/// <summary>
        /// Owner user of the content
        /// </summary>
        [StringLength(100), Key]
        public string UserId { get; set; }

        /// <summary>
        /// Date the content was uploaded
        /// </summary>
        [Required]
        public DateTime UploadDate { get; set; }


    }
}