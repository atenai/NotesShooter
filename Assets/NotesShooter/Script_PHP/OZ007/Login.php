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

echo "受信：接続に成功しました。ユーザーを表示します。<br>";

$sql = "SELECT password FROM users WHERE username = '" . $loginUser . "'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
	echo "受信：ユーザーが見つかりました。パスワードを確認します。<br>";
	while ($row = $result->fetch_assoc()) {
		echo "受信：ユーザー名: " . $loginUser . "<br>";
		echo "受信：パスワード: " . $row["password"] . "<br>";

		if ($row["password"] == $loginPass) {
			echo "受信：ログインに成功しました！";
		} else {
			echo "受信：パスワードが一致しません！";
		}
	}
} else {
	echo "受信：ログインに失敗しました。ユーザー名またはパスワードが正しくありません。";
}

$conn->close();
