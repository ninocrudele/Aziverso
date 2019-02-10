using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIRealTime
{
    class PBIProvider
    {
        private PowerBIService powerBiService { get; set; }
        private string authToken { get; set; }
        private PBIDataSet dataSet { get; set; }


        #region DynamicData
        public PBIProvider(List<dynamic> dataDomain, string dataSetName)
        {
            powerBiService = new PowerBIService();

            authToken = powerBiService.GetAccessToken().Result;
            DataTable dt = PBITable.CreateDynamicDataTable(dataDomain);
            dt.TableName = "TableName";
            var pbiTable = PBITable.FromDataTable(dt);
            dataSet = powerBiService.GetDataSets(authToken).Result.FirstOrDefault(s => s.Name == dataSetName);

            if (dataSet == null)
            {
                dataSet = new PBIDataSet();
                dataSet.Name = dataSetName;
                dataSet.Tables.Add(pbiTable);

                powerBiService.CreateDataSet(authToken, dataSet, false).Wait();
            }
        }

        public void SendData(List<dynamic> dataDomain)
        {
            powerBiService.AddTableRows(authToken, dataSet.Id, "TableName", dataDomain, 1000).Wait();
        }

        public void ClearTable(List<dynamic> dataDomain)
        {
            powerBiService.ClearTable(authToken, dataSet.Id, "TableName").Wait();
        }

        #endregion

    }
}
