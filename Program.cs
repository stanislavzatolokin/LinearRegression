using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LinearRegression
{
    class Matrix
    {
        //данные
        double[,] matrix; //матрица

        //обращение
        public int N
        {
            get
            {
                return matrix.GetLength(0);
            }
        } //кол-во строк      
        public int M
        {
            get
            {
                return matrix.GetLength(1);
            }
        } //кол-во столбцов
        public double this[int n, int m]
        {
            get
            {
                return matrix[n, m];
            }
            set
            {
                matrix[n, m] = value;
            }
        } //обращение к элементам матрицы

        //конструкторы
        public Matrix(int n = 1, int m = 1)
        {
            matrix = new double[n, m];
            if (n == 1 && m == 1)
                matrix[0, 0] = 0;
        } //создание матрицы n на m(по умолчанию - число 0) 
        public Matrix(Matrix m)
        {
            matrix = new double[m.N, m.M];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    matrix[i,j] = m[i, j];
        } //копирование

        //операции
        public static Matrix operator * (Matrix m1, Matrix m2)
        {
            //если перемножение возможно
            if (m1.M == m2.N)
            {
                Matrix result = new Matrix(m1.N, m2.M);
                for (int i = 0; i < m1.N; i++)
                    for (int j = 0; j < m2.M; j++)
                    {
                        double x = 0;
                        for (int p = 0; p < m1.M; p++)
                            x += m1[i, p] * m2[p, j];
                        result[i, j] = x;
                    }                    
                return result;
            }
            //иначе - число 0
            return new Matrix();
        } //произведение матриц
        public static Matrix operator ! (Matrix m) //транспонированние матрицы
        {
            Matrix result = new Matrix(m.M, m.N);
            for (int i = 0; i < m.N; i++)
                for (int j = 0; j < m.M; j++)
                    result[j, i] = m[i, j];
            return result;
        }
        public static Matrix operator / (Matrix m, double x) //деление матрицы на число
        {
            Matrix result = new Matrix(m);
            for (int i = 0; i < m.N; i++)
                for (int j = 0; j < m.M; j++)
                    result[i, j] /= x;
            return result;
        }
        public void swapRow(int i, int j) //swap строк i, j в матрице
        {
            for (int p = 0; p < M; p++)
            {
                double buf = this[i, p];
                this[i, p] = this[j, p];
                this[j, p] = buf;
            }
        }
        public double determinant()
        { 
            //если вычислить определитель возможно
            if (N == M)
            {
                double result = 1;
                Matrix m = new Matrix(this);
                for (int i = 0; i < N; i++)
                {
                    int max = i;
                    for (int j = i + 1; j < N; j++)
                        if (m[j, i] > m[max, i])
                            max = j;
                    if (m[max,i] == 0)
                    {
                        result = 0;
                        break;
                    }
                    m.swapRow(i, max);
                    if (i != max)
                        result = -result;
                    result *= m[i, i];
                    for (int j = i + 1; j < N; j++)
                        m[i, j] /= m[i, i];
                    for (int j = 0; j < N; j++)
                        if (j != i && m[j, i] != 0)
                            for (int p = i + 1; p < N; p++)
                                m[j, p] -= m[i, p] * m[j, i];
                }
                return result;
            }
            //иначе - число 1
            return 1;
        } //определитель матрицы
        public Matrix union()
        {
            Matrix result = new Matrix(N, M);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    Matrix m = new Matrix(N - 1, N - 1);
                    int x = 0, y = 0;
                    for (int p = 0; p < N; p++)
                        for (int q = 0; q < N; q++)
                            if (p != i && q != j)
                            {
                                m[x, y] = (!this)[p, q];
                                if (++y > N - 2)
                                {
                                    x++;
                                    y = 0;
                                }
                            }
                    result[i, j] = m.determinant();
                    if ((i + j) % 2 == 1)
                        result[i, j] = -result[i, j];
                }
            return result;
        } //союзная матрица
        public Matrix inverse()
        {
            //если обращение возможно
            if (N == M)
                return union() / determinant();
            //иначе - число 0
            else
                return new Matrix();
        } //обратная матрица

        //ввод, вывод
        public void input()
        {
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    Console.Write("[" + i + ", " + j + "] = ");
                    this[i,j] = Convert.ToDouble(Console.ReadLine());
                }
        } //ввод матрицы
        public void output()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                    Console.Write("{0:0.##} ", this[i, j]);
                Console.WriteLine();
            }
        } //вывод матрицы
    } //работа с вещественными матрицами

    class LinearRegression
    {
        //данные
        Matrix F; //объекты
        Matrix Y; //ответы
        Matrix A; //решение

        //конструктор
        public LinearRegression(StreamReader file)
        {
            //считывание из файла
            int l = Convert.ToInt32(file.ReadLine()); //кол-во прецедентов
            int n = Convert.ToInt32(file.ReadLine()); //кол-во признаков
            Y = new Matrix(l, 1); //создание матриц
            F = new Matrix(l, n + 1);
            for (int i = 0; i < l; i++)
            {
                Y[i, 0] = Convert.ToDouble(file.ReadLine()); //ответ
                F[i, 0] = 1; //объект
                for (int j = 1; j <= n; j++)
                    F[i, j] = Convert.ToDouble(file.ReadLine());
            }
        }


    }

    class Program //основная программа
    {
        static double prediction(Matrix F, Matrix A, int l, int n)
        {
            double result = 0;
            for (int i = 0; i < n + 1; i++)
                result += F[l, i] * A[i, 0];
            return result;
        }

        static void Main()
        {
            //преобразование файла-выборки
            StreamReader sr = new StreamReader("input.txt");
            int ll = 1;
            string[] list = sr.ReadLine().Split('\t');
            int nn = list.Length - 4;
            for (; sr.ReadLine() != null; ll++) ;
            sr.Close();
            sr = new StreamReader("input.txt");
            StreamWriter sw = new StreamWriter("sample.txt");
            sw.WriteLine(ll);
            sw.WriteLine(nn);
            for (int i = 0; i < ll; i++)
            {
                string s = sr.ReadLine();
                s = s.Replace(".", ",");
                list = s.Split('\t', ' ');
                sw.Write(list[1] + " ");
                for (int j = 0; j < nn; j++)
                    sw.Write(list[j + 4] + " ");
                sw.WriteLine();
            }
            sr.Close();
            sw.Close();
                        
            //интерфейс
            Console.Write("Реализация метода линейной регрессии.\n\n" +
                "Формат файла-выборки:\n" +
                "Строка 1: кол-во прецедентов L.\n" +
                "Строка 2: кол-во признаков объекта N.\n" +
                "Следующие L строк: описание прецедентов.\n" +
                "Описание i-ого прецедента: ответ, значения N признаков(через пробел).\n\n" +
                "Введите имя файла, соответствующего описанному формату, находящегося в данной директории: ");
            
            //ввод данных
            string input = Console.ReadLine();
            sr = new StreamReader(input + ".txt");
            int l = Convert.ToInt32(sr.ReadLine());
            int n = Convert.ToInt32(sr.ReadLine());
            Matrix F = new Matrix(l, n + 1);
            Matrix Y = new Matrix(l, 1);
            for (int i = 0; i < l; i++)
            {
                string[] values = sr.ReadLine().Split(' ');
                Y[i, 0] = Convert.ToDouble(values[0]);
                for (int j = 0; j < n; j++)
                    F[i, j] = Convert.ToDouble(values[j + 1]);
                F[i, n] = 1;
            }
            sr.Close();
            
            //нахождение коэффициентов
            Matrix A = (!F * F).inverse() * !F * Y;
            
            //вывод коэффициентов
            sw = new StreamWriter(input + "A.txt");
            for (int i = 0; i < n + 1; i++)
                sw.WriteLine(A[i, 0]);
            sw.Close();
            
            //нахождение метрики
            double Q = 0;
            for (int i = 0; i < l; i++)
            {
                double x = prediction(F, A, i, n);
                x -= Y[i, 0];
                Q += Math.Abs(x);
            }
            Q /= l;
            double y = 0;
            for (int i = 0; i < l; i++)
                y += Math.Abs(Y[i, 0]);
            y /= l;

            //вывод метрики
            sw = new StreamWriter(input + "Q.txt");
            sw.WriteLine(Q + " средний модуль отклонения");
            sw.WriteLine(y + " средний модуль ответа");
            sw.WriteLine(Q * 100 / y + " отношение отклонения к ответу в %");
            sw.Close();
            
            //интерфейс
            Console.Write("\nУспешно.\n\n" +
                "В данной директории были созданы файлы с результатами метода обучения.\n" +
                "Для выхода нажмите <Enter>.");
            Console.ReadLine();
            
        }
    }
}
