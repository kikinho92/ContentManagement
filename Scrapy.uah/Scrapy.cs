using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OpenQA.Selenium;
using Library.Common;
using System.Text;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace Scrapy.uah
{
    public class Scrapy
    {
        private IWebDriver _driver;

        private readonly string CONNECTION_STRING = "Data Source=.\\SQLEXPRESS;Initial Catalog=CONTENT_MANAGEMENT;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
        private readonly string UAH_USER = "2237dc8e-add9-4103-b4d1-8a6f53228348";

        private record ContentInfo(string id, string title, string description, string link, string department, string[] grades, string[] authors, string[] licenseTypes, List<TagInfo> tags, string userid, DateTime? uploadDate);
        private record TagInfo(string id, string name, string userid, DateTime? uploadDate);

        public Scrapy(IWebDriver driver)
        {
            _driver = driver;
        }

        public string[] GetCoursesUrls(string initialUrl)
        {
            List<string> coursesUrls = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(initialUrl)) return null;

                _driver.Navigate().GoToUrl(initialUrl);

                ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                Thread.Sleep(500);

                IList<IWebElement> courses = _driver.FindElements(By.ClassName("wrapper"));
                foreach (IWebElement course in courses)
                {
                    string url = course.FindElement(By.TagName("a")).GetAttribute("href");
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

        public int GetCourse(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return 1;

                _driver.Navigate().GoToUrl(url);

                string title = _driver.FindElement(By.Id("channel_title")).Text;

                List<string> tags = new List<string>();
                try
                {
                    Thread.Sleep(500);
                    IList<IWebElement> tagsAnchor = _driver.FindElement(By.Id("tagsArea")).FindElements(By.TagName("a"));
                    foreach (IWebElement tagAnchor in tagsAnchor)
                    {
                        tags.Add(tagAnchor.Text.Trim());
                    }
                }
                catch (System.Exception)
                { 
                }

                string description = string.Empty;
                try
                {
                    description = _driver.FindElement(By.Id("channel-desc")).FindElement(By.ClassName("js-description")).GetAttribute("innerText");
                }
                catch (System.Exception)
                {
                }

                ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                Thread.Sleep(500);

                List<string> authors = new List<string>();
                IList<IWebElement> divAuthors = _driver.FindElement(By.Id("functionaries-Gestores")).FindElements(By.ClassName("elem"));
                foreach (IWebElement div in divAuthors)
                {
                    authors.Add(div.GetAttribute("innerText").ToString().Trim());
                } 

                //List<string> authors = GetAuthors();

                List<TagInfo> tagInfos = new List<TagInfo>();
                foreach (string tag in tags)
                {
                    tagInfos.Add(new TagInfo(Guid.NewGuid().ToString(), tag, UAH_USER, DateTime.Now));
                }

                ContentInfo content = new ContentInfo(
                    Guid.NewGuid().ToString(),
                    title,
                    string.Empty,
                    url,
                    string.Empty,
                    new string[] { },
                    authors.ToArray(),
                    new string[] { },
                    tagInfos,
                    UAH_USER,
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

        private List<string> GetAuthors()
        {
            List<string> authors = new List<string>();

            IList<IWebElement> liMediaList = _driver.FindElement(By.Id("gallery")).FindElements(By.TagName("li"));
            foreach (IWebElement li in liMediaList)
            {
                string urlMedia = li.FindElement(By.ClassName("item_link")).GetAttribute("href");

                string author = GetAuthorsInMedia(urlMedia);
                if(string.IsNullOrEmpty(author)) continue;

                if(!authors.Contains(author)) authors.Add(author);
            }

            return authors;
        }

        private string GetAuthorsInMedia(string urlMedia)
        {
            if (string.IsNullOrEmpty(urlMedia)) return null;

            IWebDriver _driverAux = new ChromeDriver();
            string author = string.Empty;

            try
            {
                _driverAux.Navigate().GoToUrl(urlMedia);
                author = _driverAux.FindElement(By.ClassName("userLink")).FindElement(By.TagName("span")).Text;
            }
            catch (System.Exception e)
            {
                Log.Write(Program.SERVICE_TAG, Log.LogLevel.Error, e.Message);
            }
            finally
            {
                _driverAux.Close();
            }
            return author;
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