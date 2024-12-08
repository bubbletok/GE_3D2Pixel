using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DepthNormalMapExtractor : MonoBehaviour
{
    private Camera depthCamera;
    private RenderTexture depthTexture;
    private RenderTexture grayScaleTexture;
    [SerializeField] private RawImage depthImage;
    [SerializeField] private Material convertMaterial; // Inspector에서 할당
    private int timestamp = 0;

    void Start()
    {
        depthCamera = GetComponent<Camera>();

        // depth texture 생성
        depthTexture = new RenderTexture(Screen.width, Screen.height, 36);
        depthTexture.format = RenderTextureFormat.Depth;

        // grayscale texture 생성
        grayScaleTexture = new RenderTexture(Screen.width, Screen.height, 0);
        grayScaleTexture.format = RenderTextureFormat.ARGBFloat;

        // 카메라 설정
        depthCamera.depthTextureMode = DepthTextureMode.DepthNormals;
        depthCamera.targetTexture = grayScaleTexture;

        if (convertMaterial == null)
        {
            convertMaterial = new Material(Shader.Find("Custom/DepthNormalToGray"));
        }

        // RawImage에는 변환된 grayscale 텍스처를 표시
        depthImage.texture = grayScaleTexture;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveDepthMapPNG();
        }
    }
    

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // depth normal을 grayscale로 변환
        Graphics.Blit(source, grayScaleTexture, convertMaterial);
        Graphics.Blit(grayScaleTexture, destination);   
    }
    
    public void SaveDepthMapPNG()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = grayScaleTexture;

        Texture2D depthMap = new Texture2D(grayScaleTexture.width, grayScaleTexture.height, TextureFormat.RGBAFloat, false);
        depthMap.ReadPixels(new Rect(0, 0, grayScaleTexture.width, grayScaleTexture.height), 0, 0);
        depthMap.Apply();

        RenderTexture.active = currentRT;

        var dirPath = Application.dataPath + "/SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        byte[] bytes = depthMap.EncodeToPNG();
        File.WriteAllBytes(dirPath + $"depth_{timestamp}.png", bytes);
        Debug.Log($"Depth map saved to: {dirPath}depth_{timestamp}.png");
        timestamp++;

        Destroy(depthMap);
    }

    public void SaveColorMapPNG()
    {
        
    }


    public void SaveDepthMapEXR()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = grayScaleTexture;

        Texture2D depthMap = new Texture2D(grayScaleTexture.width, grayScaleTexture.height, TextureFormat.RFloat, false);
        depthMap.ReadPixels(new Rect(0, 0, grayScaleTexture.width, grayScaleTexture.height), 0, 0);
        depthMap.Apply();

        RenderTexture.active = currentRT;

        var dirPath = Application.dataPath + "/SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        byte[] bytes = depthMap.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
        File.WriteAllBytes(dirPath + $"depth_{timestamp}.exr", bytes);
        Debug.Log($"Depth map saved to: {dirPath}depth_{timestamp}.exr");
        timestamp++;

        Destroy(depthMap);
    }

    void OnDestroy()
    {
        if (depthTexture != null)
            depthTexture.Release();
        if (grayScaleTexture != null)
            grayScaleTexture.Release();
    }
}