using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour {
	
	public CharacterController player;
	
	void Update()
	{
		if (player.transform.position.y < -10){
			player.transform.position = transform.position;
		}
	}
}
