using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace KP_Biletchenko_PRI120
{
    public class Fern
    {
        #region Fields

        // Задаем диапазон значений для точек
        private const float MinX = -6;
        private const float MaxX = 6;
        private const float MinY = 0.1f;
        private const float MaxY = 10;
        // Количество точек для отрисовки
        private const int PointNumber = 10000;
        // Массив коэффциентов вероятностей
        private float[] _probability = new float[4]
        {
        0.01f,
        0.06f,
        0.08f,
        0.85f
        };

        // Матрица коэффициентов
        private float[,] _funcCoef = new float[4, 6]
        {
        //a      b       c      d      e  f
        {0, 0, 0, 0.16f, 0, 0}, // 1 функция
        {-0.15f, 0.28f, 0.26f, 0.24f, 0, 0.44f}, // 2 функция
        {0.2f, -0.26f, 0.23f, 0.22f, 0, 1.6f}, // 3 функция
        {0.85f, 0.04f, -0.04f, 0.85f, 0, 1.6f} // 4 функция
        };

        // коэффициент масштабируемости высоты и ширины
        // изображения фрактала для высоты и ширины нашей формы


        #endregion

        #region Initialization

        private int _width;
        private int _height;
        private Bitmap _fern;
        private Graphics _graph;

        // Передаем параметры размеров области отображения в конструктор
        public Fern(int width, int height)
        {
            _width = width;
            _height = height;
            _fern = new Bitmap(width, height);
            _graph = Graphics.FromImage(_fern);

            // Очищаем фон
            _graph.Clear(Color.Black);
        }

        #endregion

        #region Draw Method

        private float _scale = 10.0f; // Масштабирование фрактала
        private float _translateX = 40.0f; // Сдвиг по оси X
        private float _translateY = 0.0f; // Сдвиг по оси Y
        private float _rotationAngle = 90.0f;

        public void DrawFern()
        {
            Random rnd = new Random();
            // будем начинать рисовать с точки (0, 0)
            float xtemp = 0, ztemp = 0;
            // переменная хранения номера функции для вычисления следующей точки
            int func_num = 0;

            Gl.glPointSize(1.0f); // устанавливаем размер точек

            Gl.glBegin(Gl.GL_POINTS); // начинаем рисование точек

            for (int i = 1; i <= PointNumber; i++)
            {
                // рандомное число от 0 до 1
                var num = rnd.NextDouble();
                // проверяем какой функцией воспользуемся для вычисления следующей точки
                for (int j = 0; j <= 3; j++)
                {
                    // если рандомное число оказалось меньше или равно
                    // заданного коэффициента вероятности,
                    // задаем номер функции
                    num = num - _probability[j];
                    if (num <= 0)
                    {
                        func_num = j;
                        break;
                    }
                }

                // вычисляем координаты
                var x = _funcCoef[func_num, 0] * xtemp + _funcCoef[func_num, 1] * ztemp + _funcCoef[func_num, 4]; // Изменено на x и z
                var z = _funcCoef[func_num, 2] * xtemp + _funcCoef[func_num, 3] * ztemp + _funcCoef[func_num, 5];

                // сохраняем значения для следующей итерации
                xtemp = x;
                ztemp = z;
                // вычисляем значение пикселя
                x = xtemp;
                z = ztemp;

                x = _scale * x + _translateX;
                z = _scale * z + _translateY;

                // нормализуем координаты к интервалу [-1, 1]
                float normalizedX = 2 * (x - MinX) / (MaxX - MinX) - 1;
                float normalizedZ = 2 * (z - MinY) / (MaxY - MinY) - 1;

                Gl.glColor3f(0.0f, 1.0f, 0.0f); // устанавливаем цвет точек

                Gl.glVertex2f(normalizedX, normalizedZ); // рисуем точку
            }

            Gl.glEnd(); // завершаем рисование точек

            // Обновляем отображение
            Gl.glFlush();
        }

        #endregion
    }

}
