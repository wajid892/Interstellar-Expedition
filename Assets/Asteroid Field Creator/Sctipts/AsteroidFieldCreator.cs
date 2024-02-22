#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

//-----------------------------------------------------
// ASTEROID FIELD CREATOR
//-----------------------------------------------------
// By: jandd661
// Contact: jandd661@gmail.com
//-----------------------------------------------------

//---------------------------------------------------------
// INSTRUCTIONS:
//---------------------------------------------------------
// 1. Place this script on the root empty object that will be
// the containing object for the field.
//
// 2. Place AstroData.cs on all top level objects/parent
// objects that will be used as a pefab. There must only
// be one AstroData.cs on a prefab
//
// 3. Drag and drop the prefabs into the prefab list at the
// bottom of this script in the inspector.
//
// 4. Click "Create"
//---------------------------------------------------------

[ExecuteInEditMode]
public class AsteroidFieldCreator : MonoBehaviour
{
    //-----------------------------------------------------
    // passCountDelineator
    //-----------------------------------------------------
    // passCountDelineator is the character used to seperate
    // the pass count and number in the object names. If this
    // character is in the prefab name. It will be replaced
    // with a space. Mainly used for "Undo Last" functions.
    [SerializeField]
    private string passCountDelineator = "|";

    //-----------------------------------------------------
    // enablePrefabLink
    //-----------------------------------------------------
    // enablePrefabLink may or may not work in your Unity version.
    // Seems to be ok in 2018 and up. Do not change this here
    // if you are using the custom inspector. Change the value
    // "turnOnPrefabLink" at the top of the custom inspector
    // script in the Editor folder
    [SerializeField]
    private bool enablePrefabLink = false;

    //-----------------------------------------------------
    // End optional configuration - messing with anything 
    // below here may break it.
    //-----------------------------------------------------
    [SerializeField]
    private bool placementCoroutineIsRunning = false;

    [SerializeField]
    private int totalObjects = 0;
    [SerializeField]
    private float valueForProgressBar = 0f;

    [SerializeField]
    List<GameObject> objectPrefabs = new List<GameObject>();

    [SerializeField]
    private bool keepPrefabLink = false;

    [SerializeField]
    private int placementAttempts = 360;
    [SerializeField]
    [Range(-360f, 360f)]
    private float rotateDegresPerAttempt = 1f;

    [SerializeField]
    private bool randomScale = true;
    [SerializeField]
    private float scaleMinSize = 5f;
    [SerializeField]
    private float scaleMaxSize = 15f;

    [SerializeField]
    private bool adjustMassByScale = false;
    [SerializeField]
    private float massScaleFactor = 0.7f;

    [SerializeField]
    private bool randomRotation = true;

    [SerializeField]
    private float fieldSize = 1000;
    [SerializeField]
    FieldType fieldType = FieldType.Ring;
    [SerializeField]
    private float ringWidth = 500f;
    [SerializeField]
    private float ringThickness = 100f;
    [SerializeField]
    private float targetWidth = 150f;
    [SerializeField]
    private float targetWidthPositionOffset = 0f;

    [SerializeField]
    private bool showFieldGizmo = true;
    [SerializeField]
    private bool showBufferGizmo = true;
    [SerializeField]
    private bool enableMeshes = true;
    [SerializeField]
    private bool enableColliders = true;
    [SerializeField]
    private bool showLogging = false;

    private GameObject pivotObject = null;
    private static int passCount = 0; 
    private static bool currentBufferGizmoSelection = false;
    private static bool currentShowMeshes = true;
    private static bool currentShowColliders = true;
    private static float lengthPositionOffset = 0f;
    private static bool StopAndCleanGenerator = false;
    private static IEnumerator placementCoroutine = null;
    private static Vector3 handlesNormalDefault = new Vector3(0, 1, 0);

    private void OnEnable()
    {
        EditorApplication.update += InEditorUpdate;
        ResetDefaults();
        ResetGenerator();
    }

    private void InEditorUpdate()
    {
        ErrorCheckInput();
        if (placementCoroutineIsRunning == true)
        {
            placementCoroutine.MoveNext();
        }
        UpdateMeshesAndColliders(placementCoroutineIsRunning);
    }

