using UnityEngine;

namespace MscModApi.Saving
{
	public class GamePartSave
	{
		public bool installedOnCar = false;
		public SerializableVector3 position = new SerializableVector3();
		public SerializableQuaternion rotation = new SerializableQuaternion();

		public GamePartSave()
		{

		}

		public GamePartSave(bool installedOnCar, Vector3 position, Quaternion rotation)
		{
			this.installedOnCar = installedOnCar;
			this.position = position;
			this.rotation = rotation;
		}
	}
}