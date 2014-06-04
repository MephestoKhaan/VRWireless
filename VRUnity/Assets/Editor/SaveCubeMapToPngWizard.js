import System.IO;

class SaveCubeMapToPngWizard extends ScriptableWizard {

var cubemap : Cubemap;

function OnWizardUpdate () {
helpString = "Select cubemap to save to individual png";
isValid = (cubemap != null);
}

function OnWizardCreate ()
{
    var width = cubemap.width;
    var height = cubemap.height;

    Debug.Log(Application.dataPath + "/" +cubemap.name +"_PositiveX.png");
    var tex = new Texture2D (width, height, TextureFormat.RGB24, false);
    // Read screen contents into the texture        
    tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveX));        
    // Encode texture into PNG
    var bytes = tex.EncodeToPNG();      
     File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_PositiveX.png", bytes);       

     tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeX));
     bytes = tex.EncodeToPNG();     
     File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_NegativeX.png", bytes);       

     tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveY));
     bytes = tex.EncodeToPNG();     
     File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_PositiveY.png", bytes);       

     tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeY));
     bytes = tex.EncodeToPNG();     
     File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_NegativeY.png", bytes);       

     tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveZ));
     bytes = tex.EncodeToPNG();     
     File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_PositiveZ.png", bytes);       

     tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeZ));
     bytes = tex.EncodeToPNG();     
     File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_NegativeZ.png", bytes);       

     DestroyImmediate(tex);
}

@MenuItem("GameObject/Save CubeMap To Png ")
static function SaveCubeMapToPng  ()
{
    ScriptableWizard.DisplayWizard(
    "Save CubeMap To Png", SaveCubeMapToPngWizard , "Save");
}
}