using System.IO;
using UnityEngine;

public class ScreenCaptureRGBADN : MonoBehaviour
{
    private Camera depthCamera;
    int captureWidth;
    int captureHeight;
    int captureNumber;
    [SerializeField] string fPath;
    [SerializeField] private Material defaultShader;
    [SerializeField] private Material normalShader;
    [SerializeField] private Material depthShader;
    Material currmaterial;
    RenderTexture rt;

    private void CaptureWithShader(Material mat, string fileName, int shadertype, bool savePng = false)
    {
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 36);
        //렌더 텍스쳐를 생성하여
        depthCamera.targetTexture = renderTexture;
        //현재 타겟 텍스쳐에 할당한다.
        depthCamera.depthTextureMode = DepthTextureMode.DepthNormals;
        //Depth와 Normal 값을 받아올 수 있도록 모드를 변경한다.
        currmaterial = mat;
        depthCamera.Render();
        //Render를 진행하여 아래있는 OnRenderImage를 작동시키고, 현재 타겟 텍스쳐인 Render Texture가 그 값을 받을 수 있도록 한다.

        depthCamera.targetTexture = null;

        RenderTexture.active = renderTexture;
        //활성화된 렌더 텍스쳐를 해당 값으로 변경한 후
        rt = renderTexture;

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        //텍스쳐의 데이터를 읽는다.

        Texture2D screenshot = texture;
        RenderTexture.active = null;

        //저장한 텍스쳐의 데이터를 파일 위치로 저장한다.
        byte[] bytes = screenshot.EncodeToPNG();
        string filePath = Path.Combine(fPath, $"{fileName}.png");
        if (savePng)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                }

                File.WriteAllBytes(filePath, bytes);
            }
        }

        // 원래 카메라 설정 복구

        // 메모리 정리
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(screenshot);

        Debug.Log($"Screenshot saved: {filePath}");
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //현재 소스를 전달받은 매터리얼으로 변환한다.
        Graphics.Blit(source, rt, currmaterial);
        //변환한 값을 그린다.
        Graphics.Blit(rt, destination);
    }

    void CaptureNormal(string fileName)
    {
        CaptureWithShader(normalShader, fileName + "_Normal", 1);
    }

    void CaptureDefault(string fileName)
    {
        CaptureWithShader(defaultShader, fileName + "_Default", 0, true);
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
    
    void Start()
    {
        depthCamera = GetComponent<Camera>();
        captureHeight = depthCamera.pixelHeight;
        captureWidth = depthCamera.pixelWidth;
        captureNumber = 0;
        currmaterial = defaultShader;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CaptureAll("Capture" + captureNumber);
            captureNumber++;
        }
    }
}