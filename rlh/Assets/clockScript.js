var isPaused : boolean = false;
var startTime : float; //(in seconds)
var timeRemaining : float; //(in seconds)
function Start()
{
}

function Update() {
  if (!isPaused)
  {
    //startTime = 5.0;
    // make sure the timer is not paused
    DoCountdown();
   }
}



function DoCountdown() {
    timeRemaining = startTime - Time.time;
    if (timeRemaining < 0.0)
    {
        timeRemaining = 0.0;
        isPaused = true;
        TimeIsUp();
    }

}

function OnGUI(){

    if (Camera.main.enabled == true)
    {
        ShowTime();
    }

}

function PauseClock()
{
   isPaused = true;
}

function UnpauseClock()
{
   isPaused = false;
}

function ShowTime()
{
    var minutes : int;
    var seconds : int;
    var timeStr : String;
    minutes = timeRemaining/60;
    seconds = timeRemaining % 60;
    timeStr = minutes.ToString() + ":";
    timeStr += seconds.ToString("D2");
    guiText.text = timeStr;
}

function TimeIsUp()
{
    Debug.Log("Time is up!");
    //Application.LoadLevel (0);
}