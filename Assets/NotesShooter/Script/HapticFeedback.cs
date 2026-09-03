using UnityEngine;

/// <summary>
/// 端末を短く振動させる。
///
/// UnityのHandheld.Vibrateは長さを指定できず、Androidでは約0.5秒鳴ってしまう。
/// 撃つたびにそれが走ると震えっぱなしになるので、Android標準のVibratorを
/// 直接呼んで長さを指定している。
/// </summary>
public static class HapticFeedback
{
	[Tooltip("的を壊した時の長さ(ミリ秒)")]
	public const long DestroyMilliseconds = 40;
	[Tooltip("壊れなかった時の長さ(ミリ秒)。壊した時より弱くして手応えを分ける")]
	public const long DamageMilliseconds = 22;

	[Tooltip("振動の強さ。1から255まで。API26以上でのみ効く")]
	const int amplitude = 160;
	[Tooltip("VibrationEffectが使えるAndroidのバージョン")]
	const int vibrationEffectApiLevel = 26;

	[Tooltip("端末の情報を調べ終わったか。毎回調べると重いので一度だけにする")]
	static bool isPrepared = false;
	[Tooltip("振動できる端末か")]
	static bool canVibrate = false;

	[Tooltip("続けて振動させる時の最短の間隔(秒)。連射で震えっぱなしになるのを防ぐ")]
	const float minimumInterval = 0.06f;
	[Tooltip("最後に振動させた時刻")]
	static float lastPlayedTime = -1.0f;

#if UNITY_ANDROID && !UNITY_EDITOR
	static AndroidJavaObject vibrator = null;
	static AndroidJavaClass vibrationEffectClass = null;
	static int apiLevel = 0;
#endif

	/// <summary>
	/// 指定した長さだけ振動させる。振動できない端末やAndroid以外では何もしない
	/// </summary>
	public static void Play(long milliseconds)
	{
		if (milliseconds <= 0)
		{
			return;
		}

		//的が並んでいると一度に何発も当たる。そのまま全部鳴らすと震えっぱなしになる
		if (Time.unscaledTime - lastPlayedTime < minimumInterval)
		{
			return;
		}
		lastPlayedTime = Time.unscaledTime;

#if UNITY_ANDROID && !UNITY_EDITOR
		Prepare();

		if (canVibrate == false || vibrator == null)
		{
			return;
		}

		if (vibrationEffectApiLevel <= apiLevel && vibrationEffectClass != null)
		{
			//強さも指定できる新しい方
			using (AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude))
			{
				vibrator.Call("vibrate", effect);
			}
		}
		else
		{
			//古い端末向け。長さだけ指定できる
			vibrator.Call("vibrate", milliseconds);
		}
#else
		//エディタでは振動できないので、呼ばれた事だけ分かるようにしておく
		Debug.Log("振動: " + milliseconds + "ミリ秒");
#endif
	}

#if UNITY_ANDROID && !UNITY_EDITOR
	/// <summary>
	/// 端末のVibratorとAndroidのバージョンを一度だけ調べる
	/// </summary>
	static void Prepare()
	{
		if (isPrepared == true)
		{
			return;
		}
		isPrepared = true;

		try
		{
			using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using (AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
			}

			if (vibrator == null)
			{
				return;
			}

			//振動する部品を持っていない端末もある
			canVibrate = vibrator.Call<bool>("hasVibrator");

			using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
			{
				apiLevel = version.GetStatic<int>("SDK_INT");
			}

			if (vibrationEffectApiLevel <= apiLevel)
			{
				vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
			}
		}
		catch (System.Exception exception)
		{
			//振動できなくても遊べるので、失敗しても止めない
			canVibrate = false;
			Debug.LogWarning("振動の準備に失敗しました: " + exception.Message);
		}
	}
#endif
}
