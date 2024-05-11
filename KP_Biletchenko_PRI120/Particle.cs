using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace KP_Biletchenko_PRI120
{
    class Particle
    {
        // позиция частицы
        private float[] position = new float[3];
        // размер
        private float _size;
        // время жизни
        private float _lifeTime;

        // вектор гравитации
        private float[] Grav = new float[3];
        // ускорение частицы
        private float[] power = new float[3];
        // коэфицент затухания силы
        private float attenuation;

        // набранная скорость
        private float[] speed = new float[3];

        // временной интервал активации частицы
        private float LastTime = 0;

        // конструктор класса
        public Particle(float x, float y, float z, float size, float lifeTime, float start_time)
        {

            // записываем все начальные настройки частицы, устанавливаем начальный коэфицент затухания
            // и обнуляем скорость и силу, приложенную к частице
            _size = size;
            _lifeTime = lifeTime;

            position[0] = x;
            position[1] = y;
            position[2] = z;

            speed[0] = 0;
            speed[1] = 0;
            speed[2] = 0;

            Grav[0] = 0;
            Grav[1] = 0;
            Grav[2] = -10f;

            attenuation = 0.33f;

            power[0] = 0;
            power[0] = 0;
            power[0] = 0;

            LastTime = start_time;

        }

        // функция установка ускорения, действующего на частицу
        public void SetPower(float x, float y, float z)
        {

            power[0] = x;
            power[1] = y;
            power[2] = z;

        }

        private float velocityX;
        private float velocityY;
        private float velocityZ;
        public void SetVelocity(float velocityX, float velocityY, float velocityZ)
        {
            this.velocityX = velocityX;
            this.velocityY = velocityY;
            this.velocityZ = velocityZ;
        }


        // инвертирование скорости частицы по заданной оси с указанным затуханием
        // удобно использовать для простой демонстрации столкновений, например с землей
        public void InvertSpeed(int os, float attenuation)
        {

            speed[os] *= -1 * attenuation;

        }

        // получение размера частицы
        public float GetSize()
        {

            return _size;

        }

        // установка нового значения затухания
        public void setAttenuation(float new_value)
        {

            attenuation = new_value;

        }

        // обновление позиции частицы
        public void UpdatePosition(float timeNow)
        {

            // орпределяем разницу во времени, прошедшую с последнего обновления
            // позиции частицы (ведь таймер может быть не фиксированный)
            float dTime = timeNow - LastTime;
            _lifeTime -= dTime;

            // обновляем последнюю отметку временного интервала
            LastTime = timeNow;

            // перерасчитываем ускорение, движущее частицу, с учетом затухания
            for (int a = 0; a < 3; a++)
            {

                if (power[a] > 0)
                {

                    power[a] -= attenuation * dTime;

                    if (power[a] <= 0)
                        power[a] = 0;

                }

                // перерасчитываем позицию частицы с учетом гравитации, вектора ускорения и прощедшего промежутка времени
                position[a] += (speed[a] * dTime + (Grav[a] + power[a]) * dTime * dTime);

                // обновляем скорость частицы
                speed[a] += (Grav[a] + power[a]) * dTime;

            }

        }

        // проверка, не закончилось ли время жизни частицы
        public bool isLife()
        {

            if (_lifeTime > 0)
            {

                return true;

            }
            else
            {

                return false;

            }

        }

        // получение координат частицы
        public float GetPositionX()
        {

            return position[0];

        }
        public float GetPositionY()
        {

            return position[1];

        }
        public float GetPositionZ()
        {

            return position[2];

        }
    }

    class Explosion
    {

        // позиция взрыва
        private float[] position = new float[3];
        // мощность
        private float _power;
        // максимальное количество частиц
        private int MAX_PARTICLES = 1000;
        // текущее установленное количество частиц
        private int _particles_now;

        // активирован
        private bool isStart = false;

        // массив частиц на основе созданного ранее класса
        private Particle[] PartilceArray;

        // дисплейный список для рисования частицы создан
        private bool isDisplayList = false;
        // номер дисплейного списка для отрисовки
        private int DisplayListNom = 0;

        // конструктор класса; в него передаются координаты, где должен произойти взрыв, мощность и количество чатиц
        public Explosion(float x, float y, float z, float power, int particle_count)
        {

            position[0] = x;
            position[1] = y;
            position[2] = z;

            _particles_now = particle_count;
            _power = power;

            // если число частиц превышает максимально разрешенное
            if (particle_count > MAX_PARTICLES)
            {

                particle_count = MAX_PARTICLES;

            }

            // создаем массив частиц необходимого размера
            PartilceArray = new Particle[particle_count];

        }

        // функция обновления позиции взрыва
        public void SetNewPosition(float x, float y, float z)
        {

            position[0] = x;
            position[1] = y;
            position[2] = z;

        }

        // установка нового значения мощности взрыва
        public void SetNewPower(float new_power)
        {

            _power = new_power;

        }

        // создания дисплейного списка для отрисовки частицы (т.к. отрисовывать даже небольшой полигон такое количество раз очень накладно)
        private void CreateDisplayList()
        {

            // генерация дисплейного списка
            DisplayListNom = Gl.glGenLists(1);

            // начало создания списка
            Gl.glNewList(DisplayListNom, Gl.GL_COMPILE);

            // режим отрисовки квадрата
            Gl.glBegin(Gl.GL_QUADS);

            // задаем форму частицы
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0.02f, 0.02f, 0);
            Gl.glVertex3d(0.02f, 0, -0.02f);
            Gl.glVertex3d(-0.02f, 0, -0.02f);

            Gl.glEnd();

            // завершаем отрисовку частицы
            Gl.glEndList();

            // флаг - дисплейный список создан
            isDisplayList = true;

        }

        // функция, реализующая взрыв
        public void Boooom(float time_start)
        {

            // инициализируем экземпляр класса Random
            Random rnd = new Random();

            // если дисплейный список не создан, надо его создать
            if (!isDisplayList)
            {

                CreateDisplayList();

            }

            // по всем частицам
            for (int ax = 0; ax < _particles_now; ax++)
            {

                // создаем частицу
                PartilceArray[ax] = new Particle(position[0], position[1], position[2], 50.0f, 10, time_start);

                // случайным образом генериуем ориентацию вектора ускорения для данной частицы
                int direction_x = 3;//rnd.Next(1, 3);
                int direction_y = 3;//rnd.Next(1, 3);
                int direction_z = 3;//rnd.Next(1, 3);

                // если сгенерированно число 2 - то мы заменяем его на -1.
                if (direction_x == 2)
                    direction_x = -1;


                if (direction_y == 2)
                    direction_y = -1;

                if (direction_z == 2)
                    direction_z = -1;

                // задаем мощность в промежутке от 5 до 100% от указанной (чтобы частицы имели разное ускорение)
                float _power_rnd = rnd.Next((int)_power / 20, (int)_power);
                // устанавливаем затухание, равное 50% от мощности
                PartilceArray[ax].setAttenuation(_power / 2.0f);
                // устанавливаем ускорение частицы, еще раз генерируя случайное число
                // таким образом мощность определится от 10 - до 100% полученной
                // Здесь же применяем ориентацию для векторов ускорения
                PartilceArray[ax].SetPower(_power_rnd * ((float)rnd.Next(100, 1000) / 1000.0f) * direction_x, _power_rnd * ((float)rnd.Next(100, 1000) / 1000.0f) * direction_y, _power_rnd * ((float)rnd.Next(100, 1000) / 1000.0f) * direction_z);

            }

            // взрыв активирован
            isStart = true;

        }

        // калькуляция текущего взрыва
        public void Calculate(float time)
        {
            Random random = new Random();

            // только в том случае, если взрыв уже активирован
            if (isStart)
            {

                // проходим циклом по всем частицам
                for (int ax = 0; ax < _particles_now; ax++)
                {

                    // если время жизни частицы еще не вышло
                    if (PartilceArray[ax].isLife())
                    {

                        // обновляем позицию частицы
                        PartilceArray[ax].UpdatePosition(time);

                        // сохраняем текущую матрицу
                        Gl.glPushMatrix();
                        Gl.glColor3f(0.6f, 0.2f, 0.2f);

                        // получаем размер частицы
                        float size = PartilceArray[ax].GetSize();

                        // выполняем перемещение частицы в необходимую позицию
                        Gl.glTranslated(PartilceArray[ax].GetPositionX(), PartilceArray[ax].GetPositionY(), 90);
                        // масштабируем ее в соотвествии с ее размером
                        Gl.glScalef(size, size, size);
                        Gl.glRotated(random.Next(1, 360), 0, 0, 1);
                        // вызываем дисплейный список для отрисовки частицы из кеша видеоадаптера
                        Gl.glCallList(DisplayListNom);
                        // возвращаем матрицу
                        Gl.glPopMatrix();
                        // отражение от "земли"
                        // если координата Y стала меньше нуля (удар о землю)
                        if (PartilceArray[ax].GetPositionY() < 0)
                        {

                            // инвертируем проекцию скорости на ось Y, как будто частица ударилась и отскочила от земли
                            // причем скорость затухает на 40%
                            PartilceArray[ax].InvertSpeed(1, 0.6f);

                        }
                    }

                }

            }

        }

    }
}
