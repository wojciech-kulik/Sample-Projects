using System;
using System.Diagnostics;
using System.Linq;
using CoreBluetooth;
using UIKit;

namespace BluetoothTest
{
	// results will be displayed in output
	public partial class ViewController : UIViewController
	{
		private const int ScanTime = 5000;
		private const string DeviceName = "GoPro";

		private BluetoothService bluetoothService;
		private bool alreadyScanned;
		private bool alreadyDiscovered;

		protected ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			this.alreadyScanned = false;
			this.alreadyDiscovered = false;
			this.bluetoothService = new BluetoothService();
			this.bluetoothService.DiscoveredDevice += DiscoveredDevice;
			this.bluetoothService.StateChanged += StateChanged;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			this.bluetoothService.DiscoveredDevice -= DiscoveredDevice;
			this.bluetoothService.StateChanged -= StateChanged;
			this.bluetoothService.Dispose();
		}

		private async void StateChanged(object sender, CBCentralManagerState state)
		{
			if (!this.alreadyScanned && state == CBCentralManagerState.PoweredOn)
			{
				try
				{
            		this.alreadyScanned = true;
					var connectedDevice = this.bluetoothService.GetConnectedDevices("180A")
                    	?.FirstOrDefault(x => x.Name.StartsWith(DeviceName, StringComparison.InvariantCulture));

					if (connectedDevice != null)
					{
						this.DiscoveredDevice(this, connectedDevice);
					}
					else
					{
						await this.bluetoothService.Scan(ScanTime);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}

		private async void DiscoveredDevice(object sender, CBPeripheral peripheral)
		{
			if (!this.alreadyDiscovered && peripheral.Name.StartsWith(DeviceName, StringComparison.InvariantCulture))
			{
				try
				{
                    this.alreadyDiscovered = true;

					await this.bluetoothService.ConnectTo(peripheral);

					var service = await this.bluetoothService.GetService(peripheral, "180A");
					if (service != null)
					{
						var characteristics = await this.bluetoothService.GetCharacteristics(peripheral, service, ScanTime);
						foreach (var characteristic in characteristics)
						{
							var value = await this.bluetoothService.ReadValue(peripheral, characteristic);
							Debug.WriteLine($"{characteristic.UUID.Description} = {value}");
						}
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
				finally
				{
					this.bluetoothService.Disconnect(peripheral);
				}
			}
		}
	}
}