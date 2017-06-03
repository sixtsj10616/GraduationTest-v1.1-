// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.31 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.31;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:1,fgcb:1,fgca:1,fgde:0.008,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32443,y:32577|diff-1291-OUT,normal-469-RGB,alpha-1167-OUT,clip-60-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33303,y:32735,ptlb:Diffuse,ptin:_Diffuse,tex:628e341aa2ff7a34f8eda99c97ade246,ntxv:0,isnm:False|UVIN-233-OUT;n:type:ShaderForge.SFN_Color,id:21,x:33315,y:32964,ptlb:Color,ptin:_Color,glob:False,c1:0.3676471,c2:0.3676471,c3:0.3676471,c4:1;n:type:ShaderForge.SFN_Multiply,id:23,x:32939,y:32767|A-3-RGB,B-21-RGB;n:type:ShaderForge.SFN_Slider,id:44,x:33315,y:33255,ptlb:AlphaCut,ptin:_AlphaCut,min:0,cur:0.8378915,max:1;n:type:ShaderForge.SFN_Multiply,id:60,x:32845,y:33031|A-3-A,B-44-OUT;n:type:ShaderForge.SFN_Tex2d,id:225,x:33837,y:32872,ptlb:Noise,ptin:_Noise,tex:4a252347595b0a34fb3b62f3768f431e,ntxv:0,isnm:False|UVIN-229-UVOUT;n:type:ShaderForge.SFN_Slider,id:227,x:33734,y:33146,ptlb:Ripple,ptin:_Ripple,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Panner,id:229,x:33615,y:32636,spu:0.1,spv:0.1;n:type:ShaderForge.SFN_TexCoord,id:231,x:33837,y:32636,uv:0;n:type:ShaderForge.SFN_Lerp,id:233,x:33498,y:32886|A-231-UVOUT,B-225-R,T-227-OUT;n:type:ShaderForge.SFN_Tex2d,id:469,x:33098,y:32568,ptlb:Normal,ptin:_Normal,tex:19573af8a7478914f8a178b26d23ff3f,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:1101,x:33020,y:32893|A-3-A,B-1140-OUT;n:type:ShaderForge.SFN_Slider,id:1140,x:33315,y:33157,ptlb:AlphaTransparency,ptin:_AlphaTransparency,min:0,cur:1,max:1;n:type:ShaderForge.SFN_TexCoord,id:1165,x:33660,y:33270,uv:0;n:type:ShaderForge.SFN_Multiply,id:1167,x:32845,y:32893|A-1204-OUT,B-1101-OUT,C-1234-OUT,D-1327-OUT,E-1332-OUT;n:type:ShaderForge.SFN_Add,id:1204,x:33023,y:33179|A-1165-V,B-1207-OUT;n:type:ShaderForge.SFN_Slider,id:1207,x:33315,y:33353,ptlb:BlendBottom,ptin:_BlendBottom,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:1234,x:33023,y:33308|A-1237-OUT,B-1240-OUT;n:type:ShaderForge.SFN_OneMinus,id:1237,x:33139,y:33458|IN-1165-V;n:type:ShaderForge.SFN_Slider,id:1240,x:33315,y:33458,ptlb:BlendUpper,ptin:_BlendUpper,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:1269,x:32956,y:32407,ptlb:DiffusePower,ptin:_DiffusePower,min:0,cur:1.518593,max:10;n:type:ShaderForge.SFN_Multiply,id:1291,x:32737,y:32482|A-23-OUT,B-1269-OUT;n:type:ShaderForge.SFN_Add,id:1327,x:33033,y:33580|A-1165-U,B-1328-OUT;n:type:ShaderForge.SFN_Slider,id:1328,x:33315,y:33558,ptlb:BlendLeft,ptin:_BlendLeft,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:1330,x:33315,y:33659,ptlb:BlendRight,ptin:_BlendRight,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:1332,x:33033,y:33715|A-1334-OUT,B-1330-OUT;n:type:ShaderForge.SFN_OneMinus,id:1334,x:33223,y:33757|IN-1165-U;proporder:3-21-1269-1140-44-1240-1328-1330-1207-469-225-227;pass:END;sub:END;*/

