using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestService
{
    class Program
    {
        static void Main(string[] args)
        {
                    WebServiceHost engineHost;
                    engineHost = new WebServiceHost(typeof(ContractClass),
            new Uri("http://localhost:8000"));
                    engineHost.AddServiceEndpoint(typeof(iContractSample), new WebHttpBinding(),
                        "Demo");
                    var stp = engineHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                    stp.HttpHelpPageEnabled = false;
                    engineHost.Open();
            Console.WriteLine("Service running...");
            Thread.Sleep(Timeout.Infinite);

        }
    }
}
