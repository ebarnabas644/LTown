using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] public GameObject intersection;
    private Camera _camera;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 mousePos = Input.mousePosition;
            Ray castPoint = _camera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(castPoint, out RaycastHit hit, Mathf.Infinity))
            {
                Instantiate(intersection, hit.point, Quaternion.identity);
            }
        }
    }
}