using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenCaptureRGBADN : MonoBehaviour
{
    private Camera depthCamera;
    int captureWidth;
    int captureHeight;
    int captureNumber;
    [SerializeField] private Material defaultShader;
    [SerializeField] private Material normalShader;
    [SerializeField] private Material depthShader;
    Material currmaterial;
    RenderTexture rt;

    private void CaptureWithShader(Material mat, string fileName,int shadertype)
    {
        //// ���� �ؽ�ó ����
        //RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 36);

        //// ī�޶��� Ÿ�� �ؽ�ó�� �ӽ÷� ����
        //RenderTexture previousRT = depthCamera.targetTexture;
        //depthCamera.targetTexture = renderTexture;

        //// ���ο� �ؽ�ó ����
        //Texture2D screenshot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBAFloat, false);

        ////var previousRenderFlags = depthCamera.clearFlags;
        //if(shadertype != 0)
        //{
        //    depthCamera.depthTextureMode = DepthTextureMode.DepthNormals;
        //    depthCamera.SetReplacementShader(shader, "");
        //}
        //else
        //{
        //    depthCamera.depthTextureMode = DepthTextureMode.None;
        //    depthCamera.ResetReplacementShader();
        //}


        //// ���� �ؽ�ó�� Ȱ��ȭ�ϰ� �ȼ� �б�
        //RenderTexture.active = renderTexture;
        //screenshot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        //screenshot.Apply();

        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 36);
        depthCamera.targetTexture = renderTexture;
        depthCamera.depthTextureMode = DepthTextureMode.DepthNormals;
        currmaterial = mat;
        depthCamera.Render();

        depthCamera.targetTexture = null;

        RenderTexture.active = renderTexture;
        rt = renderTexture;

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        Texture2D screenshot = texture;
        RenderTexture.active = null;

        // PNG�� ����
        byte[] bytes = screenshot.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, $"../Screenshots/{fileName}.png");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllBytes(filePath, bytes);

        // ���� ī�޶� ���� ����

        // �޸� ����
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(screenshot);

        Debug.Log($"Screenshot saved: {filePath}");
    }

    void CaptureNormal(string fileName)
    {
        CaptureWithShader(normalShader, fileName + "_Normal", 1);
    }

    void CaptureDefault(string fileName)
    {
        CaptureWithShader(defaultShader, fileName + "_Default",0);
    }
    void CaptureDepth(string fileName)
    {
        CaptureWithShader(depthShader, fileName + "_Depth", 1);
    }

    void CaptureAll(string fileName)
    {
        CaptureNormal(fileName);
        CaptureDefault(fileName);
        CaptureDepth(fileName);
        Debug.Log("Capture Created");
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, rt, currmaterial);
        Graphics.Blit(rt, destination);
    }


    // Start is called before the first frame update
    void Start()
     {
        depthCamera = GetComponent<Camera>();
        captureHeight = depthCamera.pixelHeight;
        captureWidth = depthCamera.pixelWidth;
        captureNumber = 0;
        currmaterial = defaultShader;
     }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            CaptureAll("Capture" + captureNumber);
            //CaptureDefault("Capture" + captureNumber);
            captureNumber++;
        }
    }
}
