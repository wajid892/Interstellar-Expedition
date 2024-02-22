using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class CurvedSpaceDemo : MonoBehaviour
{
    RotationController m_controller = null; // control 4d rotation with mouse/keyboard

    struct MeshObject {
        public GameObject gameObject;
        public MeshFilter meshFilter;
        public Material[] materials;
        //
        public float scale; // additional 3d scale
        public Matrix4x4 rotation; // additional 4d rotation
    }

    MeshObject[] m_meshObjects = null;

    float m_initialCameraSize;

    void Start()
    {
        m_initialCameraSize = Camera.main.orthographicSize;

        m_controller = new RotationController();

        m_meshObjects = new[] {
            InitMeshObject("Venus", 1.0f, Matrix4x4.identity),
            InitMeshObject("Ball",  2.0f, Matrix4x4.identity),
        };

        // Add controller 'Using' to Description
        GameObject.Find("Description")
            .GetComponent<UnityEngine.UI.Text>()
            .text += RotationController.Using;
    }

    MeshObject InitMeshObject(string objectName, float scale, Matrix4x4 position)
    {
        var obj = GameObject.Find(objectName);

        var mr = obj.GetComponent<MeshRenderer>();
        var mf = obj.GetComponent<MeshFilter>();

        Torec.Utils.ApplyWorldTransform(obj); // apply and reset world transform: Shader works in object space

        return new MeshObject {
            gameObject = obj,
            meshFilter = mf,
            materials = mr.materials,
            //
            scale = scale,
            rotation = position,
        };
    }

    static Matrix4x4 GetBallRotation() {
        return Torec.Utils.Rotation(0, 2, Time.time * Mathf.PI * 0.25f) *
               Torec.Utils.Rotation(0, 3, 90 * Mathf.Deg2Rad);
    }

    void Update()
    {
        var state = m_controller.GetCurrentState();

        Camera.main.orthographicSize = m_initialCameraSize * state.scale;

        // update ball rotation
        m_meshObjects[1].rotation = GetBallRotation();

        foreach (var mo in m_meshObjects) {
            foreach (var m in mo.materials) {
                m.SetFloat("_Scale", mo.scale);
                m.SetMatrix("_Rotation", state.rotation * mo.rotation);
            }
        }
    }


}
