using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using Tao.OpenGl;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace KP_Biletchenko_PRI120
{
    class Modeling
    {
        float deltaColor = 0;
        public float goods_move = 0;

        public bool moving = false;

        private void setColor(float R, float G, float B)
        {
            RGB color = new RGB(R - deltaColor, G - deltaColor, B - deltaColor);
            Gl.glColor3f(color.getR(), color.getG(), color.getB());
        }


        void glutSolidSphereWithTexture(double radius, int slices, int stacks)
        {
            Gl.glBegin(Gl.GL_QUAD_STRIP);
            for (int i = 0; i <= stacks; ++i)
            {
                double lat0 = Math.PI * (-0.5 + (double)(i - 1) / stacks);
                double z0 = radius * Math.Sin(lat0);
                double zr0 = radius * Math.Cos(lat0);

                double lat1 = Math.PI * (-0.5 + (double)i / stacks);
                double z1 = radius * Math.Sin(lat1);
                double zr1 = radius * Math.Cos(lat1);

                for (int j = 0; j <= slices; ++j)
                {
                    double lng = 2 * Math.PI * (double)(j - 1) / slices;
                    double x = Math.Cos(lng);
                    double y = Math.Sin(lng);

                    Gl.glNormal3d(x * zr0, y * zr0, z0);
                    Gl.glTexCoord2d((double)(j - 1) / slices, (double)(i - 1) / stacks);
                    Gl.glVertex3d(x * zr0, y * zr0, z0);

                    Gl.glNormal3d(x * zr1, y * zr1, z1);
                    Gl.glTexCoord2d((double)(j - 1) / slices, (double)i / stacks);
                    Gl.glVertex3d(x * zr1, y * zr1, z1);
                }
            }
            Gl.glEnd();
        }
        public class Vector3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public Vector3(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        List<Vector3> snowmanSpheresPositions = new List<Vector3>();

        public float snowmanX = 0;
        public float snowmanY = 0;
        public float snowmanZ = 0;

        public float maxX = 90;
        public float moveSpeed = 10;
        public float maxRotationAngle = 90;

        public float rotationAngle = 0; // Угол поворота снеговика
        public float rotationSpeed = 1; // Скорость поворота снеговика
        public bool rotating = false;

        public void drawSnowman(uint body, float snowmanX, float snowmanY, float snowmanZ, float rotationAngle)
        {
            Gl.glPushMatrix();

            // Поднимаем снеговика
            Gl.glTranslated(snowmanX, snowmanY, snowmanZ);

            // Поворачиваем на 90 градусов относительно Y-оси (вправо)
            Gl.glRotatef(90, 1, 0, 0);

            // Добавляем немного поворота вверх
            Gl.glRotatef(-10, 1, 0, 0);

            // Нижний шар тела (большой)
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, body);
            Gl.glPushMatrix();
            Gl.glScaled(1.5, 1.5, 1.5); // Увеличиваем размер шара
            glutSolidSphereWithTexture(30, 20, 20); // Используем функцию с текстурными координатами
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Средний шар тела (средний)
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, body);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 60, 0); // Поднимаем на высоту нижнего шара тела
            Gl.glScaled(1.2, 1.2, 1.2); // Увеличиваем размер шара
            glutSolidSphereWithTexture(25, 20, 20); // Используем функцию с текстурными координатами
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Верхний шар тела (маленький)
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, body);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 105, 0); // Поднимаем на высоту среднего шара тела
            Gl.glScaled(1, 1, 1); // Размер по умолчанию
            glutSolidSphereWithTexture(20, 20, 20); // Используем функцию с текстурными координатами
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Первая рука (правая)
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, body);
            Gl.glPushMatrix();
            Gl.glTranslated(30, 65, 0); // Рядом с верхним шаром тела
            Gl.glScaled(0.6, 0.6, 0.6); // Уменьшаем размер шара
            glutSolidSphereWithTexture(8, 10, 10); // Используем функцию с текстурными координатами
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Вторая рука (левая)
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, body);
            Gl.glPushMatrix();
            Gl.glTranslated(-30, 65, 0); // Рядом с верхним шаром тела
            Gl.glScaled(0.6, 0.6, 0.6); // Уменьшаем размер шара
            glutSolidSphereWithTexture(8, 10, 10); // Используем функцию с текстурными координатами
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Первая нога (правая)
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, body);
            Gl.glPushMatrix();
            Gl.glTranslated(15, 10, 0); // Рядом с нижним шаром тела
            Gl.glScaled(0.8, 0.8, 0.8); // Уменьшаем размер шара
            glutSolidSphereWithTexture(8, 10, 10); // Используем функцию с текстурными координатами
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Вторая нога (левая)
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, body);
            Gl.glPushMatrix();
            Gl.glTranslated(-15, 10, 0); // Рядом с нижним шаром тела
            Gl.glScaled(0.8, 0.8, 0.8); // Уменьшаем размер шара
            glutSolidSphereWithTexture(8, 10, 10); // Используем функцию с текстурными координатами
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            Gl.glPushMatrix();
            Gl.glTranslated(0, 120, 0); // Поднимаем на высоту верхнего шара тела
            Gl.glRotatef(-90, 1, 0, 0);
            Gl.glScaled(1.2, 1.2, 1.2); // Увеличиваем размер шляпы
            Gl.glColor3f(0.1f, 0.1f, 0.1f); // Темно-серый цвет шляпы
            Glut.glutSolidCone(20, 40, 20, 20); // Рисуем конус (шляпу)
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(0, 105, 15); // Позиция носа
            Gl.glRotatef(0, 1, 0, 0); // Поворачиваем нос вниз
            Gl.glScaled(1.2, 1.2, 1.2); // Уменьшаем размер носа
            Gl.glColor3f(1.0f, 0.5f, 0.0f); // Оранжевый цвет носа
            Glut.glutSolidCone(5, 20, 10, 10); // Рисуем конус (нос)
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(10, 110, 20); // Позиция правого глаза
            Gl.glColor3f(0.0f, 0.0f, 0.0f); // Черный цвет глаза
            Glut.glutSolidSphere(3, 10, 10); // Рисуем правый глаз
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-10, 110, 20); // Позиция левого глаза
            Gl.glColor3f(0.0f, 0.0f, 0.0f); // Черный цвет глаза
            Glut.glutSolidSphere(3, 10, 10); // Рисуем левый глаз
            Gl.glPopMatrix();

            /*if (moving)
            {
                snowmanX += 1; // Перемещаем снеговика вправо
                snowmanY += 1; // Перемещаем снеговика вниз
                Gl.glRotatef(rotationAngle, 1, 1, 1);
                if (snowmanX < maxX)
                {
                    // Перемещаем снеговика вправо
                    snowmanX += moveSpeed;
                }
                else
                {
                    // Если достигли границы, останавливаем движение
                    moving = false;
                }

                if (rotationAngle < maxRotationAngle)
                {
                    // Поворачиваем снеговика на rotationSpeed градусов
                    rotationAngle += rotationSpeed;
                }
                else
                {
                    // Если достигли максимального угла, останавливаем поворот
                    rotating = false;
                }
            }*/
            /*if (moving)
            {
                //snowmanX += 1; // Перемещаем снеговика вправо
                //snowmanY += 1; // Перемещаем снеговика вниз

                if (snowmanX < maxX)
                {
                    // Перемещаем снеговика вправо
                    snowmanX += moveSpeed;
                }
                else
                {
                    // Если достигли границы, останавливаем движение
                    moving = false;
                }


            }
            if (rotating)
            {
                Gl.glRotatef(rotationAngle, 0, 1, 0);
                if (rotationAngle < maxRotationAngle)
                {
                    // Поворачиваем снеговика на rotationSpeed градусов
                    rotationAngle += rotationSpeed;
                }
                else
                {
                    // Если достигли максимального угла, останавливаем поворот
                    rotating = false;
                    Console.WriteLine("Значение rotationAngle: " + rotationAngle);
                    Console.WriteLine("Значение rotationSpeed: " + rotationSpeed);

                }
            }*/

            Gl.glPopMatrix();
        }

        public void DrawDynamite(float x, float y, float z, float length, float radius)
        {
            Gl.glPushMatrix();
            Gl.glTranslatef(x, y, z);

            // Нарисовать цилиндр динамита
            Gl.glColor3f(1.0f, 0.0f, 0.0f); // Красный цвет
            Glu.GLUquadric qobj = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(qobj, Glu.GLU_FILL);
            Glu.gluQuadricNormals(qobj, Glu.GLU_SMOOTH);
            Glu.gluCylinder(qobj, radius, radius, length, 20, 20);
            Glu.gluDeleteQuadric(qobj);

            // Нарисовать фитиль
            Gl.glColor3f(0.5f, 0.5f, 0.5f); // Серый цвет
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3f(0.0f, 0.0f, length);
            Gl.glVertex3f(0.0f, 0.0f, length + 5.0f); // Предположим, что фитиль имеет длину 5.0f
            Gl.glEnd();

            Gl.glPopMatrix();
        }



        private bool ornamentsEnabled = false; // Флаг для указания, нужно ли наряжать деревья шарами

        public void DrawTree()
        {
            
            // Ствол
            Gl.glColor3f(0.6f, 0.3f, 0.0f); // Коричневый цвет ствола
            Glut.glutSolidCylinder(3, 50, 20, 20); // Рисуем ствол

            // Хвоя (несколько конусов)
            Gl.glColor3f(0.0f, 0.6f, 0.0f); // Зеленый цвет хвои

            // Радиусы конусов
            float[] coneRadii = { 20, 25, 30 }; // Радиусы нижнего, среднего и верхнего конусов

            for (int i = 0; i < coneRadii.Length; i++)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(0, 0, 45 - i * 15); // Перемещаемся вверх, чтобы начать рисовать конусы

                if (!ornamentsEnabled) // Если украшения не включены, то рисуем конусы с обычным цветом
                    Gl.glColor3f(0.0f, 0.6f, 0.0f);

                Glut.glutSolidCone(coneRadii[i], 40, 20, 20); // Рисуем конусы
                Gl.glPopMatrix();

                if (ornamentsEnabled)
                {
                    // Количество шаров, соответствующее радиусу конуса
                    int numOrnaments = 4 - i;

                    // Цвета шаров вручную
                    float[,] colors = {
                {1.0f, 0.0f, 0.0f}, // Красный
                {0.0f, 1.0f, 0.0f}, // Зеленый
                {0.0f, 0.0f, 1.0f}  // Синий
            };

                    // Сохраняем текущий цвет
                    float[] currentColor = new float[3];
                    Gl.glGetFloatv(Gl.GL_CURRENT_COLOR, currentColor);

                    // Располагаем шары по окружности
                    for (int j = 0; j < numOrnaments; j++)
                    {
                        Gl.glColor3f(colors[i, 0], colors[i, 1], colors[i, 2]); // Устанавливаем цвет шара

                        float angle = j * (360 / numOrnaments);
                        float x = coneRadii[i] * (float)Math.Cos(Math.PI * angle / 180);
                        float y = coneRadii[i] * (float)Math.Sin(Math.PI * angle / 180);

                        Gl.glPushMatrix();
                        Gl.glTranslated(x, y, 45 - i * 15); // Перемещаемся на уровень конуса и располагаем шар
                        Glut.glutSolidSphere(3, 20, 20); // Рисуем шар
                        Gl.glPopMatrix();
                    }

                    // Восстанавливаем предыдущий цвет
                    Gl.glColor3f(currentColor[0], currentColor[1], currentColor[2]);
                }
            }
        }

        public void ToggleOrnaments()
        {
            ornamentsEnabled = !ornamentsEnabled; // Инвертируем значение флага
        }

        public void RefreshTrees()
        {
            // Очищаем сцену
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            // Рисуем деревья с текущим состоянием переменной ornamentsEnabled
            PlaceTrees(10); // Здесь указывается нужное количество деревьев
        }

        public void PlaceTrees(int numberOfTrees)
        {
            float radius = 150; // Радиус полукруга
            float treeSize = 2.5f; // Размер деревьев
            float yOffset = 150; // Смещение по оси Y

            for (int i = 0; i < numberOfTrees; i++)
            {
                // Вычисляем угол в радианах для текущего дерева
                float angle = (float)(i * Math.PI / numberOfTrees);

                // Вычисляем координаты X и Y на основе угла и радиуса
                float x = radius * (float)Math.Cos(angle);
                float y = radius * (float)Math.Sin(angle) + yOffset; // Добавляем смещение по оси Y

                Gl.glPushMatrix();
                Gl.glTranslated(x, y, 0); // Устанавливаем координаты с учетом смещения по оси Y
                Gl.glScaled(treeSize, treeSize, treeSize); // Увеличиваем размер дерева
                DrawTree(); // Рисуем ель
                Gl.glPopMatrix();
            }
        }

        public void PlaceFerns(int numberOfFerns)
        {
            float radius = 150; // Радиус полукруга
            float fernSize = 2.5f; // Размер папоротников
            float yOffset = 150; // Смещение по оси Y

            Fern fern = new Fern(1, 1);

            for (int i = 0; i < numberOfFerns; i++)
            {
                // Вычисляем угол в радианах для текущего папоротника
                float angle = (float)(i * Math.PI / numberOfFerns);

                // Вычисляем координаты X и Y на основе угла и радиуса
                float x = radius * (float)Math.Cos(angle);
                float y = radius * (float)Math.Sin(angle) + yOffset; // Добавляем смещение по оси Y

                Gl.glPushMatrix();
                Gl.glTranslated(x, y, 0); // Устанавливаем координаты с учетом смещения по оси Y
                Gl.glScaled(fernSize, fernSize, fernSize); // Увеличиваем размер папоротника
                Gl.glRotatef(90.0f, 1.0f, 0.0f, 0.0f);
                fern.DrawFern(); // Рисуем папоротник, вызывая метод DrawFern из экземпляра fern класса Fern
                Gl.glPopMatrix();
            }
        }

        public void DrawFractalPlant(float x1, float y1, float x2, float y2, int depth)
        {
            if (depth == 0)
            {
                Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex2f(x1, y1);
                Gl.glVertex2f(x2, y2);
                Gl.glEnd();
            }
            else
            {
                float x3 = (2 * x1 + x2) / 3;
                float y3 = (2 * y1 + y2) / 3;
                float x4 = (x1 + x2) / 2 + (y2 - y1) * 0.57735f;
                float y4 = (y1 + y2) / 2 - (x2 - x1) * 0.57735f;
                float x5 = (x1 + 2 * x2) / 3;
                float y5 = (y1 + 2 * y2) / 3;

                DrawFractalPlant(x1, y1, x3, y3, depth - 1);
                DrawFractalPlant(x3, y3, x4, y4, depth - 1);
                DrawFractalPlant(x4, y4, x5, y5, depth - 1);
                DrawFractalPlant(x5, y5, x2, y2, depth - 1);
            }
        }

        public void DrawRecursiveLines(int numberOfIterations, float startX, float startY, float endX, float endY, float thickness)
        {
            // Базовый случай: если число итераций равно 0, прекращаем рекурсию
            if (numberOfIterations <= 0)
                return;

            // Рисуем текущую линию
            Gl.glLineWidth(thickness);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2f(startX, startY);
            Gl.glVertex2f(endX, endY);
            Gl.glEnd();

            // Вычисляем новые координаты для следующей линии
            float newThickness = thickness * 0.8f; // Уменьшаем толщину для следующей итерации
            float midX = (startX + endX) / 2;
            float midY = (startY + endY) / 2;

            // Генерируем две линии, идущие из середины текущей линии под углами
            float angle1 = (float)(Math.PI / 3); // Угол наклона для первой линии
            float angle2 = (float)(-Math.PI / 3); // Угол наклона для второй линии
            float newX1 = midX + (endX - startX) * (float)Math.Cos(angle1) - (endY - startY) * (float)Math.Sin(angle1);
            float newY1 = midY + (endX - startX) * (float)Math.Sin(angle1) + (endY - startY) * (float)Math.Cos(angle1);
            float newX2 = midX + (endX - startX) * (float)Math.Cos(angle2) - (endY - startY) * (float)Math.Sin(angle2);
            float newY2 = midY + (endX - startX) * (float)Math.Sin(angle2) + (endY - startY) * (float)Math.Cos(angle2);

            // Рекурсивно вызываем этот же метод для двух новых линий
            DrawRecursiveLines(numberOfIterations - 1, midX, midY, newX1, newY1, newThickness);
            DrawRecursiveLines(numberOfIterations - 1, midX, midY, newX2, newY2, newThickness);
        }

        public void drawFloor(uint snow_texture)
        {
            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, snow_texture);

            Gl.glColor3f(0.8f, 0.9f, 1.0f); // Снежно-голубой цвет

            Gl.glPushMatrix();
            Gl.glBegin(Gl.GL_QUADS);

            Gl.glVertex3d(-1000, -500, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(1000, 0, 0);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(1000, 1000, 0);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(-1000, 1000, 0);
            Gl.glTexCoord2f(0, 1);

            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glDisable(Gl.GL_TEXTURE_2D);

            Gl.glPopMatrix();
        }

    }

    //Класс RGB для удобства задания цвета
    class RGB
    {
        private float R;
        private float G;
        private float B;

        public RGB(float R, float G, float B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public float getR()
        {
            return R;
        }

        public float getG()
        {
            return G;
        }

        public float getB()
        {
            return B;
        }
    }
}

