using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DanceOfDragons
{
    enum DragonNames
    {
        // Черные
        Vermithor,
        // Зеленые
        Vhagar
    }
    internal class Creator
    {
 
        static void CreateWarrior_1(Team team, int i, int j)
        {
            char b_g = (team == Team.BLACK_TEAM) ? 'b' : 'g';  
            Warrior warrior = new Warrior("Алебардщик", team, 100, 50, 3, Cell.cells[i][j], $"warriors/halberdier/halberdier_{b_g}0.png", false);
            warrior.Rec.Height = 3 * Cell.Height;
            warrior.Offset_y = 2 * Cell.Height;
            Canvas.SetTop(warrior.Rec, warrior.cell.PosY - warrior.Offset_y);
            Creature.creatures.Add(warrior);
        }
        static void CreateWarrior_2(Team team, int i, int j)
        {
            char b_g = (team == Team.BLACK_TEAM) ? 'b' : 'g';
            Warrior warrior = new Warrior("Рыцарь", team, 300, 150, 2, Cell.cells[i][j], $"warriors/crusader/crusader_{b_g}0.png", false);
            warrior.Offset_y = 2 * Cell.Height;
            Canvas.SetTop(warrior.Rec, warrior.cell.PosY - warrior.Offset_y);
            Creature.creatures.Add(warrior);
        }
        static void CreateRangedWarrior_1(Team team, int i, int j)
        {
            char b_g = (team == Team.BLACK_TEAM) ? 'b' : 'g';
            RangedWarrior rangedWarrior = new RangedWarrior("Стрелок", team, 100, 90, 1, Cell.cells[i][j], $"ranged_warriors/marksman/marksman_{b_g}0.png", false);
            rangedWarrior.Rec.Height = 2 * Cell.Height;
            rangedWarrior.Offset_y = Cell.Height;
            Canvas.SetTop(rangedWarrior.Rec, rangedWarrior.cell.PosY - rangedWarrior.Offset_y);
            Creature.creatures.Add(rangedWarrior);
        }
        static void CreateRangedWarrior_2(Team team, int i, int j)
        {
            char b_g = (team == Team.BLACK_TEAM) ? 'b' : 'g';
            RangedWarrior rangedWarrior = new RangedWarrior("Баллиста", team, 900, 500, 0, Cell.cells[i][j], $"ranged_warriors/ballista/ballista_{b_g}0.png", true);
            rangedWarrior.Rec.Height = 1.5 * Cell.Height;
            rangedWarrior.Rec.Width = 3.25 * Cell.Width;
            rangedWarrior.Offset_x = 0.75 * Cell.Width;
            rangedWarrior.Offset_y = 0.5 * Cell.Height;
            Canvas.SetLeft(rangedWarrior.Rec, rangedWarrior.cell.PosX - rangedWarrior.Offset_x);
            Canvas.SetTop(rangedWarrior.Rec, rangedWarrior.cell.PosY - rangedWarrior.Offset_y);
            Creature.creatures.Add(rangedWarrior);
        }

        static void CreateDragon(DragonNames dragon_name, int i, int j)
        {
            Dragon dragon;
            switch (dragon_name)
            {
                case DragonNames.Vermithor:
                    dragon = new Dragon("Вермитор", Team.BLACK_TEAM, 2500, 500, 7, Cell.cells[i][j], $"dragons/vermithor/vermithor_b0.png");
                    dragon.Rec.Height = 1.6 * Cell.Width;
                    dragon.Rec.Width = 1.8 * Cell.Width;
                    dragon.Offset_x = -0.1 * Cell.Width;
                    dragon.Offset_y = 1.5 * Cell.Height;
                    Canvas.SetTop(dragon.Rec, dragon.cell.PosX - dragon.Offset_x);
                    Canvas.SetTop(dragon.Rec, dragon.cell.PosY - dragon.Offset_y);
                    Creature.creatures.Add(dragon);
                    break;
                case DragonNames.Vhagar:
                    dragon = new Dragon("Вхагар", Team.GREEN_TEAM, 3000, 750, 7, Cell.cells[i][j], $"dragons/vhagar/vhagar_g0.png");
                    dragon.Rec.Height = 1.6 * Cell.Width;
                    dragon.Rec.Width = 1.8 * Cell.Width;
                    dragon.Offset_x = -0.1 * Cell.Width;
                    dragon.Offset_y = 1.5 * Cell.Height;
                    Canvas.SetTop(dragon.Rec, dragon.cell.PosX - dragon.Offset_x);
                    Canvas.SetTop(dragon.Rec, dragon.cell.PosY - dragon.Offset_y);
                    Creature.creatures.Add(dragon);
                    break;
            }
        }

        //Создание существ
        public static void CreateCreatures()
        {
            //Создание существ партии "Чёрные"
            for (int i = 0; i < Cell.n_ver; i += 2)
                CreateWarrior_1(Team.BLACK_TEAM, i, 4);
            //for (int i = 0; i < Cell.n_ver; i += 2)
            //    CreateWarrior_1(Team.BLACK_TEAM, i, 3);
            for (int i = 2; i < 8; i += 2)
                CreateRangedWarrior_1(Team.BLACK_TEAM, i, 2);

            CreateRangedWarrior_2(Team.BLACK_TEAM, 0, 0);
            CreateRangedWarrior_2(Team.BLACK_TEAM, Cell.n_ver - 1, 0);
            CreateWarrior_2(Team.BLACK_TEAM, 1, 0);
            //CreateWarrior_2(Team.BLACK_TEAM, 1, 1);
            CreateWarrior_2(Team.BLACK_TEAM, 0, 2);
            //CreateWarrior_2(Team.BLACK_TEAM, 1, 2);

            CreateWarrior_2(Team.BLACK_TEAM, Cell.n_ver - 2, 0);
            //CreateWarrior_2(Team.BLACK_TEAM, Cell.n_ver - 2, 1);
            CreateWarrior_2(Team.BLACK_TEAM, Cell.n_ver - 2, 2);
            //CreateWarrior_2(Team.BLACK_TEAM, Cell.n_ver - 1, 2);

            CreateDragon(DragonNames.Vermithor, 4, 0);

            //Создание существ партии "Зелёные"
            for (int i = 0; i < Cell.n_ver; i += 2)
                CreateWarrior_1(Team.GREEN_TEAM, i, Cell.n_hor - 1 - 4);
            //for (int i = 0; i < Cell.n_ver; i += 2)
            //    CreateWarrior_1(Team.GREEN_TEAM, i, Cell.n_hor - 1 - 3);
            for (int i = 2; i < 8; i += 2)
                CreateRangedWarrior_1(Team.GREEN_TEAM, i, Cell.n_hor - 1 - 2);

            CreateRangedWarrior_2(Team.GREEN_TEAM, 0, Cell.n_hor - 1 - 1 - 0);
            CreateRangedWarrior_2(Team.GREEN_TEAM, Cell.n_ver - 1, Cell.n_hor - 1 - 1 - 0);
            CreateWarrior_2(Team.GREEN_TEAM, 1, Cell.n_hor - 1 - 0);
            //CreateWarrior_2(Team.GREEN_TEAM, 1, Cell.n_hor - 1 - 1);
            CreateWarrior_2(Team.GREEN_TEAM, 0, Cell.n_hor - 1 - 2);
            //CreateWarrior_2(Team.GREEN_TEAM, 1, Cell.n_hor - 1 - 2);

            CreateWarrior_2(Team.GREEN_TEAM, Cell.n_ver - 2, Cell.n_hor - 1 - 0);
            //CreateWarrior_2(Team.GREEN_TEAM, Cell.n_ver - 2, Cell.n_hor - 1 - 1);
            CreateWarrior_2(Team.GREEN_TEAM, Cell.n_ver - 2, Cell.n_hor - 1 - 2);
            //CreateWarrior_2(Team.GREEN_TEAM, Cell.n_ver - 1, Cell.n_hor - 1 - 2);

            CreateDragon(DragonNames.Vhagar, 4, Cell.n_hor - 2);
        }
        //public static void CreateCreatures()
        //{
        //    CreateRangedWarrior_1(Team.BLACK_TEAM, 0, 5);
        //    CreateRangedWarrior_2(Team.BLACK_TEAM, 1, 5);
        //    CreateWarrior_1(Team.BLACK_TEAM, 2, 5);
        //    CreateWarrior_2(Team.BLACK_TEAM, 3, 5);
        //    CreateDragon(DragonNames.Vermithor, 4, 5);

        //    CreateRangedWarrior_1(Team.GREEN_TEAM, 0, Cell.n_hor - 1 - 5);
        //    CreateRangedWarrior_2(Team.GREEN_TEAM, 1, Cell.n_hor - 2 - 5);
        //    CreateWarrior_1(Team.GREEN_TEAM, 2, Cell.n_hor - 1 - 5);
        //    CreateWarrior_2(Team.GREEN_TEAM, 3, Cell.n_hor - 1 - 5);
        //    CreateDragon(DragonNames.Vhagar, 4, Cell.n_hor - 2 - 5);
        //}
    }
}