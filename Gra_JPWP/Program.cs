using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using Color = SFML.Graphics.Color;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Text = SFML.Graphics.Text;
using System.Diagnostics;
using System.Security.Permissions;

namespace Gra_JPWP
{

    class Program
    {
        static void Main(string[] args)
        {
            int sel = 1;
            GameWindow Game = new GameWindow();
            while (sel != 9)
            {
                if (sel == 0)
                    sel = Game.ShowGame();
                else if (sel == 1)
                    sel = Game.Menu();
                else if (sel == 2)
                    sel = Game.Levels();
            }
        }
    }

    class AirportClass
    {
        public Texture texture;
        public Sprite sprite;
        public RectangleShape rectangle;
        public AirportClass()
        {
            texture = new Texture("Lotnisko.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f(40, 40);
            sprite.Position = new Vector2f(400, 300);
            rectangle = new RectangleShape(new Vector2f(250, 60)){
                Position = new Vector2f(sprite.Position.X + 170, sprite.Position.Y - 20)
            };
        }
    }

    class AirplaneClass
    {
        public Texture texture;
        public Sprite sprite;

        List<float> Mouse_x = new List<float>();
        List<float> Mouse_y = new List<float>();

        public bool visible = true;
        public double x = 1, y = -1, rotation = 0;
        public int number;
        float GameSpeed = 1;
        public Stopwatch deatchwatch = new Stopwatch();
        


        public VertexArray lines = new VertexArray(PrimitiveType.Lines);
        uint VertexCounter = 0;
        Vector2f curPlanePos;
        Vector2i position = new Vector2i(0, 0);

        public AirplaneClass(int num, float GmS)
        {
            GameSpeed = GmS;
            number = num;
            texture = new Texture("samlot.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f(40, 40);
            Random rnd = new Random();

            int a = rnd.Next(0, 100);
            int bx = rnd.Next(0, 100), cy;
            if (bx > 50)
            {
                if(a > 50)
                    bx = 1050;
                else bx = -50;
                cy = rnd.Next(-50, 800);

            }
            else
            {
                if (a <= 50)
                    cy = 800;
                else cy = -50;
                bx = rnd.Next(-50, 1050);
            }
            
            sprite.Position = new Vector2f(bx, cy);
            double dx = 512 - bx;
            double dy = 384 - cy;
            rotation = (Math.Atan2(dy, dx)) * 180 / Math.PI;
            rotation += 180;

            x = -1 * Math.Cos((360 - rotation) * Math.PI / 180);
            y = 1 * Math.Sin((360 - rotation) * Math.PI / 180);

            Move(x, y);
            sprite.Rotation = (float)rotation;
        }

        public void Move(double x, double y)
        {
            sprite.Position += new Vector2f((float)x * GameSpeed, (float)y * GameSpeed);
        }

        public void ReverseFlight()
        {
            x = -x;
            y = -y;
            sprite.Rotation = (float)rotation;
        }

        public void MoveMath(RenderWindow window)
        {
            if ((int)lines.VertexCount > (int)VertexCounter + 1)
            {
                if (Math.Abs(lines[VertexCounter].Position.X - curPlanePos.X) < 2 && Math.Abs(lines[VertexCounter].Position.Y - curPlanePos.Y) < 2)
                    VertexCounter += 1;
                double dx = curPlanePos.X - lines[VertexCounter].Position.X;
                double dy = curPlanePos.Y - lines[VertexCounter].Position.Y;


                curPlanePos = sprite.Position;

                rotation = (Math.Atan2(dy, dx)) * 180 / Math.PI;
                rotation += 180;

                x = 1 * Math.Cos((360 - rotation) * Math.PI / 180);
                y = -1 * Math.Sin((360 - rotation) * Math.PI / 180);

                Move(x, y);
                sprite.Rotation = (float)rotation + 180;
            }
            else Move(x, y);
        }

        public void GetLines(bool wasMousePressed, RenderWindow window)
        {
            position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
            if (wasMousePressed == false && Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                lines.Clear();
                VertexCounter = 0;
            }
            else if (Mouse.IsButtonPressed(Mouse.Button.Left) && !sprite.GetGlobalBounds().Contains(position.X, position.Y))
            {
                if (lines.VertexCount == 0)
                {
                    Mouse_x.Add(position.X);
                    Mouse_y.Add(position.Y);

                    lines.Append(new Vertex(new Vector2f(Mouse_x[Mouse_x.Count() - 1], Mouse_y[Mouse_x.Count() - 1]), new Color(255, 0, 0)));
                }
                else if (Math.Abs(Math.Abs(lines[lines.VertexCount - 1].Position.X) + Math.Abs(lines[lines.VertexCount - 1].Position.Y) - Math.Abs(position.X) - Math.Abs(position.Y)) > 4)
                    if (!sprite.GetGlobalBounds().Contains(lines[lines.VertexCount - 1].Position.X, lines[lines.VertexCount - 1].Position.Y))
                    {
                        Mouse_x.Add(position.X);
                        Mouse_y.Add(position.Y);

                        lines.Append(new Vertex(new Vector2f(Mouse_x[Mouse_x.Count() - 1], Mouse_y[Mouse_x.Count() - 1]), new Color(255, 0, 0)));
                    }
            }
        }
        public void Land()
        {
            deatchwatch.Start();
        }
    }
    class MenuButton
    {
        public Sprite sprite;
        public Text text;
        public MenuButton(string typ, int offset, int offset2)
        {
            sprite = new Sprite(new Texture(typ));
            sprite.Origin = new Vector2f(sprite.GetLocalBounds().Width / 2, sprite.GetLocalBounds().Height / 2);
            sprite.Position = new Vector2f(512 + offset2, 380 + offset);

        }
    }

