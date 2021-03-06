Shader "Custom/022-Wet"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Normal("NormalMap",2D) = "bump"{}
		_NormalScale("NormalScale",Range(0,5)) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_WetColor("WetColor",Color) = (1,1,1,1)
		_WetMap("WetMap",2D) = "white"{}
		_WetGlossiness("Smoothness", Range(0,1)) = 0.5
		_WetMetallic("Metallic", Range(0,1)) = 0.0
		_Wetness("Wetness",Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _WetMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Normal;
            float2 uv_WetMap;
        };

        half _Glossiness;
        half _Metallic;
		half _WetGlossiness;
		half _WetMetallic;
        fixed4 _Color;
		half _NormalScale;
		fixed4 _WetColor;
		half _Wetness;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed wetness = tex2D(_WetMap, IN.uv_WetMap).r * _Wetness;
			
            // Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * lerp(_Color, _WetColor, wetness);
            o.Albedo = c.rgb;
			o.Normal = lerp(UnpackScaleNormal(tex2D(_Normal, IN.uv_Normal), _NormalScale), half3(0, 0, 1), wetness);
            // Metallic and smoothness come from slider variables
            o.Metallic = lerp(_Metallic, _WetMetallic, wetness);
            o.Smoothness = lerp(_Glossiness, _WetGlossiness, wetness);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
