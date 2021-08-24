﻿using System;
using System.IO;
using NAudio.Wave;

namespace VoiceRecorder.Audio
{
	public class AudioPlayer : IAudioPlayer
	{
		private WaveOut waveOut;
		private TrimWaveStream inStream;

		public void LoadFile(Stream inputStream)
		{
			CloseWaveOut();
			CloseInStream();
			inputStream.Position = 0;

			var waveFileReader = new RawSourceWaveStream(inputStream, new WaveFormat(44100, 1));
			//waveOut = new WaveOut();

			//waveOut.Init(waveFileReader);
			//waveOut.Play();

			//var reader = new Mp3FileReader(inputStream);
			inStream = new TrimWaveStream(waveFileReader);
		}

		public void Play()
		{
			CreateWaveOut();
			if (waveOut.PlaybackState == PlaybackState.Stopped)
			{
				inStream.Position = 0;
				waveOut.Play();
			}
		}

		private void CreateWaveOut()
		{
			if (waveOut == null)
			{
				waveOut = new WaveOut();
				waveOut.Init(inStream);
				waveOut.PlaybackStopped += OnPlaybackStopped;
			}
		}

		void OnPlaybackStopped(object sender, StoppedEventArgs e)
		{
			PlaybackState = PlaybackState.Stopped;
			CloseWaveOut();
			CloseInStream();
		}

		public void Stop()
		{
			waveOut.Stop();
			inStream.Position = 0;
		}

		public TimeSpan StartPosition
		{
			get { return inStream.StartPosition; }
			set { inStream.StartPosition = value; }
		}

		public TimeSpan EndPosition
		{
			get { return inStream.EndPosition; }
			set { inStream.EndPosition = value; }
		}

		public TimeSpan CurrentPosition { get; set; }
		public PlaybackState PlaybackState { get; private set; }

		public void Dispose()
		{
			CloseWaveOut();
			CloseInStream();
		}

		private void CloseInStream()
		{
			if (inStream != null)
			{
				inStream.Dispose();
				inStream = null;
			}
		}

		private void CloseWaveOut()
		{
			if (waveOut != null)
			{
				waveOut.Dispose();
				waveOut = null;
			}
		}
	}
}
