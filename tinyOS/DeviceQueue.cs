using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using tinyOS;

namespace Andy.TinyOS
{
	public class BlockingProcess
	{
		public DeviceId DeviceId { get; set; }
		public ProcessContextBlock Process { get; set; }
		public OpCodeFlag Flag { get; set; }
		public uint Argument { get; set; }
	}

	public class DeviceQueue : IEnumerable<BlockingProcess>
	{
		private readonly Dictionary<DeviceId, Queue<BlockingProcess>> _deviceQueue;

		public DeviceQueue()
		{
			_deviceQueue = Enum.GetValues(typeof (DeviceId))
				.Cast<DeviceId>()
				.ToDictionary(id => id, queue => new Queue<BlockingProcess>());
		}

		public void Enqueue(DeviceId deviceId, ProcessContextBlock pcb, OpCodeFlag flag = OpCodeFlag.Register, uint arg1 = 0)
		{
			var blockingProcess = new BlockingProcess
			{
				Process = pcb,
				Flag = flag,
				Argument = arg1,
				DeviceId = deviceId
			};

			_deviceQueue[deviceId].Enqueue(blockingProcess);
		}

		public BlockingProcess Dequeue(DeviceId deviceId)
		{
			var queue = _deviceQueue[deviceId];
			if ( queue.Count == 0 )
				return null;

			return queue.Dequeue();
		}

		public IEnumerator<BlockingProcess> GetEnumerator()
		{
			return
				(
					from e in _deviceQueue
					from blockingProcess in e.Value
					select blockingProcess
				).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}