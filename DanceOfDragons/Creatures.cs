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
        public Team team { get; protected set; }
        public int Hp { get; set; }
        public int Dmg { get; protected set; }
        public int Range { get; protected set; } // Дальность передвижения(в ячейках)
        public Cell cell { get; protected set; } // Ячейка, на которой стоит существо
        protected string sprite; // Изображение существа
        public int Num { get; protected set; } // Номер существа
        public bool Is_used { get; set; } // Сходило ли существо
        public Rectangle Rec { get; protected set; }
        protected static int creature_num = 0; // Количество всего созданных существ
        static public List<Creature> creatures = new List<Creature>(); // Список всех созданных существ

        public bool Move(Cell to_cell)
        {
            to_cell.Occupied = true;
            cell.Occupied = false;
            cell = to_cell;
            Canvas.SetLeft(Rec, cell.PosX);
            Canvas.SetTop(Rec, cell.PosY - 1.5 * Cell.Width);
            Is_used = true;
            return true;
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
            Hp = hp;
            Dmg = dmg;
            Range = range;
            this.cell = cell;
            this.sprite = sprite;
            Num = creature_num;
            creature_num++;
            cell.Occupied = true;


            ImageBrush warriorSprite = new ImageBrush();
            warriorSprite.ImageSource = new BitmapImage(new Uri(MainWindow.images_path + "warriors/" + sprite));
            Rec = new Rectangle
            {
                Tag = new Tag2(false, Num),
                Height = 3 * Cell.Height,
                Width = 0.9 * Cell.Width,
                Fill = warriorSprite
            };
            Canvas.SetLeft(Rec, cell.PosX);
            Canvas.SetTop(Rec, cell.PosY - 1.5 * Cell.Width);
        }

        public override void ShowInfo()
        {
            string b_g = (team == Team.BLACK_TEAM) ? "Черные" : "Зеленые";
            MessageBox.Show("Тип существа: Воин" + Environment.NewLine + "Фракция: " + b_g + Environment.NewLine +
                "Здоровье: " + Hp + Environment.NewLine + "Урон: " + Dmg + Environment.NewLine
                + "Дальность передвижения: " + Range + Environment.NewLine +
                "Номер ячейки: " + cell.Number, "Информация о воине № " + Num);
        }
        public override void Attack(Creature creature)
        {
            creature.Hp -= Dmg;
            if (creature.Hp <= 0)
            {
                MainWindow.removeRects.Add(creatures[creature.Num].Rec);
                creature.cell.Occupied = false;
                creatures[creature.Num] = null;
            }
            // Ответная атака от противника
            else
            {
                Hp -= creature.Dmg;
                if (Hp <= 0)
                {
                    MainWindow.removeRects.Add(Rec);
                    cell.Occupied = false;
                    creatures[Num] = null;
                }
            }
        }
    }
}