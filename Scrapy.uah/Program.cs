using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using Library.Common;
using System.Collections.Generic;

namespace Scrapy.uah
{
    public class Program
    {
        public static readonly string SERVICE_TAG = "Scrapy.uah";
        private static readonly string ROOT_URL = "https://mediateca.uah.es/";
        private static readonly string INITIAL_URL_PATH = "channels";

        public static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            try
            {
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "*******************************************************");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**                                                   **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**           STARTING TO SCRAP UAH URLS             **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**                                                   **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "*******************************************************");

                Scrapy scrapyUrls = new Scrapy(driver);
                string[] coursesUrls = scrapyUrls.GetCoursesUrls($"{ROOT_URL}{INITIAL_URL_PATH}");

                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "* Starting to scrap UAH urls of channels *");

                int success = 0;
                int disarted = 0;
                foreach (string url in coursesUrls)
                {
                     try
                    {
                        int result = scrapyUrls.GetCourse(url);
                        if(result == 0) success++; else disarted++;
                    }
                    catch (System.Exception) { }
                } 

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
