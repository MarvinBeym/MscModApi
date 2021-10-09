#region

using System.Linq;
using MscPartApi.Tools;
using UnityEngine;

#endregion

namespace MscPartApi
{
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
        private Vector3 position;
        private Vector3 rotation;
        internal float scale;
        internal float size;
        private Type type;
        internal GameObject gameObject;
        private MeshRenderer renderer;
        internal int tightness;
        internal bool showSize;
        private Collider collider;
        internal Part part;

        private static Shader textShader;
        public static GameObject nutModel;
        internal static GameObject screwModel;
        internal static GameObject normalModel;
        internal static GameObject longModel;
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public Screw(Vector3 position, Vector3 rotation, float scale = 1, float size = 10, Type type = Type.Normal,
            bool allowShowSize = true)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.size = size;
            this.type = type;

            showSize = allowShowSize;

            if (textShader == null)
            {
                textShader = Shader.Find("GUI/Text Shader");
            }
        }

        internal void LoadTightness(Screw savedScrew)
        {
            if (savedScrew != null)
            {
                tightness = savedScrew.tightness;
            }

            if (tightness >= 8)
            {
                tightness = 8;
            }

            if (tightness <= 0)
            {
                tightness = 0;
            }
        }


        internal void CreateScrewModel(int index)
        {
            switch (type)
            {
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
            gameObject.transform.localPosition = Helper.CopyVector3(position);
            gameObject.transform.localRotation = new Quaternion {eulerAngles = Helper.CopyVector3(rotation)};
            gameObject.transform.localScale = Helper.CopyVector3(new Vector3(scale, scale, scale));
            gameObject.SetActive(true);

            collider = gameObject.GetComponent<Collider>();
            collider.isTrigger = true;

            renderer = gameObject.GetComponentsInChildren<MeshRenderer>(true)[0];
        }

        internal void Verify()
        {
            if (tightness >= maxTightness)
            {
                tightness = maxTightness;
            }

            if (tightness <= 0)
            {
                tightness = 0;
            }

            if (size >= maxSize)
            {
                size = maxSize;
            }

            if (size <= minSize)
            {
                size = minSize;
            }
        }

        internal void SetPart(Part part)
        {
            this.part = part;
        }

        public void In(bool useAudio = true)
        {
            if (tightness >= maxTightness || !part.IsInstalled()) return;

            if (useAudio)
            {
                AudioSource.PlayClipAtPoint(MscPartApi.soundClip, gameObject.transform.position);
            }

            gameObject.transform.Rotate(0, 0, rotationStep);
            gameObject.transform.Translate(0f, 0f, -transformStep);
            tightness++;

            if (tightness == maxTightness)
            {
                if (part.partSave.screws.All(screw => screw.tightness == maxTightness) && !part.IsFixed())
                {
                    part.SetFixed(true);
                }
            }
        }

        public void Out(bool useAudio = true)
        {
            if (!part.IsInstalled() || tightness == 0) return;

            if (useAudio)
            {
                AudioSource.PlayClipAtPoint(MscPartApi.soundClip, gameObject.transform.position);
            }

            gameObject.transform.Rotate(0, 0, -rotationStep);
            gameObject.transform.Translate(0f, 0f, transformStep);
            tightness--;

            if (tightness < maxTightness)
            {
                part.SetFixed(false);
            }
        }

        public void InBy(int by, bool useAudio = false)
        {
            for (var i = 0; i < by; i++)
            {
                In(useAudio);
            }
        }

        public void OutBy(int by, bool useAudio = false)
        {
            for (var i = 0; i < by; i++)
            {
                Out(useAudio);
            }
        }

        internal void Highlight(bool highlight)
        {
            if (highlight)
            {
                renderer.material.shader = textShader;
                renderer.material.SetColor(Color1, Color.green);
            }
            else
            {
                renderer.material = material;
            }
        }
    }
}