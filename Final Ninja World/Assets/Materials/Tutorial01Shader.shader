﻿Shader "Unlit/Tutorial01Shader"
{//on camera, can do post processing effects
    Properties
    {
        _MainTex ("Texture", 2D) = "pink" {}//default texture color, if no texture will be pink
        _Color("Color",Color) = (1,1,1,1)//default value rgba (alpha=visible)
    }
    SubShader//alters shader for each item of a same material
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM//shader language, how things are rendered on screen
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;//vector 4
                float2 uv : TEXCOORD0;//texture(imported?)
            };

            struct v2f//vertex to fragment
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;//refers to the color in properties
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)//in (v)
            {
                v2f o;//out
                o.vertex = UnityObjectToClipPos(v.vertex);//takes object and puts it in perspective with camera
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
               
                return col*_Color;//pixel color
                 // to change texture color, multiply texture by color(rgb)
            }
            ENDCG
        }
    }
}
