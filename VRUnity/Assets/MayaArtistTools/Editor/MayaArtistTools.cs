using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * Class Emulates some handy tools from maya
 * v0.2
 * */
public class MayaArtistTools : EditorWindow
{
	const string highResScreenShotMode = "highresscreenshot";
	
	static List<Renderer> lastHiddenSelection = new List<Renderer>();
	static List<Renderer> selectedRenderers = new List<Renderer>();
	
	//Hotkeys: % ctrl & alt # shift
	
	//MAYA ARTIST TOOLS WINDOW

	int resWidth = 2048; 
    int resHeight = 1536;
	bool includeAChannel = false;
	bool useAdvancedAlpha = true;
	Camera screenshotCamera;
	string windowMode;
	
	//string alphaInformation = "";
	
	void OnGUI()
	{
		if (windowMode == highResScreenShotMode)
		{
			if (!SystemInfo.supportsRenderTextures) EditorGUILayout.HelpBox("Require Unity Pro", MessageType.Warning);
			if (screenshotCamera == null) screenshotCamera = FindObjectOfType(typeof(Camera)) as Camera;
			GUILayout.Label ("High Res Screenshot", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			screenshotCamera = (Camera)EditorGUILayout.ObjectField("Camera", screenshotCamera, typeof(Camera), true);
			if (GUILayout.Button("Scene Camera"))
			{
				if (SceneView.GetAllSceneCameras().Length>0){screenshotCamera = SceneView.GetAllSceneCameras()[0];}
				else{Debug.Log("Something went wrong, select the Scene window and try again");}
			}
			EditorGUILayout.EndHorizontal();
			resWidth = EditorGUILayout.IntField ("Resolution Width", resWidth);
			resHeight = EditorGUILayout.IntField ("Resolution Height", resHeight);
			if (resWidth*resHeight>3000*3000) EditorGUILayout.HelpBox("Screenshot Uses RenderToTexture, very high resolutions may cause crashes", MessageType.Warning);
			includeAChannel = EditorGUILayout.Toggle("Include (A) Channel", includeAChannel);
			if (includeAChannel)
			{
				useAdvancedAlpha = EditorGUILayout.Toggle("Use Advanced Alpha", useAdvancedAlpha);
				if (useAdvancedAlpha)
				{
					EditorGUILayout.HelpBox("Advanced Alpha uses replacement shaders to get more accurate results, despite efforts in some cases additional work may required to correct image", MessageType.Warning);
				}
			}
			if (GUILayout.Button("Take Screenshot")) TakeScreenshot(screenshotCamera, resWidth, resHeight, includeAChannel);
			
			
		}
	}
	
	void TakeScreenshot(Camera cam, int width, int height, bool withAChannel)
	{
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24); 
        cam.targetTexture = rt; 
        Texture2D screenShot = new Texture2D(resWidth, resHeight, (withAChannel ? TextureFormat.ARGB32 : TextureFormat.RGB24), false); 
        cam.Render(); 
        RenderTexture.active = rt; 
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0); 
        cam.targetTexture = null; 
        RenderTexture.active = null; // JC: added to avoid errors 
        DestroyImmediate(rt); 
		
