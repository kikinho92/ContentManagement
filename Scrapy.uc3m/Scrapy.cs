using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OpenQA.Selenium;
using Library.Common;
using System.Text;

namespace Scrapy.uc3m
{
    public class Scrapy
    {
        private IWebDriver _driver;

        private readonly string CONNECTION_STRING = "Data Source=.\\SQLEXPRESS;Initial Catalog=CONTENT_MANAGEMENT;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
        private readonly string UC3M_USER = "1ba978fa-3afb-4cf2-b2bc-9a86673f7ce9";

        private record ContentInfo(string id, string title, string description, string link, string department, string[] grades, string[] authors, string[] licenseTypes, List<TagInfo> tags, string userid, DateTime? uploadDate);
        private record TagInfo(string id, string name, string userid, DateTime? uploadDate);

        public Scrapy(IWebDriver driver)
        {
            _driver = driver;
        }

        public string[] GetCategoriesUrls(string rootUrl)
        {
            List<string> categoriesUrls = new List<string>();

            try
            {
                if (_driver == null) return null;

                _driver.Navigate().GoToUrl(rootUrl);

                IList<IWebElement> divsCategories = _driver.FindElements(By.ClassName("category"));
                foreach (IWebElement category in divsCategories)
                {
                    IWebElement anchorSubCategory = category.FindElement(By.TagName("a"));
                    string url = anchorSubCategory.GetAttribute("href");
                    categoriesUrls.Add(url);

                    Log.Write(Program.SERVICE_TAG, Log.LogLevel.Info, $"Category url scraped: {url}");
                }
            }
            catch (System.Exception e)
            {
                Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"{e.Message}");
            }
            return categoriesUrls.ToArray();
        }

        public string[] GetSubCategoriesUrlsByCategory(string categorieUrl)
        {
            List<string> subCategoriesUrls = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(categorieUrl)) return null;

                _driver.Navigate().GoToUrl(categorieUrl);

