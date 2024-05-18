using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Buto
{
    public class ButoVolumetricLightingAndFog : ScriptableRendererFeature
    {
        
        class RenderFogPass : ScriptableRenderPass
        {
            
            private RenderTargetIdentifier source;
            private RenderTextureDescriptor fogTargetDescriptor;
            private RenderTargetHandle fogTarget;
            private RenderTargetHandle finalMergeTarget;
            private RenderTargetHandle lowResDepthTarget;

            private VolumetricFog volumetricFog = null;
            private Material fogMaterial = null;
            private Material mergeMaterial = null;
            private Material depthMaterial = null;
            private const string fogShaderPath = "Shader Graphs/VolumetricFogShader";
            private const string mergeShaderPath = "Shader Graphs/VolumetricFogMergeShader";
            private const string depthDownscaleShaderPath = "Shader Graphs/DepthDownscaleShader";

            private const string mergeInputTexId = "_FOG_MERGE_INPUT_TEX";
            private const string lowResDepthTexId = "_DOWNSCALED_DEPTH_TEX";

            private bool isFirst = true;


            #region Temporal AA
            private const string taaInputTexId = "_BUTO_HISTORY_TEX";
            private Material taaMaterial = null;
            private const string taaShaderPath = "Shader Graphs/ButoTemporalAA";
            private RenderTargetHandle taaTarget;

            Dictionary<Camera, TAACameraData> renderTextures;
            float managedTime;
            uint frameCount;


            class TAACameraData
            {
                private uint lastFrameUsed;
                private RenderTexture renderTexture;
                private string cameraName;

                public TAACameraData(uint lastFrameUsed, RenderTexture renderTexture, string cameraName)
                {
                    LastFrameUsed = lastFrameUsed;
                    RenderTexture = renderTexture;
                    CameraName = cameraName;
                }

                public uint LastFrameUsed
                {
                    get => lastFrameUsed;
                    set => lastFrameUsed = value;
                }

                public RenderTexture RenderTexture
                {
                    get => renderTexture;
                    set => renderTexture = value;
                }

                public string CameraName
                {
                    get => cameraName;
                    set => cameraName = value;
                }
            }


            void CalculateTime()
            {
                // Get data
                float unityRealtimeSinceStartup = Time.realtimeSinceStartup;
                uint unityFrameCount = (uint)Time.frameCount;

                bool newFrame;
                if (Application.isPlaying)
                {
                    newFrame = frameCount != unityFrameCount;
                    frameCount = unityFrameCount;
                }
                else
                {
                    newFrame = (unityRealtimeSinceStartup - managedTime) > 0.0166f;
                    if (newFrame)
                        frameCount++;
                }

                if (newFrame)
                {
                    managedTime = unityRealtimeSinceStartup;
                }
            }

            void GetTemporalAARenderTexture(Camera camera, RenderTextureDescriptor descriptor)
            {
                if (renderTextures.TryGetValue(camera, out TAACameraData cameraData))
                {
                    CheckRenderTextureScale(camera, descriptor, cameraData.RenderTexture);
                }
                else
                {
                    CreateTAARenderTextureAndAddToDictionary(camera, descriptor);
                }
            }

            void CheckRenderTextureScale(Camera camera, RenderTextureDescriptor descriptor, RenderTexture renderTexture)
            {
                if (renderTexture == null)
                {
                    CreateTAARenderTextureAndAddToDictionary(camera, descriptor);
                    return;
                }

                bool rtWrongSize = (renderTexture.width != descriptor.width || renderTexture.height != descriptor.height) ? true : false;
                if (rtWrongSize)
                {
                    CreateTAARenderTextureAndAddToDictionary(camera, descriptor);
                    return;
                }
            }

            void CreateTAARenderTextureAndAddToDictionary(Camera camera, RenderTextureDescriptor descriptor)
            {
                SetupTAARenderTexture(camera, descriptor, out RenderTexture renderTexture);

                if (renderTextures.ContainsKey(camera))
                {
                    if (renderTextures[camera].RenderTexture != null)
                        renderTextures[camera].RenderTexture.Release();

                    renderTextures[camera].RenderTexture = renderTexture;
                }
                else
                {
                    renderTextures.Add(camera, new TAACameraData(frameCount, renderTexture, camera.name));
                }
            }

            void SetupTAARenderTexture(Camera camera, RenderTextureDescriptor descriptor, out RenderTexture renderTexture)
            {
                descriptor.colorFormat = RenderTextureFormat.DefaultHDR;
                renderTexture = new RenderTexture(descriptor);

                RenderTexture activeTexture = RenderTexture.active;
                RenderTexture.active = renderTexture;
                GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 1.0f));
                RenderTexture.active = activeTexture;

                renderTexture.name = camera.name + " Buto TAA History";
                renderTexture.filterMode = FilterMode.Point;
                renderTexture.wrapMode = TextureWrapMode.Clamp;

                renderTexture.Create();
            }

            void CleanupDictionary()
            {
                List<Camera> removeTargets = new List<Camera>();
                foreach (KeyValuePair<Camera, TAACameraData> entry in renderTextures)
                {
                    if (entry.Value.LastFrameUsed < frameCount - 10)
                    {
                        //Debug.Log("Cleaning up unused render texture: " + entry.Value.RenderTexture.name);
                        if (entry.Value.RenderTexture != null)
                            entry.Value.RenderTexture.Release();

                        removeTargets.Add(entry.Key);
                    }
                }

                for (int i = 0; i < removeTargets.Count; i++)
                {
                    renderTextures.Remove(removeTargets[i]);
                }
            }
            #endregion

            internal void SetupMaterials()
            {
                GetShaderAndSetupMaterial(fogShaderPath, ref fogMaterial);
                GetShaderAndSetupMaterial(mergeShaderPath, ref mergeMaterial);
                GetShaderAndSetupMaterial(depthDownscaleShaderPath, ref depthMaterial);
                GetShaderAndSetupMaterial(taaShaderPath, ref taaMaterial);
            }

            void GetShaderAndSetupMaterial(string path, ref Material target)
			{
                if (target != null)
                    return;

                Shader s = Shader.Find(path);
                if (s != null)
                {
                    target = CoreUtils.CreateEngineMaterial(s);
                }
				else
				{
                    Debug.Log("Buto missing shader reference at " + path);
				}
            }

            public RenderFogPass()
            {
                renderTextures = new Dictionary<Camera, TAACameraData>();
               
                fogTarget.Init("Buto Fog Render Target");
                taaTarget.Init("Buto TAA Target");
                lowResDepthTarget.Init("Buto Low Res Depth Target");
                finalMergeTarget.Init("Buto Final Merge Target");
            }


            public bool RegisterStackComponent()
            {
                volumetricFog = VolumeManager.instance.stack.GetComponent<VolumetricFog>();

                if (volumetricFog == null)
                    return false;

                return volumetricFog.IsActive();
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
                descriptor.colorFormat = RenderTextureFormat.DefaultHDR;

                fogTargetDescriptor = descriptor;
                fogTargetDescriptor.width /= 2;
                fogTargetDescriptor.height /= 2;

                cmd.GetTemporaryRT(fogTarget.id, fogTargetDescriptor);
                cmd.GetTemporaryRT(lowResDepthTarget.id, fogTargetDescriptor);
                cmd.GetTemporaryRT(taaTarget.id, fogTargetDescriptor);
                cmd.GetTemporaryRT(finalMergeTarget.id, descriptor);


            }

            internal bool HasAllMaterials()
			{
                if (fogMaterial != null &&
                    depthMaterial != null &&
                    mergeMaterial != null &&
                    taaMaterial != null
                    ) 
                    return true;


                return false;
            }


            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                UnityEngine.Profiling.Profiler.BeginSample("Buto Volumetric Fog");

                CalculateTime();
                CleanupDictionary();

                source = renderingData.cameraData.renderer.cameraColorTarget;
                Camera camera = renderingData.cameraData.camera;

                CommandBuffer cmd = CommandBufferPool.Get("ButoRenderPass");


                Color worldcolor;
                SetMaterialParameters();

                // Samples volumetric fog
                Blit(cmd, source, fogTarget.Identifier(), fogMaterial);
                RenderTargetHandle mergeInput = fogTarget;

                // TAA
				if (IsTaaEnabled())
				{
                    GetTemporalAARenderTexture(renderingData.cameraData.camera, fogTargetDescriptor);
                    if (renderTextures[camera].RenderTexture != null)
                    {
                        taaMaterial.SetTexture(taaInputTexId, renderTextures[camera].RenderTexture);
                    }

                    Blit(cmd, fogTarget.Identifier(), taaTarget.Identifier(), taaMaterial);

                    if (renderTextures[camera].RenderTexture == null)
                    {
                        Debug.Log("Buto Temporal AA Render Texture is missing. Please submit bug report. Missing Texture: " + renderTextures[camera].CameraName + " RT");
                    }
                    else
                    {
                        Blit(cmd, taaTarget.Identifier(), renderTextures[camera].RenderTexture);
                        renderTextures[camera].LastFrameUsed = frameCount;
                    }
                    mergeInput = taaTarget;
                }


                cmd.SetGlobalTexture(mergeInputTexId, mergeInput.Identifier());


                // Sets up low res depth texture
                Blit(cmd, source, lowResDepthTarget.Identifier(), depthMaterial);
                cmd.SetGlobalTexture(lowResDepthTexId, lowResDepthTarget.Identifier());


                // Merges to final merge target
                Blit(cmd, source, finalMergeTarget.Identifier(), mergeMaterial);
                Blit(cmd, finalMergeTarget.Identifier(), source);


				context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
                UnityEngine.Profiling.Profiler.EndSample();


                bool IsTaaEnabled()
				{
                    if (camera.cameraType != CameraType.Game)
                        return false;

                    if (!volumetricFog.temporalAntiAliasingEnabled.value)
                        return false;

                    return true;
				}

                void SetMaterialParameters()
				{
                    worldcolor = SetSphericalHarmonics();
                    SetFogMaterialData();
                    SetAdditionalLightData();
                    SetVolumeData();
                    SetTaaIntegrationRate();

                    void SetFogMaterialData()
					{
                        fogMaterial.SetInt(Params.SampleCount.Id, volumetricFog.sampleCount.value);
                        fogMaterial.SetInt(Params.AnimateSamplePosition.Id, BoolToInt(volumetricFog.animateSamplePosition.value));
                        SetKeyword(fogMaterial, Params.EnableSelfShadowing.Property, volumetricFog.selfShadowingEnabled.value);
                        fogMaterial.SetInt(Params.MaximumSelfShadowingOctaves.Id, volumetricFog.maximumSelfShadowingOctaves.value);
                        SetKeyword(fogMaterial, Params.EnableHorizonShadowing.Property, volumetricFog.horizonShadowingEnabled.value);
                        SetKeyword(fogMaterial, Params.AnalyticFogEnabled.Property, volumetricFog.analyticFogEnabled.value);
                        fogMaterial.SetInt(Params.FogMaskBlendMode.Id, (int)volumetricFog.fogMaskBlendMode.value);

                        fogMaterial.SetFloat(Params.MaxDistanceVolumetric.Id, volumetricFog.maxDistanceVolumetric.value);
                        fogMaterial.SetFloat(Params.MaxDistanceNonVolumetric.Id, volumetricFog.maxDistanceAnalytic.value);
                        fogMaterial.SetFloat(Params.Anisotropy.Id, volumetricFog.anisotropy.value);
                        fogMaterial.SetFloat(Params.BaseHeight.Id, volumetricFog.baseHeight.value);
                        fogMaterial.SetFloat(Params.AttenuationBoundarySize.Id, volumetricFog.attenuationBoundarySize.value);

                        fogMaterial.SetFloat(Params.FogDensity.Id, volumetricFog.fogDensity.value);
                        fogMaterial.SetFloat(Params.LightIntensity.Id, volumetricFog.lightIntensity.value);
                        fogMaterial.SetFloat(Params.ShadowIntensity.Id, volumetricFog.shadowIntensity.value);

                        fogMaterial.SetTexture(Params.ColorRamp.Id, volumetricFog.colorRamp.value);
                        fogMaterial.SetFloat(Params.ColorRampInfluence.Id, volumetricFog.colorRampInfluence.value);

                        fogMaterial.SetTexture(Params.NoiseTexture.Id, volumetricFog.noiseTexure.value);
                        fogMaterial.SetInt(Params.Octaves.Id, volumetricFog.octaves.value);
                        fogMaterial.SetFloat(Params.NoiseTiling.Id, volumetricFog.noiseTiling.value);
                        fogMaterial.SetVector(Params.NoiseWindSpeed.Id, volumetricFog.noiseWindSpeed.value);
                        fogMaterial.SetFloat(Params.NoiseIntensityMin.Id, volumetricFog.noiseMap.value.x);
                        fogMaterial.SetFloat(Params.NoiseIntensityMax.Id, volumetricFog.noiseMap.value.y);
                        fogMaterial.SetFloat(Params.Lacunarity.Id, volumetricFog.lacunarity.value);
                        fogMaterial.SetFloat(Params.Gain.Id, volumetricFog.gain.value);

                        
                    }

                    Color SetSphericalHarmonics()
					{
                        SphericalHarmonicsL2 sh2;
                        LightProbes.GetInterpolatedProbe(camera.transform.position, null, out sh2);

                        Vector3[] directions = new Vector3[]
                        {
                            new Vector3(0.0f, 1.0f, 0.0f),
                        };
                        Color[] results = new Color[1];

                        sh2.Evaluate(directions, results);

                        fogMaterial.SetColor("_WorldColor", results[0]);
                        return results[0];
                    }

                    void SetTaaIntegrationRate()
					{
                        float taaRate = volumetricFog.temporalAntiAliasingIntegrationRate.value;
                        if (isFirst)
                        {
                            
                            isFirst = false;
                            taaRate = 1;
                        }
                        
                        taaMaterial.SetFloat(Params.TemporalAaIntegrationRate.Id, taaRate);
                    }

                    void SetAdditionalLightData()
                    {
                        Vector4[] positions = new Vector4[ButoCommon._MAXLIGHTCOUNT];
                        float[] intensities = new float[ButoCommon._MAXLIGHTCOUNT];
                        Vector4[] colors = new Vector4[ButoCommon._MAXLIGHTCOUNT];

                        if(ButoLight.Lights.Count > ButoCommon._MAXLIGHTCOUNT)
                            ButoLight.SortByDistance(camera.transform.position);

                        int lightCount = Mathf.Min(ButoLight.Lights.Count, ButoCommon._MAXLIGHTCOUNT);
                        for (int i = 0; i < lightCount; i++)
                        {
                            positions[i] = ButoLight.Lights[i].transform.position;
                            intensities[i] = ButoLight.Lights[i].LightIntensity;
                            colors[i] = ButoLight.Lights[i].LightColor;
                        }

                        fogMaterial.SetInt(Params.LightCountButo.Id, lightCount);
                        if (lightCount > 0)
                        {
                            fogMaterial.SetVectorArray(Params.LightPosButo.Id, positions);
                            fogMaterial.SetFloatArray(Params.LightIntensityButo.Id, intensities);
                            fogMaterial.SetVectorArray(Params.LightColorButo.Id, colors);
                        }
                    }

                    void SetVolumeData()
                    {
                        Vector4[] positionsAndRadius = new Vector4[ButoCommon._MAXVOLUMECOUNT];
                        float[] intensities = new float[ButoCommon._MAXVOLUMECOUNT];

                        if(FogDensityMask.FogVolumes.Count > ButoCommon._MAXVOLUMECOUNT)
                            FogDensityMask.SortByDistance(camera.transform.position);

                        int volumeCount = Mathf.Min(FogDensityMask.FogVolumes.Count, ButoCommon._MAXVOLUMECOUNT);
                        for (int i = 0; i < volumeCount; i++)
                        {
                            positionsAndRadius[i] = FogDensityMask.FogVolumes[i].transform.position;
                            positionsAndRadius[i].w = FogDensityMask.FogVolumes[i].Radius;
                            intensities[i] = FogDensityMask.FogVolumes[i].DensityMultiplier;
                        }

                        fogMaterial.SetInt(Params.VolumeCountButo.Id, volumeCount);
                        if (volumeCount > 0)
                        {
                            fogMaterial.SetVectorArray(Params.VolumeData.Id, positionsAndRadius);
                            fogMaterial.SetFloatArray(Params.VolumeIntensity.Id, intensities);
                        }
                    }

                    void SetKeyword(Material m, string keyword, bool value)
                    {
                        if (value)
                        {
                            m.EnableKeyword(keyword);
                        }
                        else
                        {
                            m.DisableKeyword(keyword);
                        }
                    }

                    int BoolToInt(bool a)
                    {
                        return a == false ? 0 : 1;
                    }
                }
            }


            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(fogTarget.id);
                cmd.ReleaseTemporaryRT(taaTarget.id);
                cmd.ReleaseTemporaryRT(lowResDepthTarget.id);
                cmd.ReleaseTemporaryRT(finalMergeTarget.id);

            }
        }

        RenderFogPass renderFogPass;

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += Recreate;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= Recreate;
        }

        private void Recreate(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
        {
            Create();
        }

        public override void Create()
        {
            renderFogPass = new RenderFogPass();
            renderFogPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }


        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.Reflection)
                return;
            

            if (!renderFogPass.RegisterStackComponent())
                return;


            renderFogPass.SetupMaterials();
            if (!renderFogPass.HasAllMaterials())
                return;


            renderFogPass.ConfigureInput(ScriptableRenderPassInput.Motion);

            renderer.EnqueuePass(renderFogPass);
        }




        private static class Params
        {
            public readonly struct Param
            {
                public Param(string property)
                {
                    Property = property;
                    Id = Shader.PropertyToID(property);
                }

                readonly public string Property;
                readonly public int Id;
            }

            public static Param SampleCount = new Param("_SampleCount");
            public static Param AnimateSamplePosition = new Param("_AnimateSamplePosition");
            public static Param EnableSelfShadowing = new Param("_BUTO_SELF_ATTENUATION_ENABLED");
            public static Param EnableHorizonShadowing = new Param("_BUTO_HORIZON_SHADOWING_ENABLED");
            public static Param MaxDistanceVolumetric = new Param("_MaxDistanceVolumetric");
            public static Param AnalyticFogEnabled = new Param("_BUTO_ANALYTIC_FOG_ENABLED");
            public static Param MaxDistanceNonVolumetric = new Param("_MaxDistanceNonVolumetric");
            public static Param MaximumSelfShadowingOctaves = new Param("_MaximumSelfShadowingOctaves");
            public static Param FogMaskBlendMode = new Param("_FogMaskBlendMode");
            
            // TAA Param
            public static Param TemporalAaIntegrationRate = new Param("_IntegrationRate");

            public static Param FogDensity = new Param("_FogDensity");
            public static Param Anisotropy = new Param("_Anisotropy");
            public static Param LightIntensity = new Param("_LightIntensity");
            public static Param ShadowIntensity = new Param("_ShadowIntensity");

            public static Param BaseHeight = new Param("_BaseHeight");
            public static Param AttenuationBoundarySize = new Param("_AttenuationBoundarySize");

            public static Param ColorRamp = new Param("_ColorRamp");
            public static Param ColorRampInfluence = new Param("_ColorRampInfluence");

            public static Param NoiseTexture = new Param("_NoiseTexture");
            public static Param Octaves = new Param("_Octaves");
            public static Param Lacunarity = new Param("_Lacunarity");
            public static Param Gain = new Param("_Gain");
            public static Param NoiseTiling = new Param("_NoiseTiling");
            public static Param NoiseWindSpeed = new Param("_NoiseWindSpeed");
            public static Param NoiseIntensityMin = new Param("_NoiseIntensityMin");
            public static Param NoiseIntensityMax = new Param("_NoiseIntensityMax");


            public static Param LightCountButo = new Param("_LightCountButo");
            public static Param LightPosButo = new Param("_LightPosButo");
            public static Param LightIntensityButo = new Param("_LightIntensityButo");
            public static Param LightColorButo = new Param("_LightColorButo");

            public static Param VolumeCountButo = new Param("_VolumeCountButo");
            public static Param VolumeData = new Param("_VolumeDataButo");
            public static Param VolumeIntensity = new Param("_VolumeIntensityButo");
        }
    }




}