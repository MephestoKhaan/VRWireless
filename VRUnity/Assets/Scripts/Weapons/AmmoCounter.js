#pragma strict


public var grenadesInClip: int;
public var bulletsInClip : int;

var clipSize : int = 60;
var bulletsInBelt : int = 300;
private var isClipOn : boolean = true;


function CanFireBullet()
{
	return isClipOn && bulletsInClip > 0;
}

function CanFireGrenade()
{
	return grenadesInClip > 0;
}

function OnRemoveClip()
{
	isClipOn = false;
}

function OnReload()
{
	isClipOn = true;
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