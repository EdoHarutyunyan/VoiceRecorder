using System.IO;

namespace VoiceRecorder.AudioRecorder
{
	public interface IAudioRecorder
	{
		/// <summary>
		/// Start the audio recording. The recording information will fill into MemoryStream
		/// Change the RecordingState to Recording
		/// </summary>
		void Start();

		/// <summary>
		/// Stop the audio recording. Change the RecordingState to Stopped
		/// </summary>
		void Stop();

		/// <summary>
		/// Pause the audio recording. Change the RecordingState to Paused
		/// </summary>
		void Pause();

		/// <summary>
		/// Write the recorded MemoryStream to the file by given <paramref name="path"/>
		/// Clean the MemoryStream
		/// </summary>
		/// <param name="path"></param>
		void SaveRecord(string path);

		public RecordingState RecordingState { get; }
		public MemoryStream MemoryStream { get; }

		public int m_recordingDevice { set; }
	}
}
