using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace FlashTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			axShockwaveFlash.LoadMovie(0, Application.StartupPath + "\\Load.swf");
			axShockwaveFlash.Play();
		}

		private void button_Click(object sender, System.EventArgs e)
		{
			//1841
			Waddle(1005, 400, 400);
		}

		private void Waddle(int id, int x, int y)
		{
			object obj = SendPacket("s", "u#sp", new object[] { 16, x, y }, 16);
			MessageBox.Show(obj.ToString());
		}

		private object SendPacket(string extension, string command, object[] array, int internalRoomId)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].ToString();
			}
			return CallFunction("sendPacket", extension, command, array, "str", internalRoomId);
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
					WriteXmlArgument(arg, writer);
				}
				writer.WriteFullEndElement();
				writer.WriteFullEndElement();
			}
			string request = builder.ToString();
			string result = axShockwaveFlash.CallFunction(request);
			using (XmlReader reader = XmlReader.Create(new StringReader(result)))
			{
				return ReadXmlResult(reader);
			}
		}

		private void WriteXmlArgument(object arg, XmlWriter writer)
		{
			if (arg == null)
			{
				writer.WriteStartElement("null");
				writer.WriteEndElement();
			}
			else if (arg is bool)
			{
				writer.WriteStartElement((bool)arg ? "true" : "false");
				writer.WriteEndElement();
			}
			else if (arg is string)
			{
				writer.WriteElementString("string", (string)arg);
			}
			else if (arg is float || arg is double || arg is int || arg is uint)
			{
				writer.WriteElementString("number", arg.ToString());
			}
			else if (arg.GetType().IsArray)
			{
				IEnumerable enumerable = (IEnumerable)arg;
				writer.WriteStartElement("array");
				int i = 0;
				foreach (object value in enumerable)
				{
					writer.WriteStartElement("property");
					writer.WriteAttributeString("id", i.ToString());
					WriteXmlArgument(value, writer);
					writer.WriteFullEndElement();
					i++;
				}
				writer.WriteFullEndElement();
			}
		}

		private object ReadXmlResult(XmlReader reader)
		{
			reader.Read();
			switch (reader.Name)
			{
				case "null":
				case "undefined":
					return null;
				case "true":
					return true;
				case "false":
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
							object value = ReadXmlResult(reader);
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
							object value = ReadXmlResult(reader);
							obj.Add(key, value);
						} while (reader.ReadToNextSibling("property"));
					}
					reader.ReadEndElement();
					return obj;
			}
			throw new InvalidDataException("Invalid XML data");
		}
	}
}
