using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Torec;

public class RotationController
{
    private SmoothValue2 m_smoothMouseTilt = new SmoothValue2(Vector2.zero, 0.15f);
    private SmoothValue  m_smoothEvert     = new SmoothValue(0.0f, 10f);
    private SmoothValue  m_smoothScale     = new SmoothValue(0.0f, 10f);

    // current 4d rotation
    private Matrix4x4 m_rotation = Matrix4x4.identity;

    // "drag" rotation
    private bool m_dragging = false;
    private Vector2 m_prevMouseScreenPos = Vector2.zero;
    private SmoothValue2 m_smoothDragDelta = new SmoothValue2(Vector2.zero, 5f);

    public static string Using = @"Mouse drag or Keyboard cursor
to rotate the space,
Mouse wheel to evert the space,
Mouse wheel + Alt to zoom.";

    public struct State {
        public float scale;
        public Matrix4x4 rotation;
    }

    public State GetCurrentState() {
        Vector2 screenCenter = new Vector2(Screen.width, Screen.height) / 2;
        float screenRadius = Mathf.Min(screenCenter.x, screenCenter.y);

        // keyboard rotation
        float axisV = Input.GetAxis("Vertical");
        float axisH = Input.GetAxis("Horizontal");

        // mouse wheel - evert, scale, spin
        float w = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.LeftAlt)) {
            m_smoothScale.target += w;
        } else {
            m_smoothEvert.target += w;
        }

        float evertAngle = m_smoothEvert.GetValue();
        Matrix4x4 matEvert = Utils.Rotation(2, 3, evertAngle);

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 mousePos = (mouseScreenPos - screenCenter) / screenRadius; // e.g. (-0.6..0.6, -1..1) - depending on aspect

        m_smoothMouseTilt.target = mousePos;

        // dragging
        if (Input.GetMouseButton(0)) {
            if (!m_dragging) {
                m_dragging = true;
            } else {
                Vector2 delta = mouseScreenPos - m_prevMouseScreenPos;
                m_smoothDragDelta.target = delta / screenRadius;
            }
            m_prevMouseScreenPos = mouseScreenPos;
        } else {
            if (m_dragging) {
                m_dragging = false;
                m_smoothDragDelta.target = Vector2.zero;
            }
        }

        // tilt
        Matrix4x4 matTilt = Matrix4x4.identity;
        Vector2 tiltNorm = m_smoothMouseTilt.GetValue();
        float tiltMagnitude = tiltNorm.magnitude;
        if (tiltMagnitude > 0) {
            tiltNorm /= tiltMagnitude;
            matTilt =
                Utils.Rotation(0, 1, tiltNorm.x, tiltNorm.y) *
                Utils.Rotation(2, 0, tiltMagnitude * 1.4f) *
                Utils.Rotation(1, 0, tiltNorm.x, tiltNorm.y);
        }

        // drag rotation
        Matrix4x4 dragRot = Matrix4x4.identity;
        if (m_smoothDragDelta.IsMoving()) {
            Vector2 dragDelta = m_smoothDragDelta.GetValue();
            float dragMagnitude = dragDelta.magnitude;
            if (dragMagnitude > 0) {
                dragDelta /= dragMagnitude;
                dragRot =
                    Utils.Rotation(0, 1, dragDelta.x, dragDelta.y) *
                    Utils.Rotation(0, 2, dragMagnitude * Time.deltaTime * 100f) *
                    Utils.Rotation(1, 0, dragDelta.x, dragDelta.y);
            }
        }

        // keyboard rotation
        Matrix4x4 keyboardRot = Matrix4x4.identity;
        if (axisH != 0f || axisV != 0) {
            Vector2 rotNorm = new Vector2(axisH, axisV) * -10f;
            float rotMagnitude = rotNorm.magnitude;
            if (rotMagnitude > 0) {
                rotNorm /= rotMagnitude;
                keyboardRot = 
                    Utils.Rotation(0, 1, rotNorm.x, rotNorm.y) *
                    Utils.Rotation(2, 0, rotMagnitude * Time.deltaTime * 0.3f) *
                    Utils.Rotation(1, 0, rotNorm.x, rotNorm.y);
            }
        }

        m_rotation = 
            keyboardRot * 
            dragRot * 
            m_rotation;

        return new State {
            scale    = Mathf.Exp(m_smoothScale.GetValue()),
            rotation = 
                matTilt * 
                matEvert * 
                m_rotation,
        };

    }
}

namespace Torec
{
    public static class Utils
    {
        // 4D rotation
        public static Matrix4x4 Rotation(int i, int j, float a) {
            float c = Mathf.Cos(a);
            float s = Mathf.Sin(a);
            return Rotation(i, j, c, s);
        }
        public static Matrix4x4 Rotation(int i, int j, float c, float s) {
            var m = Matrix4x4.identity;
            m[i, i] = c; m[i, j] = -s;
            m[j, i] = s; m[j, j] = c;
            return m;
        }

        // Apply object world transform to mesh vertices and reset world transform
        public static void ApplyWorldTransform(GameObject obj) {
            var tr = obj.GetComponent<Transform>();
            var mf = obj.GetComponent<MeshFilter>();
            // clone the mesh
            var mesh = Object.Instantiate<Mesh>(mf.sharedMesh);
            // apply world transform to mesh vertices
            mf.sharedMesh = ApplyTransform(mesh, tr.localToWorldMatrix);
            // reset object's world transform
            tr.position = Vector3.zero;
            tr.rotation = Quaternion.identity;
            tr.localScale = Vector3.one;
        }

        public static Mesh ApplyTransform(Mesh mesh, Matrix4x4 transform) {
            mesh.vertices = mesh.vertices.Select(v => transform.MultiplyPoint(v)).ToArray();
            if (mesh.normals != null) {
                mesh.normals = mesh.normals.Select(n => transform.MultiplyVector(n)).ToArray();
            }
            if (mesh.tangents != null) {
                mesh.tangents = mesh.tangents.Select(t => {
                    var v = transform.MultiplyVector(new Vector3(t.x, t.y, t.z));
                    return new Vector4(v.x, v.y, v.z, t.w);
                }).ToArray();
            }
            return mesh;
        }

    }

    // Smoothing a changing value like Unity-s Axis
    public class SmoothValue {
        public  float target = 0.0f;
        private float value = 0.0f;
        private float velocity = 0.1f;
        public SmoothValue(float value, float velocity) {
            this.value = this.target = value;
            this.velocity = velocity;
        }
        public bool IsMoving() {
            return Mathf.Abs(target - value) > 0.0001f;
        }
        public float GetValue() {
            if (value != target) {
                value = Mathf.Lerp(value, target, velocity * Time.deltaTime);
            }
            return value;
        }
        public float GetDelta() {
            float prev = value;
            return GetValue() - prev;
        }
    }

    public class SmoothValue2 { // SmoothValue 2D
        private SmoothValue x;
        private SmoothValue y;
        public Vector2 target {
            set { x.target = value.x; y.target = value.y; }
            get { return new Vector2(x.target, y.target); }
        }
        public SmoothValue2(Vector2 value, float velocity) {
            x = new SmoothValue(value.x, velocity);
            y = new SmoothValue(value.y, velocity);
        }
        public bool IsMoving() {
            return x.IsMoving() || y.IsMoving();
        }
        public Vector2 GetValue() {
            return new Vector2(x.GetValue(), y.GetValue());
        }
        public Vector2 GetDelta() {
            return new Vector2(x.GetDelta(), y.GetDelta());
        }
    }

}