using System;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

// Our code will include eight classes; player, Score, rock, gem, shooter, falling objects, Point, & Game. 

namespace cse210_student_csharp_Greed
{
    // Main Function is to run the window and run the subsquent Class/Methods.
    public class GreedGame
    {
        public static void Main()
        {
        int ScreenHeight = 480;
        int ScreenWidth = 800;
        int RectangleSize = 15;
        int MovementSpeed = 4;
        int count = 0;
        int score = 0;
        var Objects = new List<Moving_objects>();
        Rectangle PlayerRectangle = new Rectangle(ScreenWidth - (RectangleSize * 2), ScreenHeight - (RectangleSize * 2), RectangleSize, RectangleSize);
        
        Fall fall = new Fall(MovementSpeed, Objects, ScreenHeight, ScreenWidth, RectangleSize, count, score);
        Player player = new Player(MovementSpeed, PlayerRectangle, RectangleSize);

        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Greed");
        Raylib.SetTargetFPS(60);
        

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            fall.newScore();
            player.Input();
            fall.Step(player.PlayerRectangle);
            player.drawPlayer();
        
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
        }
    }

    // Player deals with all Player inputs and Positioning for future reference.
    public class Player
    {
        public Player(int MovementSpeed, Rectangle Player, int size)
        {
            PlayerRectangle = Player;
            Speed = MovementSpeed;
            Size = size;
            // var playerPosition = new Vector2(PlayerRectangle.x, PlayerRectangle.y); 
        }
        public Rectangle PlayerRectangle;
        int Speed;
        int Size;
        // Vector2 playerPosition;
        
