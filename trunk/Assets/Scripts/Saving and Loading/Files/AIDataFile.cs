using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("AIDataCollection")]
public class AIDataFile
{
	[XmlArray("AIData")]
	public List<AIData> AIDataList = new List<AIData>();
}