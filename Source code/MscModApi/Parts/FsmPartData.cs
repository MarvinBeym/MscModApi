using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace MscModApi.Parts
{
	public class FsmPartData
	{
		protected Part part;

		/// <summary>
		/// The name of the PlayMakerFSM component which can be found on the part GameObject
		/// </summary>
		public const string FsmName = "MscModApi.Part.Data";

		/// <summary>
		/// The PlayMakerFSM component added to the parts GameObject
		/// </summary>
		protected PlayMakerFSM playMakerFsmPartData;

		/// <summary>
		/// The cleanName property of the part
		/// </summary>
		protected FsmString cleanName;

		/// <summary>
		/// The installed property of the part
		/// </summary>
		protected FsmBool installed;

		/// <summary>
		/// The installedOnCar property of the part
		/// </summary>
		protected FsmBool installedOnCar;

		/// <summary>
		/// The bolted property of the part
		/// </summary>
		protected FsmBool bolted;

		/// <summary>
		/// The hasBolts property of the part
		/// </summary>
		protected internal FsmBool hasBolts;

		/// <summary>
		/// The bought property of the part
		/// </summary>
		protected internal FsmBool bought;

		/// <summary>
		/// The installBlocked property of the part
		/// </summary>
		protected internal FsmBool installBlocked;

		/// <summary>
		/// The hasParent property of the part
		/// </summary>
		protected FsmBool hasParent;

		/// <summary>
		/// The parent property of the part
		/// </summary>
		protected FsmGameObject parent;

		/// <summary>
		/// The defaultPosition property of the part
		/// </summary>
		protected FsmVector3 defaultPosition;

		/// <summary>
		/// The defaultRotation property of the part
		/// </summary>
		protected FsmVector3 defaultRotation;

		/// <summary>
		/// The installPosition property of the part
		/// </summary>
		protected FsmVector3 installPosition;

		/// <summary>
		/// The installRotation property of the part
		/// </summary>
		protected FsmVector3 installRotation;

		/// <summary>
		/// Setups the part data object for a part
		/// </summary>
		/// <param name="part">The part object</param>
		public FsmPartData(Part part)
		{
			this.part = part;
			playMakerFsmPartData = part.gameObject.AddComponent<PlayMakerFSM>();
			playMakerFsmPartData.FsmName = FsmName;

			//"Constant" variables
			cleanName = AddFsmVariable("cleanName", part.cleanName);
			hasParent = AddFsmVariable("hasParent", part.hasParent);
			parent = AddFsmVariable("parent", part.hasParent ? part.parent.gameObject : null);
			defaultPosition = AddFsmVariable("defaultPosition", part.defaultPosition);
			defaultRotation = AddFsmVariable("defaultRotation", part.defaultRotation);
			installPosition = AddFsmVariable("installPosition", part.installPosition);
			installRotation = AddFsmVariable("installRotation", part.installRotation);

			//Updated through PartEvents
			installed = AddFsmVariable("installed", part.installed);
			installedOnCar = AddFsmVariable("installedOnCar", part.installedOnCar);
			bolted = AddFsmVariable("bolted", part.bolted);

			hasBolts = AddFsmVariable("hasBolts", part.hasBolts);
			bought = AddFsmVariable("bought", part.bought);
			installBlocked = AddFsmVariable("installBlocked", part.installBlocked);

			SetupFsmBoolVariableUpdate(installed, PartEvent.Type.Install);
			SetupFsmBoolVariableUpdate(installedOnCar, PartEvent.Type.InstallOnCar);
			SetupFsmBoolVariableUpdate(bolted, PartEvent.Type.Bolted);
		}

		/// <summary>
		/// Setup simple FsmBool variables that can be set through PartEvents
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="trueEventType"></param>
		protected void SetupFsmBoolVariableUpdate(FsmBool variable, PartEvent.Type trueEventType)
		{
			part.AddEventListener(PartEvent.Time.Post, trueEventType, () =>
			{
				variable.Value = true;
			});

			part.AddEventListener(PartEvent.Time.Post, PartEvent.GetOppositeEvent(trueEventType), () =>
			{
				variable.Value = false;
			});
		}

		/// <summary>
		/// Add a FsmFloat object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmFloat object</returns>
		public FsmFloat AddFsmVariable(string name, float initialValue = 0f)
		{
			var variable = new FsmFloat
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmInt object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmInt object</returns>
		public FsmInt AddFsmVariable(string name, int initialValue = 0)
		{
			var variable = new FsmInt
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmBool object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmBool object</returns>
		public FsmBool AddFsmVariable(string name, bool initialValue = false)
		{
			var variable = new FsmBool
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmGameObject object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmGameObject object</returns>
		public FsmGameObject AddFsmVariable(string name, GameObject initialValue)
		{
			var variable = new FsmGameObject
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmString object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmString object</returns>
		public FsmString AddFsmVariable(string name, string initialValue)
		{
			var variable = new FsmString
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmVector2 object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmVector2 object</returns>
		public FsmVector2 AddFsmVariable(string name, Vector2 initialValue)
		{
			var variable = new FsmVector2
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmVector3 object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmVector3 object</returns>
		public FsmVector3 AddFsmVariable(string name, Vector3 initialValue)
		{
			var variable = new FsmVector3
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmColor object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmColor object</returns>
		public FsmColor AddFsmVariable(string name, Color initialValue)
		{
			var variable = new FsmColor
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmRect object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmRect object</returns>
		public FsmRect AddFsmVariable(string name, Rect initialValue)
		{
			var variable = new FsmRect
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmMaterial object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmMaterial object</returns>
		public FsmMaterial AddFsmVariable(string name, Material initialValue)
		{
			var variable = new FsmMaterial
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmTexture object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmTexture object</returns>
		public FsmTexture AddFsmVariable(string name, Texture initialValue)
		{
			var variable = new FsmTexture
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmQuaternion object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmQuaternion object</returns>
		public FsmQuaternion AddFsmVariable(string name, Quaternion initialValue)
		{
			var variable = new FsmQuaternion
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}

		/// <summary>
		/// Add a FsmObject object to the part data PlayMakerFSM component
		/// </summary>
		/// <param name="name"></param>
		/// <param name="initialValue"></param>
		/// <returns>The created FsmObject object</returns>
		public FsmObject AddFsmVariable(string name, Object initialValue)
		{
			var variable = new FsmObject
			{
				Name = name,
				Value = initialValue
			};
			playMakerFsmPartData.AddVariable(variable);
			return variable;
		}
	}
}