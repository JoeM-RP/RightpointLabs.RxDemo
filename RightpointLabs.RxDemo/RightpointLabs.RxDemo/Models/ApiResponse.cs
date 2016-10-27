using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace RightpointLabs.RxDemo.Models
{
    [XmlRoot(ElementName = "ctatt")]
    public class ApiResponse
    {
        /// <summary>
        /// Shows time when response was generated in format: yyyyMMdd HH:mm:ss (24-hour format, time local to Chicago) 
        /// </summary>
        [XmlElement(ElementName = "tmst")]
        public string TimestampString { get; set; }

        public DateTime Timestamp
            => DateTime.ParseExact(TimestampString, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);

        /// <summary>
        /// Numeric error code (see appendices) 
        /// </summary>
        [XmlElement(ElementName = "errCd")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Textual error description/message (see appendices) 
        /// </summary>
        [XmlElement(ElementName = "errNm")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Container element (one per individual prediction) 
        /// </summary>
        [XmlElement(ElementName = "eta")]
        public List<Arrival> ArrivalList { get; set; }
    }
}
