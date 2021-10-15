using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

		public static string ToXY(this bool value, string trueText, string falseText)
		{
			return value ? trueText : falseText;
		}

		public static void SetNameLayerTag(this GameObject gameObject, string name, string tag = "PART", string layer = "Parts")
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
			return Math.Abs(vector3.x - other.x) < tolerance && Math.Abs(vector3.y - other.y) < tolerance && Math.Abs(vector3.z - other.z) < tolerance;
		}

		public static PlayMakerFSM FindFsm(this GameObject gameObject, string fsmName)
		{
			return gameObject.GetComponents<PlayMakerFSM>().FirstOrDefault(fsm => fsm.FsmName == fsmName);
		}

		public static Vector3 CopyVector3(this Vector3 old)
		{
			return new Vector3(old.x, old.y, old.z);
		}

		public static GameObject FindChild(this GameObject gameObject, string childName)
		{
			return gameObject.transform.FindChild(childName)?.gameObject;
		}

		public static void InvokeAll(this List<Action> actions)
		{
			foreach (var action in actions)
			{
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
	}
}