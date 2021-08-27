using System;
using System.IO;
using NAudio.Wave;

namespace VoiceRecorder.AudioPlayer
{
	public interface IAudioPlayer : IDisposable
	{
		/// <summary>
		/// Load the existing record file by given <paramref name="path"/> to play the existing record
		/// </summary>
		/// <param name="path">The record path</param>
		void LoadFile(string path);

		/// <summary>
		/// Load the current recording <paramref name="inputStream"/> to play the current record
		/// </summary>
		/// <param name="inputStream">The current record stream</param>
		void LoadFile(Stream inputStream);

		/// <summary>
		/// Play the selected record file or currently recording stream
		/// </summary>
		void Play();

		/// <summary>
		/// Pause the selected record file or currently recording stream
		/// </summary>
		void Pause();

		/// <summary>
		/// This event will be called when the audio player is stopped
		/// </summary>
		event EventHandler PlaybackStopped;

		public PlayerMode PlayerMode { get; set; }
		public PlaybackState PlaybackState { get; }
	}
}
