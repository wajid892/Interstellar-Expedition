#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------
// ASTEROID FIELD CREATOR
//-----------------------------------------------------
// By: jandd661
// Contact: jandd661@gmail.com
//-----------------------------------------------------

//-----------------------------------------------------
// INSTRUCTIONS:
//-----------------------------------------------------
// 1. See the comments in AsteroidFieldCreator.cs or
// the PDF included with this package.
//-----------------------------------------------------

[ExecuteInEditMode]
public class AstroData : MonoBehaviour
{
    public float BufferZoneDiameter = 2f;
    public Rigidbody RigidbodyToScale = null;
    public bool ShowBufferGizmo = true;

    [SerializeField]
    private Color BufferZoneColor = Color.magenta;

    public bool DoNotDelete = false;
    public bool DoNotScale = false;
    public bool DoNotRotate = false;

    [SerializeField]
    private List<MeshRenderer> meshesToHide = new List<MeshRenderer>();
    public bool ShowMesh = true;
    private bool currentShowMesh = true;
    [SerializeField]
    private List<Collider> collidersToHide = new List<Collider>();
    public bool ShowColliders = true;
    private bool currentShowColliders = true;

    [HideInInspector]
    public bool HasCollision = false;

    private List<GameObject> deleteList = new List<GameObject>();

    public void Update()
    {
        if(currentShowMesh != ShowMesh)
        {
            ToggleMesh(ShowMesh);
        }
        if(currentShowColliders != ShowColliders)
        {
            ToggleColliders(ShowColliders);
        }

    }

    public void CheckPlacement()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetBufferZoneRadius());
        if (hitColliders.Length > 0f)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].transform.IsChildOf(transform) == false)
                {
                    HasCollision = true;
                }
            }
        }

        if (HasCollision == true)
        {
            DestroyImmediate(gameObject);
        }
    }

    public void EnforceMyBufferZone()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetBufferZoneRadius());
        if (hitColliders.Length > 0f)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].transform.IsChildOf(transform) == false)
                    {
                    AstroData otherAstroData = hitColliders[i].gameObject.GetComponent<AstroData>();
                    if(otherAstroData != null)
                    {
                        if(otherAstroData.DoNotDelete == false)
                        {
                            deleteList = AddToDeleteList(hitColliders[i].gameObject, deleteList);
                        }
                    }
                    else
                    {
                        Transform parentTransform = hitColliders[i].transform.parent;
                        int loopCount = 0;
                        while (parentTransform != null && loopCount < 100)
                        {
                            AstroData parentAstroData = parentTransform.gameObject.GetComponent<AstroData>();
                            if(parentAstroData != null)
                            {
                                deleteList = AddToDeleteList(parentTransform.gameObject, deleteList);
                                parentTransform = null;
                            }
                            else
                            {
                                parentTransform = parentTransform.parent;
                            }
                            loopCount++;
                        }
                    }
                }
            }
            if(deleteList.Count > 0)
            {
                for (int i = 0; i < deleteList.Count; i++)
                {
                    DestroyImmediate(deleteList[i]);
                }
            }
        }
        deleteList.Clear();
    }

    private List<GameObject> AddToDeleteList(GameObject objectToDelete, List<GameObject> inDeleteList)
    {
        if(inDeleteList.Contains(objectToDelete) == false)
        {
            inDeleteList.Add(objectToDelete);
        }
        return inDeleteList;
    }

    private void ToggleMesh(bool enabled)
    {
        for (int i = 0; i < meshesToHide.Count; i++)
        {
            meshesToHide[i].enabled = enabled;
        }
        currentShowMesh = enabled;
    }

    private void ToggleColliders(bool enabled)
    {
        for (int i = 0; i < collidersToHide.Count; i++)
        {
            collidersToHide[i].enabled = enabled;
        }
        currentShowColliders = enabled;
    }

    private float GetBufferZoneRadius()
    {
        float outSize = BufferZoneDiameter;
        float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        outSize = (BufferZoneDiameter / 2f) * maxScale;
        return outSize;
    }

    private void OnDrawGizmos()
    {
        if(ShowBufferGizmo == true)
        {
            float bufferZone = GetBufferZoneRadius();
            Gizmos.color = BufferZoneColor;
            Gizmos.DrawWireSphere(transform.position, bufferZone);
        }
    }
}
#endif
