// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/WenliShader"
{
    Properties
    {
       // _Color ("Color", Color) = (1,1,1,1)
       _Specular("Specular",Color)=(1,1,1,1)
        _Gloss("Grass",Range(8.0,256))=20
       //  _Diffuse("Diffuse",Color)=(1,1,1,1)
         _MainTex("MainTex",2D)="white"{}
         _BumpMap("BumpMap",2D)="bump"{}
         _BumpScale("BumpScale",float)= 1.0
         _Color("Color",Color)=(1,1,1,1)
    }
    SubShader
    {
       
        Pass{
            Tags{
                "Lightmode"="ForwardBase"
            }

           CGPROGRAM
 
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "UnityCG.cginc"


            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _BumpMap;
            float _BumpScale;
            float4 _MainTex_ST;
              float4 _BumpMap_ST;
             fixed4  _Specular;
             float _Gloss;
            // fixed4  _Diffuse;

             struct a2v {

                 float4 vertex : POSITION;
                 float3 normal:  NORMAL;
                 float4 texcoord:TEXCOORD0;
               
                 float4 tangent : TANGENT;
             };
             struct v2f {
                  float4 pos : SV_POSITION;
                  float3 lightDir : TEXCOORD0;
                  float3 viewDir:TEXCOORD1;
                  float4 uv:TEXCOORD2;
                  
             };
            
         

             v2f vert(a2v v) 
             {
                 v2f o;
                 o.pos=UnityObjectToClipPos(v.vertex);
            
              
              o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                TANGENT_SPACE_ROTATION;
 
				//ObjSpaceLightDir��ObjSpaceViewDir��Ϊunity���ú�������ȡģ�Ϳռ��µĹ��պ��ӽǷ���
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;

                 return o;
             }

             fixed4 frag(v2f i) : SV_Target
             {
           fixed3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentViewDir = normalize(i.viewDir);
 
				//����tex2D������_BumpMap��������������
				fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
				fixed3 tangentNormal;
				//���_BumpMap��normal map���ͣ�����Ҫ���ô˺����ѷ��߷�ӳ�������normal map�д洢����
				//�ѷ��߾���ӳ��������ֵ�����߷���ķ�����Χ��[-1,1],�����صķ�����Χ��[0,1],ͨ����ӳ����
				//pixel = (normal + 1)/2���������ﷴӳ����Ϊnormal = pixel * 2 - 1,���ɵõ�ԭ�ȵķ��߷���
				tangentNormal = UnpackNormal(packedNormal);
				tangentNormal.xy *= _BumpScale;
				//����tangentNormal.x��tangentNormal.y�����㸱���߷���
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
 
				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;  //������ļ���
 
				//������ļ���
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));
 
				//���淴��ļ���
				fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(tangentNormal, halfDir)), _Gloss);
 
				return fixed4(ambient + diffuse + specular, 1.0);

             }
             ENDCG
        }
       
      
       
    }
    FallBack "Specular"

}
