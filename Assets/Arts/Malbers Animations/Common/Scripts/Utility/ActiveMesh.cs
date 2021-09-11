using UnityEngine;
using System;

namespace MalbersAnimations.Utilities
{
    [Serializable]
    public class ActiveMesh : MonoBehaviour
    {
        public string Name = "NameHere";
        public Transform[] meshes;
        [SerializeField]
        protected int current;

        public virtual void ToogleMesh()
        {
            foreach (var item in meshes)
            {
                if (item) item.gameObject.SetActive(false);
            }
            current++;
            if (current >= meshes.Length) current = 0;
            if (meshes[current]) meshes[current].gameObject.SetActive(true);
        }
    }
}