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

		private const int SAMPLE_RATE = 44100;
		private const int CHANNELS = 1;

		public AudioRecorder()
		{
			m_recordingState = RecordingState.Stopped;
			m_memoryStream = new MemoryStream();
		}

		///<inheritdoc cref = "IAudioRecorder.Start()"/>
		public void Start()
		{
			if (m_recordingState == RecordingState.Recording)
			{
				throw new InvalidOperationException("Recording is already started...");
			}

			InitializeRecorder();

			m_recordingState = RecordingState.Recording;
			m_waveIn.StartRecording();
		}

		///<inheritdoc cref = "IAudioRecorder.Stop()"/>
		public void Stop()
		{
			if (m_recordingState == RecordingState.Stopped)
			{
				return;
			}

			m_waveIn.StopRecording();
			m_recordingState = RecordingState.Stopped;
		}

		///<inheritdoc cref = "IAudioRecorder.Pause()"/>
		public void Pause()
		{
			if (m_recordingState == RecordingState.Stopped)
			{
				return;
			}

			m_waveIn.StopRecording();
			m_recordingState = RecordingState.Paused;
		}

		///<inheritdoc cref = "IAudioRecorder.SaveRecord(string)"/>
		public void SaveRecord(string path)
		{
			WriteToFile(m_memoryStream.GetBuffer(), (int)m_memoryStream.Length, path);
			m_memoryStream.SetLength(0);
		}

		/// <summary>
		/// Create and initialize new recorder
		/// </summary>
		private void InitializeRecorder()
		{
			m_waveIn = new WaveIn
			{
				WaveFormat = new WaveFormat(SAMPLE_RATE, CHANNELS),
				DeviceNumber = m_recordingDevice
			};
			m_waveIn.DataAvailable += OnDataAvailable;
		}

		/// <summary>
		/// This callback will be called when the new recording data is available
		/// </summary>
		void OnDataAvailable(object sender, WaveInEventArgs e)
		{
			m_memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
		}

		/// <summary>
		/// Write the recorded MemoryStream to the file by given <paramref name="path"/>
		/// </summary>
		private void WriteToFile(byte[] buffer, int bytesRecorded, string path)
		{
			long maxFileLength = m_waveIn.WaveFormat.AverageBytesPerSecond * 60;

			if (m_recordingState != RecordingState.Recording)
			{
				m_writer = new WaveFileWriter(path, m_waveIn.WaveFormat);

				if (maxFileLength - m_writer.Length > 0)
				{
					m_writer.Write(buffer, 0, bytesRecorded);
				}

				m_writer.Dispose();
			}
		}
	}
}
