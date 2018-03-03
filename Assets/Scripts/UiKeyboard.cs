using UnityEngine;
using UnityEngine.UI;

public class UiKeyboard : MonoBehaviour
{
    public InputField input;

    public void ClickKey(string character)
    {
        Debug.Log(character);
        input.text += character;
    }

    public void Backspace()
    {
        if (input.text.Length > 0)
        {
            input.text = input.text.Substring(0, input.text.Length - 1);
        }
    }

    public void Enter()
    {
        //VRTK_Logger.Info("You've typed [" + input.text + "]");
        input.text = "";
    }

    private void Start()
    {
        //input = GetComponentInChildren<InputField>();
    }
}
