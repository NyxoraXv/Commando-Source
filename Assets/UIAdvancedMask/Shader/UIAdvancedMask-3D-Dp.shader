// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Advanced Masked/3D with Depth"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 100


		Pass
		{
			ZWrite On
			Offset 1,1
			ColorMask 0
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ ALPHA_FLIPPED
			#pragma multi_compile __ FORCE_CLAMP_X
			#pragma multi_compile __ FORCE_CLAMP_Y
			#pragma multi_compile __ RED_AS_ALPHA

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 uvShadow : TEXCOORD1;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;
			float4x4 _Projection;
			float _AlphaMultiply;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvShadow = mul(_Projection,mul(unity_ObjectToWorld, v.vertex));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 uv =  UNITY_PROJ_COORD(i.uvShadow);
				#ifdef FORCE_CLAMP_X
				uv.x = clamp(uv.x,0,1);
				#endif
				#ifdef FORCE_CLAMP_Y
				uv.y = clamp(uv.y,0,1);
				#endif
				#ifdef RED_AS_ALPHA
				half a = tex2Dproj (_MaskTex,uv).r;
				#else
				half a = tex2Dproj (_MaskTex,uv).a;
				#endif
				#ifdef ALPHA_FLIPPED
				a = 1-a;
				#endif
				a *=_AlphaMultiply;

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*_Color;
				col.a *=a;
				clip(col.a-0.99);
				return 0;
			}
			ENDCG
		}
		Pass
		{
			ZWrite Off
			Offset 0,0
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask rgb
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ ALPHA_FLIPPED
			#pragma multi_compile __ FORCE_CLAMP_X
			#pragma multi_compile __ FORCE_CLAMP_Y
			#pragma multi_compile __ RED_AS_ALPHA

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 uvShadow : TEXCOORD1;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;
			float4x4 _Projection;
			float _AlphaMultiply;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvShadow = mul(_Projection,mul(unity_ObjectToWorld, v.vertex));
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 uv =  UNITY_PROJ_COORD(i.uvShadow);
				#ifdef FORCE_CLAMP_X
				uv.x = clamp(uv.x,0,1);
				#endif
				#ifdef FORCE_CLAMP_Y
				uv.y = clamp(uv.y,0,1);
				#endif
				#ifdef RED_AS_ALPHA
				half a = tex2Dproj (_MaskTex,uv).r;
				#else
				half a = tex2Dproj (_MaskTex,uv).a;
				#endif
				#ifdef ALPHA_FLIPPED
				a = 1-a;
				#endif
				a *=_AlphaMultiply;

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*_Color;
				col.a *=a;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
