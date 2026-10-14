using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private bool hasLost;
    private float movementX;
    private float movementY;

    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 11)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // The enemy tag may be on its body collider, while its movement script is on the parent.
        bool touchedEnemy = collision.collider.CompareTag("Enemy") ||
                            collision.collider.GetComponentInParent<EnemyMovement>() != null;

        if (hasLost || !touchedEnemy)
        {
            return;
        }

        hasLost = true;
        winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        winTextObject.SetActive(true);

        // Keep the player object alive so the camera can continue following it.
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
