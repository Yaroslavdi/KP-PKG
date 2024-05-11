using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP_Biletchenko_PRI120
{
    public static class ImageFilters
    {
        /*public static int[,] ApplyBlurFilter(int[,] image, int width, int height)
        {
            int[,] result = new int[width, height];

            // Собираем матрицу для размытия
            float[] mat = new float[9];
            mat[0] = 0.05f;
            mat[1] = 0.05f;
            mat[2] = 0.05f;
            mat[3] = 0.05f;
            mat[4] = 0.6f;
            mat[5] = 0.05f;
            mat[6] = 0.05f;
            mat[7] = 0.05f;
            mat[8] = 0.05f;

            // Применяем фильтр к каждому пикселю
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sumR = 0, sumG = 0, sumB = 0;
                    int count = 0;

                    // Проходим по 3x3 окрестности каждого пикселя
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int nx = x + kx;
                            int ny = y + ky;

                            // Проверяем, чтобы не выйти за границы изображения
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                // Получаем значение RGB пикселя и умножаем на соответствующий коэффициент из матрицы
                                sumR += image[nx, ny] >> 16 & 0xFF * mat[count];
                                sumG += image[nx, ny] >> 8 & 0xFF * mat[count];
                                sumB += image[nx, ny] & 0xFF * mat[count];
                                count++;
                            }
                        }
                    }

                    // Усредняем значения и сохраняем в результирующем изображении
                    result[x, y] = ((int)sumR << 16) | ((int)sumG << 8) | (int)sumB;
                }
            }

            return result;
        }*/
    }
}
