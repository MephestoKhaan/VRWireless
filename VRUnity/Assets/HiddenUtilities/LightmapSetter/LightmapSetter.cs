using UnityEngine;
using System.Collections;

public class LightmapSetter : MonoBehaviour {
	
	public Texture2D[] lightmaps;
	public LightProbes lightProbes;
	public bool setLightmapSettingsOnStart = false;
	public bool setOriginalLightmapsOnDestroy = false;
	public LightmapData[] rd;
	
	static LightmapSetter previousLightmapSettings;
	
	void Start()
	{
		if (setLightmapSettingsOnStart)
		{
			SetLightmapSettings();
		}
	}
	
	void OnDestroy()
	{
		if (setOriginalLightmapsOnDestroy && previousLightmapSettings!=null)
		{
			previousLightmapSettings.SetLightmapSettings();
			previousLightmapSettings = null;
		}
	}
	
	void SetLightmapSettings()
	{
		for (int i = 0; i<LightmapSettings.lightmaps.Length; i++)
		{
			if (LightmapSettings.lightmaps[i].lightmapFar != null)
			{
				Resources.UnloadAsset(LightmapSettings.lightmaps[i].lightmapFar);
			}
			
			if (LightmapSettings.lightmaps[i].lightmapNear != null)
			{
				Resources.UnloadAsset(LightmapSettings.lightmaps[i].lightmapNear);
			}
		}
		
		rd = new LightmapData[lightmaps.Length];
		
		for (int i = 0; i<lightmaps.Length; i++)
		{
			rd[i] = new LightmapData();
			rd[i].lightmapFar = lightmaps[i];
		}
		
		LightmapSettings.lightmaps = rd;
		LightmapSettings.lightProbes = lightProbes;
		
		if (!setOriginalLightmapsOnDestroy) previousLightmapSettings = this;
	}
	
	[System.SerializableAttribute]
	public class LightmapDataWrapper
	{
		public Texture2D lightmapFar;
		public Texture2D lightmapNear;
	}
}
