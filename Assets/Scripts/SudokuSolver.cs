
public class SudokuSolver
{
    static bool isSafe(int[,] mat, int row, int col, int num)
    {
        // Check if num exists in the row
        for (int x = 0; x < 9; x++)
        {
            if (mat[row, x] == num)
            {
                return false;                
            }
        }


        // Check if num exists in the col
        for (int x = 0; x < 9; x++)
        {
            if (mat[x, col] == num)
            {
                return false;
            }
        }
        
        // Check if num exists in the 3x3 sub-grid
        //get start row and col of subgrid with given row col
        int startRow = (row/3)*3, startCol = (col/3)*3;
        //int startRow = row - (row % 3), startCol = col - (col % 3); this also works
        
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            if (mat[i + startRow, j + startCol] == num)
            {
                return false;
            }
        }

        return true;
    }   
    // Function to solve the Sudoku problem
    static bool solveSudokuRec(int[,] mat, int row, int col) {
      
        // base case: Reached nth column of the last row
        if (row == 8 && col == 9)
            return true;

        // If last column of the row go to the next row
        if (col == 9) 
        {
            row++;
            col = 0;
        }

        // If cell is already occupied then move forward
        if (mat[row, col] != 0)
            return solveSudokuRec(mat, row, col + 1);

        for (int num = 1; num <= 9; num++) 
        {
            // If it is safe to place num at current position
            if (isSafe(mat, row, col, num)) 
            {
                mat[row, col] = num;
                if (solveSudokuRec(mat, row, col + 1))
                {
                    return true;

                }
                mat[row, col] = 0;
            }
        }

        return false;
    }
}
