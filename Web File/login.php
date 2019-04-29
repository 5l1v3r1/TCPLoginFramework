<?php
error_reporting(0);
ini_set('display_errors', 0);

$username = $_GET["username"];
$password = $_GET["password"];

if ($username == "LoaderTesting" && $password == "LoaderTesting")
{
    echo "TestTrue";
}
else if (strtolower($username) == "thaisen" && $password == "PassWord123")
{
    echo "1";
}
else
{
    echo "0";
}

?>