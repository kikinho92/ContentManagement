using System;
using System.ComponentModel.DataAnnotations;

namespace Scrapy.uc3m.Data
{
    /// <summary>
    /// Data entity: information about the user.
    /// </summary>
    public class Content
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
        /// Large description of the content
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Grade to which content belongs
        /// </summary>
        [StringLength(500)]
        public string Grades { get; set; }

        /// <summary>
        /// Link to conect with the aplication which bring up the content
        /// </summary>
        [StringLength(200), Required]
        public string Link { get; set; }

        /// <summary>
        /// Department to which the content belongs.
        /// </summary>
        [StringLength(200)]
        public string Department { get; set; }

        /// <summary>
        /// Author the content belong to. It can be more than one. It is gonna be separate by commas (,)
        /// </summary>
        [StringLength(200)]
        public string Authors { get; set; }

        /// <summary>
        /// Content´s license types. It can be more than one. It is gonna be separate by commas (,)
        /// </summary>
        [StringLength(200)]
        public string LicenseTypes { get; set; }
		
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