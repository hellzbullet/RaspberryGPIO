using System;
using System.Collections.Generic;
using System.Linq;
using RaspberryGPIO.GPIO;
using SolarSystemDatabase.Database;
using SolarSystemDatabase.Models;

namespace SolarSystem.Utils
{
	public class PowerOutput
	{
		public static void Output(int power)
		{
			var factory = NHibernateConfig.SessionFactory.Value;

			using (var session = factory.OpenSession()) {
				using (var transaction = session.BeginTransaction()) {
					var devices = session.CreateCriteria<Device>().List<Device>().ToList();

					foreach (var device in devices.Where(x => x.IsUserControlled == false)) 
					{
						device.Stop(false);
					}

					foreach (var optimalDevice in GetOptimalDevices(devices.Where(x => x.IsUserControlled == false).ToList(), power))
					{
						optimalDevice.Start(false);
					}

					foreach (var device in devices) {
						var outputPin = GPIOPinFactory.Instance.Value.CreateOutputPin((PhysicalPin) device.Pin);
						outputPin.Write(device.IsRunning);
						session.SaveOrUpdate(device);
					}

					session.Save(new Power() { Date = DateTime.Now.Ticks, PowerValue = power, UsedPowerValue = devices.Where(x => x.IsRunning).Sum(x => x.DevicePower) });

					transaction.Commit();
				}
			}
		}

		private static List<Device> GetOptimalDevices(List<Device> devices, int power)
		{
			// Subtract user controlled devices values from the current power
			var remainingPower = devices.Where(x => x.IsUserControlled && x.IsRunning).Aggregate(power, (current, device) => current - device.DevicePower);

			// Add devices to dictionary by priority
			var priorityList = new Dictionary<int, List<Device>>();
			foreach (var device in devices.Where(x => x.IsUserControlled == false))
			{
				if (!priorityList.ContainsKey(device.Priority))
				{
					priorityList[device.Priority] = new List<Device>();
				}
				priorityList[device.Priority].Add(device);
			}

			var optimalDevices = new List<Device>();
			// Get the optimal power solution
			foreach (var key in priorityList.Keys.OrderByDescending(x => x))
			{
				var optimalForPriority = FindOptimalDevices(priorityList[key], remainingPower);
				remainingPower -= optimalForPriority.Sum(x => x.DevicePower);
				optimalDevices.AddRange(optimalForPriority);
			}

			return optimalDevices;
		} 

		private static List<Device> FindOptimalDevices(List<Device> devices, int remainingPower)
		{
			var optimalDevices = new List<Device>();
			FindOptimalDevices(devices, remainingPower, optimalDevices, null, 0);
			return optimalDevices;
		}

		private static void FindOptimalDevices(IReadOnlyList<Device> devices, int remainingPower, List<Device> optimalDevices, List<Device> currentSolution, int index)
		{
			if (optimalDevices == null) optimalDevices = new List<Device>();
			if (currentSolution == null) currentSolution = new List<Device>();

			var solution = new List<Device>(currentSolution);
		
			if (index >= devices.Count) {
				if (solution.Sum(x => x.DevicePower) > optimalDevices.Sum(x => x.DevicePower)) {
					optimalDevices.Clear();
					optimalDevices.AddRange(solution);
				}
				return;
			}
		
			var pwrRemaining = remainingPower;
			for (var i = index; i < devices.Count; i++) {
				if (devices[i].DevicePower <= pwrRemaining && solution.Contains(devices[i]) == false) {
					solution.Add(devices[i]);
					pwrRemaining -= devices[i].DevicePower;
				}
				FindOptimalDevices(devices, pwrRemaining, optimalDevices, solution, index + 1);
			}
		}
	}
}
