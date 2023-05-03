using System;

namespace Library.Common
{
    public class CommonConstants
    {
        /// <summary>
        /// Service base URL from localhost.
        /// Used by SDK constructor from services accesing other services internally for development enviroment.
        /// </summary>
        public static readonly string SERVICE_LOCAL_URL_AUTH = "http://192.168.1.9:8001";
        public static readonly string SERVICE_LOCAL_URL_USER = "http://192.168.1.9:8002";
        public static readonly string SERVICE_LOCAL_URL_CONTENT = "http://192.168.1.9:8003";
        public static readonly string SERVICE_LOCAL_URL_INTEGRATION = "http://192.168.1.9:8004";
    }

    public class CommonConstantsStaging
    {
        /// <summary>
        /// Service base URL from localhost.
        /// Used by SDK constructor from services accesing other services internally for stating enviromment.
        /// </summary>
        public static readonly string SERVICE_LOCAL_URL_AUTH = "http://10.96.249.12:8001";
        public static readonly string SERVICE_LOCAL_URL_USER = "http://10.96.249.12:8002";
        public static readonly string SERVICE_LOCAL_URL_CONTENT = "http://10.96.249.12:8003";
        public static readonly string SERVICE_LOCAL_URL_INTEGRATION = "http://10.96.249.12:8004";
    }
}
