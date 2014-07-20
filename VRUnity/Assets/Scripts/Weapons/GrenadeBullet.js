#pragma strict

public var speed : float = 100.0;
public var lifeTime : float = 1.5;
public var damageAmount : float = 5;
public var forceAmount : float = 5;
public var radius : float = 1.0;
public var ignoreLayers : LayerMask;
public var noise : float = 0.0;
public var explosionPrefab : GameObject;

private var dir : Vector3;
private var spawnTime : float;
private var tr : Transform;

function OnEnable () {
	tr = transform;
	dir = transform.forward;
	spawnTime = Time.time;
	
	this.rigidbody.AddForce(tr.forward * speed, ForceMode.Impulse);
	
}

function Update ()
{
	if (Time.time > spawnTime + lifeTime)
	{
		Spawner.Destroy (gameObject);
	}
	
	tr.rotation = this.rigidbody.rotation;// Quaternion.LookRotation(dir); 	
	
	// Check if this one hits something
	var hits : Collider[] = Physics.OverlapSphere (tr.position, radius, ~ignoreLayers.value);
	var collided : boolean = false;
	
	for (var c : Collider in hits)
	{
		// Don't collide with triggers
		if (c.isTrigger)
			continue;
		
		var targetHealth : Health = c.GetComponent.<Health> ();
		if (targetHealth) {
			// Apply damage
			targetHealth.OnDamage (damageAmount, -tr.forward);
		}
		// Get the rigidbody if any
		if (c.rigidbody) {
			// Apply force to the target object
			var force : Vector3 = tr.forward * forceAmount;
			force.y = 0;
			c.rigidbody.AddForce (force, ForceMode.Impulse);
		}
		collided = true;
	}
	
	if (collided)
	{
		Spawner.Destroy (gameObject);
		Spawner.Spawn (explosionPrefab, transform.position, transform.rotation);
	}
}