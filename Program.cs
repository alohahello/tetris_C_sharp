// This line allows us to use basic functionality like numbers, text, and exceptions.
using System;
// Used here to manage timed events like moving the Tetris blocks down automatically.
using System.Threading;

namespace Tetris
{
    class Program
    {
        // Game variables
        const int mapSizeX = 10; // Width of the game board
        const int mapSizeY = 20; // Height of the game board
        static char[,] bg = new char[mapSizeY, mapSizeX]; // Background grid to represent the game board
        static char currentChar; // Character representing the current falling block
        static int currentX, currentY; // Current position of the falling block
        static bool[,] currentShape; // Current shape of the falling block (as a boolean grid)
        static bool gameRunning = true; // Flag to control the game loop
        static int speed = 500; // Speed of the falling blocks (milliseconds)

        static void Main()
        {
            Console.CursorVisible = false; // Hide the console cursor
            InitGame(); // Initialize the game
            while (gameRunning) // Main game loop
            {
                Update(); // Update game state
                Draw(); // Draw game board
                Thread.Sleep(speed); // Control the speed of the falling blocks
            }
        }

        static void InitGame()
        {
            // Initialize game variables
            currentX = mapSizeX / 2; // Start position of the falling block (centered horizontally)
            currentY = 0; // Start position of the falling block (top of the game board)
            currentChar = 'O'; // Default character for the falling blocks
            currentShape = new bool[,] { { true, true }, { true, true } }; // Default shape of the falling blocks (2x2 square)
            for (int y = 0; y < mapSizeY; y++) // Initialize the background grid with empty spaces
            {
                for (int x = 0; x < mapSizeX; x++)
                {
                    bg[y, x] = '-';
                }
            }
        }

        static void Update()
        {
            // Game logic (e.g., moving blocks down)
            if (!Collision(currentX, currentY + 1)) // Check if the block can move down
            {
                currentY++; // Move the block down
            }
            else // If the block cannot move down
            {
                LockShape(); // Lock the block in place on the game board
                gameRunning = !GameOver(); // Check for game over condition
                if (gameRunning) // If the game is still running
                {
                    // Generate a new random shape and position
                    currentX = mapSizeX / 2; // Start position of the new block (centered horizontally)
                    currentY = 0; // Start position of the new block (top of the game board)
                    currentShape = new bool[,] { { true, true }, { true, true } }; // Placeholder, you can replace this with your own shape generation logic
                }
            }

            // Handle keyboard input
            if (Console.KeyAvailable) // Check if a key is pressed
            {
                ConsoleKeyInfo key = Console.ReadKey(true); // Get the pressed key
                switch (key.Key) // Handle arrow key input
                {
                    case ConsoleKey.LeftArrow:
                        if (!Collision(currentX - 1, currentY)) // Check if the block can move left
                            currentX--; // Move the block left
                        break;
                    case ConsoleKey.RightArrow:
                        if (!Collision(currentX + 1, currentY)) // Check if the block can move right
                            currentX++; // Move the block right
                        break;
                    case ConsoleKey.DownArrow:
                        while (!Collision(currentX, currentY + 1)) // Move the block down until it collides with something
                            currentY++;
                        break;
                }
            }
        }

        static void Draw()
        {
            // Draw game board
            Console.Clear(); // Clear the console
            for (int y = 0; y < mapSizeY; y++) // Loop through each row of the game board
            {
                for (int x = 0; x < mapSizeX; x++) // Loop through each column of the game board
                {
                    if (x >= currentX && x < currentX + currentShape.GetLength(1) && y >= currentY && y < currentY + currentShape.GetLength(0)) // Check if the current position is within the bounds of the falling block
                    {
                        if (currentShape[y - currentY, x - currentX]) // Check if the current position corresponds to a filled cell in the falling block shape
                        {
                            Console.Write(currentChar); // Print the falling block character
                        }
                        else
                        {
                            Console.Write(bg[y, x]); // Print the background character
                        }
                    }
                    else // If the current position is outside the falling block
                    {
                        Console.Write(bg[y, x]); // Print the background character
                    }
                }
                Console.WriteLine(); // Move to the next row
            }
        }

        static bool Collision(int x, int y)
        {
            // Check collision with game board boundaries or other blocks
            if (x < 0 || x + currentShape.GetLength(1) > mapSizeX || y + currentShape.GetLength(0) > mapSizeY)
            {
                return true; // Collision detected
            }

            for (int i = 0; i < currentShape.GetLength(0); i++) // Loop through each row of the falling block
            {
                for (int j = 0; j < currentShape.GetLength(1); j++) // Loop through each column of the falling block
                {
                    if (currentShape[i, j] && bg[y + i, x + j] != '-') // Check if the current position in the falling block is filled and the corresponding position on the game board is not empty
                    {
                        return true; // Collision detected
                    }
                }
            }
            return false; // No collision detected
        }

        static void LockShape()
        {
            // Place the current shape onto the game board
            for (int i = 0; i < currentShape.GetLength(0); i++) // Loop through each row of the falling block
            {
                for (int j = 0; j < currentShape.GetLength(1); j++) // Loop through each column of the falling block
                {
                    if (currentShape[i, j]) // Check if the current position in the falling block is filled
                    {
                        bg[currentY + i, currentX + j] = currentChar; // Place the falling block on the game board
                    }
                }
            }
        }

        static bool GameOver()
        {
            // Check if the game is over (if the new shape cannot be placed at the top)
            for (int i = 0; i < currentShape.GetLength(0); i++) // Loop through each row of the falling block
            {
                for (int j = 0; j < currentShape.GetLength(1); j++) // Loop through each column of the falling block
                {
                    if (currentShape[i, j] && bg[i, j + currentX] != '-') // Check if the current position in the falling block is filled and the corresponding position on the game board is not empty
                    {
                        return true; // Game over condition met
                    }
                }
            }
            return false; // Game continues
        }
    }
}
