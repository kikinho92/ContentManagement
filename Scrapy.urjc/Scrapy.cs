using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OpenQA.Selenium;
using Library.Common;
using System.Text;

namespace Scrapy.urjc
{
    public class Scrapy
    {
        private IWebDriver _driver;

        private readonly string CONNECTION_STRING = "Data Source=.\\SQLEXPRESS;Initial Catalog=CONTENT_MANAGEMENT;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
        private readonly string URJC_USER = "060501d8-0e65-4ba5-8713-e1c929241cac";

        private record ContentInfo(string id, string title, string description, string link, string department, string[] grades, string[] authors, string[] licenseTypes, List<TagInfo> tags, string userid, DateTime? uploadDate);
        private record TagInfo(string id, string name, string userid, DateTime? uploadDate);

        public Scrapy(IWebDriver driver)
        {
            _driver = driver;
        }

        public string[] GetCoursesUrls(string initalUrl)
        {
            List<string> coursesUrls = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(initalUrl)) return null;

                _driver.Navigate().GoToUrl(initalUrl);

                bool endOfPages = false;
                do
                {
                    IList<IWebElement> anchorUrl = _driver.FindElements(By.ClassName("k2ReadMore"));
                    foreach (IWebElement anchor in anchorUrl)
                    {
                        string url = anchor.GetAttribute("href");
                        coursesUrls.Add(url);

                        Log.Write(Program.SERVICE_TAG, Log.LogLevel.Info, $"Course url scraped: {url}");
                    }

                    IWebElement nextLiPage = _driver.FindElement(By.ClassName("pagination-next"));
                    try
                    {
                        IWebElement nextAnchorPage = nextLiPage.FindElement(By.TagName("a"));
                          string nextPage = nextAnchorPage.GetAttribute("href");
                        _driver.Navigate().GoToUrl(nextPage);
                    }
                    catch (System.Exception)
                    {
                        endOfPages = true;
                    }
                } while (!endOfPages);
            }
            catch (System.Exception e)
            {
                Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"{e.Message}");
            }
            return coursesUrls.ToArray();
        }

        public int GetCourse(string url)
        {
            try
            {
                 if (string.IsNullOrEmpty(url)) return 1;

                _driver.Navigate().GoToUrl(url);

                string title = _driver.FindElement(By.ClassName("itemTitle")).Text;

                List<string> tags = new List<string>();
                tags.Add(_driver.FindElement(By.ClassName("itemCategory")).FindElement(By.TagName("a")).Text);

                IWebElement liLink = _driver.FindElement(By.ClassName("aliasEnlaceAsignatura"));
                string link = liLink.FindElement(By.TagName("a")).GetAttribute("href");

                List<string> authors = new List<string>();
                IWebElement liAuthors = _driver.FindElement(By.ClassName("aliasAutor"));
                authors.AddRange(liAuthors.FindElement(By.ClassName("itemExtraFieldsValue")).Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

                List<string> grades = new List<string>();
                IList<IWebElement> anchorGrades = _driver.FindElement(By.ClassName("itemTags")).FindElements(By.TagName("a"));
                foreach (IWebElement grade in anchorGrades)
                {
                    grades.Add(grade.Text);
                }
                
                List<TagInfo> tagInfos = new List<TagInfo>();
                foreach (string tag in tags)
                {
                    tagInfos.Add(new TagInfo(Guid.NewGuid().ToString(), tag, URJC_USER, DateTime.Now));
                }

                ContentInfo content = new ContentInfo(
                    Guid.NewGuid().ToString(),
                    title,
                    string.Empty,
                    link,
                    string.Empty,
                    grades.ToArray(),
                    authors.ToArray(),
                    new string[] { },
                    tagInfos,
                    URJC_USER,
                    DateTime.Now
                );
                PostContent(content);

                return 0;
            }
             catch (System.Exception e)
            {
                Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, $"{e.Message}");
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
            return str.Replace("Titulación:", "").Replace("'", "");
        }
    }
}