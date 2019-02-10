using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerBIRealTime
{
    public class PBIDataSet
    {
        public PBIDataSet()
        {
            this.Tables = new List<PBITable>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string DefaultRetentionPolicy { get; set; }

        public List<PBITable> Tables { get; set; }

    }

    
}