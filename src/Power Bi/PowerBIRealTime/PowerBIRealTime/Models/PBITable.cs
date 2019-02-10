using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PowerBIRealTime
{
    public class PBITable
    {
        public PBITable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.Name = name;
            this.Columns = new List<PBIColumn>();
            this.Rows = new List<dynamic>();
        }

        public string Name { get; set; }

        public List<PBIColumn> Columns { get; set; }

        [JsonIgnore]
        public List<dynamic> Rows { get; set; }

        public static PBITable FromDataTable(DataTable dataTable, bool onlySchema = false)
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable");

            var pbiTable = new PBITable(dataTable.TableName);

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                pbiTable.Columns.Add(new PBIColumn(dataColumn.ColumnName, dataColumn.DataType));
            }

            if (!onlySchema)
            {
                foreach (DataRow dataRow in dataTable.AsEnumerable())
                {
                    dynamic dyn = new ExpandoObject();

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        var dic = (IDictionary<string, object>)dyn;

                        var obj = dataRow[column];

                        dic[column.ColumnName] = (obj != DBNull.Value ? obj : null);
                    }

                    pbiTable.Rows.Add(dyn);
                }
            }

            return pbiTable;
        }

        public static DataTable CreateDynamicDataTable<T>(IEnumerable<T> list)
        {
            IDictionary<String, Object> firstRecord = (IDictionary<String, Object>)list.First();
            DataTable dataTable = new DataTable();

            foreach (var field in firstRecord)
            {
                Type r = field.Value.GetType();
                dataTable.Columns.Add(new DataColumn(field.Key, Nullable.GetUnderlyingType(field.Value.GetType()) ?? field.Value.GetType()));
            }

            foreach (T entity in list)
            {
                IDictionary<String, Object> record = (IDictionary<String, Object>)entity;
                DataRow dr = dataTable.NewRow();
                foreach (var field in record)
                {
                    dr[field.Key] = field.Value;
                }
            }
            return dataTable;
        }

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }


        public static DataTable ObjectToData(object o)
        {
            DataTable dt = new DataTable("OutputData");

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            o.GetType().GetProperties().ToList().ForEach(f =>
            {
                try
                {
                    f.GetValue(o, null);
                    dt.Columns.Add(f.Name, f.PropertyType);
                    dt.Rows[0][f.Name] = f.GetValue(o, null);
                }
                catch { }
            });
            return dt;
        }
    }

    public class PBIColumn
    {
        public PBIColumn(string name, Type dataType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            this.Name = name;

            switch (dataType.Name)
            {
                case "Int32":
                case "Int64":
                    this.DataType = "Int64";
                    break;
                case "Double":
                    this.DataType = "Double";
                    break;
                case "Boolean":
                    this.DataType = "bool";
                    break;
                case "DateTime":
                    this.DataType = "DateTime";
                    break;
                case "String":
                    this.DataType = "string";
                    break;
                default:
                    throw new ApplicationException(string.Format("Type {0} isn't supporter by the PowerBI.", dataType.FullName));
            }
        }

 
        public PBIColumn(string name, string dataType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(dataType))
                throw new ArgumentNullException("dataType");

            this.Name = name;
            this.DataType = dataType;
        }

        public string Name { get; set; }

        public string DataType { get; set; }
    }
}