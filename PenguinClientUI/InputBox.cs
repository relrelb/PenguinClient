using System;
using System.Windows.Forms;

namespace PenguinClientUI
{
	public partial class InputBox : Form
	{
		#region Constructors

		private InputBox()
		{
			InitializeComponent();
		}

		private InputBox(string text) : this()
		{
			this.text.Text = text;
		}

		private InputBox(string text, string caption) : this(text)
		{
			Text = caption;
		}

		#endregion

		#region Methods

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ActiveControl = input;
		}

		private static string Show(InputBox box)
		{
			DialogResult result = box.ShowDialog();
			if (result == DialogResult.OK)
				return box.input.Text;
			return null;
		}

		public static new string Show()
		{
			return Show(new InputBox());
		}

		public static string Show(string text)
		{
			return Show(new InputBox(text));
		}

		public static string Show(string text, string caption)
		{
			return Show(new InputBox(text, caption));
		}

		private static string Show(InputBox box, IWin32Window owner)
		{
			DialogResult result = box.ShowDialog(owner);
			if (result == DialogResult.OK)
				return box.input.Text;
			return null;
		}

		public static new string Show(IWin32Window owner)
		{
			return Show(new InputBox(), owner);
		}

		public static string Show(IWin32Window owner, string text)
		{
			return Show(new InputBox(text), owner);
		}

		public static string Show(IWin32Window owner, string text, string caption)
		{
			return Show(new InputBox(text, caption), owner);
		}

		#endregion
	}
}
