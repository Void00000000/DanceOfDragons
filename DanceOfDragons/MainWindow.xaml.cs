using System;
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

namespace DanceOfDragons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    enum Team
    {
        BLACK_TEAM,
        GREEN_TEAM
    }

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
        static public List<Cell> cells = new List<Cell>();
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
                Tag = new Tag2(true, Number - 1),
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

    abstract class Creature
    {
        public Team team { get; protected set; }
        protected int hp; // Здоровье
        protected int dmg; // Урон
        public int Range { get; protected set; } // Дальность передвижения(в ячейках)
        public Cell cell { get; protected set; } // Ячейка, на которой стоит существо
        protected string sprite; // Изображение существа
        protected int num; // Номер существа
        public Rectangle Rec { get; protected set; }
        public static int creature_num = 0; // Количество всего созданных существ
        static public List<Creature> creatures = new List<Creature>(); // Список всех созданных существ

        public bool Move(Cell to_cell)
        {
            if (to_cell.Occupied)
            {
                MessageBox.Show("Ячейка занята, поэтому на неё нельзя переместить выбранное существо",
                    "Нельзя переместить");
                return false;
            }
            else
            {
                cell = to_cell;
                return true;
            }
        }
        abstract public void Attack(Creature creature);
        abstract public void ShowInfo();
    }

    // Боец ближнего боя
    class Warrior : Creature
    {
        public Warrior(Team team, int hp, int dmg, int range, Cell cell, string sprite)
        {
            this.team = team;
            this.hp = hp;
            this.dmg = dmg;
            this.Range = range; 
            this.cell = cell;
            this.sprite = sprite;
            num = creature_num;
            creature_num++;

            ImageBrush warriorSprite = new ImageBrush();
            warriorSprite.ImageSource = new BitmapImage(new Uri(MainWindow.images_path + "warriors/" + sprite));
            Rec = new Rectangle
            {
                Tag = new Tag2(false, num),
                Height = 3 * Cell.Height,
                Width = 0.9 * Cell.Width,
                Fill = warriorSprite
            };
            Canvas.SetLeft(Rec, cell.PosX);
            Canvas.SetTop(Rec, cell.PosY - 1.5 * Cell.Width);
        }

        public override void ShowInfo()
        {
            string b_g = (this.team == Team.BLACK_TEAM) ? "Черные" : "Зеленые";
            MessageBox.Show("Тип существа: Воин" + Environment.NewLine + "Фракция: " + b_g + Environment.NewLine +
                "Здоровье: " + hp + Environment.NewLine + "Урон: " + dmg + Environment.NewLine
                + "Дальность передвижения: " + Range + Environment.NewLine +
                "Номер ячейки: " + cell.Number, "Информация о воине № " + num);
        }
        public override void Attack(Creature creature)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        Team current_turn = Team.BLACK_TEAM; // Чей сейчас ход(черных или зеленых)
        bool is_creature_selected = false; // Выбрано ли существо

        public static string images_path = "pack://application:,,,/images/";
        int beg_left = 0; // Откуда по оси X начинается рисование ячеек
        int beg_top = 152; // Откуда по оси Y начинается рисование ячеек

        // Создание ячеек(количество по горизонтали, количество по вертикали)
        void CreateCells(int cells_num_hor, int cells_num_ver)
        {
            // Заполнение массива ячеек
            for (int i = 0; i < cells_num_ver; i++)
            {
                for (int j = 0; j < cells_num_hor; j++)
                {
                    Cell.cells.Add(new Cell(beg_left + j * Cell.Width, beg_top + i * Cell.Height,
                        j + i * cells_num_hor + 1, false));
                }
            }
        }

        // Создание существ
        void CreateCreatures()
        {
            Creature.creatures.Add(new Warrior(Team.BLACK_TEAM, 300, 150, 5, Cell.cells[153], "crusader/crusader1.png"));
            Creature.creatures.Add(new Warrior(Team.GREEN_TEAM, 300, 150, 5, Cell.cells[0], "crusader/crusader1.png"));
        }


        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus(); // Чтобы считывалось нажатие клавиш
            // Установка фонового рисунка
            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri(images_path + "backgrounds/map0.png"));
            MyCanvas.Background = bg;

            CreateCells(15, 11);
            CreateCreatures();


            // Рисование ячеек
            foreach (Cell cell in Cell.cells)
            {
                MyCanvas.Children.Add(cell.Rec);
            }

            // Рисование существ
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
            Turn.Content = (current_turn == Team.BLACK_TEAM) ? "Ход черных" : "Ход зелёных";
            Turn.Foreground = (current_turn == Team.BLACK_TEAM) ? Brushes.Black : Brushes.Green;
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
                    Cell.cells[tag2.index].ShowInfo();
                }
                else
                {
                    Creature.creatures[tag2.index].ShowInfo();
                }
            }
        }

        void DetermineAvailableWays(bool plus, int range, int cell_num, List<int> cell_nums)
        {
            //Func<int, bool> check_func;
            //if (plus)
            //    check_func = j => cell_num + j < Cell.cells.Count;
            //else
            //    check_func = j => cell_num - j >= 0;

            //int s = plus ? 1 : -1;
            //int p = s; int i = p; int k = 0;
            //while (check_func(i) && k < range)
            //{
            //    cell_nums.Add(Cell.cells[cell_num - 1 + i].Number);
            //    i += p; k++;
            //}
            //p = s * 14; i = p; k = 0;
            //while (check_func(i) && k < range)
            //{
            //    cell_nums.Add(Cell.cells[cell_num - 1 + i].Number);
            //    i += p;
            //    k++;
            //}
            //p = s * 15; i = p; k = 0;
            //while (check_func(i) && k < range)
            //{
            //    cell_nums.Add(Cell.cells[cell_num - 1+ i].Number);
            //    i += p;
            //    k++;
            //}
            //p = s * 16; i = p; k = 0;
            //while (check_func(i) && k < range)
            //{
            //    cell_nums.Add(Cell.cells[cell_num - 1 + i].Number);
            //    i += p;
            //    k++;
            //}
        }

        private void LeftClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            List<int> cell_nums = new List<int>();
            if (e.OriginalSource is Rectangle)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                Tag2 tag2 = (Tag2)rect.Tag;
                if (tag2.IsCell)
                {
                    if (is_creature_selected)
                    {
                        
                    }
                }
                // Если клик был на существо
                else
                {
                    if (Creature.creatures[tag2.index].team == current_turn)
                    {
                        is_creature_selected = true;
                        int cell_num = Creature.creatures[tag2.index].cell.Number;
                        int range = Creature.creatures[tag2.index].Range;
                        DetermineAvailableWays(true, 1, cell_num, cell_nums);
                        DetermineAvailableWays(false, 1, cell_num, cell_nums);

                        foreach (int num in cell_nums)
                        {
                            Cell.cells[num - 1].Rec.Fill = new SolidColorBrush(Color.FromArgb(125, 0, 0, 0));
                        }
                    }
                }
            }
        }
    }
}