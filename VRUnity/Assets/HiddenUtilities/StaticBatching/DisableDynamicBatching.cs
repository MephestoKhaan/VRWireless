using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
  * Class Stops dynamic batching of objects with the same material by duplicating the material of each child so they cannot be batched
 **/
public class DisableDynamicBatching : MonoBehaviour {
	
	//List<Material> duplicatedMaterials = new List<Material>(); //!< List of materials thats duplicated
	
	/**
	 * Add extra vertices to each mesh to avoid the batching
	 * */
	void Awake()
	{
		foreach (MeshFilter m in GetComponentsInChildren<MeshFilter>())
		{
			List<Vector3> vs = new List<Vector3>(m.mesh.vertices);
			while(vs.Count< 301)
				vs.Add(Vector3.zero);
			
			m.mesh.vertices = vs.ToArray();
				
		}
	}
	


}
