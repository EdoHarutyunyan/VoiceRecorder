using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using NAudio.Wave;
using NAudio.Mixer;

namespace VoiceRecorder.Audio
{
	public class AudioRecorder : IAudioRecorder
	{
		private WaveIn m_waveIn;
		private RecordingState m_recordingState;
		private WaveFileWriter m_writer;


		public MemoryStream m_memoryStream { get; private set; }


		private string m_fileName { get; set; }
		private int m_recordingDevice { get; set; }
		public event EventHandler Stopped = delegate { };

		public AudioRecorder(string fileName, int recordingDevice)
		{
			m_waveIn = new WaveIn {WaveFormat = new WaveFormat(44100, 1)};
			m_recordingState = RecordingState.Stopped;
			m_fileName = fileName;
			m_recordingDevice = recordingDevice;
		}

		private void InitializeRecorder()
		{

			m_memoryStream = new MemoryStream();

			m_waveIn.DeviceNumber = m_recordingDevice;
			m_waveIn.DataAvailable += OnDataAvailable;
			m_waveIn.RecordingStopped += OnRecordingStopped;
		}

		void OnRecordingStopped(object sender, StoppedEventArgs e)
		{
			if (m_recordingState == RecordingState.Stopped)
			{
				m_writer = new WaveFileWriter(m_fileName, m_waveIn.WaveFormat);
				WriteToFile(m_memoryStream.GetBuffer(), (int)m_memoryStream.Length);
				m_writer.Dispose();
			}

			Stopped(this, EventArgs.Empty);
		}

		public void StartRecording()
		{
			if (m_recordingState == RecordingState.Recording)
			{
				throw new InvalidOperationException("Recording is already started...");
			}
			
			if (m_recordingState == RecordingState.Stopped)
			{
				InitializeRecorder();
			}

			m_recordingState = RecordingState.Recording;
			m_waveIn.StartRecording();
		}

		public void StopRecording()
		{
			if (m_recordingState == RecordingState.Stopped)
			{
				return;
			}

			m_recordingState = RecordingState.Stopped;
			m_waveIn.StopRecording();
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

		public RecordingState RecordingState => m_recordingState;

		public TimeSpan RecordedTime
		{
			get
			{
				if (m_writer == null)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromSeconds((double)m_writer.Length / m_writer.WaveFormat.AverageBytesPerSecond);
			}
		}

		void OnDataAvailable(object sender, WaveInEventArgs e)
		{
			m_memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
		}

		private void WriteToFile(byte[] buffer, int bytesRecorded)
		{
			long maxFileLength = this.m_waveIn.WaveFormat.AverageBytesPerSecond * 60;

			if (m_recordingState == RecordingState.Stopped)
			{
				var toWrite = (int)Math.Min(maxFileLength - m_writer.Length, bytesRecorded);
				if (toWrite > 0)
				{
					m_writer.Write(buffer, 0, bytesRecorded);
				}
				else
				{
					StopRecording();
				}
			}
		}
	}
}
