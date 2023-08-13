using HutongGames.PlayMaker;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Caching
{
	public class CarH
	{
		private static GameObject _satsuma;
		private static Drivetrain _drivetrain;
		private static AxisCarController _axisController;
		private static CarController _carController;
		private static FsmBool _electricsOk;
		private static GameObject _electricity;
		private static FsmString _playerCurrentVehicle;

		public static bool running => drivetrain.rpm > 20;
		public static bool playerInCar => playerCurrentVehicle == "Satsuma";

		public static string playerCurrentVehicle
		{
			get
			{
				if (_playerCurrentVehicle != null) return _playerCurrentVehicle.Value;
				_playerCurrentVehicle = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle");

				return _playerCurrentVehicle.Value;
			}
		}

		public static GameObject electricity
		{
			get
			{
				if (_electricity != null) return _electricity;
				_electricity = satsuma.FindChild("Electricity").gameObject;

				return _electricity;
			}
		}

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

		public static CarController carController
		{
			get
			{
				if (_carController != null) return _carController;
				_carController = satsuma.GetComponent<CarController>();

				return _carController;
			}
		}

		public static AxisCarController axisCarController
		{
			get
			{
				if (_axisController != null) return _axisController;
				_axisController = satsuma.GetComponent<AxisCarController>();

				return _axisController;
			}
		}

		public static Drivetrain drivetrain
		{
			get
			{
				if (_drivetrain != null) return _drivetrain;
				_drivetrain = satsuma.GetComponent<Drivetrain>();

				return _drivetrain;
			}
		}

		public static GameObject satsuma
		{
			get
			{
				if (_satsuma != null) return _satsuma;
				_satsuma = Cache.Find("SATSUMA(557kg, 248)");

				return _satsuma;
			}
		}

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