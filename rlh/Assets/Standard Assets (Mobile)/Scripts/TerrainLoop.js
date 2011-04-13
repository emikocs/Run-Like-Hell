var xmax = 0.0; 
var xmin = 0.0; 
var zmax = 0.0; 
var zmin = 0.0; 
function Update () {
	var pos = transform.position;
	transform.position = Vector3((pos.x-xmin) % (xmax-xmin) + xmin, pos.y, (pos.z-zmin) % (zmax-zmin) + zmin);
	}