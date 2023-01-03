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
        static int height = 64;
        static int width = 102;
        static public int Height { get => height; }
        static public int Width { get => width; }
        // Список всех ячеек
        static public Cell[][] cells;
        // Координаты ячейки
        public int PosX { get; }
        public int PosY { get; }
        // Номер ячейки
        public int Number { get; }
        // Может ли перейти эту ячейку нелетающее существо
        public bool Obstacle { get; set; }
        // Находится ли на данной ячейке существо.
        public bool Occupied { get; set; }
        public Rectangle Rec { get; set; }

        public Cell(int posX, int posY, int number, bool obstacle)
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