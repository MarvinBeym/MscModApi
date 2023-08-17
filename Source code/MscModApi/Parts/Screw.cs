using MscModApi.Tools;
using Newtonsoft.Json;
using System.Linq;
using MscModApi.Parts;
using UnityEngine;

namespace MscModApi.Parts
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Screw
	{
		internal const float minSize = 5;
		internal const float maxSize = 15;
		internal const int maxTightness = 8;
		internal const int rotationStep = 360 / maxTightness;
		internal const float transformStep = 0.0025f;

		public enum Type
		{
			Nut,
			Screw,
			Normal,
			Long
		}

		internal Collider parentCollider;

		internal static Material material;
		internal Vector3 position;
		internal Vector3 rotation;
		internal float scale;
		internal float size;
		internal Type type;
		internal GameObject gameObject;
		private MeshRenderer renderer;

		[JsonProperty] internal int tightness;

		internal bool showSize;
		private Collider collider;
		internal Part part;

		private static Shader textShader;
		public static GameObject nutModel;
		internal static GameObject screwModel;
		internal static GameObject normalModel;
		internal static GameObject longModel;
		private static int color1;
		internal static AudioClip soundClip;

		public Screw(Vector3 position, Vector3 rotation, Type type = Type.Normal, float scale = 1, float size = 10,
			bool allowShowSize = true)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
			this.size = size;
			this.type = type;

			showSize = allowShowSize;

			if (textShader == null) {
				textShader = Shader.Find("GUI/Text Shader");
			}
		}

		internal void LoadTightness(Screw savedScrew)
		{
			if (savedScrew != null) {
				tightness = savedScrew.tightness;
			}

			if (tightness >= 8) {
				tightness = 8;
			}

			if (tightness <= 0) {
				tightness = 0;
			}
		}


		internal void CreateScrewModel(int index)
		{
			switch (type) {
				case Type.Nut:
					gameObject = GameObject.Instantiate(nutModel);
					break;
				case Type.Screw:
					gameObject = GameObject.Instantiate(screwModel);
					break;
				case Type.Long:
					gameObject = GameObject.Instantiate(longModel);
					break;
				default:
					gameObject = GameObject.Instantiate(normalModel);
					break;
			}

			gameObject.SetNameLayerTag($"{parentCollider.gameObject.name}_screw_{index}", "PART", "DontCollide");

			gameObject.transform.SetParent(parentCollider.transform);
			gameObject.transform.localPosition = position.CopyVector3();
			gameObject.transform.localRotation = new Quaternion { eulerAngles = rotation.CopyVector3() };
			gameObject.transform.localScale = new Vector3(scale, scale, scale);
			gameObject.SetActive(true);

			collider = gameObject.GetComponent<Collider>();
			collider.isTrigger = true;

			renderer = gameObject.GetComponentsInChildren<MeshRenderer>(true)[0];
		}

		internal void Verify()
		{
			if (tightness >= maxTightness) {
				tightness = maxTightness;
			}

			if (tightness <= 0) {
				tightness = 0;
			}

			if (size >= maxSize) {
				size = maxSize;
			}

			if (size <= minSize) {
				size = minSize;
			}
		}

		internal void SetPart(Part part)
		{
			this.part = part;
		}

		public void In(bool useAudio = true)
		{
			if (tightness >= maxTightness || !part.installed) return;

			if (useAudio) {
				AudioSource.PlayClipAtPoint(soundClip, gameObject.transform.position);
			}

			gameObject.transform.Rotate(0, 0, rotationStep);
			gameObject.transform.Translate(0f, 0f, -transformStep);

			bool changingToFixedState = false;
			if (tightness + 1 == maxTightness) {
				int screwCount = part.screws.Count;
				int totalTightness = 0;
				part.screws.ForEach((Screw screw) => { totalTightness += screw.tightness; });

				if (totalTightness + 1 == screwCount * maxTightness) {
					changingToFixedState = true;
				}
			}

			if (changingToFixedState) {
				part.GetEvents(PartEvent.EventTime.Pre, PartEvent.EventType.Bolted).InvokeAll();
				if (part.installedOnCar)
				{
					part.GetEvents(PartEvent.EventTime.Pre, PartEvent.EventType.BoltedOnCar).InvokeAll();

					foreach (Part childPart in part.GetChilds())
					{
						if (childPart.bolted && childPart.installedOnCar)
						{
							childPart.GetEvents(PartEvent.EventTime.Pre, PartEvent.EventType.BoltedOnCar).InvokeAll();
						}
					}
				}
			}

			tightness++;

			if (changingToFixedState) {
				part.GetEvents(PartEvent.EventTime.Post, PartEvent.EventType.Bolted).InvokeAll();
				if (part.installedOnCar)
				{
					part.GetEvents(PartEvent.EventTime.Post, PartEvent.EventType.BoltedOnCar).InvokeAll();

					foreach (Part childPart in part.GetChilds())
					{
						if (childPart.bolted && childPart.installedOnCar)
						{
							childPart.GetEvents(PartEvent.EventTime.Post, PartEvent.EventType.BoltedOnCar).InvokeAll();
						}
					}
				}
			}
		}

		public void Out(bool useAudio = true)
		{
			if (!part.installed || tightness == 0) return;

			if (useAudio) {
				AudioSource.PlayClipAtPoint(soundClip, gameObject.transform.position);
			}

			gameObject.transform.Rotate(0, 0, -rotationStep);
			gameObject.transform.Translate(0f, 0f, transformStep);

			bool changingToUnfixed = part.bolted;

			if (changingToUnfixed) {
				part.GetEvents(PartEvent.EventTime.Pre, PartEvent.EventType.Unbolted).InvokeAll();
				if (part.installedOnCar)
				{
					part.GetEvents(PartEvent.EventTime.Pre, PartEvent.EventType.UnboltedOnCar).InvokeAll();

					foreach (Part childPart in part.GetChilds())
					{
						if (!childPart.bolted && childPart.installedOnCar)
						{
							childPart.GetEvents(PartEvent.EventTime.Pre, PartEvent.EventType.UnboltedOnCar).InvokeAll();
						}
					}
				}
			}

			tightness--;

			if (changingToUnfixed) {
				part.GetEvents(PartEvent.EventTime.Post, PartEvent.EventType.Unbolted).InvokeAll();
				if (part.installedOnCar)
				{
					part.GetEvents(PartEvent.EventTime.Post, PartEvent.EventType.UnboltedOnCar).InvokeAll();

					foreach (Part childPart in part.GetChilds())
					{
						if (!childPart.bolted && childPart.installedOnCar)
						{
							childPart.GetEvents(PartEvent.EventTime.Post, PartEvent.EventType.UnboltedOnCar).InvokeAll();
						}
					}
				}
			}
		}

		public void InBy(int by, bool useAudio = false, bool setTightnessToZero = false)
		{
			if (setTightnessToZero) {
				tightness = 0;
			}

			for (var i = 0; i < by; i++) {
				In(useAudio);
			}
		}

		public void OutBy(int by, bool useAudio = false, bool setTightnessToZero = false)
		{
			if (setTightnessToZero) {
				tightness = 0;
			}

			for (var i = 0; i < by; i++) {
				Out(useAudio);
			}
		}

		internal void Highlight(bool highlight)
		{
			if (highlight) {
				renderer.material.shader = textShader;
				renderer.material.SetColor(color1, Color.green);
			}
			else {
				renderer.material = material;
			}
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			material = assetBundle.LoadAsset<Material>("screw_material.mat");
			soundClip = assetBundle.LoadAsset<AudioClip>("screw_sound.wav");
			nutModel = assetBundle.LoadAsset<GameObject>("nut.prefab");
			screwModel = assetBundle.LoadAsset<GameObject>("screw.prefab");
			normalModel = assetBundle.LoadAsset<GameObject>("screw_normal.prefab");
			longModel = assetBundle.LoadAsset<GameObject>("screw_long.prefab");
		}

		public static void LoadCleanup()
		{
			material = null;
			textShader = null;
			nutModel = null;
			screwModel = null;
			normalModel = null;
			longModel = null;
			color1 = Shader.PropertyToID("_Color");
			soundClip = null;
		}
	}
}