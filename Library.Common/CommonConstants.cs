using System;

namespace Library.Common
{
    public class CommonConstants
    {
        /// <summary>
        /// Service base URL from staging enviroment.
        /// Used by SDK constructor from services accesing other services internally for staging enviroment.
        /// </summary>
        public static readonly string SERVICE_LOCAL_URL_AUTH = "http://10.96.249.12:8001";
        public static readonly string SERVICE_LOCAL_URL_USER = "http://10.96.249.12:8002";
        public static readonly string SERVICE_LOCAL_URL_CONTENT = "http://10.96.249.12:8003";
        public static readonly string SERVICE_LOCAL_URL_INTEGRATION = "http://10.96.249.12:8004";

        /* public static readonly string SERVICE_LOCAL_URL_AUTH = "http://localhost:8001";
        public static readonly string SERVICE_LOCAL_URL_USER = "http://localhost:8002";
        public static readonly string SERVICE_LOCAL_URL_CONTENT = "http://localhost:8003";
        public static readonly string SERVICE_LOCAL_URL_INTEGRATION = "http://localhost:8004"; */
    }
}
