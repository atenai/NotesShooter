<?php

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "oz007";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
	die("Connection failed: " . $conn->connect_error);
}

echo "Connected successfully, now we will show the users.";
echo "<br>";
echo "<br>";
echo "<br>";

$sql = "SELECT username, level FROM users";
$result = $conn->query($sql);
if ($result->num_rows > 0) {
	while ($row = $result->fetch_assoc()) {
		echo "Username: " . $row["username"] . " - Level: " . $row["level"] . "<br>";
	}
} else {
	echo "No users found.";
}

$conn->close();
