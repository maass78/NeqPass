using System;
using System.Net;

namespace NeqPass.GUI.Utilities
{
    class TimeoutedWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);
            req.Timeout = 3000;
            return req;
        }
    }
}
