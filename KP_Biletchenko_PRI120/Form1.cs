using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace KP_Biletchenko_PRI120
{
    public partial class Form1 : Form
    {
        double angle = 3, angleX = -96, angleY = 0, angleZ = -30;
        double sizeX = 1, sizeY = 1, sizeZ = 1;

        double translateX = -9, translateY = -60, translateZ = -10;

        double cameraSpeed;
        float global_time = 0;

        bool night = false;

        double deltaZStone;

        public int level;
        public int starter = 0;

        private Random random = new Random();
        private List<(double, double)> points = new List<(double, double)>();

        //Текстуры
        uint floorSign, bodySign;
        int imageId;
        string floorTexture = "floor.png";
        string bodyTexture = "body.png";


        //Взрыв с использованием системы частиц
        private readonly Explosion explosion = new Explosion(50, 120, 26, 40, 50);

        //Проигрывание аудио

        bool applyBlur = false;

        private uint postProcessingTexture;
        private uint postProcessingFBO;

        private bool particlesEnabled = false;
        private bool drawParticles = false;

        private int counter = 0;


        private Explosion BOOOOM_1 = new Explosion(1, 10, 1, 300, 500);
        Random rnd = new Random();


        private bool explosionRequested = false; // Флаг для отслеживания запроса на взрыв


        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация openGL (glut)
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);

            // цвет очистки окна
            Gl.glClearColor(255, 255, 255, 1);

            // настройка порта просмотра
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60, (float)AnT.Width / (float)AnT.Height, 0.1, 900);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 2;
            comboBox3.SelectedIndex = 0;
            cameraSpeed = 5;

            floorSign = genImage(floorTexture);
            bodySign = genImage(bodyTexture);

            RenderTimer.Start();

            // Включение освещения
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_LIGHT1);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_NORMALIZE);
        }


        private int explosionDelay = 1000;
        private int lastExplosionTime = 0;
        private int lastButtonClickTime = 0;

        private bool dynamiteReady = false;
        private bool dynamiteReadyReady = false;
        private bool dynamiteReadyReadyReady = false;

        private float snowmanX = 0;
        private float snowmanY = 80;
        private float snowmanZ = 30;
        private async void Draw()
        {
            // Очищаем буферы и устанавливаем цвет фона
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearColor(255, 255, 255, 1);

            Gl.glLoadIdentity();
            Gl.glPushMatrix();
            Gl.glRotated(angleX, 1, 0, 0);
            Gl.glRotated(angleY, 0, 1, 0);
            Gl.glRotated(angleZ, 0, 0, 1);
            Gl.glTranslated(translateX, translateY, translateZ);
            Gl.glScaled(sizeX, sizeY, sizeZ);


            // Отрисовываем сцену
            //explosion.Calculate(global_time);
            modeling.drawFloor(floorSign);
            CreateParticle2(0, 0, 0, 50, 100);
            drawFractal(5);
            modeling.PlaceTrees(10);
            modeling.PlaceFerns(10);
            if (modeling.rotating)
            {
                Gl.glPushMatrix();
                Gl.glRotatef(modeling.rotationAngle, 0, 1, 0);
                if (modeling.rotationAngle < modeling.maxRotationAngle)
                {
                    // Поворачиваем снеговика на rotationSpeed градусов
                    //modeling.rotationAngle += modeling.rotationSpeed;
                }
                else
                {
                    // Если достигли максимального угла, останавливаем поворот
                    modeling.rotating = false;
                    //Console.WriteLine("Значение rotationAngle: " + modeling.rotationAngle);
                    //Console.WriteLine("Значение rotationSpeed: " + modeling.rotationSpeed);

                }
                //Gl.glPopMatrix();

            }
            modeling.drawSnowman(bodySign, snowmanX, snowmanY, snowmanZ, modeling.rotationAngle);
            if (modeling.moving)
            {
                //snowmanX += 1; // Перемещаем снеговика вправо
                //snowmanY += 1; // Перемещаем снеговика вниз
                
                if (snowmanX < modeling.maxX)
                {
                    // Перемещаем снеговика вправо
                    snowmanX += modeling.moveSpeed;
                }
                else
                {
                    // Если достигли границы, останавливаем движение
                    modeling.moving = false;
                    dynamiteReadyReadyReady = true;
                }

                
            }


            drawParticles = true;
                if (dynamiteReadyReady)
            {
                if(!dynamiteReadyReadyReady)
                { 
                CreateParticle(0, 0, 0, 50, 100);


                //Частица на фитиле
                Particle particle = new Particle(0, 0, 0, 10.0f, 100, 0);
                Gl.glColor3f(1.0f, 0.0f, 0.0f); // Например, красный цвет
                                                // Перемещение частицы в указанную позицию
                Gl.glTranslated(particle.GetPositionX(), particle.GetPositionY(), particle.GetPositionZ());
                // Отрисовка сферы в качестве частицы
                Glut.glutSolidSphere(1.0, 10, 10);
                particle.UpdatePosition(global_time);
                }
            }




            /*Explosion explosion = new Explosion(0, 0, 0, 100, 100);
            explosion.Boooom(global_time);
            explosion.Calculate(global_time);*/


            /*if (explosionRequested && (Environment.TickCount - lastExplosionTime >= explosionDelay))
            {
                Particle particle = new Particle(0, 0, 0, 10.0f, 100, 0);
                Gl.glColor3f(1.0f, 0.0f, 0.0f); // Красный цвет частицы
                                                // Перемещение частицы в указанную позицию
                Gl.glTranslated(particle.GetPositionX(), particle.GetPositionY(), particle.GetPositionZ());
                // Отрисовка сферы в качестве частицы
                Glut.glutSolidSphere(1.0, 10, 10);
                particle.UpdatePosition(global_time);
                lastExplosionTime = Environment.TickCount;


                //modeling.DrawDynamite(0.0f, 0.0f, 0.0f, 10.0f, 1.0f);

                explosionRequested = false; // Сбрасываем флаг после создания частицы
            }*/
            if (dynamiteReady)
            {
                // Рисуем динамит
                if(!dynamiteReadyReady)
                modeling.DrawDynamite(0.0f, 0.0f, 0.0f, 10.0f, 1.0f);
                
                await Task.Delay(2000);
                modeling.moving = true;
                modeling.rotating = true;
                dynamiteReadyReady = true;
                CreateParticle(0, 0, 0, 50, 100);
            }
                




            

   


            Gl.glColor3f(0.0f, 0.6f, 0.0f); // Зеленый цвет


            // Создание экземпляра класса Fern
            /*Fern fern = new Fern(AnT.Width, AnT.Height);

            // Вызов метода DrawFern, передавая элемент управления SimpleOpenGlControl
            fern.DrawFern();*/


            Gl.glPopMatrix();
            Gl.glFlush();
            AnT.Invalidate();
        }


        
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        


        private List<Particle> particles = new List<Particle>(); // Список для хранения частиц 

        public void CreateParticle(float centerX, float centerY, float centerZ, float radius, int particleCount)
        {
            /*Random random = new Random();

            // Заполнение массива частиц
            for (int i = 0; i < particleCount; i++)
            {
                // Генерация случайных углов в сферических координатах
                float theta = (float)(random.NextDouble() * 2 * Math.PI); // Угол theta от 0 до 2π
                float phi = (float)(random.NextDouble() * Math.PI); // Угол phi от 0 до π

                // Преобразование сферических координат в декартовы координаты
                float x = centerX + radius * (float)(Math.Sin(phi) * Math.Cos(theta));
                float y = centerY + radius * (float)(Math.Sin(phi) * Math.Sin(theta));
                float z = centerZ + radius * (float)Math.Cos(phi);

                // Создание частицы с заданными координатами и начальной скоростью
                Particle particle = new Particle(x, y, z, 10.0f, 100, 0); // Например, начальная скорость 10 по всем осям

                // Отображение частицы
                Gl.glPushMatrix();
                Gl.glTranslated(particle.GetPositionX(), particle.GetPositionY(), particle.GetPositionZ());
                Gl.glColor3f(1.0f, 0.0f, 0.0f); // Например, красный цвет частицы
                Glut.glutSolidSphere(1.0, 10, 10);
                Gl.glPopMatrix();

                // Добавление частицы в список
                particles.Add(particle);

                
            }*/
            Particle[] particles = new Particle[100]; // Например, 100 частиц

            // Заполнение массива частиц
            for (int i = 0; i < particles.Length; i++)
            {
                // Генерация начальной позиции частицы в пределах некоторой области
                float x = (float)(random.NextDouble() * 10 - 5); // Например, от -5 до 5 по X
                float y = (float)(random.NextDouble() * 10 - 5); // Например, от -5 до 5 по Y
                float z = (float)(random.NextDouble() * 10 - 5); // Например, от -5 до 5 по Z

                // Создание частицы с заданными координатами и начальной скоростью
                particles[i] = new Particle(x, y, z, 10.0f, 100, 0); // Например, начальная скорость 10 по всем осям
            }

            // Отображение частиц
            foreach (var particle in particles)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(particle.GetPositionX(), particle.GetPositionY(), particle.GetPositionZ());
                Gl.glColor3f(1.0f, 0.0f, 0.0f); // Например, красный цвет частицы
                Glut.glutSolidSphere(1.0, 10, 10);
                Gl.glPopMatrix();

                // Обновление позиции частицы с течением времени
                particle.UpdatePosition(global_time);
            }
        }
        public void CreateParticle2(float centerX, float centerY, float centerZ, float radius, int particleCount)
        {
            
            Particle[] particles = new Particle[500]; // 500 частиц

            // Заполнение массива частиц
            for (int i = 0; i < particles.Length; i++)
            {
                // Генерация начальной позиции частицы в пределах некоторой области
                float x = (float)(random.NextDouble() * 500 - 250); // От -250 до 250 по X
                float y = (float)(random.NextDouble() * 500 - 250); // От -250 до 250 по Y
                float z = (float)(random.NextDouble() * 500 - 250); // От -250 до 250 по Z

                // Создание частицы с заданными координатами и начальной скоростью
                particles[i] = new Particle(x, y, z, 1.0f, 100, 0); // Например, начальная скорость 1 по всем осям
            }

            // Отображение частиц
            foreach (var particle in particles)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(particle.GetPositionX(), particle.GetPositionY(), particle.GetPositionZ());
                Gl.glColor3f(0.0f, 0.0f, 1.0f); // Синий цвет частицы
                Glut.glutSolidSphere(1.0, 10, 10);
                Gl.glPopMatrix();

                // Обновление позиции частицы с течением времени
                particle.UpdatePosition(global_time);
            }
        }

        public void DrawParticles()
        {
            // Отображение и обновление позиции всех частиц в списке
            foreach (var particle in particles)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(particle.GetPositionX(), particle.GetPositionY(), particle.GetPositionZ());
                Gl.glColor3f(1.0f, 0.0f, 0.0f); // Например, красный цвет частицы
                Glut.glutSolidSphere(1.0, 10, 10);
                Gl.glPopMatrix();

                // Обновление позиции частицы с течением времени

                particle.UpdatePosition(global_time);
            }
            
        }

        private int explosionStartTime = 0;
        private void button5_Click(object sender, EventArgs e)
        {
            explosionRequested = true;
            explosionStartTime = Environment.TickCount;
            elapsedTime = DateTime.Now - startTime; // Считаем время, прошедшее с запуска программы
            dynamiteDelay = TimeSpan.FromSeconds(2); // Устанавливаем время ожидания в 2 секунды
            Console.WriteLine("Прошло времени с момента запуска программы: " + elapsedTime.ToString());
            Console.WriteLine("Переменная DateTime (время сейчас): " + DateTime.Now.ToString());
            Console.WriteLine("Переменная startTime (время при запуске): " + startTime.ToString());
            CheckExplosion()/*.Wait()*/;

        }
        private /*async Task*/ void CheckExplosion()
        {
            dynamiteReady = true;
            //await Task.Delay(2000);
        }
        private DateTime startTime; // Время начала программы
        private TimeSpan elapsedTime; // Время, прошедшее с запуска программы
        private TimeSpan dynamiteDelay = TimeSpan.FromSeconds(2);
        private bool isDynamiteRequested = false;

        
        private void RenderTimer_Tick(object sender, EventArgs e)
        {

            global_time += (float)RenderTimer.Interval / 1000;
            Draw();
        }

        

        // Поместите этот код в ваш метод отрисовки OpenGL (например, OnPaint для SimpleOpenGlControl)
        private void AnT_Paint(object sender, PaintEventArgs e)
        {
            if (comboBox3.SelectedIndex == 0)
            {
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glClearColor(1f, 1f, 1f, 1f); // Белый цвет фона
            }
            else
            {
                if (true)
                {
                    // Применяем размытие
                    //ApplyBlurFilter();
                    // Сбрасываем флаг после применения размытия
                    applyBlur = false;
                }
            }

            Draw(); // Вызываем метод отрисовки
        }



        // Метод для получения цвета пикселя
        /*private float[] GetPixelColor(int x, int y)
        {
            byte[] pixelData = new byte[3];
            Gl.glReadPixels(x, AnT.Height - y, 1, 1, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixelData);

            // Преобразование байтов в диапазон 0-1
            float[] color = new float[3];
            for (int i = 0; i < 3; i++)
            {
                color[i] = pixelData[i] / 255f;
            }

            return color;
        }*/

            // Метод для установки цвета пикселя

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            starter = 0;
        }
        
        public void button4_Click(object sender, EventArgs e)
        {
            modeling.ToggleOrnaments(); // Меняем состояние флага
            AnT.Invalidate();
            AnT.Focus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                angle = 3; angleX = -90; angleY = 0; angleZ = -10;
                sizeX = 1; sizeY = 1; sizeZ = 1;
                translateX = -80; translateY = 160; translateZ = -55;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                translateX = -50; translateY = -30; translateZ = -420;
                angleX = -00;
                angleZ = -90;
            }
            AnT.Focus();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AnT.Focus();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }       

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AnT_Load(object sender, EventArgs e)
        {

        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                translateY -= cameraSpeed;

            }
            if (e.KeyCode == Keys.S)
            {
                translateY += cameraSpeed;
            }
            if (e.KeyCode == Keys.A)
            {
                translateX += cameraSpeed;
            }
            if (e.KeyCode == Keys.D)
            {
                translateX -= cameraSpeed;

            }
            if (e.KeyCode == Keys.ControlKey)
            {
                translateZ += cameraSpeed;

            }
            if (e.KeyCode == Keys.Space)
            {
                translateZ -= cameraSpeed;
            }
            if (e.KeyCode == Keys.E)
            {
                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        angleX += angle;

                        break;
                    case 1:
                        angleY += angle;

                        break;
                    case 2:
                        angleZ += angle;

                        break;
                    default:
                        break;
                }
            }
            if (e.KeyCode == Keys.Q)
            {
                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        angleX -= angle;
                        break;
                    case 1:
                        angleY -= angle;
                        break;
                    case 2:
                        angleZ -= angle;
                        break;
                    default:
                        break;
                }
            }


        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        Modeling modeling = new Modeling();

        public Form1()
        {
            InitializeComponent();
            AnT.Paint += AnT_Paint;
            AnT.InitializeContexts();
            explosionRequested = true;
            startTime = DateTime.Now;
        }

        /*public void drawFractal(int level)
        {
            Gl.glPushMatrix();

            Gl.glTranslated(55, 105, 82);
            Gl.glRotated(80, 0, 1, 0);
            Gl.glRotated(90, 0, 0, 1);
            Gl.glRotated(90, 0, 1, 0);
            Gl.glScalef(1, 0.65f, 0.5f);

            Gl.glBegin(Gl.GL_LINES);
            if (level >= 0 & level <= 3)
            {
                DrawKochLine(-10, -10, 20, 0, level);
            }
            Gl.glEnd();
            Gl.glPopMatrix();
        }*/
            public void drawFractal(int level)
            {
                Gl.glPushMatrix();

                Gl.glTranslated(95, 95, 0);
                Gl.glRotated(00, 0, 1, 0);
                Gl.glRotated(90, 0, 0, 1);
                Gl.glRotated(90, 0, 1, 0);
                Gl.glScalef(1, 0.65f, 0.5f);

                Gl.glBegin(Gl.GL_LINES);
                if (level >= 0)
                {
                    BarnsleyFern(0, 0, 0, -50, 0.5, level); // Начинаем с вертикальной ветви
                }
                Gl.glEnd();
                Gl.glPopMatrix();
            }

        //private const double SQRT_3 = 1.7320508075688772;

        /*public void DrawKochLine(double x1, double y1, double x2, double y2, int level)
        {
            if (level == 0)
            {
                // прямая линия
                Gl.glLineWidth(5f);
                Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3f(0, 0, 0);
                Gl.glVertex2d(x1, y1);
                Gl.glVertex2d(x2, y2);
                Gl.glEnd();
            }
            else
            {
                //делим на 4 части
                double dx = (x2 - x1) / 3.0;
                double dy = (y2 - y1) / 3.0;

                // считаем координаты пиковых точек
                double peakX = x1 + dx - dy * Math.Cos(Math.PI / 3.0);
                double peakY = y1 + dy + dx * Math.Sin(Math.PI / 3.0);

                // рисовалка 4х сегментов
                DrawKochLine(x1, y1, x1 + dx, y1 + dy, level - 1);
                DrawKochLine(x1 + dx, y1 + dy, peakX, peakY, level - 1);
                DrawKochLine(peakX, peakY, x1 + 2 * dx, y1 + 2 * dy, level - 1);
                DrawKochLine(x1 + 2 * dx, y1 + 2 * dy, x2, y2, level - 1);
            }
        }*/

        public void BarnsleyFern(double x1, double y1, double angle, double length, double width, int level)
        {
            if (level == 0)
            {
                return; // Базовый случай для выхода из рекурсии
            }

            double x2 = x1 + length * Math.Cos(angle * Math.PI / 180.0); // Вычисляем конечные координаты
            double y2 = y1 + length * Math.Sin(angle * Math.PI / 180.0);

            Gl.glLineWidth((float)width); // Задаем толщину линии

            // Рисуем линию от (x1, y1) до (x2, y2)
            Gl.glVertex2d(x1, y1);
            Gl.glVertex2d(x2, y2);

            // Вызываем функцию для двух следующих ветвей с уменьшенной длиной, углом и уровнем
            BarnsleyFern(x2, y2, angle - 25, length * 0.6, width * 0.7, level - 1);
            BarnsleyFern(x2, y2, angle + 25, length * 0.6, width * 0.7, level - 1);
        }


        private void информацияОПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private uint genImage(string image)
        {
            uint sign = 0;
            Il.ilGenImages(1, out imageId);
            Il.ilBindImage(imageId);
            if (Il.ilLoadImage(image))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        sign = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        sign = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }
            Il.ilDeleteImages(1, ref imageId);
            return sign;
        }

        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {
            uint texObject;
            Gl.glGenTextures(1, out texObject);
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);
            switch (Format)
            {

                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

            }
            return texObject;
        }
    }
}
