using System.Windows;

namespace VoiceRecorder.Views
{
	/// <summary>
	/// Interaction logic for RecordSaveDialog.xaml
	/// </summary>
	public partial class RecordSaveDialog : Window
	{
		private string m_fileName;
		public string FileName => m_fileName;

		public RecordSaveDialog()
		{
			InitializeComponent();
		}

		private void Cancel_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void Save_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			m_fileName = SavedName.Text + ".wav";
		}

	}
}
