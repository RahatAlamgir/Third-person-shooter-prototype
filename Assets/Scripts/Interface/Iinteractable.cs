using UnityEngine;

public interface Iinteractable
{
    public bool CanInteract();

    public void Interact(Interactor interactor);
}
