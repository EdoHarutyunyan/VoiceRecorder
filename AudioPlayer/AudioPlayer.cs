using System;
using System.IO;
using NAudio.Wave;

namespace VoiceRecorder.AudioPlayer
{
	public class AudioPlayer : IAudioPlayer
	{
		private WaveOut m_waveOut;
		private ReadStream m_inStream;
		
		private const int SAMPLE_RATE = 44100;
		private const int CHANNELS = 1;

		public event EventHandler PlaybackStopped = delegate { };

		public PlayerMode PlayerMode { get; set; } = PlayerMode.None;

		public PlaybackState PlaybackState { get; private set; }

		///<inheritdoc cref = "IAudioPlayer.LoadFile(string)"/>
		public void LoadFile(string path)
		{
			CloseWaveOut();
			CloseInStream();
			m_inStream = new ReadStream(new WaveFileReader(path));
		}

		///<inheritdoc cref = "IAudioPlayer.LoadFile(Stream)"/>
		public void LoadFile(Stream inputStream)
		{
			CloseWaveOut();
			CloseInStream();
			inputStream.Position = 0;

			var waveFileReader = new RawSourceWaveStream(inputStream, new WaveFormat(SAMPLE_RATE, CHANNELS));

			m_inStream = new ReadStream(waveFileReader);
		}

		///<inheritdoc cref = "IAudioPlayer.Play()"/>
		public void Play()
		{
			if (PlaybackState == PlaybackState.Paused)
			{
				PlaybackState = PlaybackState.Playing;
				m_waveOut.Play();
				return;
			}

			CreateWaveOut();

			if (m_waveOut.PlaybackState == PlaybackState.Stopped)
			{
				m_inStream.Position = 0;
				PlaybackState = PlaybackState.Playing;
				m_waveOut.Play();
			}
		}

		///<inheritdoc cref = "IAudioPlayer.Pause()"/>
		public void Pause()
		{
			PlaybackState = PlaybackState.Paused;

			m_waveOut.Pause();
		}

		///<inheritdoc cref = "IAudioPlayer.PlaybackStopped"/>
		private void OnPlaybackStopped(object sender, StoppedEventArgs e)
		{
			// Set the player mode to PlayerMode.None to control the parallel playing 
			PlayerMode = PlayerMode.None;

			if (PlaybackState == PlaybackState.Paused)
			{
				return;
			}

			PlaybackStopped(this, EventArgs.Empty);

			Dispose();
		}

		private void CreateWaveOut()
		{
			if (m_waveOut == null)
			{
				m_waveOut = new WaveOut();
				m_waveOut.Init(m_inStream);
				m_waveOut.PlaybackStopped += OnPlaybackStopped;
			}
		}

		public void Dispose()
		{
			CloseWaveOut();
			CloseInStream();
			PlaybackState = PlaybackState.Stopped;
		}

		private void CloseInStream()
		{
			if (m_inStream != null)
			{
				m_inStream.Dispose();
				m_inStream = null;
			}
		}

		private void CloseWaveOut()
		{
			if (m_waveOut != null)
			{
				m_waveOut.Dispose();
				m_waveOut = null;
			}
		}
	}
}
