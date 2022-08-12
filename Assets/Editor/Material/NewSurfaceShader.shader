// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewSurfaceShader"
{
    Properties
    {
     _Color("Color Tint",Color) =(1,1,1,1)
     _MainTex( "Main Tex",2D)= "white"{}
       _Specular("Specular", Color) =(1,1,1,1)
      _Gioss("Gloss", Range (8.0,256))=20
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        //LOD 200

        Pass{
            Tags{"LightModel"="ForwardBase"}
             CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members _MainTex_ST,_Gioss)
#pragma exclude_renderers d3d11
             #pragma vertex vert
               
             #pragma fragment frag

             #include "UnityCG.cginc"
              #include "Lighting.cginc"
             struct v2f{
                 fixed4 _Color;
                 sampler2D _MainTex;
                 float4 _MainTex_ST;
                 fixed4 _Specular;
                 float _Gioss;
                  float4 pos:SV_POSITION;
                  fixed4 color:COLORO;
             };
             v2f vert(appdata_full i){
                  v2f o;
                  o.pos=UnityObjectToClipPos(i.vertex);
                  o.color=fixed4(i.normal*0.5+fixed3(0.5,0.5,0.5),1.0);
                   o.color=fixed4(i.tangent*0.5+fixed3(0.5,0.5,0.5),1.0);
                   o.color=fixed4(i.tangent*0.5+fixed3(0.5,0.5,0.5),1.0);
                   
                   o.color.a=1.0;
                   return o;
             }

               //float4 vert (float4 v: POSITION): SV_POSITION{
               //       return UnityObjectToClipPos(v);

               //}
               fixed4 frag(v2f e):SV_Target{
                      return e.color;
               }
               ENDCG
         }
    }
    FallBack "Diffuse"
}
        // Physically based Standard lighting model, and enable shadows on all light types
    //    #pragma surface surf Standard fullforwardshadows

    //    // Use shader model 3.0 target, to get nicer looking lighting
    //    #pragma target 3.0

    //    sampler2D _MainTex;

    //    struct Input
    //    {
    //        float2 uv_MainTex;
    //    };

    //    half _Glossiness;
    //    half _Metallic;
    //    fixed4 _Color;

    //    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    //    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    //    // #pragma instancing_options assumeuniformscaling
    //    UNITY_INSTANCING_BUFFER_START(Props)
    //        // put more per-instance properties here
    //    UNITY_INSTANCING_BUFFER_END(Props)

    //    void surf (Input IN, inout SurfaceOutputStandard o)
    //    {
    //        // Albedo comes from a texture tinted by color
    //        fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    //        o.Albedo = c.rgb;
    //        // Metallic and smoothness come from slider variables
    //        o.Metallic = _Metallic;
    //        o.Smoothness = _Glossiness;
    //        o.Alpha = c.a;
    //    }
    //    ENDCG
    //}
    //FallBack "Diffuse"
