using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestService
{
    class ContractClass:iContractSample
    {
        public string GetMessage(Stream streamdata)
        {
            StreamReader reader = new StreamReader(streamdata);
            string result = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            Console.WriteLine(result);
            return result;
            
            return "{json}";
        }
    }
}
