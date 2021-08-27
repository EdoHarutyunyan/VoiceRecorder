using System;
using NAudio.Wave;

namespace VoiceRecorder.AudioPlayer
{
	public class ReadStream : WaveStream
	{
		private WaveStream m_source;
		private long m_endBytePosition;
		private TimeSpan m_endPosition;

		public ReadStream(WaveStream source)
		{
			m_source = source;
			EndPosition = source.TotalTime;
		}

		private TimeSpan EndPosition
		{
			get => m_endPosition;
			init
			{
				m_endPosition = value;
				m_endBytePosition = (int)Math.Round(WaveFormat.AverageBytesPerSecond * m_endPosition.TotalSeconds);
				m_endBytePosition = m_endBytePosition - (m_endBytePosition % WaveFormat.BlockAlign);
			}
		}

		public override WaveFormat WaveFormat => m_source.WaveFormat;
		public override long Length => m_endBytePosition;

		public override long Position
		{
			get => m_source.Position;
			set => m_source.Position = value;
		}

		/// <summary>
		/// Read playing a stream from the file. Called when the player is started.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			int bytesRequired = (int)Math.Min(count, Length - Position);
			int bytesRead = 0;
			if (bytesRequired > 0)
			{
				bytesRead = m_source.Read(buffer, offset, bytesRequired);
			}
			return bytesRead;
		}

		protected override void Dispose(bool disposing)
		{
			m_source.Dispose();
			base.Dispose(disposing);
		}
	}
}
