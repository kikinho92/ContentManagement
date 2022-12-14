using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Content;
using Api.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Content.Data;
using static Api.Content.IContentApi;
using static Api.User.IUserApi;

namespace Service.Content.Controllers
{
    [ApiController]
    [Route(IContentApi.SERVICE_ROUTE)]
    public class ContentController : ControllerBase
    {

        private readonly ILogger<ContentController> _logger;
        private readonly ContentDbContext _dbContext;
        private readonly IUserApi _user;

        public ContentController(ILogger<ContentController> logger,
                                    ContentDbContext dbContext,
                                    IUserApi user)
        {
            _logger = logger;
            _dbContext = dbContext;
             _user = user;
        }

        [HttpGet]
        [Route(IContentApi.PING)]
        [AllowAnonymous]
        public ActionResult<string> Ping()
        {
            return Ok("Pinged successful!");
        }

        [HttpGet]
        [Route(CONTENT_PATH + "/{" + TAG_ID_PATH + "}/{" + IContentApi.GROUP_ID_PATH + "}")]
        public async Task<ActionResult<List<ContentInfo>>> GetContents(string tagId = null, string groupId = null)
        {
            try
            {
                List<Service.Content.Data.Content> contents = (from con in _dbContext.Content
                                                              join contag in _dbContext.ContentTag
                                                                  on con.Id equals contag.IdContent
                                                              where (!string.IsNullOrEmpty(tagId) && contag.IdTag == tagId) ||
                                                                    1 == 1
                                                              select con).ToList();
                
                if (!string.IsNullOrEmpty(groupId))
                {
                    List<UserInfo> users = await _user.GetUsers(groupId);

                    contents = contents.Where(con => users.Select(u => u.id).ToList().Contains(con.UserId)).ToList();
                }

                List<ContentInfo> contentsInfo = new List<ContentInfo>();
                foreach (Data.Content content in contents)
                {
                    contentsInfo.Add(new ContentInfo(content.Id, content.Title, content.Link, content.UserId, content.UploadDate));
                }

                return Ok(contentsInfo);

            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(IContentApi.CONTENT_PATH + "/{" + IContentApi.USER_ID_PATH + "}")]
        public ActionResult<ContentInfo> GetContent(string contentId)
        {
            try
            {
                if (string.IsNullOrEmpty(contentId)) { return BadRequest("Invalid content id. Content not found"); }

                Data.Content content = _dbContext.Content.Where(c => c.Id == contentId).FirstOrDefault();
                if (content == null) { return BadRequest("Content not found"); }

                ContentInfo contentInfo = new ContentInfo(content.Id,
                                                            content.Title,
                                                            content.Link,
                                                            content.UserId,
                                                            content.UploadDate);
                return Ok(contentInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(CONTENT_PATH)]
        public ActionResult<ContentInfo> PostContent(ContentInfo content)
        {
            try
            {
                if (content == null) { return BadRequest("Invalid content. Content can not be empty"); }

                Data.Content newContent = new Data.Content()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = content.title,
                    Link = content.link,
                    UserId = content.userid,
                    UploadDate = DateTime.Now
                };

                _dbContext.Content.Add(newContent);
                _dbContext.SaveChanges();

                _logger.LogInformation($"CONTENT {newContent.Id} stored succesfully.");

                return Ok(content);
            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpDelete]
        [Route(CONTENT_PATH)]
        public ActionResult<bool> DeleteContent(string contentId)
        {
            try
            {
                if (string.IsNullOrEmpty(contentId)) return BadRequest("Invalid content. Content was not found.");

                Data.Content content = _dbContext.Content.Where(con => con.Id == contentId).FirstOrDefault();
                if (content == null) return BadRequest("Invalid content. Content was not found.");

                _dbContext.Content.Remove(content);
                _dbContext.SaveChanges();

                _logger.LogInformation($"CONTENT {content.Id} removed succesfully.");

                 return Ok(true);

            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(TAG_PATH + "/{" + IContentApi.GROUP_ID_PATH + "}")]
        public async Task<ActionResult<List<TagInfo>>> GetTags(string groupId = null)
        {
            try
            {
                List<Tag> tags = _dbContext.Tags.ToList();

                if (!string.IsNullOrEmpty(groupId))
                {
                    List<UserInfo> users = await _user.GetUsers(groupId);

                    tags = tags.Where(con => users.Select(u => u.id).ToList().Contains(con.UserId)).ToList();
                }

                List<TagInfo> tagsInfo = new List<TagInfo>();
                foreach (Tag tag in tags)
                {
                    tagsInfo.Add(new TagInfo(tag.Id, tag.Name, tag.UserId, tag.UploadDate));
                }

                return Ok(tagsInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(TAG_PATH)]
        public ActionResult<TagInfo> PostTag(TagInfo tag)
        {
            try
            {
                if (tag == null) { return BadRequest("Invalid tag. Tag can not be empty"); }

                Tag newTag = new Tag()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = tag.name,
                    UserId = tag.userid,
                    UploadDate = DateTime.Now
                };

                _dbContext.Tags.Add(newTag);
                _dbContext.SaveChanges();

                _logger.LogInformation($"TAG {newTag.Id} stored succesfully.");

                return Ok(tag);
            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }


    }
}
