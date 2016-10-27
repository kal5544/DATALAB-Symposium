Shader "Hidden/NatCam/Zoom2D" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_ZoomFactor ("Zoom Factor", float) = 0
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"RenderType"="Transparent" 
			"IgnoreProjector"="True"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil {
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}
		
		Cull Off
		Lighting Off
		//ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				half2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			uniform sampler2D _MainTex;
			uniform float _ZoomFactor;
			uniform float4 _MainTex_ST;

			float2 ZoomUV (float2 uv) { //This must be declared before the vertex function
				//Declare constants
				float minScale = 0.6;
				float maxScale = 1.0;
				//Declare return
				float2 ret = 0;
				//Flip the zoom rationfrom [0,1] to [1,0]
				float zoomFlipped = 1 - _ZoomFactor;
				//Calculate the zoom
				float zoom = (zoomFlipped * (maxScale-minScale)) + minScale;
				//Transform the UV coordinate
				ret = (uv - half2(0.5, 0.5)) * zoom + half2(0.5, 0.5);
				//Return it
				return ret;
			}

			v2f vert (appdata_t v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = ZoomUV(TRANSFORM_TEX(v.texcoord, _MainTex));
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target	{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;				
				return col;
			}
			ENDCG
		}
	}
}
