/** Generate automatically from https://xmltocsharp.azurewebsites.net/ **/

using System.Xml.Serialization;
using System.Collections.Generic;

namespace MSFSFlightFollowing.Models
{
	[XmlRoot(ElementName = "ELEV")]
	public class ELEV
	{
		[XmlAttribute(AttributeName = "UNIT")]
		public string UNIT { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "GEOLOCATION")]
	public class GEOLOCATION
	{
		[XmlElement(ElementName = "LAT")]
		public double LAT { get; set; }
		[XmlElement(ElementName = "LON")]
		public double LON { get; set; }
		[XmlElement(ElementName = "ELEV")]
		public double ELEV { get; set; }
	}

	[XmlRoot(ElementName = "RADIO")]
	public class RADIO
	{
		[XmlElement(ElementName = "FREQUENCY")]
		public string FREQUENCY { get; set; }
		[XmlElement(ElementName = "TYPE")]
		public string TYPE { get; set; }
		[XmlElement(ElementName = "DESCRIPTION")]
		public string DESCRIPTION { get; set; }
		[XmlAttribute(AttributeName = "CATEGORY")]
		public string CATEGORY { get; set; }
		[XmlElement(ElementName = "TYPESPEC")]
		public string TYPESPEC { get; set; }
	}

	[XmlRoot(ElementName = "LENGTH")]
	public class LENGTH
	{
		[XmlAttribute(AttributeName = "UNIT")]
		public string UNIT { get; set; }
		[XmlText]
		public double Text { get; set; }
	}

	[XmlRoot(ElementName = "WIDTH")]
	public class WIDTH
	{
		[XmlAttribute(AttributeName = "UNIT")]
		public string UNIT { get; set; }
		[XmlText]
		public double Text { get; set; }
	}

	[XmlRoot(ElementName = "DIRECTION")]
	public class DIRECTION
	{
		[XmlAttribute(AttributeName = "TC")]
		public double TC { get; set; }
		[XmlElement(ElementName = "RUNS")]
		public RUNS RUNS { get; set; }
	}

	[XmlRoot(ElementName = "RWY")]
	public class RWY
	{
		[XmlElement(ElementName = "NAME")]
		public string NAME { get; set; }
		[XmlElement(ElementName = "SFC")]
		public string SFC { get; set; }
		[XmlElement(ElementName = "LENGTH")]
		public LENGTH LENGTH { get; set; }
		//[XmlElement(ElementName = "WIDTH")]
		//public WIDTH WIDTH { get; set; }
		[XmlElement(ElementName = "DIRECTION")]
		public List<DIRECTION> DIRECTION { get; set; }
		[XmlAttribute(AttributeName = "OPERATIONS")]
		public string OPERATIONS { get; set; }
	}

	[XmlRoot(ElementName = "AIRPORT")]
	public class AIRPORT
	{
		//[XmlElement(ElementName = "IDENTIFIER")]
		//public string IDENTIFIER { get; set; }
		[XmlElement(ElementName = "COUNTRY")]
		public string COUNTRY { get; set; }
		[XmlElement(ElementName = "NAME")]
		public string NAME { get; set; }
		[XmlElement(ElementName = "ICAO")]
		public string ICAO { get; set; }
		[XmlElement(ElementName = "GEOLOCATION")]
		public GEOLOCATION GEOLOCATION { get; set; }
		[XmlElement(ElementName = "RADIO")]
		public List<RADIO> RADIO { get; set; }
		[XmlElement(ElementName = "RWY")]
		public List<RWY> RWY { get; set; }
		[XmlAttribute(AttributeName = "TYPE")]
		public string TYPE { get; set; }
	}

	[XmlRoot(ElementName = "TORA")]
	public class TORA
	{
		[XmlAttribute(AttributeName = "UNIT")]
		public string UNIT { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "LDA")]
	public class LDA
	{
		[XmlAttribute(AttributeName = "UNIT")]
		public string UNIT { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "RUNS")]
	public class RUNS
	{
		[XmlElement(ElementName = "TORA")]
		public TORA TORA { get; set; }
		[XmlElement(ElementName = "LDA")]
		public LDA LDA { get; set; }
	}

	[XmlRoot(ElementName = "WAYPOINTS")]
	public class WAYPOINTS
	{
		[XmlElement(ElementName = "AIRPORT")]
		public List<AIRPORT> AIRPORT { get; set; }
	}

	[XmlRoot(ElementName = "OPENAIP")]
	public class OPENAIP
	{
		[XmlElement(ElementName = "WAYPOINTS")]
		public WAYPOINTS WAYPOINTS { get; set; }
		[XmlAttribute(AttributeName = "VERSION")]
		public string VERSION { get; set; }
		[XmlAttribute(AttributeName = "DATAFORMAT")]
		public string DATAFORMAT { get; set; }
	}

}