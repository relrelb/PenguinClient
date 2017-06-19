using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace FlashTest
{
	public class InvokeRequest
	{
		#region Fields

		private string name;

		private object[] args;

		#endregion

		#region Properties

		public string Name { get { return name; } }

		public object[] Arguments { get { return args; } }

		#endregion

		#region Constructors

		public InvokeRequest(string name, IEnumerable args)
		{
			this.name = name;
			this.args = args.Cast<object>().ToArray();
		}

		public InvokeRequest(string name) : this(name, new object[0]) { }

		#endregion

		#region Methods

		public static InvokeRequest Parse(string request)
		{
			using (XmlReader reader = XmlReader.Create(new StringReader(request)))
			{
				reader.Read();
				string name = reader.GetAttribute("name");
				string returnType = reader.GetAttribute("returntype");
				reader.ReadStartElement("invoke");
				reader.ReadStartElement("arguments");
				List<object> args = new List<object>();
				while (reader.Name != "arguments" || reader.NodeType != XmlNodeType.EndElement)
				{
					object arg = Util.ReadXmlValue(reader);
					args.Add(arg);
				}
				reader.ReadEndElement();
				reader.ReadEndElement();
				return new InvokeRequest(name, args);
			}
		}

		#endregion
	}
}
