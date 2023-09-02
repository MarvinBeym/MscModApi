using HutongGames.PlayMaker;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Caching
{
	/// <summary>
	/// Utility class for everything related to the car, all cached for high performance.
	/// </summary>
	public class CarH
	{
		private static GameObject _satsuma;
		private static Drivetrain _drivetrain;
		private static AxisCarController _axisController;
		private static CarController _carController;
		private static FsmBool _electricsOk;
		private static GameObject _electricity;
		private static FsmString _playerCurrentVehicle;

		/// <summary>
		/// Returns if the car is currently running (rpm above 20).
		/// </summary>
		public static bool running => drivetrain.rpm > 20;
		/// <summary>
		/// Returns if the player is currently sitting in the car (drive mode).
		/// </summary>
		public static bool playerInCar => playerCurrentVehicle == "Satsuma";

		/// <summary>
		/// Returns the current vehicle the player is in (drive mode).
		/// </summary>
		public static string playerCurrentVehicle
		{
			get
			{
				if (_playerCurrentVehicle != null) return _playerCurrentVehicle.Value;
				_playerCurrentVehicle = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle");

				return _playerCurrentVehicle.Value;
			}
		}

		/// <summary>
		/// Returns the cars electricity object.
		/// </summary>
		public static GameObject electricity
		{
			get
			{
				if (_electricity != null) return _electricity;
				_electricity = satsuma.FindChild("Electricity").gameObject;

				return _electricity;
			}
		}

		/// <summary>
		/// Returns if the cars power is currently on.
		/// </summary>
		public static bool hasPower
		{
			get
			{
				if (_electricsOk != null) return _electricsOk.Value;
				var carElectricsPower = PlayMakerFSM.FindFsmOnGameObject(electricity, "Power");
				_electricsOk = carElectricsPower.FsmVariables.FindFsmBool("ElectricsOK");
				return _electricsOk.Value;
			}
		}

		/// <summary>
		/// Returns the UnityCar CarController object of the satsuma.
		/// </summary>
		public static CarController carController
		{
			get
			{
				if (_carController != null) return _carController;
				_carController = satsuma.GetComponent<CarController>();

				return _carController;
			}
		}

		/// <summary>
		/// Returns the UnityCar AxisCarController object of the satsuma.
		/// </summary>
		public static AxisCarController axisCarController
		{
			get
			{
				if (_axisController != null) return _axisController;
				_axisController = satsuma.GetComponent<AxisCarController>();

				return _axisController;
			}
		}

		/// <summary>
		/// Returns the UnityCar Drivetrain object of the satsuma.
		/// </summary>
		public static Drivetrain drivetrain
		{
			get
			{
				if (_drivetrain != null) return _drivetrain;
				_drivetrain = satsuma.GetComponent<Drivetrain>();

				return _drivetrain;
			}
		}

		/// <summary>
		/// Returns the satsuma GameObject object.
		/// </summary>
		public static GameObject satsuma
		{
			get
			{
				if (_satsuma != null) return _satsuma;
				_satsuma = Cache.Find("SATSUMA(557kg, 248)");

				return _satsuma;
			}
		}

		/// <summary>
		/// Called when the MscModApi mod loads to cleanup static data
		/// </summary>
		public static void LoadCleanup()
		{
			_satsuma = null;
			_satsuma = null;
			_drivetrain = null;
			_axisController = null;
			_carController = null;
			_electricsOk = null;
			_electricity = null;
			_playerCurrentVehicle = null;
		}
	}
}