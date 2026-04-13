<?php

// データベース接続設定
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "oz007";

// フォーム送信された値を受け取る（POST）
// ※ 本実装は入力のバリデーションやサニタイズを行っていない
$loginUser = $_POST['loginUser'];
$loginPass = $_POST['loginPass'];

// MySQLiでデータベースに接続
$conn = new mysqli($servername, $username, $password, $dbname);

// 接続確認（失敗したら処理を停止してエラーメッセージを表示）
if ($conn->connect_error) {
	die("Connection failed: " . $conn->connect_error);
}

// ユーザー名の重複チェック用SQLを組み立てて実行
// （該当ユーザー名の行が存在するかを確認する）
$sql = "SELECT username FROM users WHERE username = '" . $loginUser . "'";
$result = $conn->query($sql);

// 処理開始メッセージ（デバッグ用）
echo "受信：接続に成功しました。ユーザーを登録します。<br>";

// クエリ結果の行数で存在判定を行う
if ($result->num_rows > 0) {
	// 既に同じユーザー名が登録されている場合の処理
	echo "受信：ユーザーは既に存在します。";
} else {
	// 存在しない場合はINSERTで新規登録
	// 注意: ここでは値を直接連結しているためSQLインジェクションの危険がある
	$sql2 = "INSERT INTO users (username, password) VALUES ('" . $loginUser . "', '" . $loginPass . "')";
	if ($conn->query($sql2) === TRUE) {
		// 登録成功時のメッセージ
		echo "受信：ユーザーの登録に成功しました。";
	} else {
		// 登録失敗時のメッセージ
		echo "受信：ユーザーの登録に失敗しました。";
	}
}

// 接続を閉じる
$conn->close();

/*
  セキュリティと改善提案:
  - SQLインジェクション対策としてプリペアドステートメントを使用する。
  - パスワードは平文で保存せず、必ず password_hash() でハッシュ化して保存する。
  - 入力値のバリデーション（空チェック、長さ制限、許可文字の検査）を行う。
*/
