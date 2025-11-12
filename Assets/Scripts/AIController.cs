using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // Declaración de variables (necesitarás algunas más)
    public float turnTime = 0.5f;
    GameObject[][] map;

    // El Bot comienza a Jugar. Este Código no hay que cambiarlo
    public void StartBot()
    {
        Debug.Log("Bot lanzado.");
        StopAllCoroutines(); // por si acaso
        map = Generator.gen.GetMap();
        StartCoroutine(Play());
    }

    System.Collections.IEnumerator Play()
    {
        while (!GameManager.instance.endgame)
        {
            bool actionDone = LogicPlay();
            if (!actionDone)
            {
                // Si no hay lógica aplicable, jugar aleatoriamente
                RandomPlay();
            }

            yield return new WaitForSeconds(turnTime);
        }
    }


    // Lógica general del bot

    bool LogicPlay()
    {
        // por si el bot empieza antes de que exista el tablero (no debería pasar ahora mismo)
        if (map == null)
            return false;

        bool action = false;

        // Recorremos todo el mapa...
        for (int x = 0; x < map.Length; x++)
        {
            for (int y = 0; y < map[x].Length; y++)
            {
                // Obtenemos la pieza de las coordenadas actuales
                Piece centerPiece = map[x][y].GetComponent<Piece>();
                // ¿Está comprobada? Vamos a por sus vecinos
                if (centerPiece.isCheck())
                {
                    // Obtenemos vecinos
                    List<Piece> neighbors = GetNeighbors(x, y);

                    // Obtenemos el número de bombas alrededor (solo devuelve un int NO DONDE ESTÁN)
                    int bombsAround = Generator.gen.GetBombsAround(x, y);

                    // Contamos cuántas están ocultas (ni check ni bandera) y cuántas tienen bandera
                    int flaggedCount = neighbors.Count(p => p.isFlag());
                    List<Piece> hiddenNeighbors = neighbors.Where(p => !p.isCheck() && !p.isFlag()).ToList();

                    // Debug.Log($"({x},{y}) ocultas: {hidden.Count}, banderas: {flagged}");

                    // YA TENEMOS LA INFORMACIÓN, APLICAMOS LAS REGLAS:
                    // --- Regla 1: todas ocultas son minas: click_derecho (Flag) ---
                    // si bombas - banderas = ocultas -> todas son minas
                    if (bombsAround - flaggedCount == hiddenNeighbors.Count && hiddenNeighbors.Count > 0)
                    {
                        foreach (Piece hidden in hiddenNeighbors)
                            hidden.DrawFlag();

                        action = true; // ya hicimos una acción
                    }
                    // --- Regla 2: todas ocultas son seguras: clic_izquierdo (Flag) ---
                    // si bombasAround = banderas -> ocultas son seguras
                    if (bombsAround == flaggedCount && hiddenNeighbors.Count > 0)
                    {
                        foreach (Piece hidden in hiddenNeighbors)
                            hidden.DrawBomb();

                        action = true; // ya hicimos una acción
                    }
                }
            }
        }

        return action;
    }

    List<Piece> GetNeighbors(int x, int y)
    {
        List<Piece> neighbors = new List<Piece>();

        int width, height;
        Generator.gen.GetWidth(out width);
        Generator.gen.GetHeight(out height);

        // CHULETA:
        // (-1, +1) | (0, +1) | (+1, +1)
        // (-1,  0) |    X    | (+1,  0)
        // (-1, -1) | (0, -1) | (+1, -1)

        // Irá de izquierda a derecha (-1, 0, 1)
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            // Y de arriba a abajo (-1, 0, 1)
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (offsetX == 0 && offsetY == 0) continue; // saltar la propia celda

                // Mi posición + offset
                int neighborX = x + offsetX;
                int neighborY = y + offsetY;

                // validar límites
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    Piece neighborPiece = Generator.gen.GetMap()[neighborX][neighborY].GetComponent<Piece>();
                    neighbors.Add(neighborPiece);
                }
            }
        }
        return neighbors;
    }


    void RandomPlay()
    {
        // De todas las celdas que no se han chequeado, click_izquierdo en una de forma aleatoria;

        if (map == null) return; // evita crasheo si no existe mapa

        List<Piece> candidates = new List<Piece>();

        for (int x = 0; x < map.Length; x++)
        {
            for (int y = 0; y < map[x].Length; y++)
            {
                Piece piece = map[x][y].GetComponent<Piece>();

                if (!piece.isCheck() && !piece.isFlag())
                    candidates.Add(piece);
            }
        }

        if (candidates.Count == 0)
            return;

        int randomIndex = Random.Range(0, candidates.Count);
        candidates[randomIndex].DrawBomb();
    }
}

