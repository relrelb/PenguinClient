using System.Windows.Forms;
using PenguinClient;
using System.Drawing;

namespace PenguinClientUI
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			LogWriter output = new LogWriter(log);
			LogWriter error = new LogWriter(log);
			error.ForeColor = Color.Red;
			Client client = new Client(output, error);
			bool connected = client.Connect("158.69.214.194", 6112, 6115, "nagitest", "test");
		}
	}
}
