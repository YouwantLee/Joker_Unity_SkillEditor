// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PBRMaskTint"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_SAM("SAM", 2D) = "white" {}
		_Color01("Color01", Color) = (0,0.1394524,0.8088235,0)
		_Color02("Color02", Color) = (0.4557808,0,0.6176471,0)
		_Color03("Color03", Color) = (0.4557808,0,0.6176471,0)
		_Color01Power("Color01Power", Range( 0 , 4)) = 1
		_Color02Power("Color02Power", Range( 0 , 4)) = 2
		_Color03Power("Color03Power", Range( 0 , 4)) = 1
		_Brightness("Brightness", Range( 0 , 4)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float4 _Color01;
		uniform float _Color01Power;
		uniform float4 _Color02;
		uniform float _Color02Power;
		uniform float4 _Color03;
		uniform float _Color03Power;
		uniform float _Brightness;
		uniform sampler2D _SAM;
		uniform float4 _SAM_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode5 = tex2D( _Mask, uv_Mask );
			float4 temp_cast_0 = (tex2DNode5.r).xxxx;
			float4 temp_cast_1 = (tex2DNode5.g).xxxx;
			float4 temp_cast_2 = (tex2DNode5.b).xxxx;
			float4 blendOpSrc20 = tex2DNode2;
			float4 blendOpDest20 = ( ( min( temp_cast_0 , _Color01 ) * _Color01Power ) + ( min( temp_cast_1 , _Color02 ) * _Color02Power ) + ( min( temp_cast_2 , _Color03 ) * _Color03Power ) );
			float4 lerpResult19 = lerp( tex2DNode2 , ( ( saturate( ( blendOpSrc20 * blendOpDest20 ) )) * _Brightness ) , ( tex2DNode5.r + tex2DNode5.g + tex2DNode5.b ));
			o.Albedo = lerpResult19.rgb;
			float2 uv_SAM = i.uv_texcoord * _SAM_ST.xy + _SAM_ST.zw;
			float4 tex2DNode4 = tex2D( _SAM, uv_SAM );
			o.Metallic = tex2DNode4.b;
			o.Smoothness = tex2DNode4.r;
			o.Occlusion = tex2DNode4.g;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
7;29;1906;1004;2806.809;843.2213;2.334039;True;True
Node;AmplifyShaderEditor.ColorNode;13;-1572.117,265.3254;Float;False;Property;_Color02;Color02;5;0;Create;True;0;0;False;0;0.4557808,0,0.6176471,0;0.4557808,0,0.6176471,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-1557.96,21.53706;Float;False;Property;_Color01;Color01;4;0;Create;True;0;0;False;0;0,0.1394524,0.8088235,0;0,0.1394524,0.8088235,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;31;-1580.33,537.1399;Float;False;Property;_Color03;Color03;6;0;Create;True;0;0;False;0;0.4557808,0,0.6176471,0;0.4557808,0,0.6176471,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-1575.187,-237.0183;Float;True;Property;_Mask;Mask;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMinOpNode;32;-1235.426,654.4802;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;16;-1224.938,240.3867;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;15;-1241.712,-66.45076;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1146.145,931.8398;Float;False;Property;_Color03Power;Color03Power;9;0;Create;True;0;0;False;0;1;1.4;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1259.98,516.8667;Float;False;Property;_Color02Power;Color02Power;8;0;Create;True;0;0;False;0;2;1.4;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1214.198,159.3364;Float;False;Property;_Color01Power;Color01Power;7;0;Create;True;0;0;False;0;1;1.4;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-966.4029,522.0165;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1015.061,286.6656;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1035.951,76.28061;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-797.9374,29.71212;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-1216.314,-532.9558;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;20;-597.9333,-143.9451;Float;False;Multiply;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-514.3958,0.1053672;Float;False;Property;_Brightness;Brightness;10;0;Create;True;0;0;False;0;1;1.8;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-300.5845,-167.9458;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-1032.066,-298.3265;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;-56.85875,-419.8503;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-346.5083,147.707;Float;True;Property;_SAM;SAM;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-168.2225,412.748;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;275.9479,-186.7888;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PBRMaskTint;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;5;3
WireConnection;32;1;31;0
WireConnection;16;0;5;2
WireConnection;16;1;13;0
WireConnection;15;0;5;1
WireConnection;15;1;6;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;37;0;16;0
WireConnection;37;1;38;0
WireConnection;35;0;15;0
WireConnection;35;1;36;0
WireConnection;18;0;35;0
WireConnection;18;1;37;0
WireConnection;18;2;34;0
WireConnection;20;0;2;0
WireConnection;20;1;18;0
WireConnection;22;0;20;0
WireConnection;22;1;23;0
WireConnection;21;0;5;1
WireConnection;21;1;5;2
WireConnection;21;2;5;3
WireConnection;19;0;2;0
WireConnection;19;1;22;0
WireConnection;19;2;21;0
WireConnection;0;0;19;0
WireConnection;0;1;1;0
WireConnection;0;3;4;3
WireConnection;0;4;4;1
WireConnection;0;5;4;2
ASEEND*/
//CHKSM=A54B2E37C02183AA1E7B75104324AE14B12869AE