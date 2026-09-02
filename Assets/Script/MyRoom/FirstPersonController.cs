using UnityEngine;
using UnityEngine.InputSystem; // นำเข้าระบบอินพุตใหม่ (New Input System)

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f; // ความเร็วขณะวิ่ง (Shift)
    [SerializeField] private float lookSpeed = 0.1f; // สเกลเมาส์ของระบบใหม่จะเร็วกว่า แนะนำเริ่มต้นที่ 0.1f
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // ค้นหา Camera ที่อยู่ในลูกอัตโนมัติหากไม่ได้ใส่ข้อมูลใน Inspector
        if (cameraTransform == null)
        {
            Camera childCamera = GetComponentInChildren<Camera>();
            if (childCamera != null)
            {
                cameraTransform = childCamera.transform;
                Debug.Log($"[FirstPersonController] ค้นหาและตั้งค่ากล้องอัตโนมัติสำเร็จ: {cameraTransform.name}", this);
            }
            else
            {
                Debug.LogError("FirstPersonController: ไม่พบกล้องในวัตถุลูก กรุณากำหนด cameraTransform ใน Inspector เพื่อให้กล้องหมุนตามตัวละครได้", this);
            }
        }
        else
        {
            Debug.Log($"[FirstPersonController] เริ่มต้นการทำงานด้วยกล้อง: {cameraTransform.name}", this);
        }
        
        // ซ่อนและล็อกเคอร์เซอร์เมาส์ให้อยู่กึ่งกลางหน้าจอ
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("[FirstPersonController] ทำการล็อกและซ่อนเคอร์เซอร์เมาส์แล้ว", this);
    }

    private void Update()
    {
        // ⚡ [แก้ไขเพิ่มเติม] หากเคอร์เซอร์ไม่ได้ล็อกอยู่ ให้ตรวจจับว่าคลิกเมาส์กลับเข้าเกมหรือไม่ (หากไม่มีเมนูหรือบทสนทนาเปิดอยู่)
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Time.timeScale > 0f)
            {
                DialogueManager dm = Object.FindAnyObjectByType<DialogueManager>();
                InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();
                PauseMenuController pm = Object.FindAnyObjectByType<PauseMenuController>();

                bool isDialogueOpen = dm != null && dm.IsDialogueActive();
                bool isInventoryOpen = inv != null && inv.inventoryPanel != null && inv.inventoryPanel.activeSelf;
                bool isPauseMenuOpen = pm != null && pm.pauseMenuPanel != null && pm.pauseMenuPanel.activeSelf;

                if (!isDialogueOpen && !isInventoryOpen && !isPauseMenuOpen)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Debug.Log("[FirstPersonController] คลิกหน้าจอเพื่อล็อกเมาส์กลับเข้าสู่มุมมองบุคคลที่หนึ่ง", this);
                }
            }
            return;
        }

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector2 inputMove = Vector2.zero;

        // รับค่าการเคลื่อนที่จาก Keyboard ในระบบใหม่
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) inputMove.y = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) inputMove.y = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) inputMove.x = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) inputMove.x = 1f;
        }

        // ตรวจสอบการกดปุ่มวิ่ง (Left Shift) ในระบบใหม่
        float currentSpeed = moveSpeed;
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
        {
            currentSpeed = sprintSpeed;
        }

        // ปรับทิศทางการเคลื่อนที่ตามการหันของตัวละครและทำให้ความเร็วเฉียงไม่เร็วกว่าปกติ (Normalize)
        Vector3 move = Vector3.zero;
        if (inputMove.sqrMagnitude > 0.01f)
        {
            inputMove.Normalize();
            move = (transform.right * inputMove.x + transform.forward * inputMove.y) * currentSpeed;
        }

        // คำนวณแรงโน้มถ่วงเพื่อยึดตัวละครติดพื้น
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        
        // รวมการเคลื่อนที่ระนาบและแนวดิ่ง
        move.y = velocity.y;
        
        characterController.Move(move * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (cameraTransform == null) return;

        Vector2 mouseDelta = Vector2.zero;

        // รับค่าตำแหน่งการเลื่อนของเมาส์ในระบบใหม่
        if (Mouse.current != null)
        {
            mouseDelta = Mouse.current.delta.ReadValue();
        }

        // คำนวณความเร็วในการหัน (คูณด้วย MouseSensitivity จากหน้าต่าง Settings และคูณด้วย 0.05f เพื่อความนุ่มนวล)
        float mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        float mouseX = mouseDelta.x * lookSpeed * mouseSensitivity * 0.05f;
        float mouseY = mouseDelta.y * lookSpeed * mouseSensitivity * 0.05f;

        // ควบคุมการก้มและเงยหน้าของกล้อง
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // ควบคุมการหันซ้ายและขวาของตัวละคร
        transform.Rotate(Vector3.up * mouseX);
    }
}
