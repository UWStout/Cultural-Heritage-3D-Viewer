// Template source: https://gist.github.com/phi-lira/225cd7c5e8545be602dca4eb5ed111ba 
// When creating shaders for Universal Render Pipeline you can you the ShaderGraph which is super AWESOME!
// However, if you want to author shaders in shading language you can use this teamplate as a base.
// Please note, this shader does not necessarily match perfomance of the built-in URP Lit shader.
// This shader works with URP 7.1.x and above
Shader "Custom/BasisIBR"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

        _SpecGlossMap("Specular", 2D) = "white" {}
        _RoughnessMap("Roughness", 2D) = "gray" {}

        _ReflectanceScale("Reflectance Scale", Range(0.0, 10.0)) = 1.0

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _BasisWeights0123("Weights 0-3", 2D) = "black" {}
        _BasisWeights4567("Weights 4-7", 2D) = "black" {}
        _BasisWeights89AB("Weights 8-11", 2D) = "black" {}
        _BasisFunctions("Basis Functions", 2DArray) = "" {}

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }

    SubShader
    {
        // With SRP we introduce a new "RenderPipeline" tag in Subshader. This allows to create shaders
        // that can match multiple render pipelines. If a RenderPipeline tag is not set it will match
        // any render pipeline. In case you want your subshader to only run in LWRP set the tag to
        // "UniversalRenderPipeline"
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
        LOD 300
        Cull off // TODO For now; to make the tiger hat look OK

        // ------------------------------------------------------------------
        // Forward pass. Shades GI, emission, fog and all lights in a single pass.
        // Compared to Builtin pipeline forward renderer, LWRP forward renderer will
        // render a scene with multiple lights with less drawcalls and less overdraw.
        Pass
        {
            // "Lightmode" tag must be "UniversalForward" or not be defined in order for
            // to render objects.
            Name "StandardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            //Cull[_Cull]  // TODO Commented out for now; to make the tiger hat look OK

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #define _NORMALMAP 1
            #define _METALLICSPECGLOSSMAP 1
            #define _OCCLUSIONMAP 1
            #define _SPECULAR_SETUP 1

            // -------------------------------------
            // Universal Render Pipeline keywords
            // When doing custom shaders you most often want to copy and past these #pragmas
            // These multi_compile variants are stripped from the build depending on:
            // 1) Settings in the LWRP Asset assigned in the GraphicsSettings at build time
            // e.g If you disable AdditionalLights in the asset then all _ADDITIONA_LIGHTS variants
            // will be stripped from build
            // 2) Invalid combinations are stripped. e.g variants with _MAIN_LIGHT_SHADOWS_CASCADE
            // but not _MAIN_LIGHT_SHADOWS are invalid and therefore stripped.
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma require 2darray

            // Including the following two function is enought for shading with Universal Pipeline. Everything is included in them.
            // Core.hlsl will include SRP shader library, all constant buffers not related to materials (perobject, percamera, perframe).
            // It also includes matrix/space conversion functions and fog.
            // Lighting.hlsl will include the light functions/data to abstract light constants. You should use GetMainLight and GetLight functions
            // that initialize Light struct. Lighting.hlsl also include GI, Light BDRF functions. It also includes Shadows.

            // Required by all Universal Render Pipeline shaders.
            // It will include Unity built-in shader variables (except the lighting variables)
            // (https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
            // It will also include many utilitary functions. 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Include this if you are doing a lit shader. This includes lighting shader variables,
            // lighting and shadow functions
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Material shader variables are not defined in SRP or LWRP shader library.
            // This means _BaseColor, _BaseMap, _BaseMap_ST, and all variables in the Properties section of a shader
            // must be defined by the shader itself. If you define all those properties in CBUFFER named
            // UnityPerMaterial, SRP can cache the material properties between frames and reduce significantly the cost
            // of each drawcall.
            // In this case, for sinmplicity LitInput.hlsl is included. This contains the CBUFFER for the material
            // properties defined above. As one can see this is not part of the ShaderLibrary, it specific to the
            // LWRP Lit shader.
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                float2 uvLM         : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float2 uvLM                     : TEXCOORD1;
                float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
                half3  normalWS                 : TEXCOORD3;
                half3 tangentWS                 : TEXCOORD4;
                half3 bitangentWS               : TEXCOORD5;

#ifdef _MAIN_LIGHT_SHADOWS
                float4 shadowCoord              : TEXCOORD6; // compute shadow coord per-vertex for the main light
#endif
                float4 positionCS               : SV_POSITION;
            };

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output;

                // VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space)
                // Our compiler will strip all unused references (say you don't use view space).
                // Therefore there is more flexibility at no additional cost with this struct.
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                // Similar to VertexPositionInputs, VertexNormalInputs will contain normal, tangent and bitangent
                // in world space. If not used it will be stripped.
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                // Computes fog factor per-vertex.
                float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                // TRANSFORM_TEX is the same as the old shader library.
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
                output.normalWS = vertexNormalInput.normalWS;

                // Here comes the flexibility of the input structs.
                // In the variants that don't have normal map defined
                // tangentWS and bitangentWS will not be referenced and
                // GetVertexNormalInputs is only converting normal
                // from object to world space
                output.tangentWS = vertexNormalInput.tangentWS;
                output.bitangentWS = vertexNormalInput.bitangentWS;

#ifdef _MAIN_LIGHT_SHADOWS
                // shadow coord for the main light is computed in vertex.
                // If cascades are enabled, LWRP will resolve shadows in screen space
                // and this coord will be the uv coord of the screen space shadow texture.
                // Otherwise LWRP will resolve shadows in light space (no depth pre-pass and shadow collect pass)
                // In this case shadowCoord will be the position in light space.
                output.shadowCoord = GetShadowCoord(vertexInput);
#endif
                // We just use the homogeneous clip position from the vertex input
                output.positionCS = vertexInput.positionCS;
                return output;
            }

            sampler2D _RoughnessMap;

            // For IBR
            sampler2D _BasisWeights0123;
            sampler2D _BasisWeights4567;
            sampler2D _BasisWeights89AB;
            TEXTURE2D_ARRAY(_BasisFunctions);
            SAMPLER(linear_clamp_sampler_BasisFunctions);

            float _ReflectanceScale;

            #define PI 3.1415926535897932384626433832795 // For convenience
            #define BASIS_COUNT 12

            float3 getMFDistEstimate(float weights[BASIS_COUNT], float nDotH)
            {
                float3 estimate = float3(0, 0, 0);
                float w = sqrt(max(0.0, acos(nDotH) * 3.0 / PI));

                for (int b = 0; b < BASIS_COUNT; b++)
                {
                    estimate += weights[b] * SAMPLE_TEXTURE2D_ARRAY(_BasisFunctions, linear_clamp_sampler_BasisFunctions, float2(w, 0), b).rgb;
                }

                return estimate * PI; // Reflectance functions in Unity appear to be calculated with an additional factor of PI.
            }

            // Based on com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
            // Computes the scalar specular term for Minimalist CookTorrance BRDF
            // NOTE: needs to be multiplied with reflectance f0, i.e. specular color to complete
            half3 DirectBRDFSpecularIBR(BRDFData brdfData, float weights[BASIS_COUNT], half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS)
            {
                float3 halfDir = SafeNormalize(float3(lightDirectionWS) +float3(viewDirectionWS));
                float NoH = saturate(dot(normalWS, halfDir));
                half LoH = saturate(dot(lightDirectionWS, halfDir));

                // Distribution from basis functions multiplied by combined approximation of Visibility and Fresnel
                // BRDFspec = (D * V * F) / 4.0
                // V * F = 1.0 / ( LoH^2 * (roughness + 0.5) )
                // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
                // https://community.arm.com/events/1155

                // Final BRDFspec = D / (LoH^2 * (roughness + 0.5) * 4.0)
                // We further optimize a few light invariant terms
                // brdfData.normalizationTerm = (roughness + 0.5) * 4.0 rewritten as roughness * 4.0 + 2.0 to a fit a MAD.
                half LoH2 = LoH * LoH;
                half3 specularTerm = getMFDistEstimate(weights, NoH) / (max(0.1h, LoH2) * brdfData.normalizationTerm);

                // On platforms where half actually means something, the denominator has a risk of overflow
                // clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
                // sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
                specularTerm = specularTerm - HALF_MIN;
                specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif

                return specularTerm;
            }

            // Based on com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
            half3 LightingPhysicallyBasedIBR(BRDFData brdfData, float weights[BASIS_COUNT],
                half3 lightColor, half3 lightDirectionWS, half lightAttenuation,
                half3 normalWS, half3 viewDirectionWS)
            {
                half NdotL = saturate(dot(normalWS, lightDirectionWS));
                half3 radiance = lightColor * (lightAttenuation * NdotL);

                half3 brdf = brdfData.diffuse;

                // Use IBR for the highlight
                brdf += DirectBRDFSpecularIBR(brdfData, weights, normalWS, lightDirectionWS, viewDirectionWS);

                return step(0.0, NdotL) * brdf * radiance;
            }

            // Based on com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
            half3 LightingPhysicallyBasedIBR(BRDFData brdfData, float weights[BASIS_COUNT], Light light, half3 normalWS, half3 viewDirectionWS)
            {
                return LightingPhysicallyBasedIBR(brdfData, weights, light.color, light.direction, light.distanceAttenuation * light.shadowAttenuation, normalWS, viewDirectionWS);
            }

            half4 LitPassFragment(Varyings input) : SV_Target
            {
                // Surface data contains albedo, metallic, specular, smoothness, occlusion, emission and alpha
                // InitializeStandarLitSurfaceData initializes based on the rules for standard shader.
                // You can write your own function to initialize the surface data of your shader.
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.uv, surfaceData);

                // Override smoothness using our roughness map.
                surfaceData.smoothness = 1.0 - tex2D(_RoughnessMap, input.uv).r;

                half3 normalWS = TransformTangentToWorld(surfaceData.normalTS,
                    half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
                normalWS = normalize(normalWS);

#ifdef LIGHTMAP_ON
                // Normal is required in case Directional lightmaps are baked
                half3 bakedGI = SampleLightmap(input.uvLM, normalWS);
#else
                // Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
                // are also defined in case you want to sample some terms per-vertex.
                half3 bakedGI = SampleSH(normalWS);
#endif

                float3 positionWS = input.positionWSAndFogFactor.xyz;
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);
               
                // BRDFData holds energy conserving diffuse and specular material reflections and its roughness.
                // It's easy to plugin your own shading fuction. You just need replace LightingPhysicallyBased function
                // below with your own.
                BRDFData brdfData;
                InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

                // Light struct is provide by LWRP to abstract light shader variables.
                // It contains light direction, color, distanceAttenuation and shadowAttenuation.
                // LWRP take different shading approaches depending on light and platform.
                // You should never reference light shader variables in your shader, instead use the GetLight
                // funcitons to fill this Light struct.
