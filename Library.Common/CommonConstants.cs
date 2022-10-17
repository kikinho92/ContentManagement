using System;

namespace Library.Common
{
    public class CommonConstants
    {
        /// <summary>
        /// Service base URL from localhost.
        /// Used by SDK constructor from services accesing other services internally.
        /// </summary>
        public static readonly string SERVICE_LOCAL_URL_AUTH = "http://127.0.0.1:8001";
        public static readonly string SERVICE_LOCAL_URL_USER = "http://127.0.0.1:8002";
        public static readonly string SERVICE_LOCAL_URL_CONTENT = "http://127.0.0.1:8003";
        public static readonly string SERVICE_LOCAL_URL_INTEGRATION = "http://127.0.0.1:8004";
    }
}
