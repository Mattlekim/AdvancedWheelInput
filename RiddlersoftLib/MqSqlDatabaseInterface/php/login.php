<?php

if (!isset($_SESSION)) session_start();

$_SESSION['PowerUser'] = false; //if user is power user or not
if (isset($_SESSION['user'])) //check for login
{
	//kill login
	session_unset(); 
    session_destroy();
}
session_regenerate_id(true); //create new id

//log user in
$_SESSION['user'] = $_POST['username'];
$_SESSION['userId'] = $_user . rand(0, 10000); //create a id for the user
$_SESSION['lastActivity'] = time(); //keep track of when user loged in
	
?>

