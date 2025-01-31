﻿// Template source: https://gist.github.com/phi-lira/225cd7c5e8545be602dca4eb5ed111ba 
// When creating shaders for Universal Render Pipeline you can you the ShaderGraph which is super AWESOME!
// However, if you want to author shaders in shading language you can use this teamplate as a base.
// Please note, this shader does not necessarily match perfomance of the built-in URP Lit shader.
// This shader works with URP 7.1.x and above
Shader "Custom/PTM"
{
    Properties
    {
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _Weight0("Weight0", 2D) = "" {}
        _Weight1("Weight1", 2D) = "" {}
        _Weight2("Weight2", 2D) = "" {}
        _Weight3("Weight3", 2D) = "" {}
        _Weight4("Weight4", 2D) = "" {}
        _Weight5("Weight5", 2D) = "" {}
        _Weight6("Weight6", 2D) = "" {}
        _Weight7("Weight7", 2D) = "" {}
        _Weight8("Weight8", 2D) = "" {}
        _Weight9("Weight9", 2D) = "" {}

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
            Cull[_Cull]

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
            half3  normalOS                 : TEXCOORD4;

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
            output.uv = input.uv; //TRANSFORM_TEX(input.uv, _BaseMap);
            output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

            output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
            output.normalWS = vertexNormalInput.normalWS;

            //// Here comes the flexibility of the input structs.
            //// In the variants that don't have normal map defined
            //// tangentWS and bitangentWS will not be referenced and
            //// GetVertexNormalInputs is only converting normal
            //// from object to world space
            //output.tangentWS = vertexNormalInput.tangentWS;
            //output.bitangentWS = vertexNormalInput.bitangentWS;

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

        sampler2D  _Weight0;
        sampler2D  _Weight1;
        sampler2D  _Weight2;
        sampler2D  _Weight3;
        sampler2D  _Weight4;
        sampler2D  _Weight5;
        sampler2D  _Weight6;
        sampler2D  _Weight7;
        sampler2D  _Weight8;
        sampler2D  _Weight9;

         #define BASIS_COUNT 10

        float3 EvaluatePTM(float3 weights[BASIS_COUNT], float3 lightDirection)
        {
            float3 pixelColor = float3(0, 0, 0);
            float u = -lightDirection.x;  // Unity flips x-coordinate for some reason.
            float v = lightDirection.y;
            float w = lightDirection.z;
            float row[BASIS_COUNT];

            row[0] = 1.0f;
            row[1] = u;
            row[2] = v;
            row[3] = w;
            row[4] = u * u;
            row[5] = v * v;
            row[6] = w * w;
            row[7] = u * v;
            row[8] = u * w;
            row[9] = v * w;

            pixelColor = pixelColor + weights[0] * row[0];
            pixelColor = pixelColor + weights[1] * row[1];
            pixelColor = pixelColor + weights[2] * row[2];
            pixelColor = pixelColor + weights[3] * row[3];
            pixelColor = pixelColor + weights[4] * row[4];
            pixelColor = pixelColor + weights[5] * row[5];
            pixelColor = pixelColor + weights[6] * row[6];
            pixelColor = pixelColor + weights[7] * row[7];
            pixelColor = pixelColor + weights[8] * row[8];
            pixelColor = pixelColor + weights[9] * row[9];

            return pixelColor.xyz;
        }

        // Based on com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
        float3 LightingPTM(float3 weights[BASIS_COUNT], Light light, float3 normalWS)
        {
            // Prevent negative n.l
            float3 adjustedLightDirection = normalize(light.direction - min(0, dot(light.direction, normalWS)) * normalWS);

            return EvaluatePTM(weights, normalize(mul(unity_WorldToObject, float4(adjustedLightDirection, 0)).xyz)) * light.color
                * light.distanceAttenuation * light.shadowAttenuation * smoothstep(-0.5, 0.0, dot(light.direction, normalWS));
        }

        half4 LitPassFragment(Varyings input) : SV_Target
        {
            float3 normalWS = normalize(input.normalWS);

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

            // Extract weights for PTM
            float3 weights[BASIS_COUNT];
            weights[0] = 2 * tex2D(_Weight0, input.uv).rgb - 1;
            weights[1] = 2 * tex2D(_Weight1, input.uv).rgb - 1;
            weights[2] = 2 * tex2D(_Weight2, input.uv).rgb - 1;
            weights[3] = 2 * tex2D(_Weight3, input.uv).rgb - 1;
            weights[4] = 2 * tex2D(_Weight4, input.uv).rgb - 1;
            weights[5] = 2 * tex2D(_Weight5, input.uv).rgb - 1;
            weights[6] = 2 * tex2D(_Weight6, input.uv).rgb - 1;
            weights[7] = 2 * tex2D(_Weight7, input.uv).rgb - 1;
            weights[8] = 2 * tex2D(_Weight8, input.uv).rgb - 1;
            weights[9] = 2 * tex2D(_Weight9, input.uv).rgb - 1;

            // BRDFData holds energy conserving diffuse and specular material reflections and its roughness.
            // It's easy to plugin your own shading fuction. You just need replace LightingPhysicallyBased function
            // below with your own.
            // Fake BRDFData for global illumination -- evaluate PTM at normal direction as an estimate of albedo.
            float3 albedo = EvaluatePTM(weights, normalize(mul(unity_WorldToObject, float4(normalWS, 0)).xyz));
            BRDFData brdfData;
            float alpha = 1.0; // needs to be an l-value
            InitializeBRDFData(albedo, 0.0, float3(0.0, 0.0, 0.0), 0.0, alpha, brdfData);

            // Mix diffuse GI with environment reflections.
            float3 color = GlobalIllumination(brdfData, bakedGI, 1.0, normalWS, viewDirectionWS);

            color += LightingPTM(weights, mainLight, normalWS);

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
                //color += LightingPTM(weights, light, normalWS);
            }
#endif

            float fogFactor = input.positionWSAndFogFactor.w;

            // Mix the pixel color with fogColor. You can optionaly use MixFogColor to override the fogColor
            // with a custom one.
            color = MixFog(color, fogFactor);
            return half4(color, /*surfaceData.alpha*/ 1);
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