#ifdef _MAIN_LIGHT_SHADOWS
                // Main light is the brightest directional light.
                // It is shaded outside the light loop and it has a specific set of variables and shading path
                // so we can be as fast as possible in the case when there's only a single directional light
                // You can pass optionally a shadowCoord (computed per-vertex). If so, shadowAttenuation will be
                // computed.
                Light mainLight = GetMainLight(input.shadowCoord);
#else
                Light mainLight = GetMainLight();
#endif

                float visibility = smoothstep(-0.1, 0.0, dot(input.normalWS, viewDirectionWS));

                // Mix diffuse GI with environment reflections.
                half3 color = visibility * GlobalIllumination(brdfData, bakedGI, surfaceData.occlusion, normalWS, viewDirectionWS);

                // Extract weights for IBR
                float4 weights0123 = tex2D(_BasisWeights0123, input.uv);
                float4 weights4567 = tex2D(_BasisWeights4567, input.uv);
                float4 weights89AB = tex2D(_BasisWeights89AB, input.uv);
                float weights[BASIS_COUNT];

                int b;
                for (b = 0; b < 4; b++)
                {
                    weights[b] = weights0123[b];
                }
                for (b = 4; b < 8; b++)
                {
                    weights[b] = weights4567[b - 4];
                }
                for (b = 8; b < 12; b++)
                {
                    weights[b] = weights89AB[b - 8];
                }

                // For debugging:
                //float diff = LightingPhysicallyBasedIBR(brdfData, weights, float3(1, 1, 1), float3(1, 0, 0), 1, float3(1, 0, 0), float3(1, 0, 0))[1]
                //    //- LightingPhysicallyBased(brdfData, float3(1, 1, 1), float3(1, 0, 0), 1, float3(1, 0, 0), float3(1, 0, 0))[1];
                //    - (brdfData.diffuse.g + brdfData.specular.g * 0.25 / (brdfData.roughness2 * (brdfData.roughness + 0.5)));
                //return half4(saturate(0.5 * half3(diff, -diff, -diff)), 1);

                // For debugging:
                //return float4(1-(weights[0] + weights[1] + weights[2] + weights[3] + weights[4] + weights[5] + weights[6] + weights[7]), 0,0, 1);

                // LightingPhysicallyBased computes direct light contribution.
                color += visibility * LightingPhysicallyBasedIBR(brdfData, weights, mainLight, normalWS, viewDirectionWS);
                //color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);

                // For debugging:
                //color = LightingPhysicallyBasedIBR(brdfData, weights, mainLight, normalWS, viewDirectionWS) - LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);
                //return half4(saturate(float3(color.r, -color.gb)), surfaceData.alpha);

                // Additional lights loop
