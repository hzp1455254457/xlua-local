// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TestShader1"
{
    Properties
    {
       // _Color ("Color", Color) = (1,1,1,1)
       _Diffuse("Diffuse",Color)=(1,1,1,1)
       //_Lighting("Lighting",Color)=(1,1,1,1)
    }
    SubShader
    {
       
        Pass{
            Tags{
                "Lightmode"="ForwardBase"
            }
            CGPROGRAM
            # pragma vertex vert
             # pragma fragment frag
             #include "Lighting.cginc"


          //   fixed4 _Color;
             fixed4 _Diffuse;
             //fixed4  _Lighting;
             struct a2v {

                 float4 vertex : POSITION;
                 float3 normal:  NORMAL;
             };
             struct v2f {
                  float4 pos : SV_POSITION;
                  float3 worldNormal : TEXCOORD0;
                   float3 worldPos:TEXCOORD1;
             };
            
         

             v2f vert(a2v v) 
             {
                 v2f o;
                 o.pos=UnityObjectToClipPos(v.vertex);
                // fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz;
                // fixed3 worldNormal=normalize(mul(v.normal, unity_WorldToObject));
                // fixed3 worldLight=normalize(_WorldSpaceLightPos0.xyz);
                // fixed3 diffuse=_LightColor0.rgb*_Diffuse.rgb*saturate(dot(worldNormal,worldLight));
                // o.color=ambient+diffuse;
                   o.worldPos=mul( unity_ObjectToWorld,v.vertex);
                o.worldNormal=mul(v.normal, unity_WorldToObject);
                 return o;
             }

             fixed4 frag(v2f i) : SV_Target
             {
                //  fixed3 c=i.color;
                fixed3 worldNormal=normalize(i.worldNormal);
                   fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz;
                    fixed3 worldLight=normalize(_WorldSpaceLightPos0.xyz);
                    fixed3 diffuse=_LightColor0.rgb*_Diffuse.rgb*(0.5*dot(worldNormal,worldLight)+0.5);
                    fixed3 color=ambient+diffuse;
                 // c*=_Color.rgb;
                  return fixed4(color,1.0);
             }
             ENDCG
        }
       
      
       
    }
    FallBack "Diffuse"
}
