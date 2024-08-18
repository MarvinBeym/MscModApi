using UnityEngine;

namespace MscModApi.Saving
{
	public class GamePartSave
	{
		public bool installed = false;
		public SerializableVector3 position = new SerializableVector3();
		public SerializableQuaternion rotation = new SerializableQuaternion();

		public GamePartSave()
		{

		}

		public GamePartSave(bool installed, Vector3 position, Quaternion rotation)
		{
			this.installed = installed;
			this.position = position;
			this.rotation = rotation;
		}
	}
}