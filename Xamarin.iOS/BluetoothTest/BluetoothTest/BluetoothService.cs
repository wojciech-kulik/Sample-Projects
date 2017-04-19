using System;
using CoreBluetooth;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Foundation;

namespace BluetoothTest
{
	public class BluetoothService : IDisposable
	{
		private const int ConnectionTimeout = 10000;
		private readonly CBCentralManager manager = new CBCentralManager();

		public EventHandler<CBPeripheral> DiscoveredDevice;
		public EventHandler<CBCentralManagerState> StateChanged;

		public BluetoothService()
		{
            this.manager.DiscoveredPeripheral += this.DiscoveredPeripheral;
			this.manager.UpdatedState += this.UpdatedState;
		}

		public void Dispose()
		{
			this.manager.DiscoveredPeripheral -= this.DiscoveredPeripheral;
			this.manager.UpdatedState -= this.UpdatedState;
			this.StopScan();
		}

		public async Task Scan(int scanDuration, string serviceUuid = "")
		{
			Debug.WriteLine("Scanning started");
			var uuids = string.IsNullOrEmpty(serviceUuid)
				? new CBUUID[0]
				: new[] { CBUUID.FromString(serviceUuid) };
			this.manager.ScanForPeripherals(uuids);

			await Task.Delay(scanDuration);
			this.StopScan();
		}

		public void StopScan()
		{
			this.manager.StopScan();
			Debug.WriteLine("Scanning stopped");
		}

		public async Task ConnectTo(CBPeripheral peripheral)
		{
			var taskCompletion = new TaskCompletionSource<bool>();
			var task = taskCompletion.Task;
			EventHandler<CBPeripheralEventArgs> connectedHandler = (s, e) =>
			{
				if (e.Peripheral.Identifier?.ToString() == peripheral.Identifier?.ToString())
				{
					taskCompletion.SetResult(true);
				}
			};

			try
			{
				this.manager.ConnectedPeripheral += connectedHandler;
				this.manager.ConnectPeripheral(peripheral);
				await this.WaitForTaskWithTimeout(task, ConnectionTimeout);
				Debug.WriteLine($"Bluetooth device connected = {peripheral.Name}");
			}
			finally
			{
				this.manager.ConnectedPeripheral -= connectedHandler;
			}
		}

		public void Disconnect(CBPeripheral peripheral)
		{
			this.manager.CancelPeripheralConnection(peripheral);
			Debug.WriteLine($"Device {peripheral.Name} disconnected");
		}

		public CBPeripheral[] GetConnectedDevices(string serviceUuid)
		{
			return this.manager.RetrieveConnectedPeripherals(new[] { CBUUID.FromString(serviceUuid) });
		}

		public async Task<CBService> GetService(CBPeripheral peripheral, string serviceUuid)
		{
			var service = this.GetServiceIfDiscovered(peripheral, serviceUuid);
			if (service != null)
			{
				return service;
			}

			var taskCompletion = new TaskCompletionSource<bool>();
			var task = taskCompletion.Task;
			EventHandler<NSErrorEventArgs> handler = (s, e) =>
			{
				if (this.GetServiceIfDiscovered(peripheral, serviceUuid) != null)
				{
					taskCompletion.SetResult(true);
				}
			};

			try
			{
				peripheral.DiscoveredService += handler;
				peripheral.DiscoverServices(new[] { CBUUID.FromString(serviceUuid) });
				await this.WaitForTaskWithTimeout(task, ConnectionTimeout);
				return this.GetServiceIfDiscovered(peripheral, serviceUuid);
			}
			finally
			{
				peripheral.DiscoveredService -= handler;
			}
		}

		public CBService GetServiceIfDiscovered(CBPeripheral peripheral, string serviceUuid)
		{
			serviceUuid = serviceUuid.ToLowerInvariant();
			return peripheral.Services
            	?.FirstOrDefault(x => x.UUID?.Uuid?.ToLowerInvariant() == serviceUuid);
		}

		public async Task<CBCharacteristic[]> GetCharacteristics(CBPeripheral peripheral, CBService service, int scanTime)
		{
			peripheral.DiscoverCharacteristics(service);
			await Task.Delay(scanTime);
			return service.Characteristics;
		}

		public async Task<NSData> ReadValue(CBPeripheral peripheral, CBCharacteristic characteristic)
		{
			var taskCompletion = new TaskCompletionSource<bool>();
			var task = taskCompletion.Task;
			EventHandler<CBCharacteristicEventArgs> handler = (s, e) =>
			{
				if (e.Characteristic.UUID?.Uuid == characteristic.UUID?.Uuid)
				{
					taskCompletion.SetResult(true);
				}
			};

			try
			{
				peripheral.UpdatedCharacterteristicValue += handler;
				peripheral.ReadValue(characteristic);
				await this.WaitForTaskWithTimeout(task, ConnectionTimeout);
				return characteristic.Value;
			}
			finally
			{
				peripheral.UpdatedCharacterteristicValue -= handler;
			}
		}

		public async Task<NSError> WriteValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSData value)
		{
			var taskCompletion = new TaskCompletionSource<NSError>();
			var task = taskCompletion.Task;
			EventHandler<CBCharacteristicEventArgs> handler = (s, e) =>
			{
				if (e.Characteristic.UUID?.Uuid == characteristic.UUID?.Uuid)
				{
					taskCompletion.SetResult(e.Error);
				}
			};

			try
			{
				peripheral.WroteCharacteristicValue += handler;
				peripheral.WriteValue(value, characteristic, CBCharacteristicWriteType.WithResponse);
				await this.WaitForTaskWithTimeout(task, ConnectionTimeout);
				return task.Result;
			}
			finally
			{
				peripheral.WroteCharacteristicValue -= handler;
			}
		}

		private void DiscoveredPeripheral(object sender, CBDiscoveredPeripheralEventArgs args)
		{
			var device = $"{args.Peripheral.Name} - {args.Peripheral.Identifier?.Description}";
			Debug.WriteLine($"Discovered {device}");
			this.DiscoveredDevice?.Invoke(sender, args.Peripheral);
		}

		private void UpdatedState(object sender, EventArgs args)
		{
			Debug.WriteLine($"State = {this.manager.State}");
			this.StateChanged?.Invoke(sender, this.manager.State);
		}

		private async Task WaitForTaskWithTimeout(Task task, int timeout)
		{
			await Task.WhenAny(task, Task.Delay(ConnectionTimeout));
			if (!task.IsCompleted)
			{
				throw new TimeoutException();
			}
		}
	}
}