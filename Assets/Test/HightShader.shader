// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/HightShader"
{
    Properties
    {
       // _Color ("Color", Color) = (1,1,1,1)
       _Specular("Specular",Color)=(1,1,1,1)
        _Grass("Grass",Range(8.0,256))=20
       //  _Diffuse("Diffuse",Color)=(1,1,1,1)
         _MainTex("MainTex",2D)="white"{}
         _Color("Color",Color)=(1,1,1,1)
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
             #include "UnityCG.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
             fixed4  _Specular;
             float _Grass;
            // fixed4  _Diffuse;

             struct a2v {

                 float4 vertex : POSITION;
                 float3 normal:  NORMAL;
                 float4 texcoord:TEXCOORD0;
             };
             struct v2f {
                  float4 pos : SV_POSITION;
                  float3 worldNormal : TEXCOORD0;
                  float3 worldPos:TEXCOORD1;
                  float2 uv:TEXCOORD2;
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
                o.worldNormal=mul(v.normal, unity_WorldToObject);
                o.worldPos=mul( unity_ObjectToWorld,v.vertex);
                o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);

                 return o;
             }

             fixed4 frag(v2f i) : SV_Target
             {
                //  fixed3 c=i.color;
                fixed3 worldNormal=normalize(i.worldNormal);
                 fixed3 albedo=tex2D(_MainTex,i.uv).rgb*_Color.rgb;
                   fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
                    fixed3 worldLight=normalize(_WorldSpaceLightPos0.xyz);
                    fixed3 diffuse=_LightColor0.rgb*albedo*saturate(dot(worldNormal,worldLight));
                    fixed3 reflectDir=normalize(reflect(-worldLight,worldNormal) );
                    fixed3 viewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));
                    fixed3 halfDir=normalize(viewDir+worldLight);
                   
                    fixed3 specular=_LightColor0.rgb*_Specular.rgb*pow( saturate(dot(reflectDir,viewDir)),_Grass);

                    fixed3 color=ambient+diffuse+specular;
                 // c*=_Color.rgb;
                  return fixed4(color,1.0);
             }
             ENDCG
        }
       
      
       
    }
    FallBack "Diffuse"
}
