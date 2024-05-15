using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace OccaSoftware.Buto
{
    [ExecuteAlways]
    public sealed class FogDensityMask : ButoPlaceableObject
    {

        [SerializeField][Min(0)] private float densityMultiplier = 1;
        public float DensityMultiplier
        {
            get
            {
                return densityMultiplier;
            }
        }


        [SerializeField][Min(0)] private float radius = 10;
        public float Radius
        {
            get
            {
                return radius;
            }
        }


        public static void SortByDistance(Vector3 c)
        {
            fogVolumes = fogVolumes.OrderBy(x => x.GetSqrMagnitude(c)).ToList();
        }

        private static List<FogDensityMask> fogVolumes = new List<FogDensityMask>();
        public static List<FogDensityMask> FogVolumes
        {
            get { return fogVolumes; }
        }

        protected override void Reset()
        {
            ButoCommon.CheckMaxFogVolumeCount(FogVolumes.Count, this);
        }


        protected override void OnEnable()
        {
            fogVolumes.Add(this);
        }

        protected override void OnDisable()
        {
            fogVolumes.Remove(this);
        }

		private void OnDrawGizmosSelected()
		{
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, Radius);
		}
	}
}
