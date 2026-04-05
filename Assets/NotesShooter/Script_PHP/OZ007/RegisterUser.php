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

$sql = "SELECT username FROM users WHERE username = '" . $loginUser . "'";
$result = $conn->query($sql);

echo "受信：接続に成功しました。ユーザーを登録します。<br>";

if ($result->num_rows > 0) {
	echo "受信：ユーザーは既に存在します。";
} else {
	// ユーザーが存在しない場合はINSERT INTOでSQLにデータを新規登録
	$sql2 = "INSERT INTO users (username, password) VALUES ('" . $loginUser . "', '" . $loginPass . "')";
	if ($conn->query($sql2) === TRUE) {
		echo "受信：ユーザーの登録に成功しました。";
	} else {
		echo "受信：ユーザーの登録に失敗しました。";
	}
}

$conn->close();
