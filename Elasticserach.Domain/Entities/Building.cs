using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Elasticserach.Domain.Interfaces
{
    public class Building
    {
        /// <summary>
        /// Id formed by Record Id and Type
        /// </summary>
        public string UniqueId { get {
                return $"{Type}-{Id}";
            } }
        /// <summary>
        /// Original Record Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Property/Management Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Property/Management Market
        /// </summary>
        public string Market { get; set; }
        /// <summary>
        /// Property/Management State
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Property's City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Property's Formername
        /// </summary>
        public string Formername { get; set; }
        /// <summary>
        /// Type of Record P= Property, M = Management
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Property's Street Address
        /// </summary>
        public string StreetAddress { get; set; }
        /// <summary>
        /// Property Latitude
        /// </summary>
        public decimal Longitud{ get; set; }
        /// <summary>
        /// Property Longitude
        /// </summary>
        public decimal Latitude { get; set; }
    }
}
