using UnityEngine;
using System.Collections;

public class StaticBatch : MonoBehaviour {

	void Start()
	{
		StaticBatchingUtility.Combine(this.gameObject);
	}
}
