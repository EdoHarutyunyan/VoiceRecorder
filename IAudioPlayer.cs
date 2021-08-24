using System;
using System.IO;
using NAudio.Wave;

namespace VoiceRecorder.Audio
{
	public interface IAudioPlayer : IDisposable
	{
		void LoadFile(WaveStream inputStream);
		void Play();
		void Stop();
		TimeSpan CurrentPosition { get; set; }
		TimeSpan StartPosition { get; set; }
		TimeSpan EndPosition { get; set; }
	}
}
