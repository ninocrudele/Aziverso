using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevScope.Framework.Common.Utils;
using Newtonsoft.Json;

namespace PowerBIRealTime
{
    class Program
    {
        static void Main(string[] args)
        {


            var dataDomain10 = new List<dynamic>();
            dynamic dataObject10 = new ExpandoObject();
            dataObject10.Guid = Guid.NewGuid().ToString();
            dataObject10.Source = "Milan,Italy";
            dataObject10.Target = "London,UK";
            dataObject10.Type = "File";
            dataObject10.Events = 1;
            dataDomain10.Add(dataObject10);

            PBIProvider pbiProvider = new PBIProvider(dataDomain10, "MultiLocations123");
            pbiProvider.ClearTable(dataDomain10);

            Random rnd = new Random();
            while (true)
            {
               



                var dataDomain = new List<dynamic>();
                dynamic dataObject = new ExpandoObject();
                dataObject.Guid = Guid.NewGuid().ToString();
                dataObject.Source = "Londra";
                dataObject.Target = "London";
                dataObject.Type = "File";
                dataObject.Events = rnd.Next(1,5);
                dataDomain.Add(dataObject);
                pbiProvider.SendData(dataDomain);


                var dataDomain1 = new List<dynamic>();
                dynamic dataObject1 = new ExpandoObject();
                dataObject1.Guid = Guid.NewGuid().ToString();
                dataObject1.Source = "Milano";
                dataObject1.Target = "London";
                dataObject1.Type = "File";
                dataObject1.Events = rnd.Next(1, 5); 
                dataDomain1.Add(dataObject1);
                pbiProvider.SendData(dataDomain1);

                //var dataDomain2 = new List<dynamic>();
                //dynamic dataObject2 = new ExpandoObject();
                //dataObject2.Guid = Guid.NewGuid().ToString();
                //dataObject2.Source = "London,UK";
                //dataObject2.Target = "Madrid,Spain";
                //dataObject2.Type = "File";
                //dataObject2.Events = rnd.Next(1, 10);
                //dataDomain2.Add(dataObject2);
                //pbiProvider.SendData(dataDomain2);

                //var dataDomain3 = new List<dynamic>();
                //dynamic dataObject3 = new ExpandoObject();
                //dataObject3.Guid = Guid.NewGuid().ToString();
                //dataObject3.Source = "Rome,Italy";
                //dataObject3.Target = "London,UK";
                //dataObject3.Type = "SQL";
                //dataObject3.Events = rnd.Next(1, 10); ;
                //dataDomain3.Add(dataObject3);
                //pbiProvider.SendData(dataDomain3);

                Console.WriteLine("Data sent " + DateTime.Now);
                //Thread.Sleep(5000);


            }
        

        }
    }
}
