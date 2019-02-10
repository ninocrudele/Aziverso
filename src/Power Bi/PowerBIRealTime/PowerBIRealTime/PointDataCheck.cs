using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIRealTime
{
    public class PointDataCheck
    {
        public PointDataCheck(DateTime date, string location, int events)
        {
            this.Date = date;
            this.Location = location;
            this.Events = events;
        }

        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Events { get; set; }
    }
}
