#pragma strict

var grenadesInClip: int;
var bulletsInClip : int;

var clipSize : int = 60;
var bulletsInBelt : int = 300;



function OnReload()
{
	var bulletsRequested : int = clipSize - bulletsInClip;
	
	var bulletsReloaded : int = Mathf.Min(bulletsInBelt, bulletsRequested);
	
	bulletsInBelt -= bulletsReloaded;
	bulletsInClip += bulletsReloaded;
}


function UseBullet()
{
	bulletsInClip = Mathf.Max(0, bulletsInClip -1);
}
function UseGrenade()
{
	grenadesInClip = Mathf.Max(0, grenadesInClip -1);
}


var reload : boolean = false;
function Update()
{
	if(reload)
	{
		reload = false;
		OnReload();
	}
}