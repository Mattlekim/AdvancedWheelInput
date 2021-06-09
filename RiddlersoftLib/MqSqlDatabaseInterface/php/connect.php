<?php
if (!isset($_SESSION)) session_start();

if (isset($_SESSION['user'])) //make suer we have a user
{	
	if (time() - $_SESSION['lastActivity'] >  600) //if longer than 10 minutes
	{
		//destroy session
		session_unset();
		session_destroy();
		die("#error-=#login required"); //kill paGE
	}
	else
		$_SESSION['lastActivity'] = time(); //update time last loged in
}
else
{
	die("#error-=#login required"); //KILL PAGE
}

$dbUserName; 
$dbPassword; 
$database;
function GetCredentials($type, $db)
{
	global $dbUserName;
	global $dbPassword;
	global $database; 
	//database logins
	$powerUserName = "";
	$powerPassword = "";

	//view credentials
	$dbUserName = "view user name";
	$dbPassword = "view passord";

	
	$database = $db;

	if ($_SESSION['PowerUser'] == true) //if we are a poweruser
	{
		$dbUserName = $powerUserName;
	    $dbPassword = $powerPassword;
	}
	else
		if ($type == "add")
		{
			//add credentials
			$dbUserName = "add score username";
		    $dbPassword = "add score password";
		}
}


function Connect()
{
	global $dbUserName;
	global $dbPassword;
	global $database;

	//if all is well connect to database
	mysql_connect("localhost", $dbUserName , $dbPassword) or die("did not connect");

	mysql_select_db($database) or die(mysql_error());
}
?>


