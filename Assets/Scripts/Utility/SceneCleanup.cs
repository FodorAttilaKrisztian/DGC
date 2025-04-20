using UnityEngine;

public static class SceneCleanup
{
    public static void DestroyPersistents()
    {
        if (PlayerController.instance != null) Object.Destroy(PlayerController.instance.gameObject);
        if (Managers.instance != null) Object.Destroy(Managers.instance.gameObject);
        if (PersistentCanvas.instance != null) Object.Destroy(PersistentCanvas.instance.gameObject);
        if (PersistentCamera.instance != null) Object.Destroy(PersistentCamera.instance.gameObject);
        if (PauseMenu.instance != null) Object.Destroy(PauseMenu.instance.gameObject);
    }
}