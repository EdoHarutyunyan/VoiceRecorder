using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace VoiceRecorder.Audio
{
	public interface IAudioRecorder
	{
		void StartRecording();
		void StopRecording();
		void PauseRecording();

		event EventHandler Stopped;

		TimeSpan RecordedTime { get; }
	}
}
