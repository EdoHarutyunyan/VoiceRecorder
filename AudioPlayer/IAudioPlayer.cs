using System;
using System.IO;
using NAudio.Wave;

namespace VoiceRecorder.AudioPlayer
{
	public interface IAudioPlayer : IDisposable
	{
		void LoadFile(string path);
		void LoadFile(Stream inputStream);
		void Play();
		void Stop();
		void Pause();

		public PlaybackState PlaybackState { get; }
	}
}