                IList<IWebElement> divsSubCategories = _driver.FindElements(By.ClassName("category"));
                foreach (IWebElement category in divsSubCategories)
                {
                    IWebElement anchorSubCategory = category.FindElement(By.TagName("a"));
                    string url = anchorSubCategory.GetAttribute("href");
                    subCategoriesUrls.Add(url);

                    Log.Write(Program.SERVICE_TAG, Log.LogLevel.Info, $"Subcategory url scraped: {url}");
                }
            }
            catch (System.Exception e)
            {
                Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"{e.Message}");
            }
            return subCategoriesUrls.ToArray();
        }

        public string[] GetCoursesUrlsBySubcategory(string subCategorieUrl)
        {
            List<string> coursesUrls = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(subCategorieUrl)) return null;

                _driver.Navigate().GoToUrl(subCategorieUrl);

                IList<IWebElement> divsCourses = _driver.FindElements(By.ClassName("info"));
                foreach (IWebElement course in divsCourses)
                {
                    IWebElement anchorSubCategory = course.FindElement(By.TagName("a"));
                    string url = anchorSubCategory.GetAttribute("href");
                    coursesUrls.Add(url);

                    Log.Write(Program.SERVICE_TAG, Log.LogLevel.Info, $"Course url scraped: {url}");
                }
            }
            catch (System.Exception e)
            {
                Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"{e.Message}");
            }
            return coursesUrls.ToArray();
        }

        public int GetCourse(string courseUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(courseUrl)) return 1;

                _driver.Navigate().GoToUrl(courseUrl);

                IWebElement breadcrumb = _driver.FindElement(By.TagName("ol"));
                IList<IWebElement> contentCategories = breadcrumb.FindElements(By.CssSelector("span[itemprop='title']"));
                List<string> tags = new List<string>();
                if (contentCategories.Count > 3)
                {
                    tags.Add(contentCategories[contentCategories.Count - 2].Text);
                    tags.Add(contentCategories[contentCategories.Count - 3].Text);
                }

                IList<IWebElement> tds = _driver.FindElements(By.TagName("td"));
                IWebElement tdContent = null;
                if (tds.Count == 1)
                {
                    tdContent = tds[0];
                }
                else if (tds.Count == 2)
                {
                    tdContent = tds[1];
                }
                else
                {
                    Log.Write(Program.SERVICE_TAG, Log.LogLevel.Warning, $"Course: {courseUrl} DISCARTED due to not standar format");
                    return 1;
                }

                string title = string.Empty;
                try
                {
                    title = tdContent.FindElement(By.TagName("strong")).Text;
                }
                catch (System.Exception)
                {
                    try
                    {
                        title = tdContent.FindElement(By.TagName("b")).Text;
                    }
                    catch (System.Exception)
                    {
                        Log.Write(Program.SERVICE_TAG, Log.LogLevel.Warning, $"Course: {courseUrl} DISCARTED due to not standar format");
                        return 1;
                    }
                }

                IList<IWebElement> divsContent = tdContent.FindElements(By.TagName("div"));
                int indexAuthor;
                int indexContent;
                if(divsContent.Count == 1)
                {
                    indexAuthor = 0;
                    indexContent = 0;
                }
                else if(divsContent.Count == 2)
                {
                    indexAuthor = 0;
                    indexContent = 1;
                }
                else if (divsContent.Count == 3)
                {
                    indexAuthor = 0;
                    indexContent = 2;
                }
                else
                {
                    Log.Write(Program.SERVICE_TAG, Log.LogLevel.Warning, $"Course: {courseUrl} DISCARTED due to not standar format");
                    return 1;
                }

                List<string> authors = new List<string>();
                try
                {
                    authors.AddRange(divsContent[indexAuthor].FindElement(By.TagName("span")).FindElement(By.TagName("span")).FindElement(By.TagName("span")).Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                }
                catch (System.Exception)
                {
                    try
                    {
                        authors.AddRange(divsContent[indexAuthor].FindElement(By.TagName("p")).FindElement(By.TagName("span")).Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    catch (System.Exception)
                    {
                        Log.Write(Program.SERVICE_TAG, Log.LogLevel.Warning, $"Course: {courseUrl} DISCARTED due to not standar format");
                        return 1;
                    }
                }

                IList<IWebElement> paragraphContent = divsContent[indexContent].FindElements(By.TagName("p"));
                string department = string.Empty;
                List<string> grades = new List<string>();
                if (paragraphContent.Count == 5)
                {
                    authors.AddRange(paragraphContent[0].FindElement(By.TagName("span")).Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

                    department = paragraphContent[1].FindElement(By.TagName("span")).FindElements(By.TagName("span"))[0].Text;
                    grades.AddRange(paragraphContent[3].FindElement(By.TagName("span")).Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (paragraphContent.Count == 4)
                {
                    department = paragraphContent[0].FindElement(By.TagName("span")).Text;
                    grades.AddRange(paragraphContent[2].FindElement(By.TagName("span")).Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                }

                List<TagInfo> tagInfos = new List<TagInfo>();
                foreach (string tag in tags)
                {
                    tagInfos.Add(new TagInfo(Guid.NewGuid().ToString(), tag, UC3M_USER, DateTime.Now));
                }

                ContentInfo content = new ContentInfo(
                    Guid.NewGuid().ToString(),
                    title,
                    string.Empty,
                    courseUrl,
                    department,
                    grades.ToArray(),
                    authors.ToArray(),
                    new string[] { },
                    tagInfos,
                    UC3M_USER,
                    DateTime.Now
                );
                PostContent(content);
                return 0;

            }
            catch (System.Exception e)
            {
                Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"Course: {courseUrl} DISCARTED {e.Message}");
                return 1;
            }
        }

        private void PostContent(ContentInfo content)
        {
            if (content == null) { Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"Invalid content scraped."); }

            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                SqlCommand command = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    command.Connection = conn;
                    command.Transaction = transaction;

                    string query = string.Empty;
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

                    query = $"INSERT INTO Content VALUES(" +
                        $"'{newContent.Id}'," +
                        $"'{RemoveSpecialCharacters(newContent.Title)}'," +
                        $"'{RemoveSpecialCharacters(newContent.Description)}'," +
                        $"'{RemoveSpecialCharacters(newContent.Link)}'," +
                        $"'{RemoveSpecialCharacters(newContent.Authors)}'," +
                        $"'{RemoveSpecialCharacters(newContent.LicenseTypes)}'," +
                        $"'{newContent.UserId}'," +
                        $"'{newContent.UploadDate}'," +
                        $"'{RemoveSpecialCharacters(newContent.Department)}'," +
                        $"'{RemoveSpecialCharacters(newContent.Grades)}'" +
                    ")";
                    command.CommandText = query;
                    command.ExecuteNonQuery();

                    foreach (TagInfo tagInfo in content.tags)
                    {
                        query = $"SELECT Id FROM Tags WHERE Name = '{tagInfo.name}' AND UserId = '{tagInfo.userid}'";
                        command.CommandText = query;

                        string tagId = string.Empty;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tagId = reader[0].ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(tagId))
                        {
                            tagId = Guid.NewGuid().ToString();
                            Data.Tag newTag = new Data.Tag()
                            {
                                Id = tagId,
                                Name = tagInfo.name,
                                UserId = tagInfo.userid,
                                UploadDate = DateTime.Now
                            };
                            query = $"INSERT INTO Tags VALUES(" +
                                $"'{newTag.Id}'," +
                                $"'{newTag.Name}'," +
                                $"'{newTag.UserId}'," +
                                $"'{newTag.UploadDate}'" +
                            ")";
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }

                        query = $"SELECT IdContent, IdTag FROM ContentTag WHERE IdTag = '{tagId}' AND IdContent = '{newContent.Id}'";
                        command.CommandText = query;

                        bool existRelation = false;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            existRelation = reader.HasRows;
                        }

                        if (!existRelation)
                        {
                            Data.ContentTag newContentTag = new Data.ContentTag()
                            {
                                IdContent = newContent.Id,
                                IdTag = tagId
                            };

                            query = @$"INSERT INTO ContentTag VALUES(" +
                               $"'{newContentTag.IdContent}'," +
                               $"'{newContentTag.IdTag}'" +
                           ")";
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();

                    Log.Write(Program.SERVICE_TAG, Log.LogLevel.Info, $"Content {newContent.Link} STORED succesfully.");
                }
                catch (Exception e)
                {
                    Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"Course: {content.link} DISCARTED {e.Message}");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (System.Exception)
                    {
                        Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"{e.Message}");
                    }
                }
            }

        }

        private string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ' || c == '/' || c == ':' || c == ',' || c == '?' || c == '=' || c == '&')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Replace("Titulación:", "");
        }

    }
}