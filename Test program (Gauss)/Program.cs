using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test_program__Gauss_
{
    public class GaussMatrix
    {
        private int size;
        private float[,] matrix { get; set; }
        private float[,] originalMatrix { get; set; }
        private float[] vector { get; set; }
        private float[] originalVector { get; set; }
        private float[] answers { get; set; }
        private float[] answersSequence { get; set; }
        private float[] error { get; set; }


        public GaussMatrix(int tempSize)
        {
            size = tempSize;
            matrix = new float[size, size];
            vector = new float[size];
            originalMatrix = new float[size, size];
            originalVector = new float[size];
            answers = new float[size]; // Вектор відповідей
            answersSequence = new float[size];
            error = new float[size];
        }

        public void EnterMatrix()
        {
            Console.WriteLine("How do you want to enter values?\nFrom console - type \"C\" (english keyboard should be set)\nFrom file - type \"F\"\n");
            string selection = null;
            while (selection != "c" && selection != "f")
            {
                selection = Console.ReadLine();
                selection.ToLower();
            }
            if (selection == "c")
            {
                Console.WriteLine("\nEnter matrix of coefficients");
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Console.Write("[{0},{1}] = ", i + 1, j + 1);
                        matrix[i, j] = float.Parse(Console.ReadLine());
                    }
                }

                Console.WriteLine("\nEnter vector of answers");

                for (int i = 0; i < size; i++)
                {
                    Console.Write("[{0}] = ", i + 1);
                    vector[i] = float.Parse(Console.ReadLine());
                }
            }

            else
            {
                Console.WriteLine("Main matrix and vector of answers should be in separate files. One value in one line.");
                Console.WriteLine("Values are read from left to right, filling up matrix by lines: [1,1], [1,2], [1,3]...");
                Console.WriteLine("Enter full path to your file with main matrix in format \"D:\\\\...\\%filename%.txt\"");
                string pathToMatrix = Console.ReadLine();
                FileStream matrixFile = new FileStream(pathToMatrix, FileMode.Open);
                StreamReader readMatrix = new StreamReader(matrixFile);
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        matrix[j, i] = float.Parse(readMatrix.ReadLine());
                    }
                }

                Console.WriteLine("Enter full path to your file with vector of answers in format \"D:\\\\...\\%filename%.txt\"");
                string pathToVector = Console.ReadLine();
                FileStream vectorFile = new FileStream(pathToVector, FileMode.Open);
                StreamReader readVector = new StreamReader(vectorFile);
                for (int i = 0; i < size; i++)
                {
                    vector[i] = float.Parse(readVector.ReadLine());
                }

            }

            for (int i = 0; i < size; i++) // Створюємо копії початкових матриць
            {
                for (int j = 0; j < size; j++)
                    originalMatrix[i, j] = matrix[i, j];
                originalVector[i] = vector[i];
                error[i] = vector[i]; //Перенос вільних членів у вектор нев'язки (для бчислення похибки)
            }

            for (int i = 0; i < size; i++) // Масив з індексами коренів. Масив відповідей буде відсортований відповідно до цієї послідовності
                answersSequence[i] = i;
        }

        public void PrintData()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write("{0:0.###} ", matrix[i, j]);
                }
                Console.Write(" = {0:0.###}", vector[i]);
                Console.WriteLine();
            }
        }

        public void MainGaussMethod()
        {

            GaussForward();
            GaussBackwards();
            Console.WriteLine("\nANSWERS:");
            foreach (var x in answers)
                Console.WriteLine(x);
            GaussError();
        }

        public void GaussForward()
        {
            float mainElement = 0; // Ведучий елемент. mainIndex - індекси ведучого елементу
            for (int mainIndex = 0; mainIndex < size; mainIndex++) // Прямий хід
            {
                Console.WriteLine("\nMAIN ELEMENT [{0},{0}] = {1}\n", mainIndex, mainElement);
                Console.WriteLine("\nBEFORE SWAP\n");
                PrintData();
                
                SwapColumns(mainIndex); // Вибір головного елементу по стрічці
                Console.WriteLine("\nAFTER SWAP\n");
                PrintData();

                mainElement = matrix[mainIndex, mainIndex];

                for (int j = 0; j < size; j++) // Ділимо рядок на головний елемент, щоб головний елемент був == 1
                {
                    matrix[mainIndex, j] = matrix[mainIndex, j] / mainElement;
                }
                vector[mainIndex] = vector[mainIndex] / mainElement;

                for (int i = mainIndex + 1; i < size; i++) // Домножаємо попередній рядок на перший коефіцієнт головного рядка. Робимо нулі
                {
                    float lowMainElement = matrix[i, mainIndex]; // Перший елемент наступного рядка, який перетворюємо в 0
                    for (int j = 0; j < size; j++) // Цикл по стовпцям, множить попередній рядок на lowMainElement і віднімає від поточного рядка
                    {
                        Console.WriteLine("matrix[{0}, {1}]{4} = matrix[{0}, {1}]{4} - {2} * matrix[{3}, {1}]{5};", i, j, lowMainElement, mainIndex, matrix[i, j], matrix[mainIndex, j]);
                        matrix[i, j] = matrix[i, j] - lowMainElement * matrix[mainIndex, j];
                    }

                    vector[i] = vector[i] - lowMainElement * vector[mainIndex];
                }
            }
        }

        public void GaussBackwards()
        {
            answers[size - 1] = vector[size - 1] / matrix[size - 1, size - 1]; // Остання невідома х в результаті перетворень

            for (int i = size - 2; i >= 0; i--) // Зворотній хід
            {
                for (int j = size - 1; j > i; j--)
                {
                    vector[i] = vector[i] - matrix[i, j] * answers[j]; // від вектора вільних членів послідовно віднімаємо кожен коефіцієнт, множений на відповідний йому х
                    Console.WriteLine();
                    PrintData();
                }
                answers[i] = (float)Math.Round(vector[i], 2);
            }

            float temp = 0, temp2 = 0, temp3 = 0;

            for (int write = 0; write < size; write++) // Сортування бульбашкою вектора відповідей у відповідності до переставлених стовпчиків
            {
                for (int sort = 0; sort < size - 1; sort++) 
                {
                    if (answersSequence[sort] > answersSequence[sort + 1]) 
                    {
                        temp = answersSequence[sort + 1]; // СОртуються елементи масиву, що містить послідовність коренів
                        answersSequence[sort + 1] = answersSequence[sort];
                        answersSequence[sort] = temp;

                        temp2 = answers[sort + 1]; // Сортується масив відповідей
                        answers[sort + 1] = answers[sort];
                        answers[sort] = temp2;

                        temp3 = error[sort + 1]; // Сортується масив відповідей
                        error[sort + 1] = error[sort];
                        error[sort] = temp3;
                    }
                }
            }

            Console.WriteLine();
            PrintData();
        }

        public void GaussError()
        {
            for (int i = 0; i < size; i++) // Обчислення нев'язки для першого кроку системи
                for (int j = 0; j < size; j++)
                {
                    error[i] = error[i] - originalMatrix[i, j] * answers[j];
                }

            Console.WriteLine("\nERRORS:");
            foreach (var x in error)
                Console.WriteLine(x);
        }

        public void SwapColumns(int mainIndex)
        {
            float maxElement = matrix[mainIndex, mainIndex];
            int indexWithMaxElement = mainIndex;

            for (int j = mainIndex; j < size; j++) // Пошук найбільшого елемента в рядку
            {
                if (Math.Abs(matrix[mainIndex, j]) > Math.Abs(maxElement))
                {
                    maxElement = matrix[mainIndex, j];
                    indexWithMaxElement = j;
                }
            }

            Console.WriteLine("The biggest element in line {0} is {1}, column {2}", mainIndex, maxElement, indexWithMaxElement);

            if (maxElement != matrix[mainIndex, mainIndex])
            {
                for (int i = 0; i < size; i++) // Заміна стовпців
                {
                    float tempForMatrix = matrix[i, mainIndex];
                    matrix[i, mainIndex] = matrix[i, indexWithMaxElement];
                    matrix[i, indexWithMaxElement] = tempForMatrix;
                }
                
                Console.WriteLine("\nANSWERS\n");
                foreach (var x in answersSequence)
                    Console.Write("{0} ",x);
                Console.WriteLine();
                float tempForAnswers = answersSequence[mainIndex];
                answersSequence[mainIndex] = answersSequence[indexWithMaxElement];
                answersSequence[indexWithMaxElement] = tempForAnswers;
                foreach (var x in answersSequence)
                    Console.Write("{0} ",x);

                Console.WriteLine("\nERRORS\n");
                foreach (var x in error)
                    Console.Write("{0} ",x);
                Console.WriteLine();
                float tempForErrors = error[mainIndex];
                error[mainIndex] = error[indexWithMaxElement];
                error[indexWithMaxElement] = tempForErrors;
                foreach (var x in error)
                    Console.Write("{0} ",x);
                

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int tempSize = 0;
            Console.WriteLine("Enter size of matrix");
            tempSize = int.Parse(Console.ReadLine());
            GaussMatrix GaussObject = new GaussMatrix(tempSize);
            GaussObject.EnterMatrix();
            Console.WriteLine("\nYou entered:");
            GaussObject.PrintData();
            Console.WriteLine();
            GaussObject.MainGaussMethod();
            Console.Read();
        }
    }
}
