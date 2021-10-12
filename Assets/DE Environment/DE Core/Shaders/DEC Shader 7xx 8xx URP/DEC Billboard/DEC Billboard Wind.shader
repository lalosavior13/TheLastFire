// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DEC/Billboard/Billboard Wind"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Header(DEBUG SETTINGS)][Enum(Off,0,On,1)]_ZWriteMode("ZWrite Mode", Int) = 1
		[Enum(None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15)]_ColorMask("Color Mask Mode", Int) = 15
		[Header(GLOBAL SETTINGS)][Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Int) = 0
		[Enum(Default,0,Flip,1,Mirror,2)]_NormalMode("Normal Mode", Int) = 0
		[Enum(Off,0,Active,1)]_GlancingClipMode("Clip Glancing Angle Mode", Int) = 1
		[Header(MAP MAIN TEXTURE)]_Color("Albedo Tint", Color) = (1,1,1,1)
		[SingleLineTexture]_MainTex("Albedo Map", 2D) = "white" {}
		_Brightness("Brightness", Range( 0 , 2)) = 1.5
		_AlphaCutoffBias("Alpha Cutoff Bias", Range( 0 , 1)) = 0.5
		_AlphaCutoffBiasShadow("Alpha Cutoff Bias Shadow", Range( 0.01 , 1)) = 0.5
		_TilingX("Tiling X", Float) = 1
		_TilingY("Tiling Y", Float) = 1
		_OffsetX("Offset X", Float) = 0
		_OffsetY("Offset Y", Float) = 0
		[Normal][SingleLineTexture]_BumpMap("Normal Map", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Float) = 1
		_MetallicStrength("Metallic Strength", Range( 0 , 1)) = 0
		[Enum(Default,0,Baked,1)]_OcclusionSourceMode("Occlusion Source Mode", Int) = 1
		_OcclusionStrengthAO("Occlusion Strength", Range( 0 , 1)) = 0
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 1)) = 0.1
		[Enum(Off,0,Global,1,Local,2)][Header(WIND)]_WindMode("Wind Mode", Int) = 1
		[Header(WIND MODE GLOBAL)]_GlobalWindInfluenceBillboard("Wind Strength", Float) = 1
		[Header(WIND MODE LOCAL)]_LocalWindStrength("Wind Strength", Float) = 1
		_LocalWindPulse("Wind Pulse", Float) = 0.1
		_LocalWindDirection("Wind Direction", Float) = 1
		[ASEEnd]_LocalRandomWindOffset("Wind Random Offset", Float) = 0.2

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry-10" "NatureRendererInstancing"="True" }
		Cull [_CullMode]
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 2.0

		#pragma prefer_hlslcc gles
		


		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" "NatureRendererInstancing"="True" }
			
			Blend One Zero, One Zero
			ZWrite [_ZWriteMode]
			ZTest LEqual
			Offset 0 , 0
			ColorMask [_ColorMask]
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#define _ALPHATEST_SHADOW_ON 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70503
			#define ASE_USING_SAMPLING_MACROS 1

			
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd
			#pragma multi_compile GPU_FRUSTUM_ON __
			#include "Nature Renderer.cginc"
			#pragma multi_compile_local _ NATURE_RENDERER


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			int _CullMode;
			int _GlancingClipMode;
			half _OcclusionStrengthAO;
			int _OcclusionSourceMode;
			float _SmoothnessStrength;
			half _MetallicStrength;
			int _NormalMode;
			half _NormalStrength;
			half _Brightness;
			float _OffsetY;
			float _OffsetX;
			float _TilingY;
			float _TilingX;
			float _LocalWindDirection;
			float _LocalWindPulse;
			float _LocalRandomWindOffset;
			float _LocalWindStrength;
			float _GlobalWindInfluenceBillboard;
			int _WindMode;
			int _ZWriteMode;
			int _ColorMask;
			half _AlphaCutoffBias;
			half _AlphaCutoffBiasShadow;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			int _Global_Wind_Main_Fade_Enabled;
			int _Global_Wind_Billboard_Enabled;
			float _Global_Wind_Billboard_Intensity;
			float _Global_Wind_Main_Intensity;
			float _Global_Wind_Main_RandomOffset;
			float _Global_Wind_Main_Pulse;
			float _Global_Wind_Main_Direction;
			float _Global_Wind_Main_Fade_Bias;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_trilinear_repeat);
			TEXTURE2D(_BumpMap);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float floatswitch2460_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2468_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2312_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2456_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 float3switch2465_g3278( int m_switch, float3 m_Off, float3 m_On )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_On;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2328_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2453_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch3050_g3278( int m_switch, float3 m_Off, float3 m_ActiveFadeOut, float3 m_ActiveFadeIn )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_ActiveFadeOut;
				else if(m_switch ==2)
					return m_ActiveFadeIn;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch56_g39555( int m_switch, float3 m_Default, float3 m_Flip, float3 m_Mirror )
			{
				if(m_switch ==0)
					return m_Default;
				else if(m_switch ==1)
					return m_Flip;
				else if(m_switch ==2)
					return m_Mirror;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch37_g39555( int m_switch, float3 m_Default, float3 m_Flip, float3 m_Mirror )
			{
				if(m_switch ==0)
					return m_Default;
				else if(m_switch ==1)
					return m_Flip;
				else if(m_switch ==2)
					return m_Mirror;
				else
				return float3(0,0,0);
			}
			
			float3 AdditionalLightsFlat( float3 WorldPosition )
			{
				float3 Color = 0;
				#ifdef _ADDITIONAL_LIGHTS
				int numLights = GetAdditionalLightsCount();
				for(int i = 0; i<numLights;i++)
				{
					Light light = GetAdditionalLight(i, WorldPosition);
					Color += light.color *(light.distanceAttenuation * light.shadowAttenuation);
					
				}
				#endif
				return Color;
			}
			
			float floatswitch609_g37836( int m_switch, float m_Default, float m_Baked )
			{
				if(m_switch ==0)
					return m_Default;
				else if(m_switch ==1)
					return m_Baked;
				else
				return float(0);
			}
			
			float floatswitch500_g37836( int m_switch, float m_Off, float m_Active )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Active;
				else
				return float(0);
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				int m_switch3050_g3278 = _Global_Wind_Main_Fade_Enabled;
				int m_switch2453_g3278 = _WindMode;
				float3 m_Off2453_g3278 = float3(0,0,0);
				int _WIND_MODE2462_g3278 = _WindMode;
				int m_switch2328_g3278 = _WIND_MODE2462_g3278;
				float3 VERTEX_POSITION_MATRIX2352_g3278 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz , 0.0 ) ).xyz;
				float3 m_Off2328_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2465_g3278 = _Global_Wind_Billboard_Enabled;
				float3 m_Off2465_g3278 = float3(0,0,0);
				float3 break2265_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2460_g3278 = _WIND_MODE2462_g3278;
				float m_Off2460_g3278 = 1.0;
				float m_Global2460_g3278 = ( ( _GlobalWindInfluenceBillboard * _Global_Wind_Billboard_Intensity ) * _Global_Wind_Main_Intensity );
				float m_Local2460_g3278 = _LocalWindStrength;
				float localfloatswitch2460_g3278 = floatswitch2460_g3278( m_switch2460_g3278 , m_Off2460_g3278 , m_Global2460_g3278 , m_Local2460_g3278 );
				float _WIND_STRENGHT2400_g3278 = localfloatswitch2460_g3278;
				int m_switch2468_g3278 = _WIND_MODE2462_g3278;
				float m_Off2468_g3278 = 1.0;
				float m_Global2468_g3278 = _Global_Wind_Main_RandomOffset;
				float m_Local2468_g3278 = _LocalRandomWindOffset;
				float localfloatswitch2468_g3278 = floatswitch2468_g3278( m_switch2468_g3278 , m_Off2468_g3278 , m_Global2468_g3278 , m_Local2468_g3278 );
				float4 transform3073_g3278 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g3278 = (float2(transform3073_g3278.x , transform3073_g3278.z));
				float dotResult2341_g3278 = dot( appendResult2307_g3278 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g3278 = lerp( 0.8 , ( ( localfloatswitch2468_g3278 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g3278 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g3278 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g3278 );
				float _WIND_TUBULENCE_RANDOM2274_g3278 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 15.0 ) ) ) * 0.5 );
				int m_switch2312_g3278 = _WIND_MODE2462_g3278;
				float m_Off2312_g3278 = 1.0;
				float m_Global2312_g3278 = _Global_Wind_Main_Pulse;
				float m_Local2312_g3278 = _LocalWindPulse;
				float localfloatswitch2312_g3278 = floatswitch2312_g3278( m_switch2312_g3278 , m_Off2312_g3278 , m_Global2312_g3278 , m_Local2312_g3278 );
				float _WIND_PULSE2421_g3278 = localfloatswitch2312_g3278;
				float FUNC_Angle2470_g3278 = ( _WIND_STRENGHT2400_g3278 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g3278 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 50.0 ) ) - ( v.ase_color.r / 20.0 ) ) ) ) * sqrt( v.ase_color.r ) * _WIND_PULSE2421_g3278 );
				float FUNC_Angle_SinA2424_g3278 = sin( FUNC_Angle2470_g3278 );
				float FUNC_Angle_CosA2362_g3278 = cos( FUNC_Angle2470_g3278 );
				int m_switch2456_g3278 = _WIND_MODE2462_g3278;
				float m_Off2456_g3278 = 1.0;
				float m_Global2456_g3278 = _Global_Wind_Main_Direction;
				float m_Local2456_g3278 = _LocalWindDirection;
				float localfloatswitch2456_g3278 = floatswitch2456_g3278( m_switch2456_g3278 , m_Off2456_g3278 , m_Global2456_g3278 , m_Local2456_g3278 );
				float _WindDirection2249_g3278 = localfloatswitch2456_g3278;
				float2 localDirectionalEquation2249_g3278 = DirectionalEquation( _WindDirection2249_g3278 );
				float2 break2469_g3278 = localDirectionalEquation2249_g3278;
				float _WIND_DIRECTION_X2418_g3278 = break2469_g3278.x;
				float lerpResult2258_g3278 = lerp( break2265_g3278.x , ( ( break2265_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2265_g3278.x * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_X2418_g3278);
				float3 break2340_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float3 break2233_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float _WIND_DIRECTION_Y2416_g3278 = break2469_g3278.y;
				float lerpResult2275_g3278 = lerp( break2233_g3278.z , ( ( break2233_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2233_g3278.z * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_Y2416_g3278);
				float3 appendResult2235_g3278 = (float3(lerpResult2258_g3278 , ( ( break2340_g3278.y * FUNC_Angle_CosA2362_g3278 ) - ( break2340_g3278.z * FUNC_Angle_SinA2424_g3278 ) ) , lerpResult2275_g3278));
				float3 VERTEX_POSITION_Neg3006_g3278 = appendResult2235_g3278;
				float3 m_On2465_g3278 = ( VERTEX_POSITION_Neg3006_g3278 - VERTEX_POSITION_MATRIX2352_g3278 );
				float3 localfloat3switch2465_g3278 = float3switch2465_g3278( m_switch2465_g3278 , m_Off2465_g3278 , m_On2465_g3278 );
				float3 m_Global2328_g3278 = localfloat3switch2465_g3278;
				float3 VERTEX_POSITION2282_g3278 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g3278 , 0.0 ) ).xyz - v.vertex.xyz );
				float3 m_Local2328_g3278 = VERTEX_POSITION2282_g3278;
				float3 localfloat3switch2328_g3278 = float3switch2328_g3278( m_switch2328_g3278 , m_Off2328_g3278 , m_Global2328_g3278 , m_Local2328_g3278 );
				float3 m_Global2453_g3278 = localfloat3switch2328_g3278;
				float3 m_Local2453_g3278 = localfloat3switch2328_g3278;
				float3 localfloat3switch2453_g3278 = float3switch2453_g3278( m_switch2453_g3278 , m_Off2453_g3278 , m_Global2453_g3278 , m_Local2453_g3278 );
				float3 m_Off3050_g3278 = localfloat3switch2453_g3278;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float temp_output_3048_0_g3278 = saturate( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _Global_Wind_Main_Fade_Bias ) , 5.0 ) );
				float3 m_ActiveFadeOut3050_g3278 = ( localfloat3switch2453_g3278 * ( 1.0 - temp_output_3048_0_g3278 ) );
				float3 m_ActiveFadeIn3050_g3278 = ( localfloat3switch2453_g3278 * temp_output_3048_0_g3278 );
				float3 localfloat3switch3050_g3278 = float3switch3050_g3278( m_switch3050_g3278 , m_Off3050_g3278 , m_ActiveFadeOut3050_g3278 , m_ActiveFadeIn3050_g3278 );
				float3 temp_output_457_0_g37836 = localfloat3switch3050_g3278;
				
				o.ase_texcoord7.xy = v.texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = temp_output_457_0_g37836;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						, FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 appendResult128_g37836 = (float2(_TilingX , _TilingY));
				float2 appendResult125_g37836 = (float2(_OffsetX , _OffsetY));
				float2 texCoord2_g37836 = IN.ase_texcoord7.xy * appendResult128_g37836 + appendResult125_g37836;
				float4 tex2DNode3_g37836 = SAMPLE_TEXTURE2D( _MainTex, sampler_trilinear_repeat, texCoord2_g37836 );
				float4 ALBEDO_RGBA529_g37836 = tex2DNode3_g37836;
				float3 temp_output_28_0_g37836 = ( (_Color).rgb * (ALBEDO_RGBA529_g37836).rgb * _Brightness );
				
				float4 NORMAL_PC_RGB531_g37836 = SAMPLE_TEXTURE2D( _BumpMap, sampler_trilinear_repeat, texCoord2_g37836 );
				float4 temp_output_1_0_g39527 = NORMAL_PC_RGB531_g37836;
				float temp_output_8_0_g39527 = _NormalStrength;
				float3 unpack52_g39527 = UnpackNormalScale( temp_output_1_0_g39527, temp_output_8_0_g39527 );
				unpack52_g39527.z = lerp( 1, unpack52_g39527.z, saturate(temp_output_8_0_g39527) );
				float3 temp_output_604_59_g37836 = unpack52_g39527;
				float3 worldToViewDir560_g37836 = normalize( mul( UNITY_MATRIX_V, float4( SafeNormalize(_MainLightPosition.xyz), 0 ) ).xyz );
				float dotResult307_g37836 = dot( temp_output_604_59_g37836 , worldToViewDir560_g37836 );
				float3 appendResult306_g37836 = (float3(dotResult307_g37836 , dotResult307_g37836 , dotResult307_g37836));
				float3 NORMAL_IN42_g39555 = ( temp_output_604_59_g37836 + saturate( appendResult306_g37836 ) );
				int m_switch56_g39555 = _NormalMode;
				float3 m_Default56_g39555 = float3(1,1,1);
				float3 m_Flip56_g39555 = float3(-1,-1,-1);
				float3 m_Mirror56_g39555 = float3(1,1,-1);
				float3 localfloat3switch56_g39555 = float3switch56_g39555( m_switch56_g39555 , m_Default56_g39555 , m_Flip56_g39555 , m_Mirror56_g39555 );
				float3 switchResult58_g39555 = (((ase_vface>0)?(NORMAL_IN42_g39555):(( NORMAL_IN42_g39555 * localfloat3switch56_g39555 ))));
				int m_switch37_g39555 = _NormalMode;
				float3 m_Default37_g39555 = NORMAL_IN42_g39555;
				float3 m_Flip37_g39555 = ( NORMAL_IN42_g39555 * ase_vface );
				float3 break33_g39555 = NORMAL_IN42_g39555;
				float3 appendResult41_g39555 = (float3(break33_g39555.x , break33_g39555.y , ( break33_g39555.z * ase_vface )));
				float3 m_Mirror37_g39555 = appendResult41_g39555;
				float3 localfloat3switch37_g39555 = float3switch37_g39555( m_switch37_g39555 , m_Default37_g39555 , m_Flip37_g39555 , m_Mirror37_g39555 );
				float3 temp_output_620_30_g37836 = localfloat3switch37_g39555;
				float3 NORMAL_OUT575_g37836 = temp_output_620_30_g37836;
				float3 temp_output_16_0_g39529 = NORMAL_OUT575_g37836;
				float3 tanToWorld0 = float3( WorldTangent.x, WorldBiTangent.x, WorldNormal.x );
				float3 tanToWorld1 = float3( WorldTangent.y, WorldBiTangent.y, WorldNormal.y );
				float3 tanToWorld2 = float3( WorldTangent.z, WorldBiTangent.z, WorldNormal.z );
				float3 tanNormal134_g39529 = temp_output_16_0_g39529;
				float3 worldNormal134_g39529 = float3(dot(tanToWorld0,tanNormal134_g39529), dot(tanToWorld1,tanNormal134_g39529), dot(tanToWorld2,tanNormal134_g39529));
				float3 NORMAL_TANGENTSPACE135_g39529 = worldNormal134_g39529;
				float3 WorldPosition60_g39529 = NORMAL_TANGENTSPACE135_g39529;
				float3 localAdditionalLightsFlat60_g39529 = AdditionalLightsFlat( WorldPosition60_g39529 );
				float3 ADDITIONAL_LIGHT567_g37836 = localAdditionalLightsFlat60_g39529;
				
				int m_switch609_g37836 = _OcclusionSourceMode;
				float m_Default609_g37836 = ( 1.0 - _OcclusionStrengthAO );
				float4 color453_g37836 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float4 temp_cast_3 = (IN.ase_color.a).xxxx;
				float4 lerpResult452_g37836 = lerp( color453_g37836 , temp_cast_3 , _OcclusionStrengthAO);
				float m_Baked609_g37836 = lerpResult452_g37836.r;
				float localfloatswitch609_g37836 = floatswitch609_g37836( m_switch609_g37836 , m_Default609_g37836 , m_Baked609_g37836 );
				
				float ALBEDO_A414_g37836 = tex2DNode3_g37836.a;
				int m_switch500_g37836 = _GlancingClipMode;
				float m_Off500_g37836 = 1.0;
				float3 normalizeResult472_g37836 = normalize( cross( ddx( WorldPosition ) , ddy( WorldPosition ) ) );
				float dotResult475_g37836 = dot( WorldViewDirection , normalizeResult472_g37836 );
				float temp_output_497_0_g37836 = ( 1.0 - abs( dotResult475_g37836 ) );
				float m_Active500_g37836 = ( 1.0 - ( temp_output_497_0_g37836 * temp_output_497_0_g37836 ) );
				float localfloatswitch500_g37836 = floatswitch500_g37836( m_switch500_g37836 , m_Off500_g37836 , m_Active500_g37836 );
				float OPACITY_OUTMASK494_g37836 = localfloatswitch500_g37836;
				
				float AlphaCutoffBias501_g37836 = _AlphaCutoffBias;
				
				float3 Albedo = ( float4( temp_output_28_0_g37836 , 0.0 ) + float4(0,0,0,0) ).xyz;
				float3 Normal = ( switchResult58_g39555 + ADDITIONAL_LIGHT567_g37836 );
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = _MetallicStrength;
				float Smoothness = _SmoothnessStrength;
				float Occlusion = saturate( localfloatswitch609_g37836 );
				float Alpha = ( ALBEDO_A414_g37836 * OPACITY_OUTMASK494_g37836 );
				float AlphaClipThreshold = AlphaCutoffBias501_g37836;
				float AlphaClipThresholdShadow = _AlphaCutoffBiasShadow;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal, 0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#define _ALPHATEST_SHADOW_ON 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70503
			#define ASE_USING_SAMPLING_MACROS 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd
			#pragma multi_compile GPU_FRUSTUM_ON __
			#include "Nature Renderer.cginc"
			#pragma multi_compile_local _ NATURE_RENDERER


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			int _CullMode;
			int _GlancingClipMode;
			half _OcclusionStrengthAO;
			int _OcclusionSourceMode;
			float _SmoothnessStrength;
			half _MetallicStrength;
			int _NormalMode;
			half _NormalStrength;
			half _Brightness;
			float _OffsetY;
			float _OffsetX;
			float _TilingY;
			float _TilingX;
			float _LocalWindDirection;
			float _LocalWindPulse;
			float _LocalRandomWindOffset;
			float _LocalWindStrength;
			float _GlobalWindInfluenceBillboard;
			int _WindMode;
			int _ZWriteMode;
			int _ColorMask;
			half _AlphaCutoffBias;
			half _AlphaCutoffBiasShadow;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			int _Global_Wind_Main_Fade_Enabled;
			int _Global_Wind_Billboard_Enabled;
			float _Global_Wind_Billboard_Intensity;
			float _Global_Wind_Main_Intensity;
			float _Global_Wind_Main_RandomOffset;
			float _Global_Wind_Main_Pulse;
			float _Global_Wind_Main_Direction;
			float _Global_Wind_Main_Fade_Bias;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_trilinear_repeat);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float floatswitch2460_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2468_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2312_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2456_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 float3switch2465_g3278( int m_switch, float3 m_Off, float3 m_On )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_On;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2328_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2453_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch3050_g3278( int m_switch, float3 m_Off, float3 m_ActiveFadeOut, float3 m_ActiveFadeIn )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_ActiveFadeOut;
				else if(m_switch ==2)
					return m_ActiveFadeIn;
				else
				return float3(0,0,0);
			}
			
			float floatswitch500_g37836( int m_switch, float m_Off, float m_Active )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Active;
				else
				return float(0);
			}
			

			float3 _LightDirection;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				int m_switch3050_g3278 = _Global_Wind_Main_Fade_Enabled;
				int m_switch2453_g3278 = _WindMode;
				float3 m_Off2453_g3278 = float3(0,0,0);
				int _WIND_MODE2462_g3278 = _WindMode;
				int m_switch2328_g3278 = _WIND_MODE2462_g3278;
				float3 VERTEX_POSITION_MATRIX2352_g3278 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz , 0.0 ) ).xyz;
				float3 m_Off2328_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2465_g3278 = _Global_Wind_Billboard_Enabled;
				float3 m_Off2465_g3278 = float3(0,0,0);
				float3 break2265_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2460_g3278 = _WIND_MODE2462_g3278;
				float m_Off2460_g3278 = 1.0;
				float m_Global2460_g3278 = ( ( _GlobalWindInfluenceBillboard * _Global_Wind_Billboard_Intensity ) * _Global_Wind_Main_Intensity );
				float m_Local2460_g3278 = _LocalWindStrength;
				float localfloatswitch2460_g3278 = floatswitch2460_g3278( m_switch2460_g3278 , m_Off2460_g3278 , m_Global2460_g3278 , m_Local2460_g3278 );
				float _WIND_STRENGHT2400_g3278 = localfloatswitch2460_g3278;
				int m_switch2468_g3278 = _WIND_MODE2462_g3278;
				float m_Off2468_g3278 = 1.0;
				float m_Global2468_g3278 = _Global_Wind_Main_RandomOffset;
				float m_Local2468_g3278 = _LocalRandomWindOffset;
				float localfloatswitch2468_g3278 = floatswitch2468_g3278( m_switch2468_g3278 , m_Off2468_g3278 , m_Global2468_g3278 , m_Local2468_g3278 );
				float4 transform3073_g3278 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g3278 = (float2(transform3073_g3278.x , transform3073_g3278.z));
				float dotResult2341_g3278 = dot( appendResult2307_g3278 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g3278 = lerp( 0.8 , ( ( localfloatswitch2468_g3278 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g3278 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g3278 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g3278 );
				float _WIND_TUBULENCE_RANDOM2274_g3278 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 15.0 ) ) ) * 0.5 );
				int m_switch2312_g3278 = _WIND_MODE2462_g3278;
				float m_Off2312_g3278 = 1.0;
				float m_Global2312_g3278 = _Global_Wind_Main_Pulse;
				float m_Local2312_g3278 = _LocalWindPulse;
				float localfloatswitch2312_g3278 = floatswitch2312_g3278( m_switch2312_g3278 , m_Off2312_g3278 , m_Global2312_g3278 , m_Local2312_g3278 );
				float _WIND_PULSE2421_g3278 = localfloatswitch2312_g3278;
				float FUNC_Angle2470_g3278 = ( _WIND_STRENGHT2400_g3278 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g3278 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 50.0 ) ) - ( v.ase_color.r / 20.0 ) ) ) ) * sqrt( v.ase_color.r ) * _WIND_PULSE2421_g3278 );
				float FUNC_Angle_SinA2424_g3278 = sin( FUNC_Angle2470_g3278 );
				float FUNC_Angle_CosA2362_g3278 = cos( FUNC_Angle2470_g3278 );
				int m_switch2456_g3278 = _WIND_MODE2462_g3278;
				float m_Off2456_g3278 = 1.0;
				float m_Global2456_g3278 = _Global_Wind_Main_Direction;
				float m_Local2456_g3278 = _LocalWindDirection;
				float localfloatswitch2456_g3278 = floatswitch2456_g3278( m_switch2456_g3278 , m_Off2456_g3278 , m_Global2456_g3278 , m_Local2456_g3278 );
				float _WindDirection2249_g3278 = localfloatswitch2456_g3278;
				float2 localDirectionalEquation2249_g3278 = DirectionalEquation( _WindDirection2249_g3278 );
				float2 break2469_g3278 = localDirectionalEquation2249_g3278;
				float _WIND_DIRECTION_X2418_g3278 = break2469_g3278.x;
				float lerpResult2258_g3278 = lerp( break2265_g3278.x , ( ( break2265_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2265_g3278.x * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_X2418_g3278);
				float3 break2340_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float3 break2233_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float _WIND_DIRECTION_Y2416_g3278 = break2469_g3278.y;
				float lerpResult2275_g3278 = lerp( break2233_g3278.z , ( ( break2233_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2233_g3278.z * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_Y2416_g3278);
				float3 appendResult2235_g3278 = (float3(lerpResult2258_g3278 , ( ( break2340_g3278.y * FUNC_Angle_CosA2362_g3278 ) - ( break2340_g3278.z * FUNC_Angle_SinA2424_g3278 ) ) , lerpResult2275_g3278));
				float3 VERTEX_POSITION_Neg3006_g3278 = appendResult2235_g3278;
				float3 m_On2465_g3278 = ( VERTEX_POSITION_Neg3006_g3278 - VERTEX_POSITION_MATRIX2352_g3278 );
				float3 localfloat3switch2465_g3278 = float3switch2465_g3278( m_switch2465_g3278 , m_Off2465_g3278 , m_On2465_g3278 );
				float3 m_Global2328_g3278 = localfloat3switch2465_g3278;
				float3 VERTEX_POSITION2282_g3278 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g3278 , 0.0 ) ).xyz - v.vertex.xyz );
				float3 m_Local2328_g3278 = VERTEX_POSITION2282_g3278;
				float3 localfloat3switch2328_g3278 = float3switch2328_g3278( m_switch2328_g3278 , m_Off2328_g3278 , m_Global2328_g3278 , m_Local2328_g3278 );
				float3 m_Global2453_g3278 = localfloat3switch2328_g3278;
				float3 m_Local2453_g3278 = localfloat3switch2328_g3278;
				float3 localfloat3switch2453_g3278 = float3switch2453_g3278( m_switch2453_g3278 , m_Off2453_g3278 , m_Global2453_g3278 , m_Local2453_g3278 );
				float3 m_Off3050_g3278 = localfloat3switch2453_g3278;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float temp_output_3048_0_g3278 = saturate( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _Global_Wind_Main_Fade_Bias ) , 5.0 ) );
				float3 m_ActiveFadeOut3050_g3278 = ( localfloat3switch2453_g3278 * ( 1.0 - temp_output_3048_0_g3278 ) );
				float3 m_ActiveFadeIn3050_g3278 = ( localfloat3switch2453_g3278 * temp_output_3048_0_g3278 );
				float3 localfloat3switch3050_g3278 = float3switch3050_g3278( m_switch3050_g3278 , m_Off3050_g3278 , m_ActiveFadeOut3050_g3278 , m_ActiveFadeIn3050_g3278 );
				float3 temp_output_457_0_g37836 = localfloat3switch3050_g3278;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = temp_output_457_0_g37836;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult128_g37836 = (float2(_TilingX , _TilingY));
				float2 appendResult125_g37836 = (float2(_OffsetX , _OffsetY));
				float2 texCoord2_g37836 = IN.ase_texcoord2.xy * appendResult128_g37836 + appendResult125_g37836;
				float4 tex2DNode3_g37836 = SAMPLE_TEXTURE2D( _MainTex, sampler_trilinear_repeat, texCoord2_g37836 );
				float ALBEDO_A414_g37836 = tex2DNode3_g37836.a;
				int m_switch500_g37836 = _GlancingClipMode;
				float m_Off500_g37836 = 1.0;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = SafeNormalize( ase_worldViewDir );
				float3 normalizeResult472_g37836 = normalize( cross( ddx( WorldPosition ) , ddy( WorldPosition ) ) );
				float dotResult475_g37836 = dot( ase_worldViewDir , normalizeResult472_g37836 );
				float temp_output_497_0_g37836 = ( 1.0 - abs( dotResult475_g37836 ) );
				float m_Active500_g37836 = ( 1.0 - ( temp_output_497_0_g37836 * temp_output_497_0_g37836 ) );
				float localfloatswitch500_g37836 = floatswitch500_g37836( m_switch500_g37836 , m_Off500_g37836 , m_Active500_g37836 );
				float OPACITY_OUTMASK494_g37836 = localfloatswitch500_g37836;
				
				float AlphaCutoffBias501_g37836 = _AlphaCutoffBias;
				
				float Alpha = ( ALBEDO_A414_g37836 * OPACITY_OUTMASK494_g37836 );
				float AlphaClipThreshold = AlphaCutoffBias501_g37836;
				float AlphaClipThresholdShadow = _AlphaCutoffBiasShadow;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#define _ALPHATEST_SHADOW_ON 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70503
			#define ASE_USING_SAMPLING_MACROS 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd
			#pragma multi_compile GPU_FRUSTUM_ON __
			#include "Nature Renderer.cginc"
			#pragma multi_compile_local _ NATURE_RENDERER


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			int _CullMode;
			int _GlancingClipMode;
			half _OcclusionStrengthAO;
			int _OcclusionSourceMode;
			float _SmoothnessStrength;
			half _MetallicStrength;
			int _NormalMode;
			half _NormalStrength;
			half _Brightness;
			float _OffsetY;
			float _OffsetX;
			float _TilingY;
			float _TilingX;
			float _LocalWindDirection;
			float _LocalWindPulse;
			float _LocalRandomWindOffset;
			float _LocalWindStrength;
			float _GlobalWindInfluenceBillboard;
			int _WindMode;
			int _ZWriteMode;
			int _ColorMask;
			half _AlphaCutoffBias;
			half _AlphaCutoffBiasShadow;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			int _Global_Wind_Main_Fade_Enabled;
			int _Global_Wind_Billboard_Enabled;
			float _Global_Wind_Billboard_Intensity;
			float _Global_Wind_Main_Intensity;
			float _Global_Wind_Main_RandomOffset;
			float _Global_Wind_Main_Pulse;
			float _Global_Wind_Main_Direction;
			float _Global_Wind_Main_Fade_Bias;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_trilinear_repeat);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float floatswitch2460_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2468_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2312_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2456_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 float3switch2465_g3278( int m_switch, float3 m_Off, float3 m_On )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_On;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2328_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2453_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch3050_g3278( int m_switch, float3 m_Off, float3 m_ActiveFadeOut, float3 m_ActiveFadeIn )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_ActiveFadeOut;
				else if(m_switch ==2)
					return m_ActiveFadeIn;
				else
				return float3(0,0,0);
			}
			
			float floatswitch500_g37836( int m_switch, float m_Off, float m_Active )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Active;
				else
				return float(0);
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				int m_switch3050_g3278 = _Global_Wind_Main_Fade_Enabled;
				int m_switch2453_g3278 = _WindMode;
				float3 m_Off2453_g3278 = float3(0,0,0);
				int _WIND_MODE2462_g3278 = _WindMode;
				int m_switch2328_g3278 = _WIND_MODE2462_g3278;
				float3 VERTEX_POSITION_MATRIX2352_g3278 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz , 0.0 ) ).xyz;
				float3 m_Off2328_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2465_g3278 = _Global_Wind_Billboard_Enabled;
				float3 m_Off2465_g3278 = float3(0,0,0);
				float3 break2265_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2460_g3278 = _WIND_MODE2462_g3278;
				float m_Off2460_g3278 = 1.0;
				float m_Global2460_g3278 = ( ( _GlobalWindInfluenceBillboard * _Global_Wind_Billboard_Intensity ) * _Global_Wind_Main_Intensity );
				float m_Local2460_g3278 = _LocalWindStrength;
				float localfloatswitch2460_g3278 = floatswitch2460_g3278( m_switch2460_g3278 , m_Off2460_g3278 , m_Global2460_g3278 , m_Local2460_g3278 );
				float _WIND_STRENGHT2400_g3278 = localfloatswitch2460_g3278;
				int m_switch2468_g3278 = _WIND_MODE2462_g3278;
				float m_Off2468_g3278 = 1.0;
				float m_Global2468_g3278 = _Global_Wind_Main_RandomOffset;
				float m_Local2468_g3278 = _LocalRandomWindOffset;
				float localfloatswitch2468_g3278 = floatswitch2468_g3278( m_switch2468_g3278 , m_Off2468_g3278 , m_Global2468_g3278 , m_Local2468_g3278 );
				float4 transform3073_g3278 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g3278 = (float2(transform3073_g3278.x , transform3073_g3278.z));
				float dotResult2341_g3278 = dot( appendResult2307_g3278 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g3278 = lerp( 0.8 , ( ( localfloatswitch2468_g3278 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g3278 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g3278 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g3278 );
				float _WIND_TUBULENCE_RANDOM2274_g3278 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 15.0 ) ) ) * 0.5 );
				int m_switch2312_g3278 = _WIND_MODE2462_g3278;
				float m_Off2312_g3278 = 1.0;
				float m_Global2312_g3278 = _Global_Wind_Main_Pulse;
				float m_Local2312_g3278 = _LocalWindPulse;
				float localfloatswitch2312_g3278 = floatswitch2312_g3278( m_switch2312_g3278 , m_Off2312_g3278 , m_Global2312_g3278 , m_Local2312_g3278 );
				float _WIND_PULSE2421_g3278 = localfloatswitch2312_g3278;
				float FUNC_Angle2470_g3278 = ( _WIND_STRENGHT2400_g3278 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g3278 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 50.0 ) ) - ( v.ase_color.r / 20.0 ) ) ) ) * sqrt( v.ase_color.r ) * _WIND_PULSE2421_g3278 );
				float FUNC_Angle_SinA2424_g3278 = sin( FUNC_Angle2470_g3278 );
				float FUNC_Angle_CosA2362_g3278 = cos( FUNC_Angle2470_g3278 );
				int m_switch2456_g3278 = _WIND_MODE2462_g3278;
				float m_Off2456_g3278 = 1.0;
				float m_Global2456_g3278 = _Global_Wind_Main_Direction;
				float m_Local2456_g3278 = _LocalWindDirection;
				float localfloatswitch2456_g3278 = floatswitch2456_g3278( m_switch2456_g3278 , m_Off2456_g3278 , m_Global2456_g3278 , m_Local2456_g3278 );
				float _WindDirection2249_g3278 = localfloatswitch2456_g3278;
				float2 localDirectionalEquation2249_g3278 = DirectionalEquation( _WindDirection2249_g3278 );
				float2 break2469_g3278 = localDirectionalEquation2249_g3278;
				float _WIND_DIRECTION_X2418_g3278 = break2469_g3278.x;
				float lerpResult2258_g3278 = lerp( break2265_g3278.x , ( ( break2265_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2265_g3278.x * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_X2418_g3278);
				float3 break2340_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float3 break2233_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float _WIND_DIRECTION_Y2416_g3278 = break2469_g3278.y;
				float lerpResult2275_g3278 = lerp( break2233_g3278.z , ( ( break2233_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2233_g3278.z * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_Y2416_g3278);
				float3 appendResult2235_g3278 = (float3(lerpResult2258_g3278 , ( ( break2340_g3278.y * FUNC_Angle_CosA2362_g3278 ) - ( break2340_g3278.z * FUNC_Angle_SinA2424_g3278 ) ) , lerpResult2275_g3278));
				float3 VERTEX_POSITION_Neg3006_g3278 = appendResult2235_g3278;
				float3 m_On2465_g3278 = ( VERTEX_POSITION_Neg3006_g3278 - VERTEX_POSITION_MATRIX2352_g3278 );
				float3 localfloat3switch2465_g3278 = float3switch2465_g3278( m_switch2465_g3278 , m_Off2465_g3278 , m_On2465_g3278 );
				float3 m_Global2328_g3278 = localfloat3switch2465_g3278;
				float3 VERTEX_POSITION2282_g3278 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g3278 , 0.0 ) ).xyz - v.vertex.xyz );
				float3 m_Local2328_g3278 = VERTEX_POSITION2282_g3278;
				float3 localfloat3switch2328_g3278 = float3switch2328_g3278( m_switch2328_g3278 , m_Off2328_g3278 , m_Global2328_g3278 , m_Local2328_g3278 );
				float3 m_Global2453_g3278 = localfloat3switch2328_g3278;
				float3 m_Local2453_g3278 = localfloat3switch2328_g3278;
				float3 localfloat3switch2453_g3278 = float3switch2453_g3278( m_switch2453_g3278 , m_Off2453_g3278 , m_Global2453_g3278 , m_Local2453_g3278 );
				float3 m_Off3050_g3278 = localfloat3switch2453_g3278;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float temp_output_3048_0_g3278 = saturate( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _Global_Wind_Main_Fade_Bias ) , 5.0 ) );
				float3 m_ActiveFadeOut3050_g3278 = ( localfloat3switch2453_g3278 * ( 1.0 - temp_output_3048_0_g3278 ) );
				float3 m_ActiveFadeIn3050_g3278 = ( localfloat3switch2453_g3278 * temp_output_3048_0_g3278 );
				float3 localfloat3switch3050_g3278 = float3switch3050_g3278( m_switch3050_g3278 , m_Off3050_g3278 , m_ActiveFadeOut3050_g3278 , m_ActiveFadeIn3050_g3278 );
				float3 temp_output_457_0_g37836 = localfloat3switch3050_g3278;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = temp_output_457_0_g37836;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult128_g37836 = (float2(_TilingX , _TilingY));
				float2 appendResult125_g37836 = (float2(_OffsetX , _OffsetY));
				float2 texCoord2_g37836 = IN.ase_texcoord2.xy * appendResult128_g37836 + appendResult125_g37836;
				float4 tex2DNode3_g37836 = SAMPLE_TEXTURE2D( _MainTex, sampler_trilinear_repeat, texCoord2_g37836 );
				float ALBEDO_A414_g37836 = tex2DNode3_g37836.a;
				int m_switch500_g37836 = _GlancingClipMode;
				float m_Off500_g37836 = 1.0;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = SafeNormalize( ase_worldViewDir );
				float3 normalizeResult472_g37836 = normalize( cross( ddx( WorldPosition ) , ddy( WorldPosition ) ) );
				float dotResult475_g37836 = dot( ase_worldViewDir , normalizeResult472_g37836 );
				float temp_output_497_0_g37836 = ( 1.0 - abs( dotResult475_g37836 ) );
				float m_Active500_g37836 = ( 1.0 - ( temp_output_497_0_g37836 * temp_output_497_0_g37836 ) );
				float localfloatswitch500_g37836 = floatswitch500_g37836( m_switch500_g37836 , m_Off500_g37836 , m_Active500_g37836 );
				float OPACITY_OUTMASK494_g37836 = localfloatswitch500_g37836;
				
				float AlphaCutoffBias501_g37836 = _AlphaCutoffBias;
				
				float Alpha = ( ALBEDO_A414_g37836 * OPACITY_OUTMASK494_g37836 );
				float AlphaClipThreshold = AlphaCutoffBias501_g37836;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#define _ALPHATEST_SHADOW_ON 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70503
			#define ASE_USING_SAMPLING_MACROS 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd
			#pragma multi_compile GPU_FRUSTUM_ON __
			#include "Nature Renderer.cginc"
			#pragma multi_compile_local _ NATURE_RENDERER


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			int _CullMode;
			int _GlancingClipMode;
			half _OcclusionStrengthAO;
			int _OcclusionSourceMode;
			float _SmoothnessStrength;
			half _MetallicStrength;
			int _NormalMode;
			half _NormalStrength;
			half _Brightness;
			float _OffsetY;
			float _OffsetX;
			float _TilingY;
			float _TilingX;
			float _LocalWindDirection;
			float _LocalWindPulse;
			float _LocalRandomWindOffset;
			float _LocalWindStrength;
			float _GlobalWindInfluenceBillboard;
			int _WindMode;
			int _ZWriteMode;
			int _ColorMask;
			half _AlphaCutoffBias;
			half _AlphaCutoffBiasShadow;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			int _Global_Wind_Main_Fade_Enabled;
			int _Global_Wind_Billboard_Enabled;
			float _Global_Wind_Billboard_Intensity;
			float _Global_Wind_Main_Intensity;
			float _Global_Wind_Main_RandomOffset;
			float _Global_Wind_Main_Pulse;
			float _Global_Wind_Main_Direction;
			float _Global_Wind_Main_Fade_Bias;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_trilinear_repeat);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float floatswitch2460_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2468_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2312_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2456_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 float3switch2465_g3278( int m_switch, float3 m_Off, float3 m_On )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_On;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2328_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2453_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch3050_g3278( int m_switch, float3 m_Off, float3 m_ActiveFadeOut, float3 m_ActiveFadeIn )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_ActiveFadeOut;
				else if(m_switch ==2)
					return m_ActiveFadeIn;
				else
				return float3(0,0,0);
			}
			
			float floatswitch500_g37836( int m_switch, float m_Off, float m_Active )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Active;
				else
				return float(0);
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				int m_switch3050_g3278 = _Global_Wind_Main_Fade_Enabled;
				int m_switch2453_g3278 = _WindMode;
				float3 m_Off2453_g3278 = float3(0,0,0);
				int _WIND_MODE2462_g3278 = _WindMode;
				int m_switch2328_g3278 = _WIND_MODE2462_g3278;
				float3 VERTEX_POSITION_MATRIX2352_g3278 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz , 0.0 ) ).xyz;
				float3 m_Off2328_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2465_g3278 = _Global_Wind_Billboard_Enabled;
				float3 m_Off2465_g3278 = float3(0,0,0);
				float3 break2265_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2460_g3278 = _WIND_MODE2462_g3278;
				float m_Off2460_g3278 = 1.0;
				float m_Global2460_g3278 = ( ( _GlobalWindInfluenceBillboard * _Global_Wind_Billboard_Intensity ) * _Global_Wind_Main_Intensity );
				float m_Local2460_g3278 = _LocalWindStrength;
				float localfloatswitch2460_g3278 = floatswitch2460_g3278( m_switch2460_g3278 , m_Off2460_g3278 , m_Global2460_g3278 , m_Local2460_g3278 );
				float _WIND_STRENGHT2400_g3278 = localfloatswitch2460_g3278;
				int m_switch2468_g3278 = _WIND_MODE2462_g3278;
				float m_Off2468_g3278 = 1.0;
				float m_Global2468_g3278 = _Global_Wind_Main_RandomOffset;
				float m_Local2468_g3278 = _LocalRandomWindOffset;
				float localfloatswitch2468_g3278 = floatswitch2468_g3278( m_switch2468_g3278 , m_Off2468_g3278 , m_Global2468_g3278 , m_Local2468_g3278 );
				float4 transform3073_g3278 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g3278 = (float2(transform3073_g3278.x , transform3073_g3278.z));
				float dotResult2341_g3278 = dot( appendResult2307_g3278 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g3278 = lerp( 0.8 , ( ( localfloatswitch2468_g3278 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g3278 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g3278 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g3278 );
				float _WIND_TUBULENCE_RANDOM2274_g3278 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 15.0 ) ) ) * 0.5 );
				int m_switch2312_g3278 = _WIND_MODE2462_g3278;
				float m_Off2312_g3278 = 1.0;
				float m_Global2312_g3278 = _Global_Wind_Main_Pulse;
				float m_Local2312_g3278 = _LocalWindPulse;
				float localfloatswitch2312_g3278 = floatswitch2312_g3278( m_switch2312_g3278 , m_Off2312_g3278 , m_Global2312_g3278 , m_Local2312_g3278 );
				float _WIND_PULSE2421_g3278 = localfloatswitch2312_g3278;
				float FUNC_Angle2470_g3278 = ( _WIND_STRENGHT2400_g3278 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g3278 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 50.0 ) ) - ( v.ase_color.r / 20.0 ) ) ) ) * sqrt( v.ase_color.r ) * _WIND_PULSE2421_g3278 );
				float FUNC_Angle_SinA2424_g3278 = sin( FUNC_Angle2470_g3278 );
				float FUNC_Angle_CosA2362_g3278 = cos( FUNC_Angle2470_g3278 );
				int m_switch2456_g3278 = _WIND_MODE2462_g3278;
				float m_Off2456_g3278 = 1.0;
				float m_Global2456_g3278 = _Global_Wind_Main_Direction;
				float m_Local2456_g3278 = _LocalWindDirection;
				float localfloatswitch2456_g3278 = floatswitch2456_g3278( m_switch2456_g3278 , m_Off2456_g3278 , m_Global2456_g3278 , m_Local2456_g3278 );
				float _WindDirection2249_g3278 = localfloatswitch2456_g3278;
				float2 localDirectionalEquation2249_g3278 = DirectionalEquation( _WindDirection2249_g3278 );
				float2 break2469_g3278 = localDirectionalEquation2249_g3278;
				float _WIND_DIRECTION_X2418_g3278 = break2469_g3278.x;
				float lerpResult2258_g3278 = lerp( break2265_g3278.x , ( ( break2265_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2265_g3278.x * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_X2418_g3278);
				float3 break2340_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float3 break2233_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float _WIND_DIRECTION_Y2416_g3278 = break2469_g3278.y;
				float lerpResult2275_g3278 = lerp( break2233_g3278.z , ( ( break2233_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2233_g3278.z * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_Y2416_g3278);
				float3 appendResult2235_g3278 = (float3(lerpResult2258_g3278 , ( ( break2340_g3278.y * FUNC_Angle_CosA2362_g3278 ) - ( break2340_g3278.z * FUNC_Angle_SinA2424_g3278 ) ) , lerpResult2275_g3278));
				float3 VERTEX_POSITION_Neg3006_g3278 = appendResult2235_g3278;
				float3 m_On2465_g3278 = ( VERTEX_POSITION_Neg3006_g3278 - VERTEX_POSITION_MATRIX2352_g3278 );
				float3 localfloat3switch2465_g3278 = float3switch2465_g3278( m_switch2465_g3278 , m_Off2465_g3278 , m_On2465_g3278 );
				float3 m_Global2328_g3278 = localfloat3switch2465_g3278;
				float3 VERTEX_POSITION2282_g3278 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g3278 , 0.0 ) ).xyz - v.vertex.xyz );
				float3 m_Local2328_g3278 = VERTEX_POSITION2282_g3278;
				float3 localfloat3switch2328_g3278 = float3switch2328_g3278( m_switch2328_g3278 , m_Off2328_g3278 , m_Global2328_g3278 , m_Local2328_g3278 );
				float3 m_Global2453_g3278 = localfloat3switch2328_g3278;
				float3 m_Local2453_g3278 = localfloat3switch2328_g3278;
				float3 localfloat3switch2453_g3278 = float3switch2453_g3278( m_switch2453_g3278 , m_Off2453_g3278 , m_Global2453_g3278 , m_Local2453_g3278 );
				float3 m_Off3050_g3278 = localfloat3switch2453_g3278;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float temp_output_3048_0_g3278 = saturate( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _Global_Wind_Main_Fade_Bias ) , 5.0 ) );
				float3 m_ActiveFadeOut3050_g3278 = ( localfloat3switch2453_g3278 * ( 1.0 - temp_output_3048_0_g3278 ) );
				float3 m_ActiveFadeIn3050_g3278 = ( localfloat3switch2453_g3278 * temp_output_3048_0_g3278 );
				float3 localfloat3switch3050_g3278 = float3switch3050_g3278( m_switch3050_g3278 , m_Off3050_g3278 , m_ActiveFadeOut3050_g3278 , m_ActiveFadeIn3050_g3278 );
				float3 temp_output_457_0_g37836 = localfloat3switch3050_g3278;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = temp_output_457_0_g37836;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult128_g37836 = (float2(_TilingX , _TilingY));
				float2 appendResult125_g37836 = (float2(_OffsetX , _OffsetY));
				float2 texCoord2_g37836 = IN.ase_texcoord2.xy * appendResult128_g37836 + appendResult125_g37836;
				float4 tex2DNode3_g37836 = SAMPLE_TEXTURE2D( _MainTex, sampler_trilinear_repeat, texCoord2_g37836 );
				float4 ALBEDO_RGBA529_g37836 = tex2DNode3_g37836;
				float3 temp_output_28_0_g37836 = ( (_Color).rgb * (ALBEDO_RGBA529_g37836).rgb * _Brightness );
				
				float ALBEDO_A414_g37836 = tex2DNode3_g37836.a;
				int m_switch500_g37836 = _GlancingClipMode;
				float m_Off500_g37836 = 1.0;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = SafeNormalize( ase_worldViewDir );
				float3 normalizeResult472_g37836 = normalize( cross( ddx( WorldPosition ) , ddy( WorldPosition ) ) );
				float dotResult475_g37836 = dot( ase_worldViewDir , normalizeResult472_g37836 );
				float temp_output_497_0_g37836 = ( 1.0 - abs( dotResult475_g37836 ) );
				float m_Active500_g37836 = ( 1.0 - ( temp_output_497_0_g37836 * temp_output_497_0_g37836 ) );
				float localfloatswitch500_g37836 = floatswitch500_g37836( m_switch500_g37836 , m_Off500_g37836 , m_Active500_g37836 );
				float OPACITY_OUTMASK494_g37836 = localfloatswitch500_g37836;
				
				float AlphaCutoffBias501_g37836 = _AlphaCutoffBias;
				
				
				float3 Albedo = ( float4( temp_output_28_0_g37836 , 0.0 ) + float4(0,0,0,0) ).xyz;
				float3 Emission = 0;
				float Alpha = ( ALBEDO_A414_g37836 * OPACITY_OUTMASK494_g37836 );
				float AlphaClipThreshold = AlphaCutoffBias501_g37836;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite [_ZWriteMode]
			ZTest LEqual
			Offset 0 , 0
			ColorMask [_ColorMask]

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#define _ALPHATEST_SHADOW_ON 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70503
			#define ASE_USING_SAMPLING_MACROS 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:SetupNatureRenderer forwardadd
			#pragma multi_compile GPU_FRUSTUM_ON __
			#include "Nature Renderer.cginc"
			#pragma multi_compile_local _ NATURE_RENDERER


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			int _CullMode;
			int _GlancingClipMode;
			half _OcclusionStrengthAO;
			int _OcclusionSourceMode;
			float _SmoothnessStrength;
			half _MetallicStrength;
			int _NormalMode;
			half _NormalStrength;
			half _Brightness;
			float _OffsetY;
			float _OffsetX;
			float _TilingY;
			float _TilingX;
			float _LocalWindDirection;
			float _LocalWindPulse;
			float _LocalRandomWindOffset;
			float _LocalWindStrength;
			float _GlobalWindInfluenceBillboard;
			int _WindMode;
			int _ZWriteMode;
			int _ColorMask;
			half _AlphaCutoffBias;
			half _AlphaCutoffBiasShadow;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			int _Global_Wind_Main_Fade_Enabled;
			int _Global_Wind_Billboard_Enabled;
			float _Global_Wind_Billboard_Intensity;
			float _Global_Wind_Main_Intensity;
			float _Global_Wind_Main_RandomOffset;
			float _Global_Wind_Main_Pulse;
			float _Global_Wind_Main_Direction;
			float _Global_Wind_Main_Fade_Bias;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_trilinear_repeat);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float floatswitch2460_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2468_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2312_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float floatswitch2456_g3278( int m_switch, float m_Off, float m_Global, float m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float(0);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 float3switch2465_g3278( int m_switch, float3 m_Off, float3 m_On )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_On;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2328_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch2453_g3278( int m_switch, float3 m_Off, float3 m_Global, float3 m_Local )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Global;
				else if(m_switch ==2)
					return m_Local;
				else
				return float3(0,0,0);
			}
			
			float3 float3switch3050_g3278( int m_switch, float3 m_Off, float3 m_ActiveFadeOut, float3 m_ActiveFadeIn )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_ActiveFadeOut;
				else if(m_switch ==2)
					return m_ActiveFadeIn;
				else
				return float3(0,0,0);
			}
			
			float floatswitch500_g37836( int m_switch, float m_Off, float m_Active )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_Active;
				else
				return float(0);
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				int m_switch3050_g3278 = _Global_Wind_Main_Fade_Enabled;
				int m_switch2453_g3278 = _WindMode;
				float3 m_Off2453_g3278 = float3(0,0,0);
				int _WIND_MODE2462_g3278 = _WindMode;
				int m_switch2328_g3278 = _WIND_MODE2462_g3278;
				float3 VERTEX_POSITION_MATRIX2352_g3278 = mul( GetObjectToWorldMatrix(), float4( v.vertex.xyz , 0.0 ) ).xyz;
				float3 m_Off2328_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2465_g3278 = _Global_Wind_Billboard_Enabled;
				float3 m_Off2465_g3278 = float3(0,0,0);
				float3 break2265_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				int m_switch2460_g3278 = _WIND_MODE2462_g3278;
				float m_Off2460_g3278 = 1.0;
				float m_Global2460_g3278 = ( ( _GlobalWindInfluenceBillboard * _Global_Wind_Billboard_Intensity ) * _Global_Wind_Main_Intensity );
				float m_Local2460_g3278 = _LocalWindStrength;
				float localfloatswitch2460_g3278 = floatswitch2460_g3278( m_switch2460_g3278 , m_Off2460_g3278 , m_Global2460_g3278 , m_Local2460_g3278 );
				float _WIND_STRENGHT2400_g3278 = localfloatswitch2460_g3278;
				int m_switch2468_g3278 = _WIND_MODE2462_g3278;
				float m_Off2468_g3278 = 1.0;
				float m_Global2468_g3278 = _Global_Wind_Main_RandomOffset;
				float m_Local2468_g3278 = _LocalRandomWindOffset;
				float localfloatswitch2468_g3278 = floatswitch2468_g3278( m_switch2468_g3278 , m_Off2468_g3278 , m_Global2468_g3278 , m_Local2468_g3278 );
				float4 transform3073_g3278 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g3278 = (float2(transform3073_g3278.x , transform3073_g3278.z));
				float dotResult2341_g3278 = dot( appendResult2307_g3278 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g3278 = lerp( 0.8 , ( ( localfloatswitch2468_g3278 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g3278 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g3278 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g3278 );
				float _WIND_TUBULENCE_RANDOM2274_g3278 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 15.0 ) ) ) * 0.5 );
				int m_switch2312_g3278 = _WIND_MODE2462_g3278;
				float m_Off2312_g3278 = 1.0;
				float m_Global2312_g3278 = _Global_Wind_Main_Pulse;
				float m_Local2312_g3278 = _LocalWindPulse;
				float localfloatswitch2312_g3278 = floatswitch2312_g3278( m_switch2312_g3278 , m_Off2312_g3278 , m_Global2312_g3278 , m_Local2312_g3278 );
				float _WIND_PULSE2421_g3278 = localfloatswitch2312_g3278;
				float FUNC_Angle2470_g3278 = ( _WIND_STRENGHT2400_g3278 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g3278 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g3278 ) - ( VERTEX_POSITION_MATRIX2352_g3278.z / 50.0 ) ) - ( v.ase_color.r / 20.0 ) ) ) ) * sqrt( v.ase_color.r ) * _WIND_PULSE2421_g3278 );
				float FUNC_Angle_SinA2424_g3278 = sin( FUNC_Angle2470_g3278 );
				float FUNC_Angle_CosA2362_g3278 = cos( FUNC_Angle2470_g3278 );
				int m_switch2456_g3278 = _WIND_MODE2462_g3278;
				float m_Off2456_g3278 = 1.0;
				float m_Global2456_g3278 = _Global_Wind_Main_Direction;
				float m_Local2456_g3278 = _LocalWindDirection;
				float localfloatswitch2456_g3278 = floatswitch2456_g3278( m_switch2456_g3278 , m_Off2456_g3278 , m_Global2456_g3278 , m_Local2456_g3278 );
				float _WindDirection2249_g3278 = localfloatswitch2456_g3278;
				float2 localDirectionalEquation2249_g3278 = DirectionalEquation( _WindDirection2249_g3278 );
				float2 break2469_g3278 = localDirectionalEquation2249_g3278;
				float _WIND_DIRECTION_X2418_g3278 = break2469_g3278.x;
				float lerpResult2258_g3278 = lerp( break2265_g3278.x , ( ( break2265_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2265_g3278.x * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_X2418_g3278);
				float3 break2340_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float3 break2233_g3278 = VERTEX_POSITION_MATRIX2352_g3278;
				float _WIND_DIRECTION_Y2416_g3278 = break2469_g3278.y;
				float lerpResult2275_g3278 = lerp( break2233_g3278.z , ( ( break2233_g3278.y * FUNC_Angle_SinA2424_g3278 ) + ( break2233_g3278.z * FUNC_Angle_CosA2362_g3278 ) ) , _WIND_DIRECTION_Y2416_g3278);
				float3 appendResult2235_g3278 = (float3(lerpResult2258_g3278 , ( ( break2340_g3278.y * FUNC_Angle_CosA2362_g3278 ) - ( break2340_g3278.z * FUNC_Angle_SinA2424_g3278 ) ) , lerpResult2275_g3278));
				float3 VERTEX_POSITION_Neg3006_g3278 = appendResult2235_g3278;
				float3 m_On2465_g3278 = ( VERTEX_POSITION_Neg3006_g3278 - VERTEX_POSITION_MATRIX2352_g3278 );
				float3 localfloat3switch2465_g3278 = float3switch2465_g3278( m_switch2465_g3278 , m_Off2465_g3278 , m_On2465_g3278 );
				float3 m_Global2328_g3278 = localfloat3switch2465_g3278;
				float3 VERTEX_POSITION2282_g3278 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g3278 , 0.0 ) ).xyz - v.vertex.xyz );
				float3 m_Local2328_g3278 = VERTEX_POSITION2282_g3278;
				float3 localfloat3switch2328_g3278 = float3switch2328_g3278( m_switch2328_g3278 , m_Off2328_g3278 , m_Global2328_g3278 , m_Local2328_g3278 );
				float3 m_Global2453_g3278 = localfloat3switch2328_g3278;
				float3 m_Local2453_g3278 = localfloat3switch2328_g3278;
				float3 localfloat3switch2453_g3278 = float3switch2453_g3278( m_switch2453_g3278 , m_Off2453_g3278 , m_Global2453_g3278 , m_Local2453_g3278 );
				float3 m_Off3050_g3278 = localfloat3switch2453_g3278;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float temp_output_3048_0_g3278 = saturate( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _Global_Wind_Main_Fade_Bias ) , 5.0 ) );
				float3 m_ActiveFadeOut3050_g3278 = ( localfloat3switch2453_g3278 * ( 1.0 - temp_output_3048_0_g3278 ) );
				float3 m_ActiveFadeIn3050_g3278 = ( localfloat3switch2453_g3278 * temp_output_3048_0_g3278 );
				float3 localfloat3switch3050_g3278 = float3switch3050_g3278( m_switch3050_g3278 , m_Off3050_g3278 , m_ActiveFadeOut3050_g3278 , m_ActiveFadeIn3050_g3278 );
				float3 temp_output_457_0_g37836 = localfloat3switch3050_g3278;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = temp_output_457_0_g37836;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult128_g37836 = (float2(_TilingX , _TilingY));
				float2 appendResult125_g37836 = (float2(_OffsetX , _OffsetY));
				float2 texCoord2_g37836 = IN.ase_texcoord2.xy * appendResult128_g37836 + appendResult125_g37836;
				float4 tex2DNode3_g37836 = SAMPLE_TEXTURE2D( _MainTex, sampler_trilinear_repeat, texCoord2_g37836 );
				float4 ALBEDO_RGBA529_g37836 = tex2DNode3_g37836;
				float3 temp_output_28_0_g37836 = ( (_Color).rgb * (ALBEDO_RGBA529_g37836).rgb * _Brightness );
				
				float ALBEDO_A414_g37836 = tex2DNode3_g37836.a;
				int m_switch500_g37836 = _GlancingClipMode;
				float m_Off500_g37836 = 1.0;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = SafeNormalize( ase_worldViewDir );
				float3 normalizeResult472_g37836 = normalize( cross( ddx( WorldPosition ) , ddy( WorldPosition ) ) );
				float dotResult475_g37836 = dot( ase_worldViewDir , normalizeResult472_g37836 );
				float temp_output_497_0_g37836 = ( 1.0 - abs( dotResult475_g37836 ) );
				float m_Active500_g37836 = ( 1.0 - ( temp_output_497_0_g37836 * temp_output_497_0_g37836 ) );
				float localfloatswitch500_g37836 = floatswitch500_g37836( m_switch500_g37836 , m_Off500_g37836 , m_Active500_g37836 );
				float OPACITY_OUTMASK494_g37836 = localfloatswitch500_g37836;
				
				float AlphaCutoffBias501_g37836 = _AlphaCutoffBias;
				
				
				float3 Albedo = ( float4( temp_output_28_0_g37836 , 0.0 ) + float4(0,0,0,0) ).xyz;
				float Alpha = ( ALBEDO_A414_g37836 * OPACITY_OUTMASK494_g37836 );
				float AlphaClipThreshold = AlphaCutoffBias501_g37836;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	/*ase_lod*/
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback " "
	
}
/*ASEBEGIN
Version=18913
4;29.33333;1436;766;1043.793;74.083;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;411;67.82877,135.8749;Inherit;False;350.5028;188.4201;;3;51;188;187;DEBUG SETTINGS ;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;410;71.65128,-21.5294;Inherit;False;178;128;;1;438;GLOBAL SETTINGS ;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;447;73.70647,656.9171;Inherit;False;320.6667;116;DESF Common ASE Compile Shaders;1;448;;0,0.2047877,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;445;-575.9403,337.6561;Inherit;False;DESF Module Wind;61;;3278;b135a554f7e4d0b41bba02c61b34ae74;11,938,0,2881,0,2454,2,2457,2,2432,2,2434,2,2433,2,2371,2,2752,0,2995,0,2878,0;0;2;FLOAT3;2190;FLOAT4;2970
Node;AmplifyShaderEditor.IntNode;438;91.83203,19.4014;Inherit;False;Property;_CullMode;Cull Mode;2;2;[Header];[Enum];Create;False;1;GLOBAL SETTINGS;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;2;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;188;73.08816,172.2121;Inherit;False;Property;_ColorMask;Color Mask Mode;1;1;[Enum];Create;False;1;;0;1;None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15;True;0;False;15;15;False;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;77.57893,248.4035;Inherit;False;Constant;_MaskClipValue;Mask Clip Value;19;1;[HideInInspector];Create;True;1;;0;0;True;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;446;-340.2397,336.6565;Inherit;False;DESF Core Billboard;3;;37836;e3fce2294f3bde941a48e407ffdf732f;15,504,2,502,2,572,2,139,2,582,0,490,1,503,1,617,0,69,0,540,0,361,0,541,0,612,0,613,0,614,0;2;457;FLOAT3;0,0,0;False;454;FLOAT4;0,0,0,0;False;9;FLOAT4;0;FLOAT3;556;FLOAT;56;FLOAT;50;FLOAT;57;FLOAT;49;FLOAT;351;FLOAT;146;FLOAT3;458
Node;AmplifyShaderEditor.FunctionNode;448;87.7065,696.9171;Inherit;False;DESF Common ASE Compile Shaders;-1;;39557;b85b01c42ba8a8a448b731b68fc0dbd9;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;187;256.7663,174.283;Inherit;False;Property;_ZWriteMode;ZWrite Mode;0;2;[Header];[Enum];Create;False;1;DEBUG SETTINGS;0;1;Off,0,On,1;True;0;False;1;1;False;0;1;INT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;436;63.95,341.8214;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;18;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;432;63.95,341.8214;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;18;all;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;434;63.95,341.8214;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;18;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;435;63.95,341.8214;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;18;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;433;63.95,341.8214;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;DEC/Billboard/Billboard Wind;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;2;True;438;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=-10;NatureRendererInstancing=True;True;0;True;18;all;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;True;188;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;True;187;True;3;False;283;True;True;0;False;-1;0;False;-1;True;2;LightMode=UniversalForward;NatureRendererInstancing=True;False;False;6;Include;;False;;Native;Pragma;multi_compile_instancing;False;;Custom;Pragma;instancing_options procedural:SetupNatureRenderer forwardadd;False;;Custom;Pragma;multi_compile GPU_FRUSTUM_ON __;False;;Custom;Include;Nature Renderer.cginc;False;ed9205546b797304ea7576ba0b32877e;Custom;Pragma;multi_compile_local _ NATURE_RENDERER;False;;Custom; ;0;0;Standard;38;Workflow;1;Surface;0;  Refraction Model;0;  Blend;0;Two Sided;1;Fragment Normal Space,InvertActionOnDeselection;0;Transmission;0;  Transmission Shadow;0.5,False,-1;Translucency;0;  Translucency Strength;1,False,-1;  Normal Distortion;0.5,False,-1;  Scattering;2,False,-1;  Direct;0.9,False,-1;  Ambient;0.1,False,-1;  Shadow;0.5,False,-1;Cast Shadows;1;  Use Shadow Threshold;1;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;_FinalColorxAlpha;0;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;DOTS Instancing;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Write Depth;0;  Early Z;0;Vertex Position,InvertActionOnDeselection;1;0;6;False;True;True;True;True;True;False;;True;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;437;63.95,341.8214;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;18;all;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;True;188;False;False;False;False;False;False;False;False;False;True;1;True;187;True;3;False;283;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;446;457;445;2190
WireConnection;446;454;445;2970
WireConnection;433;0;446;0
WireConnection;433;1;446;556
WireConnection;433;3;446;56
WireConnection;433;4;446;50
WireConnection;433;5;446;57
WireConnection;433;6;446;49
WireConnection;433;7;446;351
WireConnection;433;16;446;146
WireConnection;433;10;446;458
ASEEND*/
//CHKSM=63553B4378453590FAEE9457684AF0ED2CEE57E3