        public void Input()
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) 
            {
                PlayerRectangle.x += Speed;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) 
            {
                PlayerRectangle.x -= Speed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
            {
                // Fall.CreateObjects(2, -Speed, playerPosition, Size);
                Console.WriteLine("PEW");
            }
        }
        public void drawPlayer()
        {
            Raylib.DrawRectangleRec(PlayerRectangle, Color.WHITE); 
        }
        
    }
    // Fall is the creation and deletion of rocks, gems, and bolts within the scene also deals with collisions based on each frame.
    public class Fall
    {
        public Fall(int MovementSpeed, List<Moving_objects> Objects, int ScreenHeight, int ScreenWidth, int RectangleSize, int count, int score)
        {
            Speed = MovementSpeed;
            objects = Objects; 
            Height = ScreenHeight;
            Width = ScreenWidth;
            Size = RectangleSize;
            Count = count;
            Score = score;
        }
        int Speed;
        List<Moving_objects> objects;
        int Height;
        int Width;
        int Size;
        int Count;
        int Score;


        // Step is a method that creates movement variables and checks for collisions 
        public void Step(Rectangle PlayerRectangle)
        {
            var Random = new Random();
            int whichType = Random.Next(2);
            int randomY = Random.Next(2, Speed-1);
            int randomX = Random.Next(0, Width - Size);
            var position = new Vector2(randomX, 0 - Size);
            var objectsToRemove = new List<Moving_objects>();

            if (Count == 20)
            {
                CreateObjects(whichType, randomY, position, Size);
                Count = 0;
            }
            else
            {
                Count += 1;
            }

            foreach (var obj in objects) 
            {
                obj.Draw();
            }

            foreach (var obj in objects) 
            {
                obj.Move();
            }
            // Check for Collisions
            foreach (var obj in objects)
            {
                if(obj is Rock)
                {
                    Rock rock = (Rock)obj;
                    if (Raylib.CheckCollisionRecs(rock.eachRectangle(), PlayerRectangle)) 
                    {
                        subScore();
                        objectsToRemove.Add(obj);
                    }
                    foreach (var col in objects)
                    {
                        if (col is Bolt)
                        {
                            Bolt bolt = (Bolt)col;
                            if (Raylib.CheckCollisionRecs(bolt.eachboltRectangle(), rock.eachRectangle()))
                            {
                                addScore();
                                objectsToRemove.Add(obj);
                                objectsToRemove.Add(col);
                            }
                        }
                    }   
                }
                else if (obj is Gem)
                {
                    
                    Gem gem = (Gem)obj;
                    if (Raylib.CheckCollisionCircleRec(gem.Position, gem.Radius, PlayerRectangle)) 
                    {
                        addScore();
                        objectsToRemove.Add(obj);
                    }  
                    foreach (var col in objects)
                    {
                        if (col is Bolt)
                        {
                            Bolt bolt = (Bolt)col;
                            if (Raylib.CheckCollisionCircleRec(gem.Position, gem.Radius, bolt.eachboltRectangle()))
                            {
                                subScore();
                                objectsToRemove.Add(obj);
                                objectsToRemove.Add(col);
                            }
                        }
                    }   
                }

            }
            foreach (var obj in objects)
            {
                if (obj.Position.Y > Height + 20)
                {
                    objectsToRemove.Add(obj);
                }
            }
            objects = objects.Except(objectsToRemove).ToList();
        }
        // method for increasing player's score 
        public void addScore()
        {
            Score += 1;
        }
        // method for decreasing player's score 
        public void subScore()
        {
            Score -= 1;
        }
        // Method for displaying score 
        public void newScore()
        {
            int returnScore = Score;
            Raylib.DrawText("Score: " + returnScore , 12, 12, 20, Color.WHITE);
        }
        // Method that creates the individual objects and assigns them their speend and position 
        public void CreateObjects(int whichType, int randomY, Vector2 position, int Size)
        {
            switch (whichType) 
                {
                case 0:
                    var rock = new Rock(GenerateColor(), Size);
                    rock.Position = position;
                    rock.Velocity = new Vector2(0, randomY);
                    objects.Add(rock);
                    break;
                case 1:
                    var gem = new Gem(GenerateColor(), Size / 2);
                    gem.Position = position;
                    gem.Velocity = new Vector2(0, randomY);
                    objects.Add(gem);
                    break;
                case 2:
                    var bolt = new Bolt(Color.RED, Size);
                    bolt.Position = position;
                    bolt.Velocity = new Vector2(0, randomY);
                    objects.Add(bolt);
                    break;
                }
        }
        // Generates colors for the 
        public Color GenerateColor()
        {
            var Random = new Random();
            Color[] Colors = {Color.SKYBLUE, Color.BROWN, Color.BEIGE,Color.DARKPURPLE, Color.VIOLET, Color.PURPLE, Color.DARKBLUE, Color.BLUE, 
                        Color.BLACK, Color.DARKGREEN, Color.LIME, Color.GREEN, Color.MAROON, Color.RED, Color.PINK, Color.ORANGE, Color.GOLD, Color.YELLOW,
                        Color.DARKGRAY, Color.GRAY, Color.LIGHTGRAY, Color.BLANK, Color.MAGENTA, Color.RAYWHITE, Color.DARKBROWN, Color.WHITE}; 
            var randomColorNumber = Random.Next(0, Colors.Length);
            Color randomColor = Colors[randomColorNumber];
            return randomColor;
        }
    }
    // This is a Parent Class for the objects in the scene that Move to draw them and move them.
    abstract public class Moving_objects
    {
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Velocity { get; set; } = new Vector2(0, 0);

        virtual public void Draw() {
        // Base game objects do not have anything to draw
        }

        public void Move() 
        {
        Vector2 NewPosition = Position;
        NewPosition.Y += Velocity.Y;
        Position = NewPosition;
        }
    }
    // This Gets and produces the color for the objects
    public class ColoredObject: Moving_objects 
    {
        public Color Color { get; set; }
        public ColoredObject(Color color) { Color = color; }
    }
    // This is the individual rock objects handles each of the rocks information
    public class Rock: ColoredObject
    {
        public int Size { get; set; }
        public Rock(Color color, int size): base(color) {
            Size = size;
        }
        override public void Draw() {
            Raylib.DrawRectangleLines((int)Position.X, (int)Position.Y, Size, Size+(Size/3), Color);
        }
        public Rectangle eachRectangle()
        {
            Rectangle ownedRectangle = new Rectangle((int)Position.X, (int)Position.Y, Size, Size+(Size/3));
            return ownedRectangle;
        }
    }
     // This is the individual gem objects handles each of the gems information
    public class Gem: ColoredObject
    {
        public int Radius { get; set; }
        public Gem(Color color, int radius): base(color) 
        {
            Radius = radius;
        }
        override public void Draw() 
        {
            Raylib.DrawCircle((int)Position.X, (int)Position.Y, Radius, Color);
        }
    }
     // This is the individual bolt objects handles each of the bolts information
    public class Bolt: ColoredObject
    {
        public int Size { get; set; }
        public Bolt(Color color, int size): base(color) 
        {
            Size = size;
        }
        override public void Draw() {
            Raylib.DrawRectangle((int)Position.X, (int)Position.Y, Size, Size+(Size/3), Color);
        }
        public Rectangle eachboltRectangle()
        {
            Rectangle ownedboltRectangle = new Rectangle((int)Position.X, (int)Position.Y, Size, Size+(Size/3));
            return ownedboltRectangle;
        }

    }  
}