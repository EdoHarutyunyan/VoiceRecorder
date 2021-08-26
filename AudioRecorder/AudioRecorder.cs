using System;
using System.IO;
using NAudio.Wave;

namespace VoiceRecorder.AudioRecorder
{
	public class AudioRecorder : IAudioRecorder
	{
		private WaveIn m_waveIn;
		private WaveFileWriter m_writer;

		private RecordingState m_recordingState;
		public RecordingState RecordingState => m_recordingState;

		private MemoryStream m_memoryStream;
		public MemoryStream MemoryStream => m_memoryStream;

		public int m_recordingDevice { get; set; }
		public event EventHandler Stopped = delegate { };

		public AudioRecorder()
		{
			m_recordingState = RecordingState.Stopped;
			m_memoryStream = new MemoryStream();
		}

		private void InitializeRecorder()
		{
			m_waveIn = new WaveIn
			{
				WaveFormat = new WaveFormat(44100, 1),
				DeviceNumber = m_recordingDevice
			};
			m_waveIn.DataAvailable += OnDataAvailable;
			m_waveIn.RecordingStopped += OnRecordingStopped;
		}

		void OnRecordingStopped(object sender, StoppedEventArgs e)
		{
			Stopped(this, EventArgs.Empty);
		}

		public void StartRecording()
		{
			if (m_recordingState == RecordingState.Recording)
			{
				throw new InvalidOperationException("Recording is already started...");
			}

			InitializeRecorder();

			m_recordingState = RecordingState.Recording;
			m_waveIn.StartRecording();
		}

		public void StopRecording()
		{
			if (m_recordingState == RecordingState.Stopped)
			{
				return;
			}

			m_waveIn.StopRecording();
			m_recordingState = RecordingState.Stopped;
		}

		public void PauseRecording()
		{
			if (m_recordingState == RecordingState.Stopped)
			{
				return;
			}

			m_recordingState = RecordingState.Paused;
			m_waveIn.StopRecording();
		}

		void OnDataAvailable(object sender, WaveInEventArgs e)
		{
			m_memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
		}

		private void WriteToFile(byte[] buffer, int bytesRecorded, string path)
		{
			long maxFileLength = m_waveIn.WaveFormat.AverageBytesPerSecond * 60;

			if (m_recordingState == RecordingState.Stopped)
			{
				m_writer = new WaveFileWriter(path, m_waveIn.WaveFormat);

				if (maxFileLength - m_writer.Length > 0)
				{
					m_writer.Write(buffer, 0, bytesRecorded);
				}

				m_writer.Dispose();
			}
		}

		public void SaveRecord(string path)
		{
			WriteToFile(m_memoryStream.GetBuffer(), (int)m_memoryStream.Length, path);
			m_memoryStream.SetLength(0);
		}
	}
}
