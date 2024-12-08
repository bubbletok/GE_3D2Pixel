using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 2f;
    
    private void Update()
    {
        // WASD로 이동
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        transform.position += move * (moveSpeed * Time.deltaTime);
        
        // 마우스로 회전
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            transform.eulerAngles += new Vector3(-mouseY * rotateSpeed, mouseX * rotateSpeed, 0);
        }
        
        if (Input.GetKey(KeyCode.Q))
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
    }
}