using System.IO;
using NAudio.Wave;
using VoiceRecorder.Audio;

namespace VoiceRecorder.AudioPlayer
{
	public class AudioPlayer : IAudioPlayer
	{
		private WaveOut m_waveOut;
		private TrimWaveStream m_inStream;
		
		private const int SAMPLE_RATE = 44100;
		private const int CHANNELS = 1;

		public void LoadFile(string path)
		{
			CloseWaveOut();
			CloseInStream();
			m_inStream = new TrimWaveStream(new WaveFileReader(path));
		}

		public void LoadFile(Stream inputStream)
		{
			CloseWaveOut();
			CloseInStream();
			inputStream.Position = 0;

			var waveFileReader = new RawSourceWaveStream(inputStream, new WaveFormat(SAMPLE_RATE, CHANNELS));

			m_inStream = new TrimWaveStream(waveFileReader);
		}

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

		private void CreateWaveOut()
		{
			if (m_waveOut == null)
			{
				m_waveOut = new WaveOut();
				m_waveOut.Init(m_inStream);
				m_waveOut.PlaybackStopped += OnPlaybackStopped;
			}
		}

		void OnPlaybackStopped(object sender, StoppedEventArgs e)
		{
			if (PlaybackState == PlaybackState.Paused)
			{
				return;
			}

			Dispose();
		}

		public void Stop()
		{
			PlaybackState = PlaybackState.Stopped;

			m_waveOut.Stop();
			m_inStream.Position = 0;
		}

		public void Pause()
		{
			PlaybackState = PlaybackState.Paused;

			m_waveOut.Pause();
		}

		public PlaybackState PlaybackState { get; private set; }

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
