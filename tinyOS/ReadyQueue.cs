using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Andy.TinyOS
{
	public class ReadyQueue : IEnumerable<ProcessContextBlock>
	{
		private readonly byte _priorityCount;
		private readonly Dictionary<byte, Queue<ProcessContextBlock>> _readyQueue;

		public byte DefaultPriority { get; }

		public ReadyQueue(byte priorityCount)
		{
			_priorityCount = priorityCount;
			_readyQueue = Enumerable.Range(1, priorityCount)
				.ToDictionary(i => (byte )i, x => new Queue<ProcessContextBlock>());

			DefaultPriority = (byte) (_priorityCount >> 1);
		}

		public ProcessContextBlock Dequeue()
		{
			for ( byte p = _priorityCount; p > 0; p-- )
			{
				var queue = _readyQueue[p];
				if ( queue.Count == 0 )
					continue;

				return queue.Dequeue();
			}

			return null;
		}

		public void Enqueue(ProcessContextBlock pcb)
		{
			if ( pcb.Priority == 0 || pcb.Priority > _priorityCount )
				pcb.Priority = DefaultPriority;

			Queue<ProcessContextBlock> queue;
			if (_readyQueue.TryGetValue(pcb.Priority, out queue))
				queue.Enqueue(pcb);
		}

		public IEnumerator<ProcessContextBlock> GetEnumerator()
		{
			return _readyQueue
				.OrderByDescending(x => x.Key)
				.SelectMany(x => x.Value)
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}