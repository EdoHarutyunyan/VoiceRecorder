using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using VoiceRecorder.AudioPlayer;
using VoiceRecorder.AudioRecorder;
using VoiceRecorder.Commons;
using VoiceRecorder.ViewModels;
using VoiceRecorder.Views;

namespace VoiceRecorder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly IAudioRecorder m_audioRecorder = new AudioRecorder.AudioRecorder();
		private readonly IAudioPlayer m_audioPlayer = new AudioPlayer.AudioPlayer();
		private readonly MainViewModel m_mainViewModel = new MainViewModel
		{
			Microphones = new List<string>(),
			Records = new List<string>()
		};

		public MainWindow()
		{
			InitializeComponent();
			InitializeInputDevices();
			InitializeRecordLibrary();

			m_audioPlayer.PlaybackStopped += OnPlaybackStopped;
		}

		/// <summary>
		/// This callback will be called when the audio player is stopped by itself
		/// to change the state of the player buttons.
		/// </summary>
		private void OnPlaybackStopped(object sender, EventArgs e)
		{
			RecordPause.IsEnabled = true;
			PlayPauseImg.Source = ImageUtils.PlayImage;
			PlayPauseSelectedRecordImg.Source = ImageUtils.PlayImage;
		}

		private void InitializeInputDevices()
		{
			for (int waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; ++waveInDevice)
			{
				WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);

				m_mainViewModel.Microphones.Add(deviceInfo.ProductName);
			}

			Microphone.ItemsSource = null;
			Microphone.ItemsSource = m_mainViewModel.Microphones;
			Microphone.SelectedIndex = 0;

			var enumerator = new MMDeviceEnumerator();
			Speaker.Content = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console).DeviceFriendlyName;
		}

		private void InitializeRecordLibrary()
		{
			string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

			foreach (var file in Directory.GetFiles(path!, "*.wav", SearchOption.AllDirectories))
			{
				m_mainViewModel.Records.Add(Path.GetFileName(file));
			}

			Records.ItemsSource = null;
			Records.ItemsSource = m_mainViewModel.Records;
		}

		private void Record_Pause_Click(object sender, RoutedEventArgs e)
		{
			if ((m_audioRecorder.RecordingState == RecordingState.Stopped) || 
				(m_audioRecorder.RecordingState == RecordingState.Paused))
			{
				PlayPause.IsEnabled = false;
				PlayPause.Visibility = Visibility.Visible;
				Stop.Visibility = Visibility.Visible;

				RecordPauseImg.Source = ImageUtils.PauseImage;
				m_audioRecorder.Start();
			}
			else if (m_audioRecorder.RecordingState == RecordingState.Recording)
			{
				PlayPause.IsEnabled = true;
				RecordPauseImg.Source = ImageUtils.RecordImage;
				m_audioRecorder.Pause();
			}
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			Settings.IsOpen = true;
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			RecordPauseImg.Source = ImageUtils.RecordImage;
			PlayPauseImg.Source = ImageUtils.PlayImage;

			if (m_audioRecorder.RecordingState == RecordingState.Recording)
			{
				m_audioRecorder.Stop();
			}

			if (m_audioPlayer.PlaybackState == PlaybackState.Playing)
			{
				m_audioPlayer.Pause();
			}

			RecordSaveDialog dialog = new RecordSaveDialog();
			if ((dialog.ShowDialog() == true) && (dialog.FileName != null))
			{
				m_audioRecorder.SaveRecord(dialog.FileName);
				m_mainViewModel.Records.Add(dialog.FileName);

				Records.ItemsSource = null;
				Records.ItemsSource = m_mainViewModel.Records;

				// If the current record is already saved then we need to go to the initial UI state
				PlayPause.Visibility = Visibility.Collapsed;
				Stop.Visibility = Visibility.Collapsed;
			}
			else
			{
				// If the save operation is canceled enable the play button
				PlayPause.IsEnabled = true;
			}
		}

		private void Play_Pause_Click(object sender, RoutedEventArgs e)
		{
			if (m_audioPlayer.PlayerMode == PlayerMode.SelectedRecord)
			{
				MessageBox.Show("Another record is playing...");
				return;
			}

			if (m_audioPlayer.PlaybackState == PlaybackState.Playing)
			{
				RecordPause.IsEnabled = true;
				PlayPauseImg.Source = ImageUtils.PlayImage;

				m_audioPlayer.Pause();
				return;
			}

			RecordPause.IsEnabled = false;
			PlayPauseImg.Source = ImageUtils.PauseImage;

			// We need to load the new memory stream if the record is new.
			// And continue the loaded memory execution if the PlaybackState is Paused
			if (m_audioPlayer.PlaybackState == PlaybackState.Stopped)
			{
				m_audioPlayer.LoadFile(m_audioRecorder.MemoryStream);
			}

			// Set the player mode to PlayerMode.CurrentRecord to control the parallel playing 
			m_audioPlayer.PlayerMode = PlayerMode.CurrentRecord;

			m_audioPlayer.Play();
		}

		private void Play_Pause_Selected_Record_Click(object sender, RoutedEventArgs e)
		{
			if (Records.SelectedIndex == -1)
			{
				MessageBox.Show("Please select the record first");
				return;
			}

			if (m_audioPlayer.PlayerMode == PlayerMode.CurrentRecord)
			{
				MessageBox.Show("Another record is playing...");
				return;
			}

			if (m_audioPlayer.PlaybackState == PlaybackState.Playing)
			{
				PlayPauseSelectedRecordImg.Source = ImageUtils.PlayImage;

				m_audioPlayer.Pause();
				return;
			}

			PlayPauseSelectedRecordImg.Source = ImageUtils.PauseImage;

			// We need to load the new file if the record is new.
			// And continue the loaded file execution if the PlaybackState is Paused
			if (m_audioPlayer.PlaybackState == PlaybackState.Stopped)
			{
				if (Records.SelectedItem is string rec)
				{
					string fullPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + rec;

					m_audioPlayer.LoadFile(fullPath);
				}
			}

			// Set the player mode to PlayerMode.SelectedRecord to control the parallel playing 
			m_audioPlayer.PlayerMode = PlayerMode.SelectedRecord;
			m_audioPlayer.Play();
		}

		private void Settings_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			m_audioRecorder.m_recordingDevice = Microphone.SelectedIndex;
		}
	}
}