Shader "Shader Forge/Grass" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Color ("Color", Color) = (0.3676471,0.3676471,0.3676471,1)
        _DiffusePower ("DiffusePower", Range(0, 10)) = 1.518593
        _AlphaTransparency ("AlphaTransparency", Range(0, 1)) = 1
        _AlphaCut ("AlphaCut", Range(0, 1)) = 0.8378915
        _BlendUpper ("BlendUpper", Range(0, 1)) = 1
        _BlendLeft ("BlendLeft", Range(0, 1)) = 1
        _BlendRight ("BlendRight", Range(0, 1)) = 1
        _BlendBottom ("BlendBottom", Range(0, 1)) = 1
        _Normal ("Normal", 2D) = "bump" {}
        _Noise ("Noise", 2D) = "white" {}
        _Ripple ("Ripple", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _Color;
            uniform float _AlphaCut;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Ripple;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _AlphaTransparency;
            uniform float _BlendBottom;
            uniform float _BlendUpper;
            uniform float _DiffusePower;
            uniform float _BlendLeft;
            uniform float _BlendRight;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_1383 = _Time + _TimeEditor;
                float2 node_1382 = i.uv0;
                float2 node_229 = (node_1382.rg+node_1383.g*float2(0.1,0.1));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_229, _Noise));
                float2 node_233 = lerp(i.uv0.rg,float2(_Noise_var.r,_Noise_var.r),_Ripple);
                float4 node_3 = tex2D(_Diffuse,TRANSFORM_TEX(node_233, _Diffuse));
                clip((node_3.a*_AlphaCut) - 0.5);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_1382.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * ((node_3.rgb*_Color.rgb)*_DiffusePower);
                float2 node_1165 = i.uv0;
/// Final Color:
                return fixed4(finalColor,((node_1165.g+_BlendBottom)*(node_3.a*_AlphaTransparency)*((1.0 - node_1165.g)+_BlendUpper)*(node_1165.r+_BlendLeft)*((1.0 - node_1165.r)+_BlendRight)));
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _Color;
            uniform float _AlphaCut;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Ripple;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _AlphaTransparency;
            uniform float _BlendBottom;
            uniform float _BlendUpper;
            uniform float _DiffusePower;
            uniform float _BlendLeft;
            uniform float _BlendRight;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_1385 = _Time + _TimeEditor;
                float2 node_1384 = i.uv0;
                float2 node_229 = (node_1384.rg+node_1385.g*float2(0.1,0.1));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_229, _Noise));
                float2 node_233 = lerp(i.uv0.rg,float2(_Noise_var.r,_Noise_var.r),_Ripple);
                float4 node_3 = tex2D(_Diffuse,TRANSFORM_TEX(node_233, _Diffuse));
                clip((node_3.a*_AlphaCut) - 0.5);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_1384.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * ((node_3.rgb*_Color.rgb)*_DiffusePower);
                float2 node_1165 = i.uv0;
/// Final Color:
                return fixed4(finalColor * ((node_1165.g+_BlendBottom)*(node_3.a*_AlphaTransparency)*((1.0 - node_1165.g)+_BlendUpper)*(node_1165.r+_BlendLeft)*((1.0 - node_1165.r)+_BlendRight)),0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            Cull Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _AlphaCut;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Ripple;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_1387 = _Time + _TimeEditor;
                float2 node_229 = (i.uv0.rg+node_1387.g*float2(0.1,0.1));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_229, _Noise));
                float2 node_233 = lerp(i.uv0.rg,float2(_Noise_var.r,_Noise_var.r),_Ripple);
                float4 node_3 = tex2D(_Diffuse,TRANSFORM_TEX(node_233, _Diffuse));
                clip((node_3.a*_AlphaCut) - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _AlphaCut;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Ripple;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_1389 = _Time + _TimeEditor;
                float2 node_229 = (i.uv0.rg+node_1389.g*float2(0.1,0.1));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_229, _Noise));
                float2 node_233 = lerp(i.uv0.rg,float2(_Noise_var.r,_Noise_var.r),_Ripple);
                float4 node_3 = tex2D(_Diffuse,TRANSFORM_TEX(node_233, _Diffuse));
                clip((node_3.a*_AlphaCut) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
