var target : GUITexture;

function Update () {
}

function OnCollisionEnter(collision : Collision) {
	if(collision.gameObject.CompareTag("Player")){
		Debug.Log("Dog collided");
		FlashWhenHit();
		//target.color.a = 100;
	}
}

function Fade (start : float, end : float, length : float, currentObject : GUITexture) { //define Fade parmeters
if (currentObject.guiTexture.color.a == start){

for (i = 0.0; i < 1.0; i += Time.deltaTime*(1/length)) { //for the length of time
target.color.a = Mathf.Lerp(start, end, i); //lerp the value of the transparency from the start value to the end value in equal increments
yield;
target.guiTexture.color.a = end; // ensure the fade is completely finished (because lerp doesn't always end on an exact value)
        } //end for

} //end if

} //end Fade

function FlashWhenHit (){
    Fade (0, 0.8, 0.5, target);
    yield WaitForSeconds (.001);
    Fade (0.8, 0, 0.5, target);
    }