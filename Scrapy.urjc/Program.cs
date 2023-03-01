using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using Library.Common;
using System.Collections.Generic;

namespace Scrapy.urjc
{
    public class Program
    {

        public static readonly string SERVICE_TAG = "Scrapy.urjc";
        private static readonly string ROOT_URL = "https://online.urjc.es/";
        private static readonly string INITIAL_URL_PATH = "es/para-futuros-estudiantes/asignaturas-en-abierto";

        public static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            try
            {
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "*******************************************************");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**                                                   **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**           STARTING TO SCRAP URJC URLS             **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "**                                                   **");
                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "*******************************************************");

                Scrapy scrapyUrls = new Scrapy(driver);

                Log.Write(SERVICE_TAG, Log.LogLevel.Info, "* Starting to scrap URJC urls of categories *");

                string[] coursesUrls = scrapyUrls.GetCoursesUrls($"{ROOT_URL}{INITIAL_URL_PATH}");

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
