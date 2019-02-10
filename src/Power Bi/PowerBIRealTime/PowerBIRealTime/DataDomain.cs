using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIRealTime
{

    class DataDomain : IEnumerable<DataDomainRow>
    {
        private List<DataDomainRow> listData = new List<DataDomainRow>();

        public DataDomainRow this[int index]
        {
            get { return listData[index]; }
            set { listData.Insert(index, value); }
        }

        public IEnumerator<DataDomainRow> GetEnumerator()
        {
            return listData.GetEnumerator();
        }

        public void Add(DataDomainRow dataDomainRow)
        {
            listData.Add(dataDomainRow);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [DataContract]
    public class DataDomainRow
    {
        public DataDomainRow(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public object Value { get; set; }
    }


}



