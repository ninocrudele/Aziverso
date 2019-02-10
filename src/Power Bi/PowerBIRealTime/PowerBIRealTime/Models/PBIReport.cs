using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerBIRealTime
{
    public class PBIReport
    {
        public PBIReport()
        {
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string WebUrl { get; set; }

        public string EmbedUrl { get; set; }

    }
}