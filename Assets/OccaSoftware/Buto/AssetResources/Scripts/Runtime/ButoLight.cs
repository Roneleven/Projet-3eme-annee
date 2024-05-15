using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace OccaSoftware.Buto
{
    [ExecuteAlways]
    public sealed class ButoLight : ButoPlaceableObject
    {
        [SerializeField] private bool inheritDataFromLightComponent = false;
        [SerializeField] private Light lightComponent = null;

        [SerializeField] [ColorUsage(false, false)] private Color lightColor = UnityEngine.Color.white;
        public Vector4 LightColor
        {
            get 
            {
                if (inheritDataFromLightComponent && lightComponent != null)
                    return lightComponent.color;

                return lightColor; 
            }
        }

        [SerializeField] [Min(0)] private float lightIntensity = 10f;
        public float LightIntensity
        {
            get 
            {
                if (inheritDataFromLightComponent && lightComponent != null)
                    return lightComponent.intensity;

                return lightIntensity; 
            }
        }

        public static void SortByDistance(Vector3 c)
		{
            _Lights = _Lights.OrderBy(x => x.GetSqrMagnitude(c)).ToList();
		}
        
        private static List<ButoLight> _Lights = new List<ButoLight>();
        public static List<ButoLight> Lights
		{
			get { return _Lights; }
		}

        protected override void Reset()
        {
            ButoCommon.CheckMaxLightCount(Lights.Count, this);
        }

        private void OnValidate()
        {
            lightComponent = GetComponent<Light>(); 
        }

        protected override void OnEnable()
        {
            lightComponent = GetComponent<Light>();
            _Lights.Add(this);
        }

        protected override void OnDisable()
        {
            _Lights.Remove(this);
        }

        public void CheckForLight()
        {
            lightComponent = GetComponent<Light>();
        }

        public void SetInheritance(bool state)
        {
            inheritDataFromLightComponent = state;
        }

		private void OnDrawGizmosSelected()
		{
			if (!inheritDataFromLightComponent)
			{
                Gizmos.color = LightColor;
                Gizmos.DrawWireSphere(transform.position, lightIntensity);
            }
		}
	}
}