    private void ErrorCheckInput()
    {
        fieldSize = Mathf.Max(1f, fieldSize);
        ringWidth = Mathf.Max(1f, ringWidth);
        ringThickness = Mathf.Max(1f, ringThickness);
        targetWidth = Mathf.Max(1f, targetWidth);
    }

    private void ResetDefaults()
    {
        totalObjects = transform.childCount;
        GetExistingPassCount();

        currentBufferGizmoSelection = showBufferGizmo;
        currentShowMeshes = enableMeshes;
        currentShowColliders = enableColliders;
        UpdateMeshesAndColliders(false);
    }

    public void ResetGenerator()
    {
        if(placementCoroutine != null)
        {
            StopCoroutine(placementCoroutine);
            placementCoroutine = null;
        }
        lengthPositionOffset = 0f;
        StopAndCleanGenerator = false;
        placementCoroutineIsRunning = false;
        ShowDebugMessage("Generator reset!", false);
    }

    private void UpdateMeshesAndColliders(bool overrideColliders)
    {
        if ((currentShowMeshes != enableMeshes || currentShowColliders != enableColliders) || overrideColliders == true)
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                AstroData astroData = child.GetComponent<AstroData>();
                if (astroData != null)
                {
                    astroData.ShowMesh = enableMeshes;
                    if(overrideColliders == false)
                    {
                        astroData.ShowColliders = enableColliders;
                    }
                    else
                    {
                        astroData.ShowColliders = true;
                    }
                }
            }
            tempList.Clear();
            currentShowMeshes = enableMeshes;
            if(overrideColliders == false)
            {
                currentShowColliders = enableColliders;
            }
            else
            {
                currentShowColliders = true;
            }
        }
    }

    private void GetExistingPassCount()
    {
        if (totalObjects > 0)
        {
            int existingPassCount = 0;
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                string[] objectName = child.gameObject.name.Split(char.Parse(passCountDelineator));
                if(objectName.Length > 1)
                {
                    int passNumber = 0;
                    if (int.TryParse(objectName[1], out passNumber))
                    {
                        if (passNumber > existingPassCount)
                        {
                            existingPassCount = passNumber;
                        }
                    }
                }
            }
            tempList.Clear();
            passCount = existingPassCount;
        }
    }

    public void StartGenerator()
    {
        ShowDebugMessage("Starting Generator...", false);
        if (placementCoroutine == null)
        {
            placementCoroutine = GenerateField();
        }
        if (placementCoroutineIsRunning == false)
        {
            StopCoroutine(placementCoroutine);
            if (objectPrefabs == null)
            {
                ShowDebugMessage("Asteroid Generator has no prefabs!", true);
            }
            else
            {
                ShowDebugMessage("Inicializing prefabs...", false);
                for (int i = objectPrefabs.Count - 1; i > -1; i--)
                {
                    if (objectPrefabs[i] == null)
                    {
                        objectPrefabs.RemoveAt(i);
                    }
                }

                if (objectPrefabs.Count == 0)
                {
                    ShowDebugMessage("Asteroid Generator has no prefabs!", true);
                }
                else
                {
                    if(placementAttempts > 0)
                    {
                        ShowDebugMessage("Engage!", false);
                        StartCoroutine(placementCoroutine);
                    }
                    else
                    {
                        ShowDebugMessage("Placement attempts needs to be greater than 0!", true);
                    }
                }
            }
        }
        else
        {
            ShowDebugMessage("Generation already inprogress...", true);
        }

    }

    private IEnumerator GenerateField()
    {
        ShowDebugMessage("Generator comming to life...", false);

        placementCoroutineIsRunning = true;
        GameObject newAsteroid = null;
        int currentPrefabIndex = 0;
        float currentRotation = 0f;
        bool complete = false;

        if (placementAttempts < 1)
        {
            complete = true;
            ShowDebugMessage("Placement attempts less than 1!", true);
        }

        valueForProgressBar = 0f;
        passCount++;

        pivotObject = new GameObject("pivot");
        pivotObject.transform.parent = transform;
        pivotObject.transform.localPosition = Vector3.zero;

        if (fieldType == FieldType.Ribbon)
        {
            lengthPositionOffset = targetWidthPositionOffset;
        }
        else
        {
            lengthPositionOffset = 0f;
        }
        
        int loopCount = 0;
        ShowDebugMessage("Generator is running...", false);
        while (complete == false)
        {
            if (StopAndCleanGenerator == false)
            {
                Vector3 targetPosition = Vector3.zero;
                Vector3 tempTargetPosition = Vector3.zero;
                Quaternion defaultRotation = Quaternion.identity;
                Quaternion targetRotation = Quaternion.identity;
                Rigidbody newRb = null;
                string prefabName = "";
                Vector3 prefabScale = Vector3.zero;
                float scaleFactor = 1f;

                if (fieldType == FieldType.Blob_Sphere_Inside)
                {
                    tempTargetPosition = Random.insideUnitSphere * fieldSize;
                    targetPosition = tempTargetPosition + transform.position;
                }
                else if (fieldType == FieldType.Blob_Sphere_Surface)
                {
                    tempTargetPosition = Random.onUnitSphere * (Random.Range((fieldSize - ringWidth / 2f), (fieldSize + ringWidth / 2f)));
                    targetPosition = tempTargetPosition + transform.position;
                }
                else if (fieldType == FieldType.Ring || fieldType == FieldType.Ribbon)
                {
                    if (fieldType == FieldType.Ring)
                    {
                        pivotObject.transform.Rotate(0f, currentRotation, 0f);
                    }
                    tempTargetPosition.x = Random.Range((pivotObject.transform.forward.x + lengthPositionOffset) - targetWidth / 2f, (pivotObject.transform.forward.x + lengthPositionOffset) + targetWidth / 2f);
                    tempTargetPosition.y = Random.Range(pivotObject.transform.forward.y - (ringThickness / 2f), pivotObject.transform.forward.y + (ringThickness / 2f));
                    tempTargetPosition.z = Random.Range(pivotObject.transform.forward.z + fieldSize - (ringWidth / 2f), pivotObject.transform.forward.z + fieldSize + (ringWidth / 2f));
                    targetPosition = pivotObject.transform.TransformPoint(tempTargetPosition);
                }
                targetRotation = randomRotation == true ? Random.rotation : Quaternion.identity;
                scaleFactor = randomScale == true ? Random.Range(scaleMinSize, scaleMaxSize) : 1f;
                prefabName = objectPrefabs[currentPrefabIndex].name;
                prefabScale = objectPrefabs[currentPrefabIndex].transform.localScale;
                defaultRotation = objectPrefabs[currentPrefabIndex].transform.rotation;

                if (keepPrefabLink == false || enablePrefabLink == false)
                {
                    newAsteroid = Instantiate(objectPrefabs[currentPrefabIndex], targetPosition, defaultRotation, transform);
                }
                else if (keepPrefabLink == true && enablePrefabLink == true)
                {
                    newAsteroid = PrefabUtility.InstantiatePrefab(objectPrefabs[currentPrefabIndex] as GameObject) as GameObject;
                    newAsteroid.transform.parent = transform;
                    newAsteroid.transform.position = targetPosition;
                    newAsteroid.transform.rotation = defaultRotation;
                }

                if (newAsteroid != null)
                {
                    newAsteroid.name = prefabName.Replace(char.Parse(passCountDelineator), char.Parse(" ")) + passCountDelineator + passCount + passCountDelineator + loopCount;
                    AstroData astroData = newAsteroid.GetComponent<AstroData>();

                    if (astroData != null)
                    {
                        if (astroData.DoNotScale != true)
                        {
                            newAsteroid.transform.localScale = new Vector3(prefabScale.x * scaleFactor, prefabScale.y * scaleFactor, prefabScale.z * scaleFactor);
                        }
                        if (adjustMassByScale == true && astroData.DoNotScale != true)
                        {
                            if (astroData.RigidbodyToScale != null)
                            {
                                newRb = astroData.RigidbodyToScale;
                            }
                            else if (newAsteroid.GetComponent<Rigidbody>() != null)
                            {
                                newRb = newAsteroid.GetComponent<Rigidbody>();
                            }

                            if (newRb != null)
                            {
                                float targetMass = (newRb.mass * massScaleFactor) * scaleFactor;
                                newRb.mass = targetMass;
                            }
                        }
                        if (astroData.DoNotRotate != true)
                        {
                            newAsteroid.transform.rotation = targetRotation;
                        }
                        astroData.CheckPlacement();
                        astroData.ShowBufferGizmo = showBufferGizmo;
                        astroData.ShowMesh = enableMeshes;
                        astroData.ShowColliders = enableColliders;
                    }
                    else
                    {
                        ShowDebugMessage(prefabName + " has no AstroData script", true);
                        newAsteroid = null;
                    }
                    currentRotation = rotateDegresPerAttempt;
                    currentPrefabIndex++;
                    currentPrefabIndex = currentPrefabIndex == objectPrefabs.Count ? 0 : currentPrefabIndex;
                }
                loopCount++;
                valueForProgressBar = (float)loopCount / (float)placementAttempts;
                complete = loopCount == placementAttempts ? true : false;
                yield return null;
            }
            else
            {
                complete = true;
                ShowDebugMessage("E-Stop Recieved...", false);
            }
        }

        if (complete == true)
        {
            ShowDebugMessage("Stopping Generator...", false);
            if (newAsteroid != null)
            {
                DestroyImmediate(newAsteroid);
                ShowDebugMessage("Orphaned asteroids removed...", false);
            }

            if (pivotObject != null)
            {
                DestroyImmediate(pivotObject);
                ShowDebugMessage("Orphaned pivot removed...", false);
            }
            ShowDebugMessage("Object clean up complete...", false);
            valueForProgressBar = 0f;
            EnforceBufferZones();
            ResetGenerator();
        }
    }

    public void CancelPlacement()
    {
        ShowDebugMessage("Emergency stop...", false);
        if (placementCoroutineIsRunning == true)
        {
            ShowDebugMessage("Starting E-Stop...", false);
            StopAndCleanGenerator = true;
        }
        else
        {
            ShowDebugMessage("Nothing to stop", true);
        }
    }

    public void UndoLast()
    {
        if (PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.NotAPrefab)
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                string[] roidName = child.gameObject.name.Split(char.Parse(passCountDelineator));
                if (roidName.Length > 1)
                {
                    if (roidName[1] == passCount.ToString())
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }
            tempList.Clear();
            passCount--;
            passCount = passCount < 0 ? 0 : passCount;
        }
        else
        {
            ShowDebugMessage("Unable to delete. Instance is part of a prefab. Unpack the prefab to edit.", true);
        }
    }

    public void ClearAsteroids()
    {
        if(PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.NotAPrefab)
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                AstroData astroData = child.GetComponent<AstroData>();
                if (astroData != null)
                {
                    if (astroData.DoNotDelete == false)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            tempList.Clear();
            passCount = 0;
        }
        else
        {
            ShowDebugMessage("Unable to delete. Instance is part of a prefab. Unpack the prefab to edit.", true);
        }
    }

    public void AddRemoveAstroDataScripts(bool removeScripts)
    {
        var tempList = transform.Cast<Transform>().ToList();
        if (PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected)
        {
            PrefabUtility.UnpackPrefabInstance(this.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
        foreach (var child in tempList)
        {
            AstroData astroData = child.GetComponent<AstroData>();
            if (removeScripts == true)
            {
                if (astroData != null)
                {
                    GameObject targetObject = astroData.gameObject;
                    if (PrefabUtility.GetPrefabInstanceStatus(targetObject) == PrefabInstanceStatus.Connected)
                    {
                        PrefabUtility.UnpackPrefabInstance(targetObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    }
                    if (PrefabUtility.IsPartOfAnyPrefab(targetObject) == false)
                    {
                        DestroyImmediate(astroData);
                    }
                }
            }
            else
            {
                if (astroData == null)
                {
                    GameObject targetObject = child.gameObject;
                    if (PrefabUtility.GetPrefabInstanceStatus(targetObject) == PrefabInstanceStatus.Connected)
                    {
                        PrefabUtility.UnpackPrefabInstance(targetObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    }
                    if (PrefabUtility.IsPartOfAnyPrefab(targetObject) == false)
                    {
                        targetObject.AddComponent<AstroData>();
                    }
                }
            }
        }
        tempList.Clear();
    }

    private void LateUpdate()
    {
        totalObjects = transform.childCount;
    }

    public void EnforceBufferZones()
    {
        UpdateMeshesAndColliders(true);

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            if(child != null)
            {
                AstroData astroData = child.GetComponent<AstroData>();
                if (astroData != null)
                {
                    astroData.EnforceMyBufferZone();
                }
            }
        }
        tempList.Clear();
    }

    private void OnDrawGizmos()
    {
        DrawMyGizmos();
    }

    private void DrawMyGizmos()
    {
        if (showFieldGizmo == true)
        {
            if (fieldType == FieldType.Ribbon)
            {
                lengthPositionOffset = targetWidthPositionOffset;
            }
            else
            {
                lengthPositionOffset = 0f;
            }

            Vector3 ribbonLengthPositionOffset = new Vector3(lengthPositionOffset, 0f, 0f);

            Vector3 centerPosition = transform.position;
            centerPosition.z = transform.position.z + fieldSize;

            Vector3 widthStartPosition = transform.position;
            widthStartPosition.z = centerPosition.z - (ringWidth / 2f);

            Vector3 widthEndPosition = transform.position;
            widthEndPosition.z = centerPosition.z + (ringWidth / 2f);

            Vector3 thickStartPosition = transform.position;
            thickStartPosition.y = transform.position.y - (ringThickness / 2f);
            thickStartPosition.z = centerPosition.z;

            Vector3 thickEndPosition = transform.position;
            thickEndPosition.y = transform.position.y + (ringThickness / 2f);
            thickEndPosition.z = centerPosition.z;

            Vector3 lengthStartPosition = transform.position;
            lengthStartPosition.x = transform.position.x - targetWidth / 2f;
            lengthStartPosition.z = centerPosition.z;

            Vector3 lengthEndPosition = transform.position;
            lengthEndPosition.x = transform.position.x + targetWidth / 2f;
            lengthEndPosition.z = centerPosition.z;


            if (fieldType == FieldType.Ring || fieldType == FieldType.Ribbon)
            {
                if (fieldType == FieldType.Ring)
                {
                    Color ringColor = Color.yellow;
                    ringColor.a = 0.1f;
                    Handles.color = ringColor;
                    Handles.DrawSolidDisc(transform.position, handlesNormalDefault, fieldSize);
                }
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(widthStartPosition + ribbonLengthPositionOffset, widthEndPosition + ribbonLengthPositionOffset);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(thickStartPosition + ribbonLengthPositionOffset, thickEndPosition + ribbonLengthPositionOffset);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(lengthStartPosition + ribbonLengthPositionOffset, lengthEndPosition + ribbonLengthPositionOffset);

                Gizmos.color = Color.cyan;
                Vector3 boxCenter = Vector3.zero;
                boxCenter = centerPosition + ribbonLengthPositionOffset;
                Gizmos.DrawWireCube(boxCenter, new Vector3(targetWidth, ringThickness, ringWidth));
            }
            else if (fieldType == FieldType.Blob_Sphere_Inside || fieldType == FieldType.Blob_Sphere_Surface)
            {
                //Gizmos.color = Color.yellow;
                //Gizmos.DrawWireSphere(transform.position, fieldSize);
                Color ringColor = Color.cyan;
                ringColor.a = 0.1f;
                Handles.color = ringColor;
                Handles.SphereHandleCap(0, transform.position, Quaternion.identity, fieldSize * 2, EventType.Repaint);
                if (fieldType == FieldType.Blob_Sphere_Surface)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(widthStartPosition, widthEndPosition);
                }
            }
        }
        //--------------------------------------------
        // Only do this when the selection changes.
        //--------------------------------------------
        if (currentBufferGizmoSelection != showBufferGizmo)
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                AstroData astroData = child.GetComponent<AstroData>();
                if (astroData != null)
                {
                    astroData.ShowBufferGizmo = showBufferGizmo;
                }
            }
            tempList.Clear();
            currentBufferGizmoSelection = showBufferGizmo;
        }
    }

    private void ShowDebugMessage(string debugText, bool forceMessage)
    {
        if(showLogging == true || forceMessage == true)
        {
            Debug.Log(gameObject.name + ": " + debugText);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        EditorApplication.update -= InEditorUpdate;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        EditorApplication.update -= InEditorUpdate;
    }
}

public enum FieldType { Blob_Sphere_Inside = 0, Blob_Sphere_Surface = 1, Ring = 2, Ribbon = 3 }

#endif