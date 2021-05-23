using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace CaveSkirmish
{
    public class MapDirector
    {
        public SizeF Size => new SizeF(map.Dungeon.GetLength(0), map.Dungeon.GetLength(1));
        public Size LevelSize => new Size(map.Dungeon.GetLength(0), map.Dungeon.GetLength(1));

        private Map map;
        private Bitmap mapImage;

        private Point? lastMouseClick;
        private Point selected;
        private bool isSelected;
        private Turn currentTurn;
        private bool isNotTurned;

        public bool IsWon;
        public bool IsLost;        

        public MapDirector(Map currentMap)
        {
            map = currentMap;
            currentTurn = new Turn(map);
            CreateMap();
        }

        public void Update()
        {
            
        }

        public void OnMouseDown(Point location)
        {
            if (IsWon || IsLost)
                return;
            lastMouseClick = new Point(location.X, location.Y);
            var availableMove = map.Player.GetAvailable(map, currentTurn.EnemyShadows);
            if (isSelected)
            {
                if (lastMouseClick.Value.Equals(selected))
                {
                    map = currentTurn.UpdateMap(lastMouseClick.Value,
                        map.Player.GetAttackPattern(map, lastMouseClick.Value));
                    isNotTurned = true;
                }
                isSelected = false;
            }
            else if (map.InBounds(location) && availableMove.Contains(lastMouseClick.Value))
            {               
                selected = lastMouseClick.Value;
                isSelected = true;               
            }
        }

        public void OnMouseUp()
        {
            
        }

        public void Paint(Graphics g)
        {            
            g.SmoothingMode = SmoothingMode.AntiAlias;          
            DrawLevel(g);
            DrawTurn(g, currentTurn);
            if (isSelected)
            {
                var playerAttack = map.Player.GetAttackPattern(map, lastMouseClick.Value);
                DrawPlayerAtk(g, lastMouseClick.Value, playerAttack);                
            }               
            else
                DrawPlayerMove(g);

            if (map.Player.Health < 1)
            {
                IsLost = true;
            }
                
            if (map.Enemies.Count < 1)
            {
                IsWon = true;
            }
                
            if (isNotTurned)
            {
                currentTurn = new Turn(map);
                isNotTurned = false;
            }                
        }

        private void DrawLevel(Graphics graphics)
        {
            graphics.DrawImage(mapImage, new Rectangle(0, 0, LevelSize.Width, LevelSize.Height));          
        }

        private void DrawAttack(Graphics graphics, HashSet<Point> targets, Bitmap sprite)
        {
            foreach (var target in targets)
                graphics.DrawImage(sprite, new Rectangle(target.X, target.Y, 1, 1));
        }

        private void DrawShadows(Graphics graphics, Dictionary<Point, Enemy> shadows)
        {
            foreach (var shadow in shadows)
                graphics.DrawImage(EnemyMethods.EnemyToShadow(shadow.Value), new Rectangle(shadow.Key.X, shadow.Key.Y, 1, 1));
        }

        private void DrawTurn(Graphics graphics, Turn currentTurn)
        {
            DrawAttacks(graphics, currentTurn);
            DrawShadows(graphics, currentTurn.EnemyShadows);
            foreach (var enemy in map.Enemies)
                graphics.DrawImage(EnemyMethods.EnemyToBitmap(enemy), new Rectangle(enemy.Position.X, enemy.Position.Y, 1, 1));
            graphics.DrawImage(Sprites.Player, new Rectangle(map.Player.Position.X, map.Player.Position.Y, 1, 1));
        }

        private void DrawAttacks(Graphics graphics, Turn currentTurn)
        {
            DrawAttack(graphics, currentTurn.SingleAttacks, Sprites.SingleShot);
            DrawAttack(graphics, currentTurn.DoubleAttacks, Sprites.DoubleShot);
            DrawAttack(graphics, currentTurn.LethalAttacks, Sprites.LethalShot);
        }

        private void DrawPlayerAtk(Graphics graphics, Point lastClick, HashSet<Point> playerAtk)
        {
            graphics.DrawImage(Sprites.Select, new Rectangle(lastClick.X, lastClick.Y, 1, 1));
            foreach (var target in playerAtk)
                graphics.DrawImage(Sprites.SubSelect, new Rectangle(target.X, target.Y, 1, 1));
        }

        private void DrawPlayerMove(Graphics graphics)
        {
            foreach (var target in map.Player.GetAvailable(map, currentTurn.EnemyShadows))
                graphics.DrawImage(Sprites.SubMove, new Rectangle(target.X, target.Y, 1, 1));
        }

        private void CreateMap()
        {
            var cellWidth = Sprites.SpriteSize.Width;
            var cellHeight = Sprites.SpriteSize.Height;
            mapImage = new Bitmap(LevelSize.Width * cellWidth, LevelSize.Height * cellHeight);
            using (var graphics = Graphics.FromImage(mapImage))
            {
                for (var x = 0; x < map.Dungeon.GetLength(0); x++)
                {
                    for (var y = 0; y < map.Dungeon.GetLength(1); y++)
                    {
                        var image = map.Dungeon[x, y] == MapCell.Wall 
                            ? Sprites.Wall 
                            : Sprites.Grass;
                        graphics.DrawImage(image, new Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight));
                    }
                }
            }
        }        
    }
}
