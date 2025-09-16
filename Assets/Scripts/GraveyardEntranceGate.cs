using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GraveyardEntranceGate : MonoBehaviour,IInteractable
{
    [SerializeField] GameObject interactionIconPrefab;
    private GameObject interactionIcon;

    private void Start()
    {
        interactionIcon = Instantiate(interactionIconPrefab);
        interactionIcon.transform.parent = UIManager.Instance.WorldCanvas.transform;
        interactionIcon.transform.position = this.transform.position;
        interactionIcon.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            OnIntercationBegin();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            OnIntercationEnd();
        }
       
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Game");
    }

    public void OnIntercationBegin()
    {
        PlayerInputHandler.Instance.InputActions.Guardian.Interact.performed += Interact;
        interactionIcon.SetActive(true);
    }

    public void OnIntercationEnd()
    {
        PlayerInputHandler.Instance.InputActions.Guardian.Interact.performed -= Interact;
        interactionIcon.SetActive(false);
    }
}
