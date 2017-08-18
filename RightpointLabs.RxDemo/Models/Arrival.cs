using System;
using System.Globalization;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace RightpointLabs.RxDemo.Models
{
	/// <summary>
	/// Container element (one per individual prediction) 
	/// </summary>
	[XmlRoot(ElementName = "eta")]
	public class Arrival
	{
		/// <summary>
		/// Numeric GTFS parent station ID which this prediction is for (five digits in 4xxxx range) (matches “mapid” specified by requestor in query) 
		/// </summary>
		[XmlElement(ElementName = "staId")]
		public string StationId { get; set; }

		/// <summary>
		/// Numeric GTFS unique stop ID within station which this prediction is for (five digits in 3xxxx range) 
		/// </summary>
		[XmlElement(ElementName = "stpId")]
		public string StopId { get; set; }

		/// <summary>
		/// Textual proper name of parent station 
		/// </summary>
		[XmlElement(ElementName = "staNm")]
		public string StationName { get; set; }

		/// <summary>
		/// Textual description of platform for which this prediction applies 
		/// </summary>
		[XmlElement(ElementName = "stpDe")]
		public string StopDescription { get; set; }

		/// <summary>
		/// Run number of train being predicted for 
		/// </summary>
		[XmlElement(ElementName = "rn")]
		public string Run { get; set; }

		/// <summary>
		/// Textual, abbreviated route name of train being predicted for (matches GTFS routes) 
		/// </summary>
		[XmlElement(ElementName = "rt")]
		public string Route { get; set; }

		// TODO JM: move this to a behavior
		public Color Shade
		{
			get
			{
				switch (this.Route)
				{
					case ("Brn"):
						return Color.FromHex("964B00");
					case ("Pink"):
						return Color.Pink;
					case ("Grn"):
					case ("Green"):
						return Color.Lime;
					case ("Red"):
						return Color.Red;
					case ("Org"):
						return Color.FromHex("FFA500");
					default:
						return Color.FromHex("800080");
				}
			}
		}

		/// <summary>
		/// GTFS unique stop ID where this train is expected to ultimately end its service run (experimental and supplemental only—see note below) 
		/// </summary>
		[XmlElement(ElementName = "destSt")]
		public string DestinationId { get; set; }

		/// <summary>
		/// Friendly destination description (see note below) 
		/// </summary>
		[XmlElement(ElementName = "destNm")]
		public string DestinationName { get; set; }

		/// <summary>
		/// Numeric train route direction code (see appendices) 
		/// </summary>
		[XmlElement(ElementName = "trDr")]
		public string Direction { get; set; }

		/// <summary>
		/// Date-time format stamp for when the prediction was generated: yyyyMMdd HH:mm:ss (24-hour format, time local to Chicago) 
		/// </summary>
		[XmlElement(ElementName = "prdt")]
		public string PredictionGeneratedTimeString { get; set; }

		public DateTime PredictionGeneratedTime
			=> DateTime.ParseExact(PredictionGeneratedTimeString, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);

		/// <summary>
		/// Date-time format stamp for when a train is expected to arrive/depart: yyyyMMdd HH:mm:ss (24-hour format, time local to Chicago) 
		/// </summary>
		[XmlElement(ElementName = "arrT")]
		public string ArrivalTimeString { get; set; }
		public DateTime ArrivalTime
			=> DateTime.ParseExact(ArrivalTimeString, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);

		public string ETA
		{
			get
			{
				var eta = ArrivalTime - DateTime.Now;

				if (IsDelayed)
					return "Delayed";
				if (eta.Minutes < 1 || IsAproaching)
					return "Due";

				return $"{(eta):mm} min";
			}
		}

		/// <summary>
		/// Indicates that Train Tracker is now declaring “Approaching” or “Due” on site for this train 
		/// </summary>
		[XmlElement(ElementName = "isApp")]
		public bool IsAproaching { get; set; }

		/// <summary>
		/// Boolean flag to indicate whether this is a live prediction or based on schedule in lieu of live data 
		/// </summary>
		[XmlElement(ElementName = "isSch")]
		public bool IsScheduled { get; set; }

		/// <summary>
		/// Boolean flag to indicate whether a train is considered “delayed” in Train Tracker 
		/// </summary>
		[XmlElement(ElementName = "isDly")]
		public bool IsDelayed { get; set; }

		/// <summary>
		/// Boolean flag to indicate whether a potential fault has been detected (see note below) 
		/// </summary>
		[XmlElement(ElementName = "isFlt")]
		public bool IsFault { get; set; }

		/// <summary>
		/// Latitude position of the train in decimal degrees 
		/// </summary>
		[XmlElement(ElementName = "lat")]
		public string Lat { get; set; }

		/// <summary>
		/// Longitude position of the train in decimal degrees
		/// </summary>
		[XmlElement(ElementName = "lon")]
		public string Lon { get; set; }

		/// <summary>
		/// Heading, expressed in standard bearing degrees (0 = North, 90 = East, 180 = South, and 270 = West; range is 0 to 359, progressing clockwise) 
		/// </summary>
		[XmlElement(ElementName = "heading")]
		public string Heading { get; set; }

		[XmlElement(ElementName = "flags")]
		public string Flags { get; set; }
	}
}
