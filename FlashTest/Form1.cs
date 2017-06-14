using System.Collections;
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
			Waddle(16, 400, 400);
		}

		private void Waddle(int id, int x, int y)
		{
			SendPacket("s", "u#sp", new object[] { id, x, y }, int.Parse(textBox1.Text));
		}

		private void SendPacket(string extension, string command, object[] array, int internalRoomId)
		{
			CallFunction("sendPacket", "void", extension, command, array, "str", internalRoomId);
		}

		private string CallFunction(string name, string returnType, params object[] args)
		{
			StringBuilder builder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			using (XmlWriter writer = XmlWriter.Create(builder, settings))
			{
				writer.WriteStartElement("invoke");
				writer.WriteAttributeString("name", name);
				writer.WriteAttributeString("returnType", returnType);
				writer.WriteStartElement("arguments");
				foreach (object arg in args)
				{
					WriteXmlArgument(arg, writer);
				}
				writer.WriteFullEndElement();
				writer.WriteFullEndElement();
			}
			string request = builder.ToString();
			return axShockwaveFlash.CallFunction(request);
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
				foreach (object obj in enumerable)
				{
					writer.WriteStartElement("property");
					writer.WriteAttributeString("id", i.ToString());
					WriteXmlArgument(obj, writer);
					writer.WriteFullEndElement();
					i++;
				}
				writer.WriteFullEndElement();
			}
		}
	}
}
