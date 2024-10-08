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


// Сейчас
// TODO: Стартово окно
// TODO: Выбор карт
// TODO: Обработка препятствий

namespace DanceOfDragons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    // Для мемоизации
    struct HashValue {
        public int cell_num;
        public int range;

        public HashValue(int cell_num, int range)
        {
            this.cell_num = cell_num;
            this.range = range;
        }
    }

    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        Team current_turn = Team.BLACK_TEAM; // Чей сейчас ход(черных или зеленых)
        bool is_creature_selected = false; // Выбрано ли существо
        bool is_attack = false; // Атакует ли сейчас существо
        HashSet<int> cell_nums = new HashSet<int>(); // Ячейки, на которые выбранное существо может перейти
        List<int> cell_nums_attack = new List<int>(); // Ячейки, на которые выбранное существо может перейти и атаковать из них
        int current_creature_index;
        int current_creature_to_attack_index;
        string images_path = "pack://application:,,,/images/";
        public static List<Rectangle> removeRects = new List<Rectangle>(); // Уничтоженные существа
        List<Rectangle> recs_hp = new List<Rectangle>();
        List<Label> txts_hp = new List<Label>();
        ImageBrush blackImage = new ImageBrush();
        ImageBrush greenImage = new ImageBrush();

       
        void unhighlight_cells()
        {
            foreach (int num in cell_nums)
            {
                Cell.cells[Cell.to_i(num)][Cell.to_j(num)].Rec.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            foreach (int num in cell_nums_attack)
            {
                Cell.cells[Cell.to_i(num)][Cell.to_j(num)].Rec.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            cell_nums.Clear();
            cell_nums_attack.Clear();
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

            Cell.CreateCells();
            Creature.images_path = images_path;
            Creator.CreateCreatures();

            // Рисование ячеек
            for (int i = 0; i < Cell.n_ver; i++)
            {
                for (int j = 0; j < Cell.n_hor; j++)
                {
                    MyCanvas.Children.Add(Cell.cells[i][j].Rec);
                }
            }

            // Рисование
            foreach (Creature creature in Creature.creatures)
            {
                if (creature != null)
                    MyCanvas.Children.Add(creature.Rec);
            }


            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += gameLoop;
            gameTimer.Start();

        }


        void RemoveRect()
        {
            foreach (Rectangle rectangle in removeRects)
            {
                MyCanvas.Children.Remove(rectangle);
            }
            removeRects.Clear();
        }


        private void gameLoop(object sender, EventArgs e)
        {
            if (Creature.greens_num <= 0)
            {
                MessageBox.Show(@"Победила партия ""Черные"" Рейниры Таргариен", "Конец игры");
                Application.Current.Shutdown();
            }
            else if (Creature.blacks_num <= 0)
            {
                MessageBox.Show(@"Победила партия ""Зеленые"" Эйгона II Таргариена", "Конец игры");
                Application.Current.Shutdown();
            }
            Turn_bg.Fill = (current_turn == Team.BLACK_TEAM) ? blackImage : greenImage;

            // Удаление
            foreach (Creature creature in Creature.creatures)
            {
                if (creature != null)
                    MyCanvas.Children.Remove(creature.Rec);
            }
            // Удаление прямоугольников с информацией о численности
            foreach (Rectangle rec_hp in recs_hp)
            {
                MyCanvas.Children.Remove(rec_hp);
            }
            foreach (Label txt_hp in txts_hp)
            {
                MyCanvas.Children.Remove(txt_hp);
            }
            recs_hp.Clear();
            txts_hp.Clear();

            // Рисование
            foreach (Creature creature in Creature.creatures)
            {
                if (creature != null)
                    MyCanvas.Children.Add(creature.Rec);
            }
            // Рисование информации о численности
            foreach (Creature creature in Creature.creatures)
            {
                if (creature != null)
                {
                    Rectangle rec_hp = new Rectangle();
                    rec_hp.Width = 0.5 * Cell.Width;
                    rec_hp.Height = 0.25 * Cell.Height;
                    if (!creature.Is_used)
                        rec_hp.Fill = Brushes.Purple;
                    else
                        rec_hp.Fill = Brushes.Gray;
                    rec_hp.Stroke = Brushes.White;
                    Tag2 tag2 = new Tag2(false, creature.Num);
                    rec_hp.Tag = tag2;
                    Canvas.SetLeft(rec_hp, Canvas.GetLeft(creature.cell.Rec));
                    Canvas.SetTop(rec_hp, Canvas.GetTop(creature.cell.Rec) + 0.75 * Cell.Height);

                    Label txt_hp = new Label();
                    txt_hp.Content = creature.Hp;
                    txt_hp.Foreground = Brushes.White;
                    txt_hp.Tag = tag2;
                    Canvas.SetLeft(txt_hp, Canvas.GetLeft(rec_hp) + 0.2 * rec_hp.Width);
                    Canvas.SetTop(txt_hp, Canvas.GetTop(rec_hp) - 0.3 * rec_hp.Height);

                    recs_hp.Add(rec_hp);
                    txts_hp.Add(txt_hp);
                    MyCanvas.Children.Add(rec_hp);
                    MyCanvas.Children.Add(txt_hp);
                }
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
                foreach (Creature creature in Creature.creatures)
                {
                    if (creature != null)
                        creature.Is_used = false;
                }
                is_creature_selected = false;
                is_attack = false;
            }
            // Отменить выбор существа
            if (e.Key == Key.Q)
            {
                unhighlight_cells();
                is_creature_selected = false;
                is_attack = false;
            }
        }

        private void RightClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                Tag2 tag2;
                try { tag2 = (Tag2)rect.Tag;}
                catch (System.NullReferenceException) {
                    string fraction = (current_turn == Team.BLACK_TEAM) ? "черных" : "зеленых";
                    MessageBox.Show($"Сейчас ход: {fraction}", "Герб");
                    return;
                }
                if (tag2.IsCell)
                {
                    int i = Cell.to_i(tag2.index);
                    int j = Cell.to_j(tag2.index);
                    Cell.cells[i][j].ShowInfo();
                }
                else
                {
                    Creature.creatures[tag2.index].ShowInfo();
                }
            }
        }


        private void LeftClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                Tag2 tag2;
                try { tag2 = (Tag2)rect.Tag; } catch (System.NullReferenceException) { return; }
                if (tag2.IsCell && !is_attack)
                {
                    if (is_creature_selected)
                    {
                        if (cell_nums.Contains(tag2.index))
                        {
                            Creature.creatures[current_creature_index].Move(
                                Cell.cells[Cell.to_i(tag2.index)][Cell.to_j(tag2.index)]);
                            unhighlight_cells();
                            is_creature_selected = false;
                        }
                    }
                }
                else if (tag2.IsCell && is_attack && cell_nums_attack.Contains(tag2.index) && is_creature_selected)
                {
                    Creature.creatures[current_creature_index].Move(
                                Cell.cells[Cell.to_i(tag2.index)][Cell.to_j(tag2.index)]);
                    Creature.creatures[current_creature_index].Attack(Creature.creatures[current_creature_to_attack_index]);
                    unhighlight_cells();
                    is_creature_selected = false;
                    is_attack = false;
                    RemoveRect();
                }
                // Если клик был на существо
                else
                {
                    // Клик на своё существо
                    if (!is_attack && Creature.creatures[tag2.index].team == current_turn && !Creature.creatures[tag2.index].Is_used)
                    {
                        is_creature_selected = true;
                        current_creature_index = tag2.index;
                        unhighlight_cells();
                        int cell_num = Creature.creatures[tag2.index].cell.Number;
                        int range = Creature.creatures[tag2.index].Range;
                        HashSet<HashValue> cache = new HashSet<HashValue>(); // Для мемоизации
                        Creature.creatures[tag2.index].DetermineAvailableWays(Cell.to_i(cell_num), Cell.to_j(cell_num), range, true, cell_nums, cache);
                        if (Creature.creatures[tag2.index].Large)
                        {
                            int i = Cell.to_i(cell_num);
                            int j = Cell.to_j(cell_num);
                            if (j + 3 < Cell.n_hor && !Cell.cells[i][j + 2].Occupied)
                                cell_nums.Add(Cell.cells[i][j + 1].Number);

                        }
                        foreach (int num in cell_nums)
                        {
                            Cell.cells[Cell.to_i(num)][Cell.to_j(num)].Rec.Fill = new SolidColorBrush(Color.FromArgb(125, 0, 0, 0));
                        }
                    }
                    // Клик на вражеское существо
                    else if (!is_attack && Creature.creatures[tag2.index].team != current_turn && is_creature_selected && !is_attack)
                    {
                        if (Creature.creatures[current_creature_index] is RangedWarrior)
                        {
                            Creature.creatures[current_creature_index].Attack(Creature.creatures[tag2.index]);
                            is_creature_selected = false;
                            Creature.creatures[current_creature_index].Is_used = true;
                            unhighlight_cells();
                            RemoveRect();
                            return;
                        }
                        current_creature_to_attack_index = tag2.index;
                        int i = Cell.to_i(Creature.creatures[tag2.index].cell.Number);
                        int j = Cell.to_j(Creature.creatures[tag2.index].cell.Number);
                        foreach (int cell_num in cell_nums)
                        {
                            for (int ik = -1; ik < 2; ik++)
                                for (int jk = -1; jk < 2; jk++)
                                    try
                                    {
                                        if (Cell.cells[i + ik][j + jk].Number == cell_num)
                                        {
                                            cell_nums_attack.Add(cell_num);
                                        }
                                        if (Creature.creatures[current_creature_index].Large)
                                        {
                                            if (
                                                (Cell.cells[i + ik][j + jk - 1].Number == cell_num
                                                // && !Cell.cells[i + ik][j + jk].Occupied)
                                                ) ||
                                                (Cell.cells[i + ik][j + jk + 1].Number == cell_num
                                                //&& !Cell.cells[i + ik][j + jk].Occupied)
                                                )
                                               )
                                            {
                                                cell_nums_attack.Add(cell_num);
                                            }
                                        }
                                    }
                                    catch (IndexOutOfRangeException) { continue; }
                        }
                        foreach (int cell_num_attack in cell_nums_attack)
                        {
                            Cell.cells[Cell.to_i(cell_num_attack)][Cell.to_j(cell_num_attack)].Rec.Fill = new SolidColorBrush(Color.FromArgb(125, 255, 0, 0));
                        }
                        if (cell_nums_attack.Count != 0)
                            is_attack = true;
                        else
                            is_attack = false;
                    }
                }
            }
        }
    }
}