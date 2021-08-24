using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRecorder.ViewModels
{
	public class IOListViewModel
	{
		public List<IODevice> IODevices { get; set; }
	}

	public class IODevice
	{
		public int Id { get; set; }
		public string DisplayName { get; set; }
	}
}
