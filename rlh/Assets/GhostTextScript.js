var pursuers : int;

function Start ()
{

}

function Update () {

}


function PursuerTally ()

{
	GetComponent(GUIText).text = "";
	var i=0;
	for (i = 0; i < pursuers; i++)
	{
		GetComponent(GUIText).text += "*";
	}
}