using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{

    public void Interact(InputAction.CallbackContext ctx);

    public void OnIntercationBegin();
    public void OnIntercationEnd();
}
