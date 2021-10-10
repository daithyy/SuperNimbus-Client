using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject MenuCamera;

    public GameObject StartMenu;

    public InputField UsernameField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        StartMenu.SetActive(false);
        UsernameField.interactable = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Client.Instance.ConnectToServer();

        MenuCamera.SetActive(false);
    }
}
