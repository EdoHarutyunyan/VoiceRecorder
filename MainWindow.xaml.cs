using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using VoiceRecorder.Audio;
using System.Threading;

namespace VoiceRecorder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private AudioRecorder audioRecorder;
		private AudioPlayer audioPlayer = new AudioPlayer();
		
		public MainWindow()
		{
			InitializeComponent();

			audioRecorder = new AudioRecorder("test.wav", 0);

			//new Thread(() =>
			//{
			//	Thread.CurrentThread.IsBackground = true;
			//	audioRecorder.StartRecording();
			//	Thread.Sleep(10000);
			//}).Start();SSSS

			//audioRecorder.StopRecording();


			////int waveInDevices = WaveIn.DeviceCount;

			////for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
			////{
			////	WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);

			////	Console.WriteLine("Device {0}: {1}, {2} channels",
			////		waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
			////}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			audioRecorder.StartRecording();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			audioRecorder.StopRecording();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			audioPlayer.LoadFile("test.wav");
			audioPlayer.Play();
		}
	}
}
