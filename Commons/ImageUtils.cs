using System;
using System.Windows.Media.Imaging;

namespace VoiceRecorder.Commons
{
	public static class ImageUtils
	{
		public static BitmapImage PauseImage = new BitmapImage(new Uri("/VoiceRecorder;component/Icons/PauseImg.png", UriKind.Relative));
		public static BitmapImage PlayImage = new BitmapImage(new Uri("/VoiceRecorder;component/Icons/PlayImg.png", UriKind.Relative));
		public static BitmapImage RecordImage = new BitmapImage(new Uri("/VoiceRecorder;component/Icons/RecordImg.png", UriKind.Relative));
	}
}
