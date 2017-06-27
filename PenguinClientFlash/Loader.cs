using PenguinClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PenguinClientFlash
{
	public class Loader : IDisposable
	{
		#region Fields

		private AxShockwaveFlashObjects.AxShockwaveFlash axShockwaveFlash;

		private PacketQueue packets;

		#endregion

		#region Constructors

		public Loader(AxShockwaveFlashObjects.AxShockwaveFlash axShockwaveFlash, string url)
		{
			this.axShockwaveFlash = axShockwaveFlash;
			packets = new PacketQueue();
			axShockwaveFlash.LoadMovie(0, url);
			axShockwaveFlash.Play();
			axShockwaveFlash.FlashCall += AxShockwaveFlash_FlashCall;
		}

		#endregion

		#region Methods

		private void AxShockwaveFlash_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
		{
			InvokeRequest invoke = InvokeRequest.Parse(e.request);
			if (invoke.Name == "receivePacket")
			{
				string extension = (string)invoke.Arguments[0];
				string command = (string)invoke.Arguments[1];
				object[] array = (object[])invoke.Arguments[2];
				packets.Enqueue(extension, command, array);
			}
		}

		public void Dispose()
		{
			if (packets != null)
			{
				packets.Dispose();
				packets = null;
			}
		}

		#endregion

		#region Interface

		public void Init()
		{
			CallFunction("init");
		}

		public object SendPacket(string extension, string command, object[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].ToString();
			}
			return CallFunction("sendPacket", extension, command, array, "str");
		}

		public Packet ReceivePacket()
		{
			return packets.Dequeue();
		}

		#endregion

		#region Utilities

		public static string GetLiteral(object obj)
		{
			if (obj == null)
				return "null";
			if (obj == Undefined.Value)
				return "undefined";
			if (obj is bool)
				return (bool)obj ? "true" : "false";
			if (obj is string)
				return "\"" + (string)obj + "\"";
			if (obj is object[])
			{
				object[] arr = (object[])obj;
				string str = "[";
				for (int i = 0; i < arr.Length; i++)
				{
					if (i > 0)
						str += ", ";
					str += GetLiteral(arr[i]);
				}
				str += "]";
				return str;
			}
			if (obj is Dictionary<string, object>)
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)obj;
				string str = "{";
				bool comma = false;
				foreach (KeyValuePair<string, object> prop in dict)
				{
					if (comma)
						str += ", ";
					str += prop.Key + ": " + GetLiteral(prop.Value);
					comma = true;
				}
				str += "}";
				return str;
			}
			return obj.ToString();
		}

		private object CallFunction(string name, params object[] args)
		{
			StringBuilder builder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			using (XmlWriter writer = XmlWriter.Create(builder, settings))
			{
				writer.WriteStartElement("invoke");
				writer.WriteAttributeString("name", name);
				writer.WriteAttributeString("returnType", "xml");
				writer.WriteStartElement("arguments");
				foreach (object arg in args)
				{
					WriteXmlValue(arg, writer);
				}
				writer.WriteFullEndElement();
				writer.WriteFullEndElement();
			}
			string request = builder.ToString();
			string result = axShockwaveFlash.CallFunction(request);
			using (XmlReader reader = XmlReader.Create(new StringReader(result)))
			{
				reader.Read();
				return ReadXmlValue(reader);
			}
		}

		internal static void WriteXmlValue(object value, XmlWriter writer)
		{
			if (value == null)
			{
				writer.WriteStartElement("null");
				writer.WriteEndElement();
			}
			else if (value is bool)
			{
				writer.WriteStartElement((bool)value ? "true" : "false");
				writer.WriteEndElement();
			}
			else if (value is string)
			{
				writer.WriteElementString("string", (string)value);
			}
			else if (value is float || value is double || value is int || value is uint)
			{
				writer.WriteElementString("number", value.ToString());
			}
			else if (value.GetType().IsArray)
			{
				IEnumerable enumerable = (IEnumerable)value;
				writer.WriteStartElement("array");
				int i = 0;
				foreach (object item in enumerable)
				{
					writer.WriteStartElement("property");
					writer.WriteAttributeString("id", i.ToString());
					WriteXmlValue(item, writer);
					writer.WriteFullEndElement();
					i++;
				}
				writer.WriteFullEndElement();
			}
		}

		internal static object ReadXmlValue(XmlReader reader)
		{
			switch (reader.Name)
			{
				case "null":
					reader.Read();
					return null;
				case "undefined":
					reader.Read();
					return Undefined.Value;
				case "true":
					reader.Read();
					return true;
				case "false":
					reader.Read();
					return false;
				case "string":
					return reader.ReadElementContentAsString();
				case "number":
					string number = reader.ReadElementContentAsString();
					int i;
					if (int.TryParse(number, out i))
						return i;
					float f;
					if (float.TryParse(number, out f))
						return f;
					double d;
					if (double.TryParse(number, out d))
						return d;
					throw new InvalidDataException("Invalid number");
				case "array":
					List<object> list = new List<object>();
					if (reader.ReadToDescendant("property"))
					{
						do
						{
							reader.MoveToAttribute("id");
							int id = reader.ReadContentAsInt();
							object value = ReadXmlValue(reader);
							list.Add(value);
						} while (reader.ReadToNextSibling("property"));
					}
					reader.ReadEndElement();
					return list.ToArray();
				case "object":
					Dictionary<string, object> obj = new Dictionary<string, object>();
					if (reader.ReadToDescendant("property"))
					{
						do
						{
							string key = reader.GetAttribute("id");
							object value = ReadXmlValue(reader);
							obj.Add(key, value);
						} while (reader.ReadToNextSibling("property"));
					}
					reader.ReadEndElement();
					return obj;
			}
			throw new InvalidDataException("Invalid XML data");
		}

		private class Undefined
		{
			public static readonly Undefined Value = new Undefined();

			private Undefined() { }
		}

		#endregion
	}
}
