using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OccaSoftware.Buto.Demo.Runtime
{

	[ExecuteAlways]
	public class LightIntensityByRotation : MonoBehaviour
	{
		Light l;
		[SerializeField] float intensity = 1.0f;

		// Update is called once per frame
		void Update()
		{
			if (l == null)
				l = GetComponent<Light>();

			float angle = transform.rotation.eulerAngles.x;
			if (Mathf.Abs(angle) > 90)
				angle = 0;

			angle = Mathf.Clamp(angle, 0, 90);
			angle /= 90;
			l.intensity = Mathf.Lerp(0.001f, intensity, angle);
		}
	}

}