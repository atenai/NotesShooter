<?php

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "oz007";

$loginUser = $_POST['loginUser'];
$loginPass = $_POST['loginPass'];

$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
	die("Connection failed: " . $conn->connect_error);
}

//echo "受信：接続に成功しました。ユーザーを表示します。";
// レスポンスは JSON の真偽値を返す（クライアントが bool を受け取れるようにする）
header('Content-Type: application/json; charset=utf-8');

$sql = "SELECT password FROM users WHERE username = '" . $loginUser . "'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
	//echo "受信：ユーザーが見つかりました。パスワードを確認します。";
	while ($row = $result->fetch_assoc()) {
		//echo "受信：ユーザー名: " . $loginUser . "<br>";
		//echo "受信：パスワード: " . $row["password"] . "<br>";

		if ($row["password"] == $loginPass) {
			//echo "受信：ログインに成功しました！";
			// クライアント側で true を受け取れるように JSON エンコードして返す
			echo json_encode(true);
		} else {
			//echo "受信：パスワードが一致しません！";
			// クライアント側で false を受け取れるように JSON エンコードして返す
			echo json_encode(false);
		}
	}
} else {
	//echo "受信：ログインに失敗しました。ユーザー名またはパスワードが正しくありません。";
	// クライアント側で false を受け取れるように JSON エンコードして返す
	echo json_encode(false);
}

$conn->close();
