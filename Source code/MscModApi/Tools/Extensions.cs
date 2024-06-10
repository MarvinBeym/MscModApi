using MscModApi.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HutongGames.PlayMaker;
using MscModApi.Parts.ReplacePart;
using UnityEngine;

namespace MscModApi.Tools
{
	public static class Extensions
	{
		public static bool CompareQuaternion(this Quaternion a, Quaternion b, float tolerance = 0)
		{
			return 1 - Mathf.Abs(Quaternion.Dot(a, b)) < tolerance;
		}

		public static string ToOnOff(this bool value)
		{
			return value.ToXY("On", "Off");
		}

		public static float Map(this float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}

		public static string ToXY(this bool value, string trueText, string falseText)
		{
			return value ? trueText : falseText;
		}

		public static void SetNameLayerTag(this GameObject gameObject, string name, string tag = "PART",
			string layer = "Parts")
		{
			gameObject.name = name;
			gameObject.tag = tag;
			gameObject.layer = LayerMask.NameToLayer(layer);
			gameObject.FixName();
		}

		public static void FixName(this GameObject gameObject)
		{
			gameObject.name = Regex.Replace(
				gameObject.name,
				"\\(Clone\\){1,}", "(Clone)"
			);
		}

		public static bool CompareVector3(this Vector3 vector3, Vector3 other, float tolerance = 0.05f)
		{
			return Math.Abs(vector3.x - other.x) < tolerance && Math.Abs(vector3.y - other.y) < tolerance &&
			       Math.Abs(vector3.z - other.z) < tolerance;
		}

		public static PlayMakerFSM FindFsm(this GameObject gameObject, string fsmName)
		{
			return gameObject.GetComponents<PlayMakerFSM>().FirstOrDefault(fsm => fsm.FsmName == fsmName);
		}

		public static GameObject FindChild(this GameObject gameObject, string childName)
		{
			return gameObject.transform.FindChild(childName)?.gameObject;
		}

		public static void InvokeAll(this List<Action> actions)
		{
			foreach (var action in actions) {
				action.Invoke();
			}
		}

		public static Screw CloneToNew(this Screw screw)
		{
			return new Screw(screw.position, screw.rotation, screw.type, screw.scale, screw.size, screw.showSize);
		}

		public static Screw[] CloneToNew(this Screw[] screws)
		{
			var newScrews = new Screw[screws.Length];
			for (var i = 0; i < screws.Length; i++) {
				var screw = screws[i];
				newScrews[i] = screw.CloneToNew();
			}

			return newScrews;
		}

		/// <summary>
		/// Finds an fsm state by name
		/// </summary>
		/// <param name="fsm">The PlayMakerFSM object to search in</param>
		/// <param name="stateName">The name of the state to find</param>
		/// <returns>The found FsmState or null</returns>
		public static FsmState FindState(this PlayMakerFSM fsm, string stateName)
		{
			foreach (var fsmState in fsm.FsmStates) {
				if (fsmState.Name == stateName) {
					return fsmState;
				}
			}

			return null;
		}

		public static string ToStringOrEmpty(this object value)
		{
			return value == null ? "" : value.ToString();
		}

		/// <summary>
		/// Returns if all parts in the list have the event type fulfilled
		/// </summary>
		/// <param name="parts">The list of parts</param>
		/// <param name="type">The PartEvent.Type to check all parts against</param>
		/// <returns>True if all parts in the list fulfill the type</returns>
		public static bool AllHaveState(this List<BasicPart> parts, PartEvent.Type type)
		{
			switch (type)
			{
				case PartEvent.Type.Install:
					return parts.All(part => part.installed);
				case PartEvent.Type.Uninstall:
					return parts.All(part => !part.installed);
				case PartEvent.Type.Bolted:
					return parts.All(part => part.bolted);
				case PartEvent.Type.Unbolted:
					return parts.All(part => !part.bolted);
				case PartEvent.Type.InstallOnCar:
					return parts.All(part => part.installedOnCar);
				case PartEvent.Type.UninstallFromCar:
					return parts.All(part => !part.installedOnCar);
				case PartEvent.Type.BoltedOnCar:
					return parts.All(part => part.bolted && part.installedOnCar);
				case PartEvent.Type.UnboltedOnCar:
					return parts.All(part => !part.bolted && part.installedOnCar);
			}
			throw new Exception($"Unsupported PartEvent.Type '{type}' used");
		}

		/// <summary>
		/// Returns if all parts in the list have the event type fulfilled
		/// </summary>
		/// <param name="parts">The list of parts</param>
		/// <param name="type">The PartEvent.Type to check all parts against</param>
		/// <returns>True if all parts in the list fulfill the type</returns>
		public static bool AllHaveState(this List<Part> parts, PartEvent.Type type)
		{
			switch (type)
			{
				case PartEvent.Type.Install:
					return parts.All(part => part.installed);
				case PartEvent.Type.Uninstall:
					return parts.All(part => !part.installed);
				case PartEvent.Type.Bolted:
					return parts.All(part => part.bolted);
				case PartEvent.Type.Unbolted:
					return parts.All(part => !part.bolted);
				case PartEvent.Type.InstallOnCar:
					return parts.All(part => part.installedOnCar);
				case PartEvent.Type.UninstallFromCar:
					return parts.All(part => !part.installedOnCar);
				case PartEvent.Type.BoltedOnCar:
					return parts.All(part => part.bolted && part.installedOnCar);
				case PartEvent.Type.UnboltedOnCar:
					return parts.All(part => !part.bolted && part.installedOnCar);
			}
			throw new Exception($"Unsupported PartEvent.Type '{type}' used");
		}

