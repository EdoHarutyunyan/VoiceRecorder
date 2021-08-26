using System;
using System.IO;

namespace VoiceRecorder.AudioRecorder
{
	public interface IAudioRecorder
	{
		void StartRecording();
		void StopRecording();
		void PauseRecording();
		void SaveRecord(string path);

		public RecordingState RecordingState { get; }
		public MemoryStream MemoryStream { get; }

		public int m_recordingDevice { get; set; }

		event EventHandler Stopped;
	}
}
