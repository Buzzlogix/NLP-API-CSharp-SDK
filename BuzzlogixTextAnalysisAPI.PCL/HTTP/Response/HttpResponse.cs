using System;
using System.Collections.Generic;
using System.IO;

namespace com.buzzlogix.Http.Response
{
    public class HttpResponse
    {
        /// <summary>
        /// HTTP Status code of the http response
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// Headers of the http response
        /// </summary>
        public Dictionary<String,String> Headers { get; set; }

        /// <summary>
        /// Stream of the body
        /// </summary>
        public Stream RawBody { get; set; }
    }
}
