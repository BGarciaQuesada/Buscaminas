using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Se encaragará de crear el mapa inicial de juego

    [SerializeField] GameObject startMenu;
    public GameObject endMenu;
    public static GameManager instance;
    public bool endgame;

    private int totalSafePieces; // Casillas sin bomba
    private int revealedSafePieces; // Cuántas han sido reveladas

    // Awake va antes que Start, osea, antes de arrancar si quiera

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Lógica de victoria
    public void SetTotalSafePieces(int total)
    {
        totalSafePieces = total;
        revealedSafePieces = 0;
    }

    public void AddRevealedPiece()
    {
        revealedSafePieces++;
        CheckVictory();
    }

    private void CheckVictory()
    {
        if (revealedSafePieces >= totalSafePieces && !endgame)
        {
            endgame = true;
            endMenu.SetActive(true);
            ShowVictory();
        }
    }

    public void Start()
    {
        // Al empezar pone el menu
        DontDestroyOnLoad(gameObject);
        startMenu.SetActive(true);
        endMenu.SetActive(false);
    }

    public void GameStart()
    {
        // Recoger datos del menú usando los métodos de StartMenu
        if (!StartMenu.instance.TryParseInputs(out int height, out int width, out int bombsNumber, out string parseErr))
        {
            StartMenu.instance.ShowError(parseErr);
            return;
        }

        // Validaciones específicas solicitadas

        // --- EL PROFESOR LO HIZO DE OTRA MANERA CON VALIDATE USANDO BINARIO, NO LO HE ENTENDIDO, SUSTITUCION: ---
        var errors = new List<string>();

        if(width <= 0 || height <= 0 || bombsNumber <= 0) errors.Add("Ningún campo puede ser menor o igual a 0.");
        if (bombsNumber > width * height) errors.Add("Hay más bombas que celdas (alto x ancho).");
        if (bombsNumber > 100) errors.Add("El número de bombas no puede ser superior a 100.");

        if (errors.Count > 0)
        {
            StartMenu.instance.ShowError(string.Join("\n", errors));
            return;
        }
        // --------------------------------------------------------------------------------------------------------

        Generator.gen.SetHeight(height);
        Generator.gen.SetWidth(width);
        Generator.gen.SetBombsNumber(bombsNumber);

        // Ahora mismo siempre recibirá 0 porque, si hay un error, hay un return antes
        if (Generator.gen.Validate() == 0)
        {
            // Limpio antes de generar
            Generator.gen.ClearMap();

            Generator.gen.Generate();

            // Hemos generado el mapa, menú bye-bye
            StartMenu.instance.HideError();
            startMenu.SetActive(false);
        }
        else
        {
            // Si Validate devolviera error, mostramos un mensaje
            StartMenu.instance.ShowError("Error al validar los parámetros de generación.");
        }
    }

    public void ShowDefeat()
    {
        endMenu.transform.Find("Victoria").gameObject.SetActive(false);
        endMenu.transform.Find("Derrota").gameObject.SetActive(true);
        StartCoroutine(EndGameRoutine());
    }

    public void ShowVictory()
    {
        endMenu.transform.Find("Victoria").gameObject.SetActive(true);
        endMenu.transform.Find("Derrota").gameObject.SetActive(false);
        StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        // Esperar 2 segundos
        yield return new WaitForSeconds(2f);

        // Ocultar menu final y mostrar inicial
        endMenu.SetActive(false);
        startMenu.SetActive(true);

        // Reset de valores
        endgame = false;
        revealedSafePieces = 0;
    }
}