    class GameWindow
    {
        RenderWindow window = new RenderWindow(new VideoMode(1024, 768), "SkyMaster: Symulacja Lotniska");
        SFML.Graphics.Font font = new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");
        int MenuAction = 1;
        float GameSpeed = 1;
        float LastTime = 0;

        bool CheckCollision(AirportClass p1, AirplaneClass p2)
        {
            if (p1.rectangle.GetGlobalBounds().Intersects(p2.sprite.GetGlobalBounds()))
                return true;
            return false;
        }

        void Setup()
        {
            window.SetFramerateLimit(60);

            window.Closed += (obj, e) => { window.Close(); };
            window.KeyPressed +=
                (sender, e) =>{
                    Window window1 = (Window)sender;
                    if (e.Code == Keyboard.Key.Escape)
                        MenuAction = 9;
                };
        }

        public int ShowGame()
        {
            Setup();
            MenuAction = 0;

            AirportClass lotnisko1 = new AirportClass();
            int PlaneAmount = 5, SpawnedPlanes = 0;
            int selectedPlane = int.MaxValue;

            var PlaneStopwatch = new Stopwatch();
            var stopwatch = new Stopwatch();
            PlaneStopwatch.Start();
            stopwatch.Start();

            Text text = new Text();
            Text text2 = new Text();

            bool wasMousePressed = false;


            RectangleShape FlyableArea;
            List<AirplaneClass> samoloty = new List<AirplaneClass>();
            Vector2i position = new Vector2i(0, 0);

            FlyableArea = new RectangleShape(new Vector2f(1500, 1100)){
                Position = new Vector2f(-238, -116)
            };
            

            while (window.IsOpen && MenuAction == 0) //Główna pętla gry
            {
                if(PlaneStopwatch.ElapsedMilliseconds > 5000 / GameSpeed / GameSpeed / GameSpeed)
                {
                    if (PlaneAmount <= SpawnedPlanes){
                        if(samoloty.Count == 0){
                            LastTime = stopwatch.ElapsedMilliseconds / 1000;
                            stopwatch.Stop();
                            MenuAction = 1;
                        }
                    }
                    else{
                        samoloty.Add(new AirplaneClass(SpawnedPlanes, GameSpeed));
                        Console.WriteLine(SpawnedPlanes);
                        PlaneStopwatch.Restart();
                        SpawnedPlanes++;
                    }                    
                }
                
                window.DispatchEvents();
                window.Clear(new Color(200, 255, 255));
                position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));

                if (Keyboard.IsKeyPressed(Keyboard.Key.P))
                    MenuAction = 1;