#ifdef _ADDITIONAL_LIGHTS

                // Returns the amount of lights affecting the object being renderer.
                // These lights are culled per-object in the forward renderer
                int additionalLightsCount = GetAdditionalLightsCount();
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    // Similar to GetMainLight, but it takes a for-loop index. This figures out the
                    // per-object light index and samples the light buffer accordingly to initialized the
                    // Light struct. If _ADDITIONAL_LIGHT_SHADOWS is defined it will also compute shadows.
                    Light light = GetAdditionalLight(i, positionWS);

                    // Same functions used to shade the main light.
                    color += visibility * LightingPhysicallyBasedIBR(brdfData, weights, light, normalWS, viewDirectionWS);
                    //color += LightingPhysicallyBased(brdfData, light, normalWS, viewDirectionWS);
                }
#endif

                // Adjust intensity as necessary.
                color *= _ReflectanceScale;

                float fogFactor = input.positionWSAndFogFactor.w;

                // Mix the pixel color with fogColor. You can optionaly use MixFogColor to override the fogColor
                // with a custom one.
                color = MixFog(color, fogFactor);
                return half4(color, surfaceData.alpha);
            }
            ENDHLSL
        }

        // Used for rendering shadowmaps
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"

        // Used for depth prepass
        // If shadows cascade are enabled we need to perform a depth prepass. 
        // We also need to use a depth prepass in some cases camera require depth texture
        // (e.g, MSAA is enabled and we can't resolve with Texture2DMS
        UsePass "Universal Render Pipeline/Lit/DepthOnly"

        // Used for Baking GI. This pass is stripped from build.
        UsePass "Universal Render Pipeline/Lit/Meta"
    }
}