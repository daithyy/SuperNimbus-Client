using UnityEngine;
using UnityEngine.UI;

public abstract class TextInfo : MonoBehaviour
{
    public Text textField;

    public void Awake()
    {
        textField = GetComponent<Text>();
    }

    public void Start()
    {
        textField.text = string.Empty;
    }

    public virtual void Update()
    {
        
    }
}
