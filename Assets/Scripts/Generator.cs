using UnityEngine;

public class Generator : MonoBehaviour
{
    // Declaración de variables
    [SerializeField] private GameObject piece;
    [SerializeField] private int width, height, bombsNumber;
    [SerializeField] private GameObject[][] map;
    public bool endgame; // comienza en false

    // Singleton pt1
    public static Generator gen;

    private void Awake()
    {
        // Singleton pt2
        gen = this;
    }

    public void SetWidth(int width)
    {
        this.width = width;
    }
    public void SetHeight(int height)
    {
        this.height = height;
    }
    public void SetBombsNumber(int bombsNumber)
    {
        this.bombsNumber = bombsNumber;
    }

    public void GetWidth(out int width)
    {
        width = this.width;
    }
    public void GetHeight(out int height)
    {
        height = this.height;
    }
    public void GetBombsNumber(out int bombsNumber)
    {
        bombsNumber = this.bombsNumber;
    }

    public void Generate()
    {
        GameManager.instance.SetTotalSafePieces((width * height) - bombsNumber);

        map = new GameObject[width][];
        for (int i = 0; i < map.Length; i++){
            map[i] = new GameObject[height];
        }

        for (int j = 0; j < height; j++){

            for(int i = 0; i < width; i++){

                // Asigno a cada objeto una posicion en la matriz para emplearlos luego (+generarlas por Instantiate)
                map[i][j] = Instantiate(piece, new Vector3(i, j, 0), Quaternion.identity);

                // Tras instanciar la pieza le asigno inmediatamente sus coordenadas en el mapa
                map[i][j].GetComponent<Piece>().setX(i);
                map[i][j].GetComponent<Piece>().setY(j);

            }
        }

        // cast float para que en division, la camara quede centrada en lugar de en la esquina de una casilla
        Camera.main.transform.position = new Vector3((float)(width/2 - 0.5f), (float)(height /2 - 0.5f), -10);

        for(int i = 0; i < bombsNumber; i++) {
            // Range entiende directamente que el maximo es exclusivo (aka. si el width es 10, el maximo es 9)
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            if (!map[x][y].GetComponent<Piece>().isBomb()){
                // Si no es bomba, la convierto en bomba
                map[x][y].GetComponent<Piece>().setBomb(true);
            } else {
                // Si ya es bomba, no cuenta y repito el proceso hasta colocar
                i--;
            }
        }

    }

    public int GetBombsAround(int x, int y)
    {
        int cont = 0;

        // Solo se hace si hay casillas a la encima a la izquierda
        if (x > 0 && y < height - 1 && map[x - 1][y + 1].GetComponent<Piece>().isBomb()) cont++;

        // Solo se hace si hay casillas encima
        if (y < height - 1 && map[x][y + 1].GetComponent<Piece>().isBomb()) cont++;

        // Solo se hace si hay casillas encima a la derecha
        if (x < width - 1 && y < height - 1 && map[x + 1][y + 1].GetComponent<Piece>().isBomb()) cont++;

        // Solo se hace si hay casillas a la izquierda
        if (x > 0 && map[x - 1][y].GetComponent<Piece>().isBomb()) cont++;

        // Solo se hace si hay casillas a la derecha
        if (x < width - 1 && map[x + 1][y].GetComponent<Piece>().isBomb()) cont++;

        // Solo se hace si hay casillas debajo a la izquierda
        if (x > 0 && y > 0 && map[x - 1][y - 1].GetComponent<Piece>().isBomb()) cont++;

        // Solo se hace si hay casillas debajo
        if (y > 0 && map[x][y - 1].GetComponent<Piece>().isBomb()) cont++;

        // Solo se hace si hay casillas debajo a la derecha
        if (x < width - 1 && y > 0 && map[x + 1][y - 1].GetComponent<Piece>().isBomb()) cont++;

        return cont;
    }

    public void CheckPieceAround(int x, int y)
    {
        // Solo se hace si hay casillas a la encima a la izquierda
        if (x > 0 && y < height - 1)
            map[x-1][y+1].GetComponent<Piece>().DrawBomb();

        // Solo se hace si hay casillas encima
        if (y < height - 1)
            map[x][y+1].GetComponent<Piece>().DrawBomb();

        // Solo se hace si hay casillas encima a la derecha
        if (x < width - 1 && y < height - 1)
            map[x + 1][y + 1].GetComponent<Piece>().DrawBomb();

        // Solo se hace si hay casillas a la izquierda
        if (x > 0)
            map[x-1][y].GetComponent<Piece>().DrawBomb();

        // Solo se hace si hay casillas a la derecha
        if (x < width - 1)
            map[x+1][y].GetComponent<Piece>().DrawBomb();

        // Solo se hace si hay casillas debajo a la izquierda
        if (x > 0 && y > 0)
            map[x-1][y-1].GetComponent<Piece>().DrawBomb();

        // Solo se hace si hay casillas debajo
        if (y > 0)
            map[x][y-1].GetComponent<Piece>().DrawBomb();

        // Solo se hace si hay casillas debajo a la derecha
        if (x < width - 1 && y > 0)
            map[x+1][y-1].GetComponent<Piece>().DrawBomb();
    }

    public int Validate()
    {
        int errorCode = 0;

        // Binario!
        if (width <= 0)
            errorCode += 4;

        if (height <= 0)
            errorCode += 2;

        if (!(bombsNumber > 0 && bombsNumber < width * height))
            errorCode += 1;

        return errorCode;
    }
}
