using HutongGames.PlayMaker;
using UnityEngine;

namespace MscPartApi.Caching
{
	public class Car
	{
		private static GameObject _satsuma;
		private static Drivetrain _drivetrain;
		private static AxisCarController _axisController;
		private static CarController _carController;
		private static FsmBool _electricsOk;
		private static GameObject _electricity;

		public static bool running => drivetrain.rpm > 0;

		public static GameObject electricity {
			get
			{
				if (_electricity != null) return _electricity;
				_electricity = _satsuma.transform.FindChild("Electricity").gameObject;

				return _electricity;
			}
		}

		public static bool hasPower {
			get
			{
				if (_electricsOk != null) return _electricsOk.Value;
				var carElectricsPower = PlayMakerFSM.FindFsmOnGameObject(electricity, "Power");
				_electricsOk = carElectricsPower.FsmVariables.FindFsmBool("ElectricsOK");
				return _electricsOk.Value;
			}
		}

		public static CarController carController {
			get
			{
				if (_carController != null) return _carController;
				_carController = _satsuma.GetComponent<CarController>();

				return _carController;
			}
		}

		public static AxisCarController axisCarController {
			get
			{
				if (_axisController != null) return _axisController;
				_axisController = _satsuma.GetComponent<AxisCarController>();

				return _axisController;
			}
		}

		public static Drivetrain drivetrain {
			get
			{
				if (_drivetrain != null) return _drivetrain;
				_drivetrain = _satsuma.GetComponent<Drivetrain>();

				return _drivetrain;
			}
		}

		public static GameObject satsuma {
			get
			{
				if (_satsuma != null) return _satsuma;
				_satsuma = Cache.Find("SATSUMA(557kg, 248)");

				return _satsuma;
			}
		}
	}
}