		/// <summary>
		/// Returns if all parts in the list have the event type fulfilled
		/// </summary>
		/// <param name="parts">The list of parts</param>
		/// <param name="type">The PartEvent.Type to check all parts against</param>
		/// <returns>True if all parts in the list fulfill the type</returns>
		public static bool AllHaveState(this IEnumerable<GamePart> parts, PartEvent.Type type)
		{
			switch (type)
			{
				case PartEvent.Type.Install:
					return parts.All(part => part.installed);
				case PartEvent.Type.Uninstall:
					return parts.All(part => !part.installed);
				case PartEvent.Type.Bolted:
					return parts.All(part => part.bolted);
				case PartEvent.Type.Unbolted:
					return parts.All(part => !part.bolted);
				case PartEvent.Type.InstallOnCar:
					return parts.All(part => part.installedOnCar);
				case PartEvent.Type.UninstallFromCar:
					return parts.All(part => !part.installedOnCar);
				case PartEvent.Type.BoltedOnCar:
					return parts.All(part => part.bolted && part.installedOnCar);
				case PartEvent.Type.UnboltedOnCar:
					return parts.All(part => !part.bolted && part.installedOnCar);
			}
			throw new Exception($"Unsupported PartEvent.Type '{type}' used");
		}

		/// <summary>
		/// Returns if any parts in the list have the event type fulfilled
		/// </summary>
		/// <param name="parts">The list of parts</param>
		/// <param name="type">The PartEvent.Type to check all parts against</param>
		/// <returns>True if any part fulfills  the type</returns>
		public static bool AnyHaveState(this IEnumerable<BasicPart> parts, PartEvent.Type type)
		{
			switch (type)
			{
				case PartEvent.Type.Install:
					return parts.Any(part => part.installed);
				case PartEvent.Type.Uninstall:
					return parts.Any(part => !part.installed);
				case PartEvent.Type.Bolted:
					return parts.Any(part => part.bolted);
				case PartEvent.Type.Unbolted:
					return parts.Any(part => !part.bolted);
				case PartEvent.Type.InstallOnCar:
					return parts.Any(part => part.installedOnCar);
				case PartEvent.Type.UninstallFromCar:
					return parts.Any(part => !part.installedOnCar);
				case PartEvent.Type.BoltedOnCar:
					return parts.Any(part => part.bolted && part.installedOnCar);
				case PartEvent.Type.UnboltedOnCar:
					return parts.Any(part => !part.bolted && part.installedOnCar);
			}
			throw new Exception($"Unsupported PartEvent.Type '{type}' used");
		}

		/// <summary>
		/// Returns if any parts in the list have the event type fulfilled
		/// </summary>
		/// <param name="parts">The list of parts</param>
		/// <param name="type">The PartEvent.Type to check all parts against</param>
		/// <returns>True if any part fulfills  the type</returns>
		public static bool AnyHaveState(this List<Part> parts, PartEvent.Type type)
		{
			switch (type)
			{
				case PartEvent.Type.Install:
					return parts.Any(part => part.installed);
				case PartEvent.Type.Uninstall:
					return parts.Any(part => !part.installed);
				case PartEvent.Type.Bolted:
					return parts.Any(part => part.bolted);
				case PartEvent.Type.Unbolted:
					return parts.Any(part => !part.bolted);
				case PartEvent.Type.InstallOnCar:
					return parts.Any(part => part.installedOnCar);
				case PartEvent.Type.UninstallFromCar:
					return parts.Any(part => !part.installedOnCar);
				case PartEvent.Type.BoltedOnCar:
					return parts.Any(part => part.bolted && part.installedOnCar);
				case PartEvent.Type.UnboltedOnCar:
					return parts.Any(part => !part.bolted && part.installedOnCar);
			}
			throw new Exception($"Unsupported PartEvent.Type '{type}' used");
		}

		/// <summary>
		/// Returns if any parts in the list have the event type fulfilled
		/// </summary>
		/// <param name="parts">The list of parts</param>
		/// <param name="type">The PartEvent.Type to check all parts against</param>
		/// <returns>True if any part fulfills  the type</returns>
		public static bool AnyHaveState(this List<GamePart> parts, PartEvent.Type type)
		{
			switch (type)
			{
				case PartEvent.Type.Install:
					return parts.Any(part => part.installed);
				case PartEvent.Type.Uninstall:
					return parts.Any(part => !part.installed);
				case PartEvent.Type.Bolted:
					return parts.Any(part => part.bolted);
				case PartEvent.Type.Unbolted:
					return parts.Any(part => !part.bolted);
				case PartEvent.Type.InstallOnCar:
					return parts.Any(part => part.installedOnCar);
				case PartEvent.Type.UninstallFromCar:
					return parts.Any(part => !part.installedOnCar);
				case PartEvent.Type.BoltedOnCar:
					return parts.Any(part => part.bolted && part.installedOnCar);
				case PartEvent.Type.UnboltedOnCar:
					return parts.Any(part => !part.bolted && part.installedOnCar);
			}
			throw new Exception($"Unsupported PartEvent.Type '{type}' used");
		}
	}
}