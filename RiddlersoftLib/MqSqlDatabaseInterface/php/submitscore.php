<?php 
include("connect.php");
?>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Add New Score</title>
</head>

<?php
$NotValide = false;
$error;
$scoreboard = $_POST["scoreboard"];
if (is_null($scoreboard))
{
	$notvalid = true;
	$error = "scoreboard";
}

$name = $_SESSION['user'];
if (is_null($name))
{
	$notvalid = true;
	$error = "user";
}

$score = $_POST["score"];
if (is_null($score))
{
	$notvalid = true;
	$error = "score";
}

$id = $_SESSION['userId'];
if (is_null($id))
{	
	$notvalid = true;
	$error = "user id";
}	

$sortby = $_POST["sortby"];
if (is_null($sortby))
{
	$notvalid = true;
	$error = "sort by";
}

		
function Find_Or_Create_Table($tablename)
{
mysql_query("CREATE TABLE `riddler_osr2`.`".$tablename."` (
`Name` CHAR( 255 ) NOT NULL ,
`Score` INT NOT NULL ,
`Date` DATETIME NOT NULL ,
`Id` CHAR( 255 ) NOT NULL ,
PRIMARY KEY ( `Id` )
) ENGINE = MYISAM ");
echo "Table Created";
}

function Find_And_Update_As_Requried($scoreboard,$name,$score,$id)

{
	echo "searching<br />";
	$replace = true;
	echo "SELECT * FROM `".$scoreboard."` WHERE `Id` LIKE `".$id."`";
	$entery = mysql_query("SELECT * FROM `".$scoreboard."` WHERE `Id` LIKE '".$id."'") or die(mysql_error());
	if (!empty($entery))
	{
		$row = mysql_fetch_array($entery);
		$bestscore = $row["Score"];

		if (!empty($bestscore))
		{
			echo "score already exists<br />";
			$replace = false;
			//now we need to check if the new score is better than the old score.

			if ($sortby == "d")
			{
				if ($bestscore < $score)
				{
					//now we need to update the entery
					mysql_query("UPDATE ".$scoreboard." SET Score='".$score."' WHERE Id='".$id."'");
					mysql_query("UPDATE ".$scoreboard." SET Date=NOW() WHERE Id='".$id."'");
					echo "Score Updated";
				}
				else
					echo "better score exists your score is not high enough";				
			}
			else
			{
				if ($bestscore > $score)
				{
				//now we need to update the entery
				mysql_query("UPDATE ".$scoreboard." SET Score='".$score."' WHERE Id='".$id."'");
				mysql_query("UPDATE ".$scoreboard." SET Date=NOW() WHERE Id='".$id."'");
				echo "Score Updated";
				}
				else
					echo "quicker time alread got";				
			}
		}
		else
			echo "score not found";
	}
	else
		echo "score not found";
	return $replace;
}

$database = "database name"
function Add($scoreboard,$name,$score,$id)
{
	$string = "INSERT INTO ".$scoreboard." (Name, Score, Date, Id)  VALUES('".$name."', '".$score."', NOW(), '".$id."' )";
	echo "adding record <br />";
	mysql_query($string) or die(mysql_error());
	echo $string. " added to scoreboard";
	return true;
}


//if (ValidatePassword($scoreboard,$name,$score,$pass))
//	{
		if (!$notvalid)
		{		
			echo "Connected!! <br />";
			GetCredentials("add", $database); //get credentials for the database
			Connect();  //connect to the database
			echo "Connected to old school racer 2 database<br />";
			Find_Or_Create_Table($scoreboard);
			if (Find_And_Update_As_Requried($scoreboard, $name, $score, $id))
			{
				Add($scoreboard,$name,$score,$id);
			}
		}
		else
			echo "not valid scores";
//	}
//	else
//		echo "error";
?>

<body>

</body>

</html>

