<?php 
include 'connect.php';
$leaderboard = $_POST["leaderboard"];

$sortby = $_POST["sortby"];		

$database = "database name";
GetCredentials("add", $database); //get credentials for the database
Connect();  //connect to the database


?>


<html>

<head>

<title><?php echo $leaderboard." Leaderboards"?></title>

</head>

<body>
<?php
//locate all records from the scoreboard

if ($sortby == "1")

	$result = mysql_query("SELECT * FROM `".$leaderboard."` ORDER BY Score DESC LIMIT 0,150") or die(mysql_error());

else

	$result = mysql_query("SELECT * FROM `".$leaderboard."` ORDER BY Score ASC LIMIT 0,150") or die(mysql_error());



echo "<h1>".$leaderboard." Leaderboard</h1>".$sortby;



echo "<table border='1'>";

echo "<tr> <th>Score</th> <th>name</th> <th>id</th></tr>";

// keeps getting the next row until there are no more to get

$count = 0;

if (empty($result))

	echo "no scores found";

else

	while($row = mysql_fetch_array( $result )) 

	{
		// Print out the contents of each row into a table
		$count++;
		if ($count == 2)

			$count = 0;

		echo "<tr><td>"; 

		echo $row['Score'];
		echo "</td><td>";

		echo $row['Name'];
		echo "</td><td>";
		
	    echo $row['Id'];
		echo "</td></tr>"; 
	} 



echo "</table>";

?>

</body>

</html>

