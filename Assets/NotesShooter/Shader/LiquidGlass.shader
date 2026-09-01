// 背景とガラスを1つのシェーダーで兼ねる。
// _Mode = 0 : 背景（ゆっくり動く水色のグラデーション）
// _Mode = 1 : ガラス（背景と同じ模様をぼかして屈折させ、角丸と縁の光沢を足す）
// ガラス側が背景と同じ関数を screenUV で評価するので、GrabPassや_CameraOpaqueTextureが要らない。
// モバイルのGLES3でも動くよう、ループは固定回数に展開している。
Shader "NotesShooter/LiquidGlass"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		// 0=背景（自前のグラデーションを描く）
		// 1=ガラス（背景と同じ模様をぼかす。リザルト等、背景も自前で描いている画面用）
		// 2=ガラス（実際の画面をぼかす。Screen Space Cameraのキャンバス用）
		// 3=ガラス（背景を読まず半透明で重ねる。Screen Space OverlayのHUD用）
		_Mode ("0=背景 1=ガラス(自前背景) 2=ガラス(実画面)", Float) = 0

		_ColorDeep ("奥の色", Color) = (0.055, 0.271, 0.408, 1)
		_ColorMid ("中間の色", Color) = (0.208, 0.588, 0.788, 1)
		_ColorLight ("明るい色", Color) = (0.639, 0.886, 0.984, 1)

		_Speed ("背景の動く速さ", Range(0, 1)) = 0.12

		_RectSize ("ガラスの大きさ(px)", Vector) = (760, 300, 0, 0)
		_Radius ("角丸の半径(px)", Float) = 42
		_Blur ("ぼかしの強さ", Range(0, 0.08)) = 0.03
		_Refraction ("屈折の強さ", Range(0, 0.08)) = 0.022
		_GlassTint ("ガラスの色味", Color) = (1, 1, 1, 0.10)
		_RimWidth ("縁の太さ(px)", Float) = 1.6
		_RimPower ("縁の光沢の強さ", Range(0, 3)) = 1.5

		_Color ("Tint", Color) = (1,1,1,1)

		// ScrollViewのMask等で切り抜く為の値。uGUIが実行時に書き込む。
		// この項目が無いと、マスクの中に居ても切り抜かれずにはみ出して描かれてしまう
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
			};

			float _Mode;
			fixed4 _ColorDeep;
			fixed4 _ColorMid;
			fixed4 _ColorLight;
			float _Speed;
			float4 _RectSize;
			float _Radius;
			float _Blur;
			float _Refraction;
			fixed4 _GlassTint;
			float _RimWidth;
			float _RimPower;
			fixed4 _Color;
			//URPが用意する不透明部分の画面。ゲーム中のHUDはこれをぼかして背景にする
			sampler2D _CameraOpaqueTexture;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _Color;
				o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}

			// 柔らかい光の玉。中心に近いほど1に近づく
			float Blob(float2 p, float2 center, float radius)
			{
				float d = distance(p, center);
				return saturate(1.0 - smoothstep(0.0, radius, d));
			}

			// 背景の模様。画面UV(アスペクト補正済み)と時間から色を決める
			float3 Background(float2 uv)
			{
				float t = _Time.y * _Speed;

				// ゆっくり漂う4つの光の玉を重ねる
				float2 c1 = float2(0.28 + 0.10 * sin(t * 0.9), 0.72 + 0.07 * cos(t * 1.1));
				float2 c2 = float2(0.76 + 0.09 * cos(t * 0.7), 0.34 + 0.10 * sin(t * 0.8));
				float2 c3 = float2(0.52 + 0.12 * sin(t * 0.5 + 1.7), 0.55 + 0.09 * cos(t * 0.6 + 0.4));
				float2 c4 = float2(0.12 + 0.07 * cos(t * 1.3), 0.20 + 0.08 * sin(t * 0.9 + 2.1));

				float3 col = _ColorDeep.rgb;
				col = lerp(col, _ColorMid.rgb,   Blob(uv, c1, 0.55));
				col = lerp(col, _ColorLight.rgb, Blob(uv, c2, 0.42) * 0.75);
				col = lerp(col, _ColorMid.rgb,   Blob(uv, c3, 0.60) * 0.60);
				col = lerp(col, _ColorLight.rgb, Blob(uv, c4, 0.38) * 0.50);

				return col;
			}

			// ガラスが背後として使う色。_Mode=2は実際の画面、それ以外は自前の模様
			float3 SampleBehind(float2 uv)
			{
				if (_Mode > 1.5)
				{
					return tex2D(_CameraOpaqueTexture, saturate(uv)).rgb;
				}
				return Background(uv);
			}

			// 角丸長方形までの符号付き距離(px)。中が負、外が正
			float RoundedBoxDistance(float2 positionPx, float2 halfSizePx, float radiusPx)
			{
				float2 q = abs(positionPx) - halfSizePx + radiusPx;
				return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - radiusPx;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// 画面UV。横長でも模様が歪まないようアスペクトを補正する
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float aspect = _ScreenParams.x / max(_ScreenParams.y, 1.0);
				float2 patternUV = float2(screenUV.x * aspect, screenUV.y);
				patternUV.x /= max(aspect, 0.0001);
				patternUV = float2(screenUV.x, screenUV.y);

				if (_Mode < 0.5)
				{
					// 背景モード
					float3 col = Background(patternUV);
					return fixed4(col, i.color.a) * fixed4(1, 1, 1, 1) * fixed4(i.color.rgb, 1);
				}

				// ここからガラスモード
				float2 halfSize = _RectSize.xy * 0.5;
				float2 positionPx = (i.uv - 0.5) * _RectSize.xy;
				float dist = RoundedBoxDistance(positionPx, halfSize, _Radius);

				// 角丸の外は描かない
				float inside = 1.0 - smoothstep(-1.0, 1.0, dist);
				if (inside <= 0.001)
				{
					return fixed4(0, 0, 0, 0);
				}

				// 縁に近いほど強くなる値。屈折と光沢に使う
				float edge = saturate(1.0 - saturate(-dist / max(_Radius, 1.0)));

				// 縁ほど大きく外側へずらして、ガラスの縁で背景が伸びる感じを出す
				float2 normal = normalize(float2(ddx(dist), ddy(dist)) + 1e-5);
				float2 refracted = patternUV + normal * _Refraction * edge * edge;

				// 9点サンプルのぼかし（_Mode=3では背景を読まないので使わない）
				float3 blurred = 0;
				if (_Mode < 2.5)
				{
				float2 o1 = float2(_Blur, 0);
				float2 o2 = float2(0, _Blur);
				float2 o3 = float2(_Blur, _Blur) * 0.7071;
				float2 o4 = float2(_Blur, -_Blur) * 0.7071;
				blurred += SampleBehind(refracted) * 0.28;
				blurred += SampleBehind(refracted + o1) * 0.09;
				blurred += SampleBehind(refracted - o1) * 0.09;
				blurred += SampleBehind(refracted + o2) * 0.09;
				blurred += SampleBehind(refracted - o2) * 0.09;
				blurred += SampleBehind(refracted + o3) * 0.09;
				blurred += SampleBehind(refracted - o3) * 0.09;
				blurred += SampleBehind(refracted + o4) * 0.09;
				blurred += SampleBehind(refracted - o4) * 0.09;
				}

				// ガラス自体のうっすらした白み
				float3 col = lerp(blurred, _GlassTint.rgb, _GlassTint.a);
				float baseAlpha = 1.0;

				if (_Mode > 2.5)
				{
					//背景を読めない環境用。板自体を半透明にして背後をそのまま透けさせる
					col = _GlassTint.rgb;
					baseAlpha = _GlassTint.a;
				}

				// 上から下へ薄く光を乗せて、板の厚みを感じさせる
				float sheen = saturate(0.55 - i.uv.y * 0.55) * 0.18;
				col += sheen;

				// 縁の光沢。左上が明るく右下が暗い、いわゆるガラスのふち
				float rim = 1.0 - smoothstep(0.0, _RimWidth, abs(dist));
				float rimDirection = saturate(0.5 + 0.5 * dot(normal, normalize(float2(-0.6, 0.8))));
				col += rim * rimDirection * _RimPower;

				//ガラスモードは板の色を自分で決めているので、そのままだと
				//uGUIがボタンを押した時に送ってくる色が効かず、押した事が分からない。
				//最後に掛けて、色の変化が板にも乗るようにする
				col *= i.color.rgb;

				//縁は板本体より濃くして、ガラスの輪郭がはっきり見えるようにする
				float alpha = inside * i.color.a * max(baseAlpha, rim * 0.85);
				return fixed4(col, alpha);
			}
			ENDCG
		}
	}

	Fallback "UI/Default"
}
