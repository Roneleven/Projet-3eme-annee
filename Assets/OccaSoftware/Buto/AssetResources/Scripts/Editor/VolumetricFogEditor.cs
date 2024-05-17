using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace OccaSoftware.Buto.Editor
{
    [VolumeComponentEditor(typeof(VolumetricFog))]
    public class VolumetricFogEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter sampleCount;
        SerializedDataParameter animateSamplePosition;
        SerializedDataParameter selfShadowingEnabled;
        SerializedDataParameter maximumSelfShadowingOctaves;
        SerializedDataParameter horizonShadowingEnabled;
        SerializedDataParameter maxDistanceVolumetric;
        SerializedDataParameter analyticFogEnabled;
        SerializedDataParameter maxDistanceAnalytic;
        SerializedDataParameter temporalAntiAliasingEnabled;
        SerializedDataParameter temporalAntiAliasingIntegrationRate;
        SerializedDataParameter fogMaskBlendMode;

        SerializedDataParameter fogDensity;
        SerializedDataParameter anisotropy;
        SerializedDataParameter lightIntensity;
        SerializedDataParameter shadowIntensity;

        SerializedDataParameter baseHeight;
        SerializedDataParameter attenuationBoundarySize;

        SerializedDataParameter colorRamp;
        SerializedDataParameter colorRampInfluence;

        SerializedDataParameter noiseTexture;
        SerializedDataParameter octaves;
        SerializedDataParameter lacunarity;
        SerializedDataParameter gain;
        SerializedDataParameter noiseTiling;
        SerializedDataParameter noiseWindSpeed;
        SerializedDataParameter noiseMap;

		public override void OnEnable()
		{
            PropertyFetcher<VolumetricFog> o = new PropertyFetcher<VolumetricFog>(serializedObject);
            mode = Unpack(o.Find(x => x.mode));
            sampleCount = Unpack(o.Find(x => x.sampleCount));
            animateSamplePosition = Unpack(o.Find(x => x.animateSamplePosition));
            selfShadowingEnabled = Unpack(o.Find(x => x.selfShadowingEnabled));
            maximumSelfShadowingOctaves = Unpack(o.Find(x => x.maximumSelfShadowingOctaves));
            horizonShadowingEnabled = Unpack(o.Find(x => x.horizonShadowingEnabled));
            maxDistanceVolumetric = Unpack(o.Find(x => x.maxDistanceVolumetric));
            analyticFogEnabled = Unpack(o.Find(x => x.analyticFogEnabled));
            maxDistanceAnalytic= Unpack(o.Find(x => x.maxDistanceAnalytic));
            temporalAntiAliasingEnabled = Unpack(o.Find(x => x.temporalAntiAliasingEnabled));
            temporalAntiAliasingIntegrationRate = Unpack(o.Find(x => x.temporalAntiAliasingIntegrationRate));
            fogMaskBlendMode = Unpack(o.Find(x => x.fogMaskBlendMode));


            fogDensity = Unpack(o.Find(x => x.fogDensity));
            anisotropy = Unpack(o.Find(x => x.anisotropy));
            lightIntensity = Unpack(o.Find(x => x.lightIntensity));
            shadowIntensity = Unpack(o.Find(x => x.shadowIntensity));


            baseHeight= Unpack(o.Find(x => x.baseHeight));
            attenuationBoundarySize= Unpack(o.Find(x => x.attenuationBoundarySize));


            colorRamp = Unpack(o.Find(x => x.colorRamp));
            colorRampInfluence = Unpack(o.Find(x => x.colorRampInfluence));


            noiseTexture = Unpack(o.Find(x => x.noiseTexure));
            octaves = Unpack(o.Find(x => x.octaves));
            lacunarity= Unpack(o.Find(x => x.lacunarity));
            gain= Unpack(o.Find(x => x.gain));
            noiseTiling= Unpack(o.Find(x => x.noiseTiling));
            noiseWindSpeed= Unpack(o.Find(x => x.noiseWindSpeed));
            noiseMap = Unpack(o.Find(x => x.noiseMap));
        }

		public override void OnInspectorGUI()
		{
            PropertyField(mode);
			if (mode.value.enumValueIndex == ((int)VolumetricFogMode.On))
			{
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent("Quality", "Configure settings that affect the baseline quality of the Volumetric Fog."), EditorStyles.boldLabel);
                PropertyField(sampleCount);
                
                PropertyField(maxDistanceVolumetric);
                PropertyField(analyticFogEnabled);
                if (analyticFogEnabled.value.boolValue)
                {
                    EditorGUI.indentLevel++;
                    PropertyField(maxDistanceAnalytic);
                    EditorGUI.indentLevel--;
                }

                PropertyField(temporalAntiAliasingEnabled);
				if (temporalAntiAliasingEnabled.value.boolValue)
				{
                    PropertyField(temporalAntiAliasingIntegrationRate);
                }
                PropertyField(animateSamplePosition);
                PropertyField(selfShadowingEnabled);
				if (selfShadowingEnabled.value.boolValue)
				{
                    PropertyField(maximumSelfShadowingOctaves);
				}
                PropertyField(horizonShadowingEnabled);
                PropertyField(fogMaskBlendMode);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Characteristics", EditorStyles.boldLabel);
                PropertyField(fogDensity);
                PropertyField(anisotropy);
                PropertyField(lightIntensity);
                PropertyField(shadowIntensity);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Geometry", EditorStyles.boldLabel);
                PropertyField(baseHeight);
                PropertyField(attenuationBoundarySize);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
                PropertyField(colorRamp);
                if(colorRamp.value.objectReferenceValue != null)
				{
                    EditorGUI.indentLevel++;
                    PropertyField(colorRampInfluence);
                    EditorGUI.indentLevel--;
                }
                

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Volumetric Noise", EditorStyles.boldLabel);
                PropertyField(noiseTexture);
                if(noiseTexture.value.objectReferenceValue != null)
				{
                    PropertyField(octaves);
                    if(octaves.value.intValue > 1)
					{
                        EditorGUI.indentLevel++;
                        PropertyField(lacunarity);
                        PropertyField(gain);
                        EditorGUI.indentLevel--;
                    }
                    
                    PropertyField(noiseTiling);
                    PropertyField(noiseWindSpeed);
                    PropertyField(noiseMap);
                }
            }
		}
	}
}
