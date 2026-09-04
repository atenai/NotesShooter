using System.ComponentModel;
using UnityEngine;

/// <summary>
/// SRDebuggerのOptionsタブに出す、このゲーム用の項目。
///
/// SROptionsはpartialなので、ここにメソッドを足すだけで画面にボタンが並ぶ。
/// SortAttributeはプロパティにしか付けられないので、ボタンには使えない。
/// </summary>
public partial class SROptions
{
	[Category("セーブデータ")]
	[DisplayName("セーブデータを初期化")]
	public void ResetSaveData()
	{
		ScoreRecord.DeleteAll();

		//押した結果がその場で分かるよう、SRDebuggerのコンソールにも出しておく
		Debug.Log("セーブデータを初期化しました。ハイスコア・進行状況・解除の演出を見たかの記録を消しました。"
			+ "今出ている画面には反映されないので、シーンを移動し直してください");
	}
}
