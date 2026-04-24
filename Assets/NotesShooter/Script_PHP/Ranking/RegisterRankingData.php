<?php

// データベース接続設定
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "notesshooter";

// フォーム送信された値を受け取る（POST）
// ※ 本実装は入力のバリデーションやサニタイズを行っていない
$score = $_POST['score'];

// MySQLiでデータベースに接続
$connect = new mysqli($servername, $username, $password, $dbname);

// 接続確認（失敗したら処理を停止してエラーメッセージを表示）
if ($connect->connect_error) {
	die("Connection failed: " . $connect->connect_error);
}

// 処理開始メッセージ（デバッグ用）
//echo "受信：接続に成功しました。";
// レスポンスは JSON の真偽値を返す（クライアントが bool を受け取れるようにする）
header('Content-Type: application/json; charset=utf-8');


// INSERTで新規登録
// 注意: ここでは値を直接連結しているためSQLインジェクションの危険がある
$sql = "INSERT INTO users (score) VALUES ('" . $score . "')";
if ($connect->query($sql) === TRUE) {
	// 登録成功時のメッセージ
	//echo "受信：登録に成功しました。";
	// クライアント側で true を受け取れるように JSON エンコードして返す
	echo json_encode(true);
} else {
	// 登録失敗時のメッセージ
	//echo "受信：登録に失敗しました。";
	//クライアント側で false を受け取れるように JSON エンコードして返す
	echo json_encode(false);
}

// 接続を閉じる
$connect->close();
