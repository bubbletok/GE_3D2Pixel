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
        //���� �ؽ��ĸ� �����Ͽ�
        depthCamera.targetTexture = renderTexture;
        //���� Ÿ�� �ؽ��Ŀ� �Ҵ��Ѵ�.
        depthCamera.depthTextureMode = DepthTextureMode.DepthNormals;
        //Depth�� Normal ���� �޾ƿ� �� �ֵ��� ��带 �����Ѵ�.
        currmaterial = mat;
        depthCamera.Render();
        //Render�� �����Ͽ� �Ʒ��ִ� OnRenderImage�� �۵���Ű��, ���� Ÿ�� �ؽ����� Render Texture�� �� ���� ���� �� �ֵ��� �Ѵ�.

        depthCamera.targetTexture = null;

        RenderTexture.active = renderTexture;
        //Ȱ��ȭ�� ���� �ؽ��ĸ� �ش� ������ ������ ��
        rt = renderTexture;

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        //�ؽ����� �����͸� �д´�.

        Texture2D screenshot = texture;
        RenderTexture.active = null;

        //������ �ؽ����� �����͸� ���� ��ġ�� �����Ѵ�.
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

        // ���� ī�޶� ���� ����

        // �޸� ����
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(screenshot);

        Debug.Log($"Screenshot saved: {filePath}");
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //���� �ҽ��� ���޹��� ���͸������� ��ȯ�Ѵ�.
        Graphics.Blit(source, rt, currmaterial);
        //��ȯ�� ���� �׸���.
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