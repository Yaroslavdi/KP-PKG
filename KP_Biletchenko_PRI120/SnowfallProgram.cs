using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using Tao.FreeGlut;

namespace KP_Biletchenko_PRI120
{
    /*public class SnowParticle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityY { get; set; }

        public SnowParticle(float x, float y, float velocityY)
        {
            X = x;
            Y = y;
            VelocityY = velocityY;
        }

        public void Update(float deltaTime)
        {
            Y += VelocityY * deltaTime; // Обновляем позицию частицы по оси Y
        }

        public void Draw()
        {
            // Рисуем частицу снега (например, прямоугольник или круг)
            Gl.glColor3f(1.0f, 1.0f, 1.0f); // Белый цвет для снега
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex2f(X - 2, Y - 2);
            Gl.glVertex2f(X + 2, Y - 2);
            Gl.glVertex2f(X + 2, Y + 2);
            Gl.glVertex2f(X - 2, Y + 2);
            Gl.glEnd();
        }
    }*/

    /*public class Snowfall
    {
        private List<SnowParticle> particles;
        private Random random;

        public Snowfall(int numParticles)
        {
            particles = new List<SnowParticle>();
            random = new Random();

            // Создаем начальные частицы снега
            for (int i = 0; i < numParticles; i++)
            {
                float x = (float)random.NextDouble() * Glut.glutGet(Glut.GLUT_WINDOW_WIDTH); // Случайная позиция X на экране
                float y = (float)random.NextDouble() * Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT); // Случайная позиция Y на экране
                float velocityY = (float)random.NextDouble() * 50 + 50; // Случайная скорость падения
                particles.Add(new SnowParticle(x, y, velocityY));
            }
        }

        public void Update(float deltaTime)
        {
            // Обновляем все частицы снега
            foreach (var particle in particles)
            {
                particle.Update(deltaTime);
            }
        }

        public void Draw()
        {
            // Отрисовываем все частицы снега
            foreach (var particle in particles)
            {
                particle.Draw();
            }
        }
    }*/

    /*public class SnowfallScene
    {
        private Snowfall snowfall;

        public SnowfallScene(int numParticles)
        {
            snowfall = new Snowfall(numParticles);
        }

        public void Update(float deltaTime)
        {
            snowfall.Update(deltaTime);
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glLoadIdentity();

            snowfall.Draw();

            Gl.glFlush();
        }
    }*/

    /*public class SnowfallProgram
    {
        private SnowfallScene scene;
        private float lastFrameTime;

        public SnowfallProgram()
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_SINGLE | Glut.GLUT_RGB);
            Glut.glutInitWindowSize(800, 600);
            Glut.glutCreateWindow("Snowfall");
            Glut.glutDisplayFunc(Draw);
            Glut.glutIdleFunc(Idle);
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            scene = new SnowfallScene(500); // Создаем сцену с снегопадом
            lastFrameTime = GetTime();
        }

        public void Run()
        {
            Glut.glutMainLoop();
        }

        public void Draw()
        {
            float deltaTime = GetDeltaTime();
            scene.Update(deltaTime);
            scene.Draw();
        }

        private void Idle()
        {
            Glut.glutPostRedisplay();
        }

        private float GetTime()
        {
            return Environment.TickCount / 1000.0f;
        }

        private float GetDeltaTime()
        {
            float currentTime = GetTime();
            float deltaTime = currentTime - lastFrameTime;
            lastFrameTime = currentTime;
            return deltaTime;
        }
    }*/
}
