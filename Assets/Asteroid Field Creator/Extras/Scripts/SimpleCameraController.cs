using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [SerializeField]
    private string mouseXName = "Mouse X";
    [SerializeField]
    private string mouseYName = "Mouse Y";
    [SerializeField]
    private string horizontalName = "Horizontal";
    [SerializeField]
    private string verticalName = "Vertical";
    [SerializeField]
    private string baseSpeedModifierName = "Mouse ScrollWheel";
    [SerializeField]
    private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField]
    private string cursorToggleName = "Cancel";
    [SerializeField]
    private float sensitivity = 1.8f;
    [SerializeField]
    private float sprintModifier = 3f;

    private Camera thisCamera = null;
    private float startSpeed = 0.5f;
    private float sprintInputMultiplier = 3f;
    private float currentSpeedBoost = 2f;

    private void Start()
    {
        thisCamera = gameObject.GetComponent<Camera>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(thisCamera == null)
        {
            return;
        }
        else
        {
            SetCursor();
            HandleInput();
        }
    }

    private void HandleInput()
    {
        CameraRotation(Input.GetAxis(mouseXName), Input.GetAxis(mouseYName));
        Motion(Input.GetAxis(verticalName), Input.GetAxis(horizontalName), Input.GetAxis(baseSpeedModifierName), Input.GetKey(sprintKey));
    }

    private void Motion(float forwardInput, float strafeInput, float baseSpeedModifierInput, bool sprintKeyInput)
    {
        float appliedSpeed = 0f;
        float appliedSprint = sprintKeyInput == true ? sprintModifier : 1f;
        baseSpeedModifierInput = baseSpeedModifierInput * sprintInputMultiplier;
        currentSpeedBoost = Mathf.Max(1f, currentSpeedBoost += baseSpeedModifierInput);
        appliedSpeed = (startSpeed * currentSpeedBoost) * appliedSprint;

        if(forwardInput != 0f)
        {
            thisCamera.transform.Translate(((thisCamera.transform.forward * forwardInput) * appliedSpeed), Space.World);
        }

        if(strafeInput != 0f)
        {
            thisCamera.transform.Translate(((thisCamera.transform.right * strafeInput) * appliedSpeed), Space.World);
        }
    }

    private void SetCursor()
    {
        bool toggleState = Cursor.visible;
        if (Input.GetButtonDown(cursorToggleName) == true)
        {
            toggleState = Cursor.visible == true ? false : true;
        }
        Cursor.visible = toggleState;
        Cursor.lockState = toggleState == false ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void CameraRotation(float axisX, float axisY)
    {
        //Camera Pitch
        transform.rotation *= Quaternion.AngleAxis(-axisY * sensitivity, Vector3.right);

        //Camera Yaw
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + axisX * sensitivity, transform.eulerAngles.z);
    }
}
