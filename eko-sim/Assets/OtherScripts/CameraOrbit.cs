using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    Transform camera;
    Transform center;

    Vector3 cameraRotation;
    Vector3 localCameraRotation;
    float cameraDistance = 1200f;

    public float mouseSens = 4f;
    public float scrollSens = 2f;
    public float mouseDampening = 10f;
    public float scrollDampening = 6f;

    public bool controlsDisabled = false;
        

    public void Start()
    {
        this.camera = this.transform;
        this.center = this.transform.parent;
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            controlsDisabled = false;

        }else controlsDisabled = true;

        if (!controlsDisabled)
        {
            //camera movement
            if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                cameraRotation.x += Input.GetAxis("Mouse X") * mouseSens;
                cameraRotation.y -= Input.GetAxis("Mouse Y") * mouseSens;

                cameraRotation.y = Mathf.Clamp(cameraRotation.y, -90f, 90f);
            }
        }
        //camera zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSens;

            scrollAmount *= (this.cameraDistance * 0.2f); //if closer don't scroll as much
            this.cameraDistance += scrollAmount * -1f;
            this.cameraDistance = Mathf.Clamp(this.cameraDistance, 800f, 1500f);
        }

        //camera rotation
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            localCameraRotation.y += Input.GetAxis("Horizontal") * mouseSens;
            localCameraRotation.x -= Input.GetAxis("Vertical") * mouseSens;
            

            localCameraRotation.x = Mathf.Clamp(localCameraRotation.x, -10f, 10f);
            localCameraRotation.y = Mathf.Clamp(localCameraRotation.y, -10f, 10f);
        }
        //camera rotation reset
        if (Input.GetKeyDown(KeyCode.Q))
        {
            localCameraRotation = Vector3.zero;
            Quaternion resetQT = Quaternion.Euler(0, 0, 0);
            this.camera.localRotation = Quaternion.Lerp(this.camera.localRotation, resetQT, Time.deltaTime * mouseDampening);

        }

        //camera movement
        Quaternion moveQT = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);
        this.center.rotation = Quaternion.Lerp(this.center.rotation, moveQT, Time.deltaTime * mouseDampening);

        //camera rotation
        Quaternion rotQT = Quaternion.Euler(localCameraRotation.x, localCameraRotation.y, 0);
        this.camera.localRotation = Quaternion.Lerp(this.camera.localRotation, rotQT, Time.deltaTime * mouseDampening);

        //camera zoom
        this.camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this.camera.localPosition.z, this.cameraDistance * -1f, scrollDampening));

    }
}
