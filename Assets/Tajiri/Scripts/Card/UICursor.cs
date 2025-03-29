using UnityEngine;

/// <summary>
/// マウス座標を取得するシングルトンクラス
/// </summary>
public class UICursor : MonoBehaviour
{
    public static UICursor Instance { get; private set; }

    private Vector3 cursorPos;

    /// <summary> マウスのワールド座標 </summary>
    public Vector3 CursorPos { get => cursorPos;}

    [SerializeField, Tooltip("メインカメラ")]
    private Camera mainCamera;
    
    void Awake()
    {
        // シングルトン確立
        if (Instance == null) Instance = this;

        // 既に存在する場合は削除
        else Destroy(gameObject);
    }

    private void Update()
    {
        // 毎フレームマウスカーソルの座標を更新
        cursorPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // z座標は常に0
        cursorPos.z = 0;
    }
}