		if (useAdvancedAlpha)
		{
			Debug.LogWarning("MayaArtistTools: Some unity built in transparent shaders so not write to alpha (for performance reasons) and will not create a correct alpha channel");
			
			Texture2D alphaRender = AlphaRender(cam);
			
			Color[] pixels = screenShot.GetPixels();
			Color[] alphaPixels = alphaRender.GetPixels();
			
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].a = Mathf.Max(pixels[i].a, alphaPixels[i].a);
			}
			
			screenShot.SetPixels(pixels);
				
		}
		
        byte[] bytes = screenShot.EncodeToPNG(); 
        string filename = Application.dataPath.Replace("Assets", "") + "screen" 
                        + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png"; 
        System.IO.File.WriteAllBytes(filename, bytes); 
        Debug.Log(string.Format("Took screenshot to: {0}", filename)); 	
	}
	
	Texture2D AlphaRender(Camera cam)
	{
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24); 
        cam.targetTexture = rt; 
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false); 
        cam.RenderWithShader(Shader.Find("MayaArtistTools/Internal_AlphaRender"), "RenderType");
        RenderTexture.active = rt; 
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		cam.targetTexture = null; 
        RenderTexture.active = null; // JC: added to avoid errors 
        DestroyImmediate(rt); 
		
		return screenShot;
	}
	
	
	[MenuItem("Maya Artist Tools/Toggle Fog")]
	static void ToggleFog()
	{
		RenderSettings.fog = !RenderSettings.fog;
	}
	
	
	[MenuItem("Maya Artist Tools/High Res Screenshot")]
	static void InitScreenshotWindow()
	{
		MayaArtistTools window = (MayaArtistTools)EditorWindow.GetWindow(typeof(MayaArtistTools));
		window.windowMode = highResScreenShotMode;
	}
	
	
	[MenuItem("Maya Artist Tools/Import Selected Scene")]
	static void LoadSceneAdditive()
	{ 
	    string strScenePath = AssetDatabase.GetAssetPath(Selection.activeObject); 
	    if (strScenePath == null || !strScenePath.Contains(".unity"))  {
	        EditorUtility.DisplayDialog("Select Scene", "You Must Select a Scene!", "Ok");
	        EditorApplication.Beep(); 
	        return; 
	    }
		Undo.RegisterSceneUndo("ImportedScene");
	    Debug.Log("Imported " + strScenePath + " Scene"); 
	    EditorApplication.OpenSceneAdditive(strScenePath); 
	}
	
	[MenuItem("Maya Artist Tools/Create Child %&n")]
	static void CreateChild()
	{
		Undo.RegisterSceneUndo("ChildCreated");
		if (Selection.activeTransform!=null)
		{
			GameObject child = new GameObject("Child");
			Transform ct = child.transform;
			ct.parent = Selection.activeTransform;
			ct.localPosition = Vector3.zero;
			ct.localRotation = Quaternion.identity;
			ct.localScale = Vector3.one;
			child.layer = Selection.activeTransform.gameObject.layer;
			Selection.activeGameObject = child;
		}
		else
		{
			GameObject child = new GameObject("GameObject");
			Transform ct = child.transform;
			ct.localPosition = Vector3.zero;
			ct.localRotation = Quaternion.identity;
			ct.localScale = Vector3.one;
			Selection.activeGameObject = child;
		}
	}
	
	[MenuItem("Maya Artist Tools/Move Pivot to Center of Children")]
	static void CenterPivot()
	{
		if (Selection.activeTransform!=null)
		{
			Transform sel = Selection.activeTransform;
			
			Undo.RegisterSceneUndo("CenterToChildren");
			
			if (sel.renderer==null || EditorUtility.DisplayDialog("Warning", "This gameobject has a renderer component, when this objects pivot moves the attached mesh will move also", "OK"))
			{
			
			if (sel.childCount == 0)
			{
				EditorUtility.DisplayDialog("Selected transform has no children", "This transform has no children to determine a center from", "Cancel");
			}
			else if (sel.GetComponentsInChildren<Renderer>().Length == 0)
			{
				if (EditorUtility.DisplayDialog("Selected transform has no renderers", "This transform has no renderers to determine a center from, the center will be determined by the childrens positions", "OK"))
				{
					Transform[] children = sel.GetComponentsInChildren<Transform>();
					
					Vector3 minPos = sel.GetChild(0).position;
					Vector3 maxPos = sel.GetChild(0).position;
					
					foreach (Transform child in children)
					{
						if (child != sel.transform)
							{
							minPos = new Vector3(Mathf.Min(child.position.x, minPos.x), Mathf.Min(child.position.y, minPos.y), Mathf.Min(child.position.z, minPos.z));
							maxPos = new Vector3(Mathf.Max(child.position.x, maxPos.x), Mathf.Max(child.position.y, maxPos.y), Mathf.Max(child.position.z, maxPos.z));
							}
						if (child.parent == sel)
							child.parent = null;

					}

					sel.position = (minPos + maxPos) * 0.5f;
					
					foreach (Transform child in children)
					{
						if (child.parent == null)
						{
							child.parent = sel;
						}
					}
				}
			}
			else
			{
				Renderer[] children = sel.GetComponentsInChildren<Renderer>();
				
				Vector3 minPos = children[0].bounds.center;
				Vector3 maxPos = children[0].bounds.center;
					
				foreach (Renderer child in children)
				{
					minPos = new Vector3(Mathf.Min(child.bounds.center.x, minPos.x), Mathf.Min(child.bounds.center.y, minPos.y), Mathf.Min(child.bounds.center.z, minPos.z));
					maxPos = new Vector3(Mathf.Max(child.bounds.center.x, maxPos.x), Mathf.Max(child.bounds.center.y, maxPos.y), Mathf.Max(child.bounds.center.z, maxPos.z));
				}
				
				Transform[] directChildren = new Transform[sel.childCount];
				for (int i = 0; i < sel.childCount; i++)
				{
					directChildren[i] = sel.GetChild(i);
				}
				
				foreach (Transform child in directChildren)
				{
					child.parent = null;
				}
				
				sel.position = (minPos + maxPos) * 0.5f;
				
				foreach (Transform child in directChildren)
				{
					child.parent = sel;
				}
			}
				
			}
		}
	}
	
	//[MenuItem("Maya Artist Tools/Save Texture as PNG (flattening PSD etc)")]
	static void  SaveTextureToPNG ()
	{
		Texture2D tex= Selection.activeObject as Texture2D;
		
		if (tex == null)
		{
			EditorUtility.DisplayDialog("No texture selected", "Please select a texture", "Cancel");
			return;
		}
		
		// Convert Alpha8 texture to ARGB32 texture so it can be saved as a PNG
		Color[] texPixels= tex.GetPixels();
		Texture2D tex2= new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
		tex2.SetPixels(texPixels);
		
		// Save texture (WriteAllBytes is not used here in order to keep compatibility with Unity iPhone)
		byte[] texBytes= tex2.EncodeToPNG();
		string fileName= EditorUtility.SaveFilePanel("Save font texture", "", tex.name, "png");
		if (fileName.Length > 0)
		{
			FileStream f = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryWriter b = new BinaryWriter(f);
			for (int i= 0; i < texBytes.Length; i++) b.Write(texBytes[i]);
			b.Close(); 
		}
		
		DestroyImmediate(tex2);
	}
	
	[MenuItem("Maya Artist Tools/Change Render Mode/Wireframe &5")]
	static void ChangeRenderModeWire()
	{
#if UNITY_3_5
		SceneView.currentDrawingSceneView.renderMode = SceneView.RenderMode.Wireframe;
#endif
		
#if UNITY_4_0
		SceneView.currentDrawingSceneView.renderMode = DrawCameraMode.Wireframe;
#endif
	}
	
	[MenuItem("Maya Artist Tools/Change Render Mode/Texture+Wireframe &6")]
	static void ChangeRenderModeTexWire()
	{
#if UNITY_3_5
		SceneView.currentDrawingSceneView.renderMode = SceneView.RenderMode.TexWire;
#endif
		
#if UNITY_4_0
		SceneView.currentDrawingSceneView.renderMode = DrawCameraMode.TexturedWire;
#endif
	}
	
	[MenuItem("Maya Artist Tools/Change Render Mode/Normal &7")]
	static void ChangeRenderModeNormal()
	{
#if UNITY_3_5
		SceneView.currentDrawingSceneView.renderMode = SceneView.RenderMode.Textured;
#endif
		
#if UNITY_4_0
		SceneView.currentDrawingSceneView.renderMode = DrawCameraMode.Textured;
#endif
	}
	
	[MenuItem ("Maya Artist Tools/Save Project %&s")]
	static void SaveProject()
	{
		EditorApplication.SaveAssets();
	}
	
	/**
	 * Take the renderers of the currently selected objects and disable them. Add them to the last hidden list
	 * */
	[MenuItem ("Maya Artist Tools/Hide/Hide Selection %h")]
	static void HideSelection()
	{
		selectedRenderers.Clear();
		
		foreach (GameObject go in Selection.gameObjects)
		{
			foreach (Renderer rend in go.GetComponentsInChildren<Renderer>())
			{
				selectedRenderers.Add(rend);
			}
		}
		
		if (selectedRenderers.Count > 0)
		{
			Undo.RegisterSceneUndo("HideSelection");
			
			lastHiddenSelection.Clear();
			
			foreach (Renderer rend in selectedRenderers)
			{
				if (rend.enabled)
				{
					rend.enabled = false;
					lastHiddenSelection.Add(rend);
				}
			}
		}
	}
	
	/**
	 * Take the renderers of the currently selected objects and enable them. clear the last hidden list
	 * */
	[MenuItem ("Maya Artist Tools/Show/Unhide Selection %&h")]
	static void UnhideSelection()
	{
		selectedRenderers.Clear();
		
		foreach (GameObject go in Selection.gameObjects)
		{
			foreach (Renderer rend in go.GetComponentsInChildren<Renderer>())
			{
				selectedRenderers.Add(rend);
			}
		}
		
		if (selectedRenderers.Count > 0)
		{
			Undo.RegisterSceneUndo("UnhideSelection");
			
			foreach (Renderer rend in selectedRenderers)
			{
				if (rend.enabled == false)
				{
					rend.enabled = true;
				}
			}
		}
	}
	
	/**
	 * Take the renderers of the last hidden renderers and enable them. Clear the last hidden list
	 * */
	[MenuItem ("Maya Artist Tools/Show/Show Last Hidden %#h")]
	static void ShowLastHidden()
	{
		if (lastHiddenSelection.Count>0)
		{
			Undo.RegisterSceneUndo("ShowLastHidden");
			
			foreach (Renderer rend in lastHiddenSelection)
			{
				rend.enabled = true;
			}
			
			lastHiddenSelection.Clear();
		}
	}
	
	/**
	 * Take the renderers of the currently unselected objects and disable them. Add them to the last hidden list
	 * */
	[MenuItem ("Maya Artist Tools/Hide/Hide Unselected Objects &h")]
	static void HideUnselectedObjects()
	{
		selectedRenderers.Clear();
		
		foreach (GameObject go in Selection.gameObjects)
		{
			foreach (Renderer rend in go.GetComponentsInChildren<Renderer>())
			{
				selectedRenderers.Add(rend);
			}
		}
		
		if (selectedRenderers.Count>0)
		{
			Undo.RegisterSceneUndo("HideUnselectedObjects");
			
			Renderer[] renderersInScene = FindObjectsOfType(typeof(Renderer)) as Renderer[];
			
			foreach (Renderer rend in renderersInScene)
			{
				if (!selectedRenderers.Contains(rend) && rend.enabled)
				{
					rend.enabled = false;
					lastHiddenSelection.Add(rend);
				}
			}
			
		}
	}
	
	
}
