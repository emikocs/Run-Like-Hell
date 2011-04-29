var target : GUITexture;
var colliding : System.Boolean;
var text : GUIText;
var spawn1 : Transform;
var spawn2 : Transform;
var spawn3 : Transform;

function Start()
{
    colliding = false;
}

function Update () {
}

function OnCollisionEnter(collision : Collision) {
	if(collision.gameObject.CompareTag("Player") && colliding == false){
        colliding = true;
		Debug.Log("Made it to safe house");
	    text.text +="~";
		//FlashWhenHit();
		//target.color.a = 100;
		 var randomPick : int = Mathf.Abs(Random.Range(1,3));
 
         //create a location 'Transform' type variable to store one of 3 possible locations declared at top of script
         var location : Transform;
 
         //check what randomPick is, and select one of the 3 locations, based on that number
        if(randomPick == 1){
        location = spawn1;
        Debug.Log("Chose pos 1");
        }
        else if(randomPick == 2){
         location = spawn2;
         Debug.Log("Chose pos 2");
        }
        else if(randomPick == 3){
         location = spawn3;
         Debug.Log("Chose pos 3");
        }
 
        GameObject.FindWithTag("Player").transform.position = location.position;
	}
}

function Fade (start : float, end : float, length : float, currentObject : GUITexture) { //define Fade parmeters
if (currentObject.guiTexture.color.a == start){

for (i = 0.0; i < 1.0; i += Time.deltaTime*(1/length)) { //for the length of time
target.color.a = Mathf.Lerp(start, end, i); //lerp the value of the transparency from the start value to the end value in equal increments
yield WaitForSeconds (0.5);
target.guiTexture.color.a = end; // ensure the fade is completely finished (because lerp doesn't always end on an exact value)
        } //end for

} //end if

} //end Fade

function FlashWhenHit (){
    Fade (0, 0.8, 0.5, target);
    //yield WaitForSeconds (.001);
    yield WaitForSeconds (.001);
    target.guiTexture.color.a = 0;
//    Fade (0.8, 0, 0.5, target);
    
    //yield WaitForSeconds (3);
    colliding = false;
    }