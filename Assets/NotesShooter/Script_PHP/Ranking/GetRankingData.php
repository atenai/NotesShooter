<?php

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "notesshooter";

//sql接続
$connect = new mysqli($servername, $username, $password, $dbname);

//接続確認
if ($connect->connect_error) {
	//接続失敗
	die("Connection failed: " . $connect->connect_error);
}

//usersからscoreを取得して表示
$sql = "SELECT score FROM users";
//クエリ実行
$result = $connect->query($sql);
//クエリ結果の確認
if ($result->num_rows > 0) {
	//クエリ結果がある場合、scoreを表示
	while ($row = $result->fetch_assoc()) {
		//スコアを表示
		echo $row["score"] . ",";
	}
} else {
	//クエリ結果がない場合、スコアが見つからないことを表示
	echo "スコアが見つかりませんでした。";
}

//接続を閉じる
$connect->close();
