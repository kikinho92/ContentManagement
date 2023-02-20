using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using Library.Common;
using System.Collections.Generic;

namespace Scrapy.uc3m
{
    public class Program
    {
        public static readonly string SERVICE_TAG = "Scrapy.u3m";
        private static readonly string ROOT_URL = "https://ocw.uc3m.es/";
        private static readonly string INITIAL_URL_PATH = "course/index.php";

        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            try
            {
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "*******************************************************");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**                                                   **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**           STARTING TO SCRAP UC3M URLS             **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**                                                   **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "*******************************************************");

                Scrapy scrapyUrls = new Scrapy(driver);

                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "* Starting to scrap UC3M urls of categories *");

                string[] categoriesUrl = scrapyUrls.GetCategoriesUrls($"{ROOT_URL}{INITIAL_URL_PATH}");

                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "* Starting to scrap UC3M urls of sub categories *");

                List<string> subCategoriesUrl = new List<string>();
                List<string> coursesUrl = new List<string>();
                foreach (string categoryUrl in categoriesUrl)
                {
                    subCategoriesUrl.AddRange(scrapyUrls.GetSubCategoriesUrlsByCategory(categoryUrl));
                }

                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "* Starting to scrap UC3M urls of courses *");

                foreach (string subCategory in subCategoriesUrl)
                {
                    coursesUrl.AddRange(scrapyUrls.GetCoursesUrlsBySubcategory(subCategory));
                }

                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "* Starting to scrap UC3M content of courses *");

                int success = 0;
                int disarted = 0;
                foreach (string course in coursesUrl)
                {
                    try
                    {
                        if (course.StartsWith(ROOT_URL))
                        {
                            int result = scrapyUrls.GetCourse(course);
                            if(result == 0) success++; else disarted++;
                        }
                    }
                    catch (System.Exception) { }
                } 
                //scrapyUrls.GetCourse("https://ocw.uc3m.es/course/view.php?id=204");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "* Scrap process FINISHED *");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, $"Total course scraped succesfully: {success}");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, $"Total course scraped discarted: {disarted}");
            }
            catch (System.Exception e)
            {
                Log.Write(SERVICE_TAG, Log.LogLevel.Error, e.Message);
            }
            finally
            {
                driver.Close();
            }
        }
    }
}
