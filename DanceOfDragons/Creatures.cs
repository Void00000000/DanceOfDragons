using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DanceOfDragons
{
    enum Team
    {
        BLACK_TEAM,
        GREEN_TEAM
    }


    abstract class Creature
    {
        public string Name { get; protected set; } // Имя существа
        public Team team { get; protected set; }
        public int Hp { get; set; }
        public int Dmg { get; protected set; }
        public int Range { get; protected set; } // Дальность передвижения(в ячейках)
        public Cell cell { get; protected set; } // Ячейка, на которой стоит существо
        protected string sprite; // Изображение существа
        public bool Large { get; protected set; } // Является ли существо большим (большие существа занимают две клетки) 
        public int Num { get; protected set; } // Номер существа
        public bool Is_used { get; set; } // Сходило ли существо
        protected bool dir_left; // Куда смотрит изображение (влево или вправо)
        public Rectangle Rec { get; set; }
        public double Offset_x = 0;
        public double Offset_y = 1.5 * Cell.Width;
        protected static int creature_num = 0; // Количество всего созданных существ
        static public List<Creature> creatures = new List<Creature>(); // Список всех созданных существ
        static public int greens_num = 0; // Количество существ партии "Зеленые"
        static public int blacks_num = 0; // Количество существ партии "Черные"
        static public string images_path;

        public Creature(string name, Team team, int hp, int dmg, int range, Cell cell, string sprite, bool large)
        {
            Name = name;
            this.team = team;
            Hp = hp;
            Dmg = dmg;
            Range = range;
            this.cell = cell;
            this.sprite = sprite;
            Large = large;
            Num = creature_num;
            creature_num++;
            if (team == Team.BLACK_TEAM) dir_left = false; else dir_left = true;
            if (team == Team.BLACK_TEAM) blacks_num++; else greens_num++;
            cell.Occupied = true;
            if (large) Cell.cells[Cell.to_i(cell.Number)][Cell.to_j(cell.Number) + 1].Occupied = true;

            ImageBrush warriorSprite = new ImageBrush();
            warriorSprite.ImageSource = new BitmapImage(new Uri(images_path + sprite));
            Rec = new Rectangle
            {
                Tag = new Tag2(false, Num),
                Height = 3 * Cell.Height, // Размеры по умолчанию
                Width = 0.9 * Cell.Width, // Размеры по умолчанию
                Fill = warriorSprite
            };
            Canvas.SetLeft(Rec, cell.PosX - Offset_x);
            Canvas.SetTop(Rec, cell.PosY - Offset_y);
        }

        // Отражает изображение по оси x
        protected void FlipLeft()
        {
            var newScaleTransform = new ScaleTransform();
            if (team == Team.BLACK_TEAM)
            {
                newScaleTransform.ScaleX = -1;
                Offset_x -= Rec.Width;
            }
            else
            {
                newScaleTransform.ScaleX = 1;
                Offset_x += Rec.Width;
            }
            Rec.RenderTransform = newScaleTransform;
            
            dir_left = true;
        }
        protected void FlipRight()
        {
            var newScaleTransform = new ScaleTransform();
            if (team == Team.BLACK_TEAM)
            {
                newScaleTransform.ScaleX = 1;
                Offset_x += Rec.Width;
            }
            else
            {
                newScaleTransform.ScaleX = -1;
                Offset_x -= Rec.Width;
            }
            Rec.RenderTransform = newScaleTransform;
            dir_left = false;
        }

        public bool Move(Cell to_cell)
        {
            if (!dir_left && Cell.to_j(to_cell.Number) - Cell.to_j(cell.Number) < 0)
                FlipLeft();
            if (dir_left && Cell.to_j(to_cell.Number) - Cell.to_j(cell.Number) > 0)
                FlipRight();
            cell.Occupied = false;
            if (Large) Cell.cells[Cell.to_i(cell.Number)][Cell.to_j(cell.Number) + 1].Occupied = false;
            to_cell.Occupied = true;
            if (Large) Cell.cells[Cell.to_i(to_cell.Number)][Cell.to_j(to_cell.Number) + 1].Occupied = true;
            cell = to_cell;
            Canvas.SetLeft(Rec, cell.PosX - Offset_x);
            Canvas.SetTop(Rec, cell.PosY - Offset_y);
            Is_used = true;
            return true;
        }
        virtual public void DetermineAvailableWays(int i, int j, int range, bool origin, HashSet<int> cell_nums, 
            HashSet<HashValue> cache)
        {
            // HashSet нужен для мемоизации
            int cell_num = Cell.to_cell_num(i, j);
            HashValue hashValue = new HashValue(cell_num, range);
            if (cache.Contains(hashValue))
                return;
            if (range >= 0 && i < Cell.n_ver && i >= 0
                && j < Cell.n_hor && j >= 0 && (origin || !Cell.cells[i][j].Occupied))
            {
                if ( !Large || j + 1 < Cell.n_hor && (origin || !Cell.cells[i][j+1].Occupied))
                {
                    cell_nums.Add(cell_num);
                    cache.Add(hashValue);
                    DetermineAvailableWays(i + 1, j, range - 1, false, cell_nums, cache);
                    DetermineAvailableWays(i - 1, j, range - 1, false, cell_nums, cache);
                    DetermineAvailableWays(i, j + 1, range - 1, false, cell_nums, cache);
                    DetermineAvailableWays(i, j - 1, range - 1, false, cell_nums, cache);
                    DetermineAvailableWays(i + 1, j - 1, range - 1, false, cell_nums, cache);
                    DetermineAvailableWays(i + 1, j + 1, range - 1, false, cell_nums, cache);
                    DetermineAvailableWays(i - 1, j - 1, range - 1, false, cell_nums, cache);
                    DetermineAvailableWays(i - 1, j + 1, range - 1, false, cell_nums, cache);
                }
            }
        }

        abstract public void Attack(Creature creature);
        public void ShowInfo()
        {
            string fraction = (team == Team.BLACK_TEAM) ? "Черные" : "Зеленые";
            string size = Large ? "Большой" : "Маленький";
            MessageBox.Show("Фракция: " + fraction + Environment.NewLine +
                "Здоровье: " + Hp + Environment.NewLine + "Урон: " + Dmg + Environment.NewLine
                + "Дальность передвижения: " + Range + Environment.NewLine +
                "Номер ячейки: " + cell.Number +  Environment.NewLine +
                "Размер: " + size ,Name + "|\t№: " + Num);
        }
    }


    // Боец ближнего боя
    class Warrior : Creature
    {
        public Warrior(string name, Team team, int hp, int dmg, int range, Cell cell, string sprite, bool large) :
            base(name, team, hp, dmg, range, cell, sprite, large)
        {
        }

        public override void Attack(Creature creature)
        {
            if (!dir_left && Cell.to_j(creature.cell.Number) - Cell.to_j(cell.Number) < 0)
            {
                FlipLeft();
                Canvas.SetLeft(Rec, cell.PosX - Offset_x);
                Canvas.SetTop(Rec, cell.PosY - Offset_y);
            }
            if (dir_left && Cell.to_j(creature.cell.Number) - Cell.to_j(cell.Number) > 0)
            {
                FlipRight();
                Canvas.SetLeft(Rec, cell.PosX - Offset_x);
                Canvas.SetTop(Rec, cell.PosY - Offset_y);
            }
            MessageBox.Show(Name + " нанёс урон " + creature.Name + " в размере " + Dmg + " ед.", "Информация об атаке");
            creature.Hp -= Dmg;
            if (creature.Hp <= 0)
            {
                if (creature.team == Team.BLACK_TEAM) blacks_num--; else greens_num--;
                MainWindow.removeRects.Add(creatures[creature.Num].Rec);
                creature.cell.Occupied = false;
                if (creature.Large) Cell.cells[Cell.to_i(creature.cell.Number)][Cell.to_j(creature.cell.Number) + 1].Occupied = false;
                creatures[creature.Num] = null;
            }
            // Ответная атака от противника
            else
            {
                MessageBox.Show(creature.Name + " нанёс урон " + Name + " в размере " + creature.Dmg + " ед.", "Информация об атаке");
                Hp -= creature.Dmg;
                if (Hp <= 0)
                {
                    if (team == Team.BLACK_TEAM) blacks_num--; else greens_num--;
                    MainWindow.removeRects.Add(Rec);
                    cell.Occupied = false;
                    if (Large) Cell.cells[Cell.to_i(cell.Number)][Cell.to_j(cell.Number) + 1].Occupied = false;
                    creatures[Num] = null;
                }
            }
        }
    }

    // Стрелки
    class RangedWarrior : Creature
    {
        public RangedWarrior(string name, Team team, int hp, int dmg, int range, Cell cell, string sprite, bool large) :
            base(name, team, hp, dmg, range, cell, sprite, large)
        {
        }

        public override void Attack(Creature creature)
        {
            if (!dir_left && Cell.to_j(creature.cell.Number) - Cell.to_j(cell.Number) < 0)
            {
                FlipLeft();
                Canvas.SetLeft(Rec, cell.PosX - Offset_x);
                Canvas.SetTop(Rec, cell.PosY - Offset_y);
            }
            if (dir_left && Cell.to_j(creature.cell.Number) - Cell.to_j(cell.Number) > 0)
            {
                FlipRight();
                Canvas.SetLeft(Rec, cell.PosX - Offset_x);
                Canvas.SetTop(Rec, cell.PosY - Offset_y);
            }
            MessageBox.Show(Name + " нанёс урон " + creature.Name + " в размере " + Dmg + " ед.", "Информация об атаке");
            creature.Hp -= Dmg;
            if (creature.Hp <= 0)
            {
                if (creature.team == Team.BLACK_TEAM) blacks_num--; else greens_num--;
                MainWindow.removeRects.Add(creatures[creature.Num].Rec);
                creature.cell.Occupied = false;
                if (creature.Large) Cell.cells[Cell.to_i(creature.cell.Number)][Cell.to_j(creature.cell.Number) + 1].Occupied = false;
                creatures[creature.Num] = null;
            }
        }
    }


    // Драконы
    class Dragon : Creature
    {
        public Dragon(string name, Team team, int hp, int dmg, int range, Cell cell, string sprite) :
            base(name, team, hp, dmg, range, cell, sprite, true) // Драконы всегда большие
        {
        }

        public override void Attack(Creature creature)
        {
            if (!dir_left && Cell.to_j(creature.cell.Number) - Cell.to_j(cell.Number) < 0)
            {
                FlipLeft();
                Canvas.SetLeft(Rec, cell.PosX - Offset_x);
                Canvas.SetTop(Rec, cell.PosY - Offset_y);
            }
            if (dir_left && Cell.to_j(creature.cell.Number) - Cell.to_j(cell.Number) > 0)
            {
                FlipRight();
                Canvas.SetLeft(Rec, cell.PosX - Offset_x);
                Canvas.SetTop(Rec, cell.PosY - Offset_y);
            }
            MessageBox.Show(Name + " нанёс урон " + creature.Name + " в размере " + Dmg + " ед.", "Информация об атаке");
            creature.Hp -= Dmg;
            // Существо за противником тоже получает урон
            int i = Cell.to_i(cell.Number);
            int j = Cell.to_j(cell.Number);
            int i1 = Cell.to_i(creature.cell.Number);
            int j1 = Cell.to_j(creature.cell.Number);
            int i2 = i1 + i1 - i;
            int j2 = j1 + j1 - j;
            if (i1 == i2 && j1 - j > 0)
                j2--;
            int num2 = Cell.to_cell_num(i2, j2);
            int creature2_index = -1;
            foreach (Creature creature2 in creatures)
            {
                if (creature2 != null && creature2.cell.Number == num2)
                {
                    creature2_index = creature2.Num;
                    break;
                }
            }
            if (creature2_index > 0)
            {
                MessageBox.Show(Name + " нанёс урон " + creatures[creature2_index].Name + " в размере " + Dmg + " ед.", "Информация об атаке");
                creatures[creature2_index].Hp -= Dmg;
                if (creatures[creature2_index].Hp <= 0)
                {
                    if (creatures[creature2_index].team == Team.BLACK_TEAM) blacks_num--; else greens_num--;
                    MainWindow.removeRects.Add(creatures[creature2_index].Rec);
                    creatures[creature2_index].cell.Occupied = false;
                    if (creatures[creature2_index].Large)
                        Cell.cells[Cell.to_i(creatures[creature2_index].cell.Number)][Cell.to_j(creatures[creature2_index].cell.Number) + 1].Occupied = false;
                    creatures[creature2_index] = null;
                }
            }
            // Существо, которое выбрано
            if (creature.Hp <= 0)
            {
                if (creature.team == Team.BLACK_TEAM) blacks_num--; else greens_num--;
                MainWindow.removeRects.Add(creatures[creature.Num].Rec);
                creature.cell.Occupied = false;
                if (creature.Large) Cell.cells[Cell.to_i(creature.cell.Number)][Cell.to_j(creature.cell.Number) + 1].Occupied = false;
                creatures[creature.Num] = null;
            }
            // Ответная атака от противника
            else
            {
                MessageBox.Show(creature.Name + " нанёс урон " + Name + " в размере " + creature.Dmg + " ед.", "Информация об атаке");
                Hp -= creature.Dmg;
                if (Hp <= 0)
                {
                    if (team == Team.BLACK_TEAM) blacks_num--; else greens_num--;
                    MainWindow.removeRects.Add(Rec);
                    cell.Occupied = false;
                    Cell.cells[Cell.to_i(cell.Number)][Cell.to_j(cell.Number) + 1].Occupied = false;
                    creatures[Num] = null;
                }
            }

        }

        public override void DetermineAvailableWays(int i, int j, int range, bool origin, HashSet<int> cell_nums, 
            HashSet<HashValue> cache)
        {
            // HashSet нужен для мемоизации
            int cell_num = Cell.to_cell_num(i, j);
            HashValue hashValue = new HashValue(cell_num, range);
            if (cache.Contains(hashValue))
                return;
            if (range >= 0 && i < Cell.n_ver && i >= 0 && j < Cell.n_hor && j >= 0)
            {
                cache.Add(hashValue);
                if (j + 1 < Cell.n_hor && !Cell.cells[i][j].Occupied && !Cell.cells[i][j + 1].Occupied)
                    cell_nums.Add(cell_num);
                DetermineAvailableWays(i + 1, j, range - 1, false, cell_nums, cache);
                DetermineAvailableWays(i - 1, j, range - 1, false, cell_nums, cache);
                DetermineAvailableWays(i, j + 1, range - 1, false, cell_nums, cache);
                DetermineAvailableWays(i, j - 1, range - 1, false, cell_nums, cache);
                DetermineAvailableWays(i + 1, j - 1, range - 1, false, cell_nums, cache);
                DetermineAvailableWays(i + 1, j + 1, range - 1, false, cell_nums, cache);
                DetermineAvailableWays(i - 1, j - 1, range - 1, false, cell_nums, cache);
                DetermineAvailableWays(i - 1, j + 1, range - 1, false, cell_nums, cache);
            }
        }
    }
}