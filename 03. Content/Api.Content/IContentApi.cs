using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Api.User.IUserApi;

namespace Api.Content
{
    public interface IContentApi
    {
        const string SERVICE_ROUTE = "content";
        const string PING = "ping";

        public record ContentInfo(string id, string title, string description, string link, string[] authors, string[] licenseTypes, List<TagInfo> tags, string userid, DateTime? uploadDate );
        public record TagInfo(string id, string name, string userid, DateTime? uploadDate);
        /// <summary>
        /// Provides detailed information about all the content. In case of tagId and group is not null, it will be provide the content that belong to the tag and group.
        /// </summary>
        /// <param name="tagId">Unique internal identifier of the tag</param>
		/// <param name="groupId">Unique internal identifier of the group</param>
        Task<List<ContentInfo>> GetContents(int pageSize, int page, string tagId = null, string groupId = null);
        const string CONTENT_LIST_PATH = "contentsList";
        const string TAG_ID_PATH = "tagId";
		const string GROUP_ID_PATH = "groupId";
        const string PAGE_SIZE = "pageSize";
        const string PAGE = "page";
        /// <summary>
        /// Provides detailed information about the content requested
        /// </summary>
        /// <param name="contentId">Unique internal identifier of the content</param>
        Task<ContentInfo> GetContent(string contentId);
        const string CONTENT_PATH = "contents";
        const string CONTENT_ID_PATH = "contentId";

        /// <summary>
        /// Save detailed information about content
        /// </summary>
        /// <param name="content">Content entity that contains the information</param>
        Task<ContentInfo> PostContent(ContentInfo content);

        /// <summary>
        /// Modifies content which already exist in the db
        /// </summary>
        /// <param name="content">Content entity that contains the new information</param>
        /// <param name="contentId">Unique internal identifier of the content which is going to be replaced</param>
        Task<ContentInfo> PutContent(ContentInfo content, string contentId);

        /// <summary>
        /// Removes from the system the information about content.
        /// </summary>
        /// <param name="contentId">Unique internal identifier of the content to be deleted</param>
        Task<bool> DeleteContent(string contentId);
		
		/// <summary>
        /// Provides detailed information about all the tags. In case of group is not null, it will be provide the content that belong to the group.
        /// </summary>
		/// <param name="groupId">Unique internal identifier of the group</param>
        Task<List<TagInfo>> GetTags(string groupId = null);
        const string TAG_PATH = "tag";
		
		/// <summary>
        /// Save detailed information about tag
        /// </summary>
        /// <param name="tag">Tag entity that contains the information</param>
        Task<TagInfo> PostTag(TagInfo tag);
    }
}