                if (Mouse.IsButtonPressed(Mouse.Button.Left) && wasMousePressed == false){
                    selectedPlane = int.MaxValue;
                    foreach (var o in samoloty){
                        if (o.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                            selectedPlane = o.number;
                    }
                }

                foreach (var o in samoloty.ToList()){
                    if (selectedPlane == o.number)
                        o.GetLines(wasMousePressed, window);

                    o.MoveMath(window);

                    if (!FlyableArea.GetGlobalBounds().Intersects(o.sprite.GetGlobalBounds()))
                        o.ReverseFlight();

                    if (CheckCollision(lotnisko1, o))
                        o.Land();

                    if (o.deatchwatch.ElapsedMilliseconds > 0)
                    {
                        float coeff = 1 - (o.deatchwatch.ElapsedMilliseconds / 1000f);
                        o.sprite.Scale = new Vector2f(coeff, coeff);
                        if(o.deatchwatch.ElapsedMilliseconds > 1000)
                            samoloty.Remove(o);
                    }

                    foreach (var o2 in samoloty.ToList()){
                        if (o != o2 && o2.sprite.GetGlobalBounds().Intersects(o.sprite.GetGlobalBounds())){
                            samoloty.Remove(o2);
                            samoloty.Remove(o);
                            LastTime = -1;
                            MenuAction = 1;
                        }
                    }
                }

                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    wasMousePressed = true;
                else wasMousePressed = false;

                text = new Text("Pozostała ilość samolotów: " + (PlaneAmount - SpawnedPlanes), font);
                text2 = new Text("Czas: " + stopwatch.ElapsedMilliseconds / 1000 + "s", font);
                text.CharacterSize = 40;
                text2.CharacterSize = 40;
                text.Position = new Vector2f(10, 10);
                text2.Position = new Vector2f(820, 10);
                text.FillColor = Color.Black;
                text2.FillColor = Color.Black;

                window.Draw(lotnisko1.sprite);
                foreach (var o in samoloty){
                    window.Draw(o.lines);
                    window.Draw(o.sprite);
                }

                window.Draw(text);
                window.Draw(text2);
                window.Display();
            }
            return MenuAction;
        }
        public int Menu()
        {

            Setup();
            MenuButton button1;
            MenuButton button2;
            button1 = new MenuButton("button_wybor-poziomu.png", -80, 0);
            button2 = new MenuButton("button_wyjscie.png", 40, 0);

            Text TitleText = new Text("SkyMaster: Symulacja Lotniska", font);
            TitleText.CharacterSize = 70;
            TitleText.Position = new Vector2f(30, window.Size.Y / 9f);

            Text ScoreText = new Text("Twój czas: "+LastTime.ToString()+"s", font);
            ScoreText.Position = new Vector2f(window.Size.X / 2f - 120, window.Size.Y / 1.5f);
            if (LastTime == -1){
                ScoreText = new Text("Porażka! Spróbuj jeszcze raz.", font);
                ScoreText.Position = new Vector2f(window.Size.X / 2f - 240, window.Size.Y / 1.5f);
            }
            ScoreText.CharacterSize = 40;

            while (MenuAction == 1)
            {
                window.DispatchEvents();
                window.Clear(new Color(0, 160, 255));
                Vector2i position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
                if (Mouse.IsButtonPressed(Mouse.Button.Left)){
                    if (button1.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 2;
                    if (button2.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 9;
                }

                window.Draw(button1.sprite);
                window.Draw(button2.sprite);
                if(LastTime!=0)
                    window.Draw(ScoreText);
                window.Draw(TitleText);
                window.Display();
            }
            return MenuAction;
        }
        public int Levels()
        {
            Setup();
            MenuButton button1 = new MenuButton("button(1).png", 60, -150);
            MenuButton button2 = new MenuButton("button(2).png", 60, 0);
            MenuButton button3 = new MenuButton("button(3).png", 60, 150);
            MenuButton button4 = new MenuButton("button_menu.png", 180, 0);

            Text TitleText = new Text("SkyMaster: Symulacja Lotniska", font);
            TitleText.CharacterSize = 70;
            TitleText.Position = new Vector2f(30, window.Size.Y / 9f);

            while (MenuAction == 2)
            {
                window.DispatchEvents();
                window.Clear(new Color(0, 160, 255));
                Vector2i position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
                if (Mouse.IsButtonPressed(Mouse.Button.Left)){
                    if (button1.sprite.GetGlobalBounds().Contains(position.X, position.Y)){
                        MenuAction = 0;
                        GameSpeed = 1;
                    }
                    else if (button2.sprite.GetGlobalBounds().Contains(position.X, position.Y)){
                        MenuAction = 0;
                        GameSpeed = 1.1f;
                    }
                    else if (button3.sprite.GetGlobalBounds().Contains(position.X, position.Y)){
                        MenuAction = 0;
                        GameSpeed = 1.2f;
                    }
                    else if (button4.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 1;
                }

                window.Draw(button1.sprite);
                window.Draw(button2.sprite);
                window.Draw(button3.sprite);
                window.Draw(button4.sprite);
                window.Draw(TitleText);
                window.Display();
            }
            return MenuAction;
        }
    }
}
