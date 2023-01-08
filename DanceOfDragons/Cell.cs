using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DanceOfDragons
{
    struct Tag2
    {
        // Прямоугольник это ячейка или существо
        public bool IsCell;
        // Индекс ячейки/существа в массиве ячеек/существ
        public int index;
        public Tag2(bool IsCell, int index)
        {
            this.IsCell = IsCell;
            this.index = index;
        }
    }

    // Карта игры делится на ячейки
    class Cell
    {
        // Высота и ширина всех ячеек
        static double height = 64;
        static double width = 102;
        static public double Height { get => height; }
        static public double Width { get => width; }
        // Список всех ячеек
        static public Cell[][] cells;
        // Координаты ячейки
        public double PosX { get; }
        public double PosY { get; }
        // Номер ячейки
        public int Number { get; }
        // Может ли перейти эту ячейку нелетающее существо
        public bool Obstacle { get; set; }
        // Находится ли на данной ячейке существо.
        public bool Occupied { get; set; }
        public Rectangle Rec { get; set; }

        public const double beg_left = 0; // Откуда по оси X начинается рисование ячеек
        public const double beg_top = 152; // Откуда по оси Y начинается рисование ячеек

        // Количество ячеек по вертикали
        public const int n_ver = 11;
        // Количество ячеек по горизонтали
        public const int n_hor = 15;
        public static int to_i(int cell_num)
        {
            return (cell_num - 1) / n_hor;
        }
        public static int to_j(int cell_num)
        {
            return (cell_num - 1) % n_hor;
        }
        public static int to_cell_num(int i, int j)
        {
            return j + i * n_hor + 1;
        }

        // Создание ячеек
        static public void CreateCells()
        {
            cells = new Cell[n_ver][];
            // Заполнение массива ячеек
            for (int i = 0; i < n_ver; i++)
            {
                cells[i] = new Cell[n_hor];
                for (int j = 0; j < n_hor; j++)
                {
                   cells[i][j] = new Cell(beg_left + j * Width, beg_top + i * Height,
                        to_cell_num(i, j), false);
                }
            }
        }

        public Cell(double posX, double posY, int number, bool obstacle)
        {
            PosX = posX;
            PosY = posY;
            Number = number;
            Obstacle = obstacle;

            Rec = new Rectangle
            {
                Tag = new Tag2(true, Number),
                Height = height,
                Width = width,
                Stroke = Brushes.White,
                StrokeThickness = 0.25,
                Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
            };
            Canvas.SetLeft(Rec, PosX);
            Canvas.SetTop(Rec, PosY);
        }
        // Вывод информации об ячейке
        public void ShowInfo()
        {
            string yn1 = Obstacle ? "Да" : "Нет";
            string yn2 = Occupied ? "Да" : "Нет";
            MessageBox.Show("Номер ячейки: " + Number + Environment.NewLine + "Наличие препятствия: " + yn1 +
                Environment.NewLine + "Ячейка занята: " + yn2, "Информация об ячейке");
        }
    }
}