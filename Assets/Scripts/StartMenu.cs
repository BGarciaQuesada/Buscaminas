using TMPro;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField heightInput;
    [SerializeField] private TMP_InputField widthInput;
    [SerializeField] private TMP_InputField bombsInput;

    // Panel de error en la UI y texto donde se muestran mensajes
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;

    public static StartMenu instance;

    public void Start()
    {
        instance = this;
        if (errorPanel != null) errorPanel.SetActive(false);
    }

    // Intenta parsear los tres campos; si falla, devuelve false y un mensaje explicativo
    public bool TryParseInputs(out int height, out int width, out int bombs, out string parseErrorMessage)
    {
        parseErrorMessage = string.Empty;
        height = 0; width = 0; bombs = 0;

        if (!int.TryParse(heightInput.text, out height))
        {
            parseErrorMessage = "Altura no es un número válido.";
            return false;
        }

        if (!int.TryParse(widthInput.text, out width))
        {
            parseErrorMessage = "Anchura no es un número válido.";
            return false;
        }

        if (!int.TryParse(bombsInput.text, out bombs))
        {
            parseErrorMessage = "Número de bombas no es un número válido.";
            return false;
        }

        return true;
    }

    public void ShowError(string message)
    {
        if (errorText != null) errorText.text = message;
        if (errorPanel != null) errorPanel.SetActive(true);
    }

    // Oculta el panel de error
    public void HideError()
    {
        if (errorPanel != null) errorPanel.SetActive(false);
    }
    public void OnOkButton()
    {
        HideError();
    }
}