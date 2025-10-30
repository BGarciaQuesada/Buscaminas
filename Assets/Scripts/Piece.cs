using TMPro;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // Solo para almacenar variables que pertenecen al objeto pieza que, de entrada, no tiene.
    [SerializeField] private int x, y;
    [SerializeField] private bool bomb, check, flag;
    [SerializeField] private GameObject bombPrefab, flagPrefab, flagInstance;

    public void setX(int x){
        this.x = x; 
    }

    public void setY(int y){
        this.y = y;
    }

    public void setBomb(bool bomb){
        this.bomb = bomb;
    }
    public void setCheck(bool check){
        this.check = check;
    }

    public int getX(){
        return x;
    }

    public int getY(){
        return y;
    }

    public bool isBomb(){
        return bomb;
    }

    public bool isCheck(){
        return check;
    }

    public void DrawBomb()
    {
        if (!isCheck())
        {
            setCheck(true);

            if (isBomb())
            {
                GetComponent<SpriteRenderer>().material.color = Color.red;

                GameManager.instance.endMenu.SetActive(true);
                GameManager.instance.endgame = true;
                GameManager.instance.ShowDefeat();

                // Añadir Sprite de bomba
                if (bombPrefab != null)
                {
                    // Crear bomba en la posición de la casilla
                    var obj = Instantiate(bombPrefab, transform.position, Quaternion.identity);
                    // Si lo convertimos en hijo de la casilla, al borrar el mapa se borra también la bomba
                    obj.transform.SetParent(this.transform);
                }
            } else {
                // Cambiar color casilla porque ya está comprobada
                GetComponent<SpriteRenderer>().material.color = Color.gray;

                int bombsNumber = Generator.gen.GetBombsAround(x, y);

                // Avisar al GameManager que se ha revelado una pieza segura
                GameManager.instance.AddRevealedPiece();

                if (bombsNumber != 0){
                    transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = bombsNumber.ToString();
                } else {
                    Generator.gen.CheckPieceAround(x, y);
                }
            }
        }   
    }

    private void OnMouseDown()
    {
        if (!GameManager.instance.endgame && !flag)
        {
            DrawBomb();
        }
    }

    // Si el ratón está encima de la casilla y se pulsa el botón derecho, dibujar marcador
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // 1 es el botón derecho
        {
            if (!GameManager.instance.endgame)
            {
                DrawFlag();
            }
        }
    }

    // Dibujar banderita con el botón secundario
    public void DrawFlag()
    {
        if (!flag)
        {
            // Si el MODELO de la banderita está asignado en el inspector, instanciarlo
            if (flagPrefab != null)
            {
                // Instanciar banderita en la posición de la casilla
                flagInstance = Instantiate(flagPrefab, transform.position, Quaternion.identity, transform);
                flag = true;
            }
        }
        else
        {
            // [!!!] Es por esto que guardamos la instancia, para poder destruirla
            // Si ya hay banderita, destruirla
            if (flagInstance != null)
            {
                Destroy(flagInstance);
            }
            flag = false;
        }
    }
}

