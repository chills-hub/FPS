using UnityEngine;
using UnityEngine.InputSystem;

namespace vnc.Samples
{
    public class MouseLook : MonoBehaviour
    {
        public float mouseSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor { get; private set; }

        [Space]
        public bool cameraKick = true;
        public float cameraKickOffset;
        public float cameraKickoffsetWindow;
        public float cameraKickSpeed = 10;

        private Rigidbody characterRigidbody;
        private Transform characterCamera;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private float kick = 0;

        public InputAction MouseDelta;

        private void OnEnable()
        {
            MouseDelta.Enable();
        }

        private void OnDisable()
        {
            MouseDelta.Disable();
        }

        public virtual void Init(RetroController character, Transform camera)
        {
            characterRigidbody = character.GetComponent<Rigidbody>();
            characterCamera = camera;

            m_CharacterTargetRot = characterRigidbody.rotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public virtual void LookRotation()
        {
            if (!lockCursor)
                return;

            kick -= (Time.deltaTime * cameraKickSpeed);
            kick = Mathf.Clamp(kick, 0, cameraKickOffset);
            Vector2 rotationVector = MouseDelta.ReadValue<Vector2>();


            //float yRot = Input.GetAxis("Mouse X") * mouseSensitivity;
            //float xRot = Input.GetAxis("Mouse Y") * mouseSensitivity;
            float yRot = rotationVector.x * mouseSensitivity;
            float xRot = rotationVector.y * mouseSensitivity;


            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                characterRigidbody.MoveRotation(Quaternion.Slerp(characterRigidbody.rotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime));
                characterCamera.localRotation = Quaternion.Slerp(characterCamera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                characterRigidbody.MoveRotation(m_CharacterTargetRot);
                characterCamera.localRotation = m_CameraTargetRot;

                if (cameraKick)
                {
                    var x = Mathf.Clamp(kick, 0, cameraKickOffset - cameraKickoffsetWindow);
                    characterCamera.localRotation *= Quaternion.Euler(-x, 0, 0);
                }
            }
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
        }

        public void UpdateCursorLock()
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}

