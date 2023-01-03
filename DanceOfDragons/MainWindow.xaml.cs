﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

// Завтра
// TODO: Сделать спрайты зелёных войнов и расставить их
// TODO: Сделать так, чтобы существо могло ходить только один раз
// TODO: Сделать механику атаки
// TODO: Сделать класс стрелков
// TODO: Создать спрайты для стрелков и расставить их для обеих команд

// Потом
// TODO: Избавиться от мемоизации

namespace DanceOfDragons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        Team current_turn = Team.BLACK_TEAM; // Чей сейчас ход(черных или зеленых)
        bool is_creature_selected = false; // Выбрано ли существо
        List<int> cell_nums = new List<int>(); // Ячейки, на которые выбранное существо моэет перейти
        int current_creature_index;

        public static string images_path = "pack://application:,,,/images/";

        ImageBrush blackImage = new ImageBrush();
        ImageBrush greenImage = new ImageBrush();

        int beg_left = 0; // Откуда по оси X начинается рисование ячеек
        int beg_top = 152; // Откуда по оси Y начинается рисование ячеек

        // Количество ячеек по вертикали
        const int n_ver = 11;
        // Количество ячеек по горизонтали
        const int n_hor = 15;
        int to_i(int cell_num)
        {
            return (cell_num - 1)/ n_hor;
        }
        int to_j(int cell_num)
        {
            return (cell_num - 1)% n_hor;
        }
        int to_cell_num(int i, int j)
        {
            return j + i * n_hor + 1;
        }

        // Создание ячеек
        void CreateCells()
        {
            Cell.cells = new Cell[n_ver][];
            // Заполнение массива ячеек
            for (int i = 0; i < n_ver; i++)
            {
                Cell.cells[i] = new Cell[n_hor];
                for (int j = 0; j < n_hor; j++)
                {
                    Cell.cells[i][j] = new Cell(beg_left + j * Cell.Width, beg_top + i * Cell.Height,
                        to_cell_num(i, j), false);
                }
            }
        }

        // Создание существ
        void CreateCreatures()
        {
            // Создание существ партии "Черных"
            Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 300, 150, 2, Cell.cells[0][2], "crusader/crusader1.png"));
            Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 300, 150, 2, Cell.cells[2][1], "crusader/crusader1.png"));
            Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 300, 150, 2, Cell.cells[4][2], "crusader/crusader1.png"));
            Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 300, 150, 2, Cell.cells[n_ver - 5][2], "crusader/crusader1.png"));
            Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 300, 150, 2, Cell.cells[n_ver - 3][1], "crusader/crusader1.png"));
            Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 300, 150, 2, Cell.cells[n_ver - 1][2], "crusader/crusader1.png"));
            for (int i = 0; i < n_ver; i++)
            {
                Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 100, 50, 3, Cell.cells[i][3], "halberdier/halberdier1.png"));
                Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 100, 50, 3, Cell.cells[i][4], "halberdier/halberdier1.png"));
            }
        }


        void unhighlight_cells()
        {
            foreach (int num in cell_nums)
            {
                Cell.cells[to_i(num)][to_j(num)].Rec.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            cell_nums.Clear();
        }

        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus(); // Чтобы считывалось нажатие клавиш
            // Установка фонового рисунка
            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri(images_path + "backgrounds/map0.png"));
            MyCanvas.Background = bg;

            blackImage.ImageSource = new BitmapImage(new Uri(images_path + "teams/theblacks.png"));
            greenImage.ImageSource = new BitmapImage(new Uri(images_path + "teams/thegreens.png"));

            CreateCells();
            CreateCreatures();

            // Рисование ячеек
            for (int i = 0; i < n_ver; i++)
            {
                for (int j = 0; j < n_hor; j++)
                {
                    MyCanvas.Children.Add(Cell.cells[i][j].Rec);
                }
            }

            // Рисование
            foreach (Creature creature in Creature.creatures)
            {
                MyCanvas.Children.Add(creature.Rec);
            }

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += gameLoop;
            gameTimer.Start();

        }

        private void gameLoop(object sender, EventArgs e)
        {
            Turn_bg.Fill = (current_turn == Team.BLACK_TEAM) ? blackImage : greenImage;

            // Удаление
            foreach (Creature creature in Creature.creatures)
            {
                MyCanvas.Children.Remove(creature.Rec);
            }

            // Рисование
            foreach (Creature creature in Creature.creatures)
            {
                MyCanvas.Children.Add(creature.Rec);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // При нажатии клавиши "Esc" игра выключается, и окно закрывается
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
            // При нажатии клавиши Enter игрок пропускает ход
            if (e.Key == Key.Enter)
            {
                current_turn = (current_turn == Team.BLACK_TEAM) ? Team.GREEN_TEAM : Team.BLACK_TEAM;
                unhighlight_cells();
            }
        }

        private void RightClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                Tag2 tag2 = (Tag2)rect.Tag;
                if (tag2.IsCell)
                {
                    int i = to_i(tag2.index);
                    int j = to_j(tag2.index);
                    Cell.cells[i][j].ShowInfo();
                }
                else
                {
                    Creature.creatures[tag2.index].ShowInfo();
                }
            }
        }

        void DetermineAvailableWays(int i, int j, int range, bool origin)
        {
            if (range >= 0 && i < n_ver && i >= 0
                && j < n_hor && j >= 0 && (origin || !Cell.cells[i][j].Occupied))
            {
                int cell_num = to_cell_num(i, j);
                cell_nums.Add(cell_num);
                DetermineAvailableWays(i + 1, j, range - 1, false);
                DetermineAvailableWays(i - 1, j, range - 1, false);
                DetermineAvailableWays(i, j + 1, range - 1, false);
                DetermineAvailableWays(i, j - 1, range - 1, false);
                DetermineAvailableWays(i + 1, j - 1, range - 1, false);
                DetermineAvailableWays(i + 1, j + 1, range - 1, false);
                DetermineAvailableWays(i - 1, j - 1, range - 1, false);
                DetermineAvailableWays(i - 1, j + 1, range - 1, false);
            }
        }

        private void LeftClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            
            if (e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                Tag2 tag2 = (Tag2)rect.Tag;
                if (tag2.IsCell)
                {
                    if (is_creature_selected)
                    {
                        if (cell_nums.Contains(tag2.index))
                        {
                            Creature.creatures[current_creature_index].Move(
                                Cell.cells[to_i(tag2.index)][to_j(tag2.index)]);
                            unhighlight_cells();
                        }
                    }
                }
                // Если клик был на существо
                else
                {
                    if (Creature.creatures[tag2.index].team == current_turn)
                    {
                        is_creature_selected = true;
                        current_creature_index = tag2.index;
                        unhighlight_cells();
                        int cell_num = Creature.creatures[tag2.index].cell.Number;
                        int range = Creature.creatures[tag2.index].Range;
                        DetermineAvailableWays(to_i(cell_num), to_j(cell_num), range, true);
                        foreach (int num in cell_nums)
                        {
                            Cell.cells[to_i(num)][to_j(num)].Rec.Fill = new SolidColorBrush(Color.FromArgb(125, 0, 0, 0));
                        }
                    }
                }
            }
        }
    }
}