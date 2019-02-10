// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using System;
using System.Runtime.Serialization;

#endregion

namespace AOC.AzureCostsLibrary.API.Contracts
{
    /// This is the Azure property contract, I use the reflection to get name and value, it is used to retreive all properties from a contract class
    /// </summary>
    [DataContract]
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class AzureAPIProperty : Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public AzureAPIProperty(string columnName, string description, bool complex)
        {
            ColumnName = columnName;
            Description = description;
            Complex = complex;
        }

        /// <summary>
        ///     Property name
        /// </summary>
        [DataMember]
        public string ColumnName { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        ///     If is a complex structure
        /// </summary>
        [DataMember]
        public bool Complex { get; set; }
    }
}