using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace tinyOS
{
	public class DeviceQueue : IEnumerable<Tuple<DeviceId, ProcessContextBlock>>
	{
		private readonly Dictionary<DeviceId, Queue<ProcessContextBlock>> _deviceQueue;

		public DeviceQueue()
		{
			_deviceQueue = Enum.GetValues(typeof (DeviceId))
				.Cast<DeviceId>()
				.ToDictionary(id => id, queue => new Queue<ProcessContextBlock>());
		}

		public void Enqueue(DeviceId deviceId, ProcessContextBlock pcb)
		{
			_deviceQueue[deviceId].Enqueue(pcb);
		}

		public ProcessContextBlock Dequeue(DeviceId deviceId)
		{
			var queue = _deviceQueue[deviceId];
			if ( queue.Count == 0 )
				return null;

			return queue.Dequeue();
		}

		public IEnumerator<Tuple<DeviceId, ProcessContextBlock>> GetEnumerator()
		{
			return
				(
					from e in _deviceQueue
					from pcb in e.Value
					select new Tuple<DeviceId, ProcessContextBlock>(e.Key, pcb)
				).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}