using UnityEngine;

public partial class Player
{
    const float cameraSensitivity = 0.3f;
 
    [Header("Camera")]
    public Camera mainCamera;
    
    Vector2 cameraRotation = new Vector2(0, 0);
    
    void handleCameraRotation()
    {
        var mouseMovement = Input.mousePositionDelta * Time.timeScale;
        cameraRotation.x -= mouseMovement.y * cameraSensitivity;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);
        cameraRotation.y += mouseMovement.x * cameraSensitivity;
        
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotation);

        var rotation = transform.rotation.eulerAngles;
        rotation.y = cameraRotation.y;
        transform.rotation = Quaternion.Euler(rotation);
    }

    void updateCameraPosition()
    {
        mainCamera.transform.position = transform.position - mainCamera.transform.forward * 4.0f;
    }
}
