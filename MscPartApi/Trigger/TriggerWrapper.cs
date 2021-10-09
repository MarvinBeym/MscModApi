using UnityEngine;

namespace MscPartApi.Trigger
{
    public class TriggerWrapper
    {
        private Trigger logic;
        private GameObject triggerGameObject;
        private Renderer renderer;
        private static readonly Vector3 defaultScale = new Vector3(0.05f, 0.05f, 0.05f);

        public TriggerWrapper(Part part, GameObject parentGameObject, bool disableCollisionWhenInstalled)
        {
            triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triggerGameObject.transform.SetParent(parentGameObject.transform, false);
            triggerGameObject.name = part.gameObject.name + "_trigger";
            SetPosition(part.installPosition);
            SetRotation(part.installRotation);
            SetScale(defaultScale);

            var collider = triggerGameObject.GetComponent<Collider>();
            collider.isTrigger = true;

            renderer = triggerGameObject.GetComponent<Renderer>();
            SetVisible(false);
            logic = triggerGameObject.AddComponent<Trigger>();
            logic.Init(part, parentGameObject, disableCollisionWhenInstalled);
        }

        public void SetScale(Vector3 scale)
        {
            triggerGameObject.transform.localScale = scale;
        }

        public void SetPosition(Vector3 position)
        {
            triggerGameObject.transform.localPosition = position;
        }

        public void SetRotation(Vector3 rotation)
        {
            triggerGameObject.transform.localRotation = Quaternion.Euler(rotation);
        }

        public void SetVisible(bool show)
        {
            renderer.enabled = show;
        }

        internal void Install() => logic.Install();
        internal void Uninstall() => logic.Uninstall();
    }
}