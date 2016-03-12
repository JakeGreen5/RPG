using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public static class FileManager {

	/// <summary>
	/// Save the specified classToSerialize to the path given.
	/// </summary>
	/// <param name="classToSerialize">Class to serialize.</param>
	/// <param name="path">Path.</param>
	/// <param name="extraTypes">Extra types.</param>
	public static void Save(object classToSerialize, string path, Type[] extraTypes)
	{
		XmlSerializer serializer = new XmlSerializer(classToSerialize.GetType(), extraTypes);
		
		using (FileStream stream = new FileStream(path, FileMode.Create))
		{			
			serializer.Serialize(stream, classToSerialize);
		}
	}

	/// <summary>
	/// Load the specified classType at the path given.
	/// </summary>
	/// <param name="classType">Class type.</param>
	/// <param name="path">Path.</param>
	/// <param name="extraTypes">Extra types.</param>
	public static object Load(Type classType, string path, Type[] extraTypes)		
	{
		XmlSerializer serializer = new XmlSerializer(classType, extraTypes);
		
		using (FileStream stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream);
		}
	}

	/// <summary>
	/// Deletes the file at the path given.
	/// </summary>
	/// <param name="path">Path.</param>
	public static void Delete(string path)
	{
		File.Delete(path);
	}
}
