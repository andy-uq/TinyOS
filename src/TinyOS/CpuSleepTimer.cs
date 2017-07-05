using System.Collections.Generic;

namespace Andy.TinyOS
{
	public class CpuSleepTimer
	{
		private readonly Dictionary<long, DeviceId> _sleepRegister;
		private int _deviceOffset;
		private long _tickCount = 0L;

		public CpuSleepTimer()
		{
			_sleepRegister = new Dictionary<long, DeviceId>();
		}

		public bool Tick(out DeviceId wake)
		{
			_tickCount++;
			if (_sleepRegister.TryGetValue(_tickCount, out wake))
			{
				_sleepRegister.Remove(_tickCount);
				return true;
			}

			return false;
		}

		public DeviceId Register(uint sleep)
		{
			var target = _tickCount + sleep;

			DeviceId handle;
			if (_sleepRegister.TryGetValue(target, out handle))
				return handle;

			handle = (DeviceId) (Devices.Sleep + _deviceOffset);
			_sleepRegister.Add(target, handle);
			_deviceOffset++;

			return handle;
		}
	}
}