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
using System.Drawing;
using System.Threading;
using VoiceRecorder.ViewModels;

namespace VoiceRecorder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private AudioRecorder audioRecorder;
		private AudioPlayer audioPlayer = new AudioPlayer();
		private Boolean isRecording = false;
		
		public MainWindow()
		{
			InitializeComponent();


			IOListViewModel test = new IOListViewModel();

			List<IODevice> items = new List<IODevice>();

			items.Add(new IODevice()
			{
				Id = 0,
				DisplayName = "select "
			});

			items.Add(new IODevice()
			{
				Id =  1,
				DisplayName = "test1"
			});

			items.Add(new IODevice()
			{
				Id = 2,
				DisplayName = "test2"
			});

			test.IODevices = items;

			this.DataContext = test;

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

		private void Start_Pause_Click(object sender, RoutedEventArgs e)
		{
			if (audioRecorder.RecordingState == RecordingState.Stopped || 
			    audioRecorder.RecordingState == RecordingState.Paused)
			{
				this.Play.Visibility = Visibility.Visible;
				this.Play.IsEnabled = false;
				this.Stop.Visibility = Visibility.Visible;
				this.StartPause.Content = "Pause";
				audioRecorder.StartRecording();
			}
			else if (audioRecorder.RecordingState == RecordingState.Recording)
			{
				this.Play.IsEnabled = true;
				this.StartPause.Content = "Start";
				audioRecorder.PauseRecording();
			}
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			audioRecorder.StopRecording();
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			this.Play.IsEnabled = true;
			this.Stop.Visibility = Visibility.Hidden;
			this.StartPause.Content = "Start";
			audioRecorder.StopRecording();
		}
		private void Play_Click(object sender, RoutedEventArgs e)
		{
			audioPlayer.LoadFile(audioRecorder.m_memoryStream);
			audioPlayer.Play();
		}

		//private void Button_Click_1(object sender, RoutedEventArgs e)
		//{
		//	audioRecorder.StopRecording();
		//}

		//private void Button_Click_2(object sender, RoutedEventArgs e)
		//{
		//	audioPlayer.LoadFile("test.wav");
		//	audioPlayer.Play();
		//}

		//private void StartButton_Click(object sender, RoutedEventArgs e)
		//{

		//	if (this.isRecording)
		//	{
		//		audioRecorder.PauseRecording();
		//		//this.StartButton.Content = "Stop";
		//	}
		//	else
		//	{
		//		audioRecorder.StartRecording();
		//		this.StartButton.Content = "Pause";
		//	}

		//}
	}
}
