using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 LowerLeftBound;
    public Vector2 UpperRightBound;
    public GameObject Player;
    private Camera cam;
    private Vector3 prev;

    void Start() {
        cam = GetComponent<Camera>();
        prev = Vector3.zero;
    }

    void Update()
    {
        float halfHeigth = cam.orthographicSize;
        float halfWidth = cam.orthographicSize * cam.aspect;
        transform.position = new Vector3(
            Player.transform.position.x > LowerLeftBound.x + halfWidth && Player.transform.position.x < UpperRightBound.x - halfWidth
            ? Player.transform.position.x : Player.transform.position.x < LowerLeftBound.x + halfWidth
            ? LowerLeftBound.x + halfWidth : UpperRightBound.x - halfWidth,
            Player.transform.position.y > LowerLeftBound.y + halfHeigth && Player.transform.position.y < UpperRightBound.y - halfHeigth
            ? Player.transform.position.y : Player.transform.position.y < LowerLeftBound.y + halfHeigth
            ? LowerLeftBound.y + halfHeigth : UpperRightBound.y - halfHeigth,
            -10
        );
    }
}
