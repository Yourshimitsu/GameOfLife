using UnityEngine;

public enum CellState
{
    White,
    Red,
    Blue
}

public class Cell
{
    public CellState state;
    public CellState nextState;

    public Cell(CellState initial)
    {
        state = initial;
        nextState = initial;
    }

    public bool IsAlive() => state == CellState.Red || state == CellState.Blue;
}


public class CellGridPainterAuto : MonoBehaviour
{
    public int cellSize = 10;
    public Color whiteColor = Color.white;
    public Color redColor = Color.red;
    public Color blueColor = Color.blue;
    public float updateRate = 0.2f; 

    private Texture2D texture;
    private Rect screenRect;
    private int cols, rows;
    private Cell[,] cells;

    private float timer = 0f;
    private bool isPaused = false;
    
    private int generatedRedCells = 0;
    private int generatedBlueCells = 0;
    
    private int drawnRedCells = 0;
    private int drawnBlueCells = 0;
    private int maxDrawnCells = 100;
    
    private int lastRedCount = 0;
    private int lastBlueCount = 0;
    
    private bool gameEnded = false;
    private string gameResult = "";

    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        cols = Screen.width / cellSize;
        rows = Screen.height / cellSize;

        cells = new Cell[cols, rows];

        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
                cells[x, y] = new Cell(CellState.White);

        generatedRedCells = 0;
        generatedBlueCells = 0;
        drawnRedCells = 0;
        drawnBlueCells = 0;
        lastRedCount = 0;
        lastBlueCount = 0;
        isPaused = false;
        gameEnded = false;
        gameResult = "";

