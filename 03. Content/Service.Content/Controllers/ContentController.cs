using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Content;
using Api.User;
using ClosedXML.Excel;
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
         private readonly IContentApi _content;

        public ContentController(ILogger<ContentController> logger,
                                    ContentDbContext dbContext,
                                    IUserApi user,
                                    IContentApi content)
        {
            _logger = logger;
            _dbContext = dbContext;
            _user = user;
            _content = content;
        }

        [HttpGet]
        [Route(IContentApi.PING)]
        [AllowAnonymous]
        public ActionResult<string> Ping()
        {
            return Ok("Pinged successful!");
        }

        [HttpGet]
        [Route(CONTENT_LIST_PATH)]
        public async Task<ActionResult<List<ContentInfo>>> GetContents(int pageSize, int page, string tagId = null, string groupId = null)
        {
            try
            {
                List<string> usersId = new List<string>();
                if (!string.IsNullOrEmpty(groupId))
                {
                    List<UserInfo> users = await _user.GetUsers(groupId);
                    usersId = users.Select(u => u.id).ToList();
                }

                List<Service.Content.Data.Content> contents = (from con in _dbContext.Content
                                                               join contag in _dbContext.ContentTag
                                                                   on con.Id equals contag.IdContent
                                                               where ((!string.IsNullOrEmpty(tagId) && contag.IdTag == tagId) || 1 == 1) &&
                                                                     (!string.IsNullOrEmpty(groupId) && usersId.Contains(con.UserId))
                                                               select con)
                                                              .Distinct()
                                                              .OrderBy(c => c.Title)
                                                              .Skip(pageSize * page)
                                                              .Take(pageSize)
                                                              .ToList();



                List<ContentInfo> contentsInfo = new List<ContentInfo>();
                foreach (Data.Content content in contents)
                {
                    List<Tag> tags = (from contag in _dbContext.ContentTag
                                      join tag in _dbContext.Tags
                                          on contag.IdTag equals tag.Id
                                      where contag.IdContent == content.Id
                                      select tag).ToList();

                    List<TagInfo> tagsInfo = new List<TagInfo>();
                    foreach (Tag tag in tags)
                    {
                        tagsInfo.Add(new TagInfo(tag.Id, tag.Name, tag.UserId, tag.UploadDate));
                    }

                    contentsInfo.Add(new ContentInfo(content.Id,
                                                        content.Title,
                                                        content.Description,
                                                        content.Link,
                                                        content.Department,
                                                        content.Grades != null ? content.Grades.Split(',') : new string[] { },
                                                        content.Authors != null ? content.Authors.Split(',') : new string[] { },
                                                        content.LicenseTypes != null ? content.LicenseTypes.Split(',') : new string[] { },
                                                        tagsInfo,
                                                        content.UserId,
                                                        content.UploadDate));
                }

                return Ok(contentsInfo);

            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }

        [HttpGet]
        [Route(IContentApi.CONTENT_PATH)]
        public ActionResult<ContentInfo> GetContent(string contentId)
        {
            try
            {
                if (string.IsNullOrEmpty(contentId)) { return BadRequest("ERROR - Invalid content id. Content not found"); }

                Data.Content content = _dbContext.Content.Where(c => c.Id == contentId).FirstOrDefault();
                if (content == null) { return BadRequest("ERROR - Content not found"); }

                List<Tag> tags = (from contag in _dbContext.ContentTag
                                  join tag in _dbContext.Tags
                                      on contag.IdTag equals tag.Id
                                  where contag.IdContent == content.Id
                                  select tag).ToList();

                List<TagInfo> tagsInfo = new List<TagInfo>();
                foreach (Tag tag in tags)
                {
                    tagsInfo.Add(new TagInfo(tag.Id, tag.Name, tag.UserId, tag.UploadDate));
                }

                ContentInfo contentInfo = new ContentInfo(content.Id,
                                                        content.Title,
                                                        content.Description,
                                                        content.Link,
                                                        content.Department,
                                                        content.Grades != null ? content.Grades.Split(',') : new string[] { },
                                                        content.Authors != null ? content.Authors.Split(',') : new string[] { },
                                                        content.LicenseTypes != null ? content.LicenseTypes.Split(',') : new string[] { },
                                                        tagsInfo,
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
                    Description = content.description,
                    Link = content.link,
                    Department = content.department,
                    Grades = string.Join(",", content.grades),
                    Authors = string.Join(",", content.authors),
                    LicenseTypes = string.Join(",", content.licenseTypes),
                    UserId = content.userid,
                    UploadDate = DateTime.Now
                };

                foreach (TagInfo tagInfo in content.tags)
                {
                    Tag tag = _dbContext.Tags.Where(t => t.Id == tagInfo.id && t.UserId == tagInfo.userid
                                                        || t.Name == tagInfo.name && t.UserId == tagInfo.userid).FirstOrDefault();
                    if (tag == null)
                    {
                        tag = new Tag()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = tagInfo.name
                        };
                        _dbContext.Tags.Add(tag);
                    }
                    ContentTag contentTag = _dbContext.ContentTag.Where(ct => ct.IdContent == newContent.Id && ct.IdTag == tag.Id).FirstOrDefault();
                    if (contentTag == null)
                    {
                        contentTag = new ContentTag()
                        {
                            IdContent = newContent.Id,
                            IdTag = tag.Id
                        };
                        _dbContext.ContentTag.Add(contentTag);
                    }
                }

                _dbContext.Content.Add(newContent);
                _dbContext.SaveChanges();

                _logger.LogInformation($"CONTENT {newContent.Id} stored succesfully.");

                ContentInfo returnContent = new ContentInfo(newContent.Id,
                                                       newContent.Title,
                                                       newContent.Description,
                                                       newContent.Link,
                                                       newContent.Department,
                                                       newContent.Grades != null ? newContent.Grades.Split(',') : new string[] { },
                                                       newContent.Authors != null ? newContent.Authors.Split(',') : new string[] { },
                                                       newContent.LicenseTypes != null ? newContent.LicenseTypes.Split(',') : new string[] { },
                                                       content.tags,
                                                       newContent.UserId,
                                                       newContent.UploadDate);

                return Ok(returnContent);
            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPut]
        [Route(CONTENT_PATH)]
        public ActionResult<ContentInfo> PutContent(ContentInfo content, string contentId)
        {
            try
            {
                if (content == null) { return BadRequest("ERROR - Invalid content. Content can not be empty."); }

                Data.Content contentData = _dbContext.Content.Where(c => c.Id == contentId).FirstOrDefault();
                if (contentData == null) { return BadRequest("ERROR - Content not found."); }

                contentData.Title = content.title;
                contentData.Description = content.description;
                contentData.Link = content.link;
                contentData.Department = content.department;
                contentData.Grades = string.Join(',', content.grades);
                contentData.Authors = string.Join(',', content.authors);
                contentData.LicenseTypes = string.Join(',', content.licenseTypes);

                List<ContentTag> contentTags = _dbContext.ContentTag.Where(ct => ct.IdContent == contentData.Id).ToList();
                _dbContext.ContentTag.RemoveRange(contentTags);

                foreach (TagInfo tagInfo in content.tags)
                {
                    Tag tag = _dbContext.Tags.Where(t => t.Id == tagInfo.id && t.UserId == tagInfo.userid).FirstOrDefault();
                    if (tag == null)
                    {
                        tag = new Tag()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = tagInfo.name
                        };
                        _dbContext.Tags.Add(tag);
                    }
                    ContentTag contentTag = _dbContext.ContentTag.Where(ct => ct.IdContent == contentData.Id && ct.IdTag == tag.Id).FirstOrDefault();
                    if (contentTag == null)
                    {
                        contentTag = new ContentTag()
                        {
                            IdContent = contentData.Id,
                            IdTag = tag.Id
                        };
                        _dbContext.ContentTag.Add(contentTag);
                    }
                }

                _dbContext.SaveChanges();

                return Ok(content);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
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
        [Route(TAG_PATH)]
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


        [HttpGet]
        [Route(SEARCH_PATH)]
        public ActionResult<List<ContentInfo>> SearchContents(int pageSize, int page, string search)
        {
            try
            {
                List<Data.Content> contents = new List<Data.Content>();
                if (string.IsNullOrEmpty(search))
                {
                    contents = _dbContext.Content.ToList();
                }
                else
                {
                    contents = _dbContext.Content.Where(c => c.Title.Contains(search) ||
                                                    c.Description.Contains(search) ||
                                                    c.Authors.Contains(search) ||
                                                    c.Grades.Contains(search) ||
                                                    c.Department.Contains(search))
                                                    .ToList();

                    List<Tag> searchIntags = _dbContext.Tags.Where(t => t.Name.Contains(search)).ToList();
                    foreach (Tag tag in searchIntags)
                    {
                        List<Data.Content> contentsByTag = (from con in _dbContext.Content
                                                            join contag in _dbContext.ContentTag
                                                              on con.Id equals contag.IdContent
                                                            where contag.IdTag == tag.Id
                                                            select con)
                                                        .Distinct()
                                                        .ToList();
                        contents.AddRange(contentsByTag);
                    }
                }
                contents = contents.Distinct().Skip(pageSize * page).Take(pageSize).OrderBy(c => c.Title).ToList();

                List<ContentInfo> contentsInfo = new List<ContentInfo>();
                foreach (Data.Content content in contents)
                {
                    List<Tag> tags = (from contag in _dbContext.ContentTag
                                      join tag in _dbContext.Tags
                                          on contag.IdTag equals tag.Id
                                      where contag.IdContent == content.Id
                                      select tag).ToList();

                    List<TagInfo> tagsInfo = new List<TagInfo>();
                    foreach (Tag tag in tags)
                    {
                        tagsInfo.Add(new TagInfo(tag.Id, tag.Name, tag.UserId, tag.UploadDate));
                    }

                    contentsInfo.Add(new ContentInfo(content.Id,
                                                        content.Title,
                                                        content.Description,
                                                        content.Link,
                                                        content.Department,
                                                        content.Grades != null ? content.Grades.Split(',') : new string[] { },
                                                        content.Authors != null ? content.Authors.Split(',') : new string[] { },
                                                        content.LicenseTypes != null ? content.LicenseTypes.Split(',') : new string[] { },
                                                        tagsInfo,
                                                        content.UserId,
                                                        content.UploadDate));
                }


                return Ok(contentsInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
            }
        }

        [HttpPost]
        [Route(UPLOAD_PATH)]
        public async Task<ActionResult<List<ContentInfo>>> UploadContents(string userid, IFormFileCollection files)
        {
            List<ContentInfo> returnedContents = new List<ContentInfo>();
            try
            {
                List<ContentInfo> contents = new List<ContentInfo>();
                foreach (IFormFile file in files)
                {
                    if (Path.GetExtension(file.FileName).ToLower() != ".xls" && Path.GetExtension(file.FileName).ToLower() != ".xlsx")
                    {
                        _logger.LogError($"ERROR - Unsoported file extension from file '{file.FileName}' uploaded by user: '{userid}'");
                        continue;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        try
                        {
                             file.CopyTo(ms);
                        }
                        catch (System.Exception)
                        {
                             _logger.LogError($"ERROR - Failure copying file content if uploading it '{file.FileName}' uploaded by user: '{userid}'");
                            continue;
                        }

                        XLWorkbook workbook = new XLWorkbook(ms);
                        IXLWorksheet sheet = workbook.Worksheet(1);

                        int rows = sheet.RowsUsed().Count();
                        for (int i = 1; i <= rows; i++)
                        {
                            IXLRow row = sheet.Row(i);

                            string title = row.Cell(1).GetValue<string>();
                            if (string.IsNullOrEmpty(title))
                            {
                                _logger.LogError($"ERROR - Unsoported format file {file.FileName} uploaded by user: ");
                                continue;
                            }
                            string link = row.Cell(2).GetValue<string>();
                            if (string.IsNullOrEmpty(link))
                            {
                                _logger.LogError($"ERROR - Unsoported format file {file.FileName} uploaded by user: ");
                                continue;
                            }
                            string description = row.Cell(3).GetValue<string>();
                            string[] authors = row.Cell(4).GetValue<string>().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            string[] grades = row.Cell(5).GetValue<string>().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            string[] tags = row.Cell(6).GetValue<string>().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            List<TagInfo> tagsInfo = new List<TagInfo>();
                            foreach (string tag in tags)
                            {
                                tagsInfo.Add(new TagInfo(Guid.NewGuid().ToString(), tag, userid, DateTime.Now));
                            }

                            contents.Add(new ContentInfo(Guid.NewGuid().ToString(),
                                                            title,
                                                            description,
                                                            link,
                                                            string.Empty,
                                                            grades != null ? grades : new string[] { },
                                                            authors != null ? authors : new string[] { },
                                                            new string[] { },
                                                            tagsInfo,
                                                            userid,
                                                            DateTime.Now));
                        }

                        foreach (ContentInfo content in contents)
                        {
                            returnedContents.Add(await _content.PostContent(content));
                        }
                    }
                }
                return Ok(returnedContents);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR - Internal error. {e.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR - Internal error. {e.Message}");
            }
        }


    }
}