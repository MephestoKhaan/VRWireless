#pragma strict

@script RequireComponent (PerFrameRaycast)

var ammo : AmmoCounter;

var bulletPrefab : GameObject;
var grenadePrefab : GameObject;
var spawnPoint : Transform;
var frequency : float = 10;
var coneAngle : float = 1.5;
var firing : boolean = false;
var damagePerSecond : float = 20.0;
var forcePerSecond : float = 20.0;
var hitSoundVolume : float = 0.5;

var muzzleFlashFront : GameObject;

private var lastFireTime : float = -1;
private var raycast : PerFrameRaycast;


function Awake ()
{
	muzzleFlashFront.SetActive (false);

	raycast = GetComponent.<PerFrameRaycast> ();
	if (spawnPoint == null)
		spawnPoint = transform;
}

var fireGrenade : boolean = false;
function Update () 
{
	if (firing
	&& ammo.CanFireBullet()
	&& Time.time > lastFireTime + 1 / frequency)
	{
	    ammo.UseBullet();
	    if(Network.isServer)
	    {
	        networkView.RPC("FireBullet",RPCMode.All,1);
	        networkView.RPC("FiringEffects",RPCMode.All,1);
	    }else if(!Network.isClient){
	        FireBullet(1);
	        FiringEffects(1);
	    }
	}
	else if(!firing || !ammo.CanFireBullet())
	{
		firing = false;
		if(Network.isServer)
		{
		    networkView.RPC("FiringEffects",RPCMode.All,0);
		}else if(!Network.isClient){
		    FiringEffects(0);
		}
	}
	
	if(fireGrenade)
	{
	    fireGrenade = false;
	    OnFireGrenade();
	}
}

@RPC
function FiringEffects(enable : int)
{
    var enableEffects : boolean = enable != 0;

    if(muzzleFlashFront.active == enableEffects)
    {
        return;
    }

    muzzleFlashFront.SetActive (enableEffects);

	if (audio)
	{
	    if(enableEffects && !audio.isPlaying)
	    {
	        Debug.Log("PLAYING AUDIO");
			audio.Play ();
		}
	    else if(!enableEffects && audio.isPlaying)
	    {
	        Debug.Log("STOPING AUDIO");
			audio.Stop();
		}
	}
}


function OnStartFire ()
{
	if (Time.timeScale == 0)
		return;

	firing = true;
}

function OnStopFire ()
{
	firing = false;
}

function OnFireGrenade()
{
    if(ammo.CanFireGrenade())
    {
        ammo.UseGrenade();
        if(Network.isServer)
        {
            networkView.RPC("FireGrenade",RPCMode.All,1);
        }else if(!Network.isClient){
            FireGrenade(1);
        }
    }
}

@RPC
function FireGrenade(arg : int)
{
    var coneRandomRotation = Quaternion.Euler (Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);
    Spawner.Spawn (grenadePrefab, spawnPoint.position, spawnPoint.rotation * coneRandomRotation);

}

@RPC
function FireBullet(arg : int)
{
	// Spawn visual bullet
	var coneRandomRotation = Quaternion.Euler (Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);
	var go : GameObject = Spawner.Spawn (bulletPrefab, spawnPoint.position, spawnPoint.rotation * coneRandomRotation) as GameObject;
	var bullet : SimpleBullet = go.GetComponent.<SimpleBullet> ();

	lastFireTime = Time.time;

	// Find the object hit by the raycast
	var hitInfo : RaycastHit = raycast.GetHitInfo ();
	if (hitInfo.transform)
	{
		// Get the health component of the target if any
		var targetHealth : Health = hitInfo.transform.GetComponent.<Health> ();
		if (targetHealth)
		{
			// Apply damage
			targetHealth.OnDamage (damagePerSecond / frequency, -spawnPoint.forward);
		}

		// Get the rigidbody if any
		if (hitInfo.rigidbody)
		{
			// Apply force to the target object at the position of the hit point
			var force : Vector3 = transform.forward * (forcePerSecond / frequency);
			hitInfo.rigidbody.AddForceAtPosition (force, hitInfo.point, ForceMode.Impulse);
		}

		// Ricochet sound
		var sound : AudioClip = MaterialImpactManager.GetBulletHitSound (hitInfo.collider.sharedMaterial);
		AudioSource.PlayClipAtPoint (sound, hitInfo.point, hitSoundVolume);

		bullet.dist = hitInfo.distance;
	}
	else
	{
		bullet.dist = 1000;
	}
}