        FillTexture();
    }

    void Update()
    {
        if (gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            return;
        }

        HandleMouseInput();
        HandlePause();

        if (!isPaused)
        {
            timer += Time.deltaTime;
            if (timer >= updateRate)
            {
                timer = 0f;
                UpdateCells();
                FillTexture();
                CheckGameEnd();
            }
        }
    }



    void HandleMouseInput()
    {
        Vector2 mousePos = Input.mousePosition;
        
        int x = Mathf.FloorToInt(mousePos.x / cellSize);
        int y = Mathf.FloorToInt((mousePos.y) / cellSize);

        if (x < 0 || x >= cols || y < 0 || y >= rows) return;

        bool needsUpdate = false;

        if (Input.GetMouseButton(0) && drawnRedCells < maxDrawnCells) 
        {
            if (cells[x, y].state == CellState.White)
            {
                drawnRedCells++;
            }
            cells[x, y].state = CellState.Red;
            needsUpdate = true;
        }

        if (Input.GetMouseButton(1) && drawnBlueCells < maxDrawnCells) 
        {
            if (cells[x, y].state == CellState.White)
            {
                drawnBlueCells++;
            }
            cells[x, y].state = CellState.Blue;
            needsUpdate = true;
        }

        if (needsUpdate)
        {
            FillTexture();
        }
    }

    void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            updateRate = 1.0f / 1.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            updateRate = 1.0f / 2.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            updateRate = 1.0f / 3.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            updateRate = 1.0f / 4.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            updateRate = 1.0f / 5.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            updateRate = 1.0f / 6.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            updateRate = 1.0f / 7.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            updateRate = 1.0f / 8.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            updateRate = 1.0f / 9.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            updateRate = 1.0f / 10.0f;
        }
    }

    void CheckGameEnd()
    {
        bool reachedLimit = generatedRedCells >= 1000 || generatedBlueCells >= 1000;
        bool noDrawnCellsLeft = drawnRedCells >= maxDrawnCells && drawnBlueCells >= maxDrawnCells;
        bool noProgressMade = generatedRedCells == lastRedCount && generatedBlueCells == lastBlueCount;
        
        if (reachedLimit || (noDrawnCellsLeft && noProgressMade))
        {
            gameEnded = true;
            
            if (generatedRedCells > generatedBlueCells)
            {
                gameResult = "RED WINS!";
            }
            else if (generatedBlueCells > generatedRedCells)
            {
                gameResult = "BLUE WINS!";
            }
            else
            {
                gameResult = "DRAW!";
            }
        }
        
        lastRedCount = generatedRedCells;
        lastBlueCount = generatedBlueCells;
    }

    void RestartGame()
    {
        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
                cells[x, y] = new Cell(CellState.White);

        generatedRedCells = 0;
        generatedBlueCells = 0;
        drawnRedCells = 0;
        drawnBlueCells = 0;
        lastRedCount = 0;
        lastBlueCount = 0;
        isPaused = false;
        gameEnded = false;
        gameResult = "";
        timer = 0f;

        FillTexture();
    }


    void UpdateCells()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int redCount = 0;
                int blueCount = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                        {
                            if (cells[nx, ny].state == CellState.Red) redCount++;
                            else if (cells[nx, ny].state == CellState.Blue) blueCount++;
                        }
                    }
                }

                int liveNeighbors = redCount + blueCount;

                if (liveNeighbors == 3)
                {
                    cells[x, y].nextState = (redCount >= blueCount) ? CellState.Red : CellState.Blue;
                }
                else if (liveNeighbors == 2)
                {
                    cells[x, y].nextState = cells[x, y].IsAlive() ? cells[x, y].state : CellState.White;
                }
                else
                {
                    cells[x, y].nextState = CellState.White;
                }
            }
        }

        
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (cells[x, y].state == CellState.White && cells[x, y].nextState != CellState.White)
                {
                    if (cells[x, y].nextState == CellState.Red)
                    {
                        generatedRedCells++;
                    }
                    else if (cells[x, y].nextState == CellState.Blue)
                    {
                        generatedBlueCells++;
                    }
                }
                
                cells[x, y].state = cells[x, y].nextState;
            }
        }
    }

    void FillTexture()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Color color = whiteColor;
                if (cells[x, y].state == CellState.Red) color = redColor;
                else if (cells[x, y].state == CellState.Blue) color = blueColor;

                for (int i = 0; i < cellSize; i++)
                    for (int j = 0; j < cellSize; j++)
                    {
                        int px = x * cellSize + i;
                        int py = y * cellSize + j;
                        if (px < texture.width && py < texture.height)
                            texture.SetPixel(px, py, color);
                    }
            }
        }
        texture.Apply();
    }

    void OnGUI()
    {
        GUI.DrawTexture(screenRect, texture);

        GUI.color = Color.red;
        GUI.Label(new Rect(10, 10, 200, 30), "Red Generated: " + generatedRedCells);
        GUI.Label(new Rect(10, 35, 200, 25), "Red Left: " + (maxDrawnCells - drawnRedCells));
        
        GUI.color = Color.blue;
        GUI.Label(new Rect(Screen.width - 210, 10, 200, 30), "Blue Generated: " + generatedBlueCells);
        GUI.Label(new Rect(Screen.width - 210, 35, 200, 25), "Blue Left: " + (maxDrawnCells - drawnBlueCells));
        
        if (gameEnded)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 60, 300, 120), "");

            GUI.color = Color.yellow;
            GUIStyle bigStyle = new GUIStyle(GUI.skin.label);
            bigStyle.fontSize = 24;
            bigStyle.fontStyle = FontStyle.Bold;
            bigStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 40, 300, 40), gameResult, bigStyle);

            GUI.color = Color.black;
            GUIStyle smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.fontSize = 16;
            smallStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2, 300, 30), "Press R to restart", smallStyle);
            
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 25, 300, 30), "Red: " + generatedRedCells + " Blue: " + generatedBlueCells, smallStyle);
        }
        else
        {
            GUI.color = isPaused ? Color.yellow : Color.green;
            GUI.Label(new Rect(10, 65, 200, 25), isPaused ? "PAUSED (Space/P)" : "RUNNING");
            
            GUI.color = Color.cyan;
            float currentSpeed = 1.0f / updateRate;
            GUI.Label(new Rect(10, 90, 200, 25), "Speed: " + currentSpeed.ToString("F0") + "/sec (1-9,0)");
        }
        
        GUI.color = Color.white;
    }
}
