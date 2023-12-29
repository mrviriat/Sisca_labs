namespace ClassLibrary
{
    public class BitMatrixGenerator
    {
        private int size = 10;

        private readonly int[,] matrix;

        private List<int[][]> listOfAreas = new List<int[][]>();
        private List<int[]> currentArea = new List<int[]>();
        public BitMatrixGenerator(int Size)
        {
            this.size = Size;

            matrix = new int[size, size];

            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = random.Next(2);
                }
            }
        }

        public void DisplayMatrix()
        {
            Console.WriteLine($"Битовая матрица {size}x{size}:");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        private int CompareArrays(int[] a, int[] b)
        {
            int compareResult = a[0].CompareTo(b[0]); // Сравнение по первым числам

            if (compareResult == 0)
            {
                // Если первые числа равны, сравниваем по вторым числам
                return a[1].CompareTo(b[1]);
            }

            return compareResult;
        }

        public void FindFreeAreas()
        {
            int areaNumber = 1;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        MarkAreaAsVisited(i, j, areaNumber);

                        int[][] array = currentArea.ToArray();
                        Array.Sort(array, CompareArrays);

                        listOfAreas.Add(array);
                        currentArea.Clear();
                        areaNumber++;
                    }
                }
            }

            Console.WriteLine();
        }

        private void MarkAreaAsVisited(int row, int col, int areaNumber)
        {
            if (row < 0 || row >= size || col < 0 || col >= size || matrix[row, col] != 0)
            {
                return;
            }


            matrix[row, col] = areaNumber;
            currentArea.Add(new int[] { row, col });

            MarkAreaAsVisited(row - 1, col, areaNumber); // Вверх
            MarkAreaAsVisited(row + 1, col, areaNumber); // Вниз
            MarkAreaAsVisited(row, col - 1, areaNumber); // Влево
            MarkAreaAsVisited(row, col + 1, areaNumber); // Вправо
        }

        public void ShowAreas()
        {
            foreach (var Array in listOfAreas)
            {
                string originalString = "Свободный участок: ";
                originalString += "{";
                foreach (var array in Array)
                {
                    originalString += "[" + array[0] + ", " + array[1] + "], ";
                }
                originalString = originalString.Substring(0, originalString.Length - 2);
                originalString += "}";
                Console.WriteLine(originalString);
                Console.WriteLine(new string('=', originalString.Length));
            }
        }


    }
}