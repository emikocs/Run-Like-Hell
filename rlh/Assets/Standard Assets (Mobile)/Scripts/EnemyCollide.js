var target : GUITexture;
var colliding : System.Boolean;
var pursuing : System.Boolean;
var deathtext : GUIText;
var chasetext : GhostTextScript;
//object to be followed
var detectObject: Transform;
//distance that will trigger following action
var distanceDetection: float;

function Start()
{
    colliding = false;
    pursuing = false;
}

function Update () {
	if (detectObject) {
		var dist = Vector3.Distance(detectObject.position, transform.position);
		
		//if distance is less than what is specified then do something
		if(dist<distanceDetection && pursuing == false){
			//print("attack");
			GetComponent(SmoothLookAt).enabled = true;
			GetComponent(ConstantForce).enabled = true;
			audio.Play();
			chasetext.pursuers ++;
			chasetext.PursuerTally();
			//chasetext.text += "*";
			pursuing = true;
		}else if (dist >= distanceDetection) {
			//print("stop attack");
			GetComponent(SmoothLookAt).enabled = false;
			GetComponent(ConstantForce).enabled = false;
			//chasetext.text.Replace("*", "");
			audio.Stop();
			chasetext.pursuers --;
			chasetext.PursuerTally();
			pursuing = false;
        }
	}
}

function OnCollisionEnter(collision : Collision) {
	if(collision.gameObject.CompareTag("Player") && colliding == false){
        colliding = true;
		Debug.Log("Dog collided");
	    deathtext.text +="b";
		FlashWhenHit();
		//target.color.a = 100;
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