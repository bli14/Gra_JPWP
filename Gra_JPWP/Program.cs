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
                {
                    sel = Game.Show();
                }
                else if (sel == 1)
                {
                    sel = Game.Menu();
                }
                else if (sel == 2)
                {
                    sel = Game.Levels();
                }

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
            rectangle = new RectangleShape(new Vector2f(320, 60))
            {
                Position = new Vector2f(sprite.Position.X + 100, sprite.Position.Y - 20)
            };
        }
    }

    class AirplaneClass
    {
        public Texture texture;
        public Sprite sprite;
        public bool visible = true;
        public double x = -1, y = -1, rotation = 0;
        public int number;
        float GameSpeed = 1;

        List<float> Mouse_x = new List<float>();
        List<float> Mouse_y = new List<float>();

        public VertexArray lines = new VertexArray(PrimitiveType.Lines);
        uint VertexCounter = 0;
        Vector2f curPlanePos;
        Vector2i positionOld = new Vector2i(0, 0);
        Vector2i position = new Vector2i(0, 0);
        public AirplaneClass(int num, float GmS)
        {
            GameSpeed = GmS;
            number = num;
            x = 1;
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
            positionOld = position;
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

    }
    class MenuButton
    {
        public Sprite sprite;
        public Text text;
        public MenuButton(string typ, int offset, string title, int offset2)
        {
            sprite = new Sprite(new Texture(typ));
            sprite.Origin = new Vector2f(sprite.GetLocalBounds().Width / 2, sprite.GetLocalBounds().Height / 2);
            sprite.Position = new Vector2f(512 + offset2, 380 + offset);

            text = new Text(title, new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf"));
            text.CharacterSize = 40;
            text.Position = new Vector2f(362 + offset2, 515 + offset);
            text.FillColor = Color.White;
        }

    }

    class GameWindow
    {
        RenderWindow window = new RenderWindow(new VideoMode(1024, 768), "SFML.NET");
        SFML.Graphics.Font font = new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");
        int MenuAction = 1;
        float GameSpeed = 1;
        float LastTime = 0;
        bool CheckCollision(AirportClass p1, AirplaneClass p2)
        {
            if (p1.rectangle.GetGlobalBounds().Intersects(p2.sprite.GetGlobalBounds()))
            {
                return true;
            }
            return false;
        }
        void Setup()
        {
            window.SetFramerateLimit(60);

            window.Closed += (obj, e) => { window.Close(); };
            window.KeyPressed +=
                (sender, e) =>
                {
                    Window window1 = (Window)sender;
                    if (e.Code == Keyboard.Key.Escape)
                    {
                        MenuAction = 9;
                        //window1.Close();
                    }
                };
        }
        public int Show()
        {
            Setup();
            AirportClass lotnisko1 = new AirportClass();
            MenuAction = 0;
            int PlaneAmount = 5, SpawnedPlanes = 0;
            var PlaneStopwatch = new Stopwatch();
            var stopwatch = new Stopwatch();
            PlaneStopwatch.Start();
            stopwatch.Start();


            
            Text text = new Text("Hello World!", font);
            text.CharacterSize = 40;
            float textWidth = text.GetLocalBounds().Width;
            float textHeight = text.GetLocalBounds().Height;
            float xOffset = text.GetLocalBounds().Left;
            float yOffset = text.GetLocalBounds().Top;
            text.Origin = new Vector2f(textWidth / 2f + xOffset, textHeight / 2f + yOffset);
            text.Position = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);
            Clock clock = new Clock();
            float delta = 0f;
            float angle = 0f;
            float angleSpeed = 90f;

            bool wasMousePressed = false;

            RectangleShape FlyableArea;

            FlyableArea = new RectangleShape(new Vector2f(1500, 1100))
            {
                Position = new Vector2f(-238, -116)
            };

            Vector2i position = new Vector2i(0, 0);
            List<AirplaneClass> samoloty = new List<AirplaneClass>();

            int selectedPlane = 99999;

            while (window.IsOpen && MenuAction == 0)
            {
                delta = clock.Restart().AsSeconds();
                angle += angleSpeed * delta;
                if(PlaneStopwatch.ElapsedMilliseconds > 5000 / GameSpeed / GameSpeed / GameSpeed)
                {
                    if (PlaneAmount <= SpawnedPlanes){
                        if(samoloty.Count == 0)
                        {
                            LastTime = stopwatch.ElapsedMilliseconds / 1000;
                            stopwatch.Stop();
                            MenuAction = 1;
                        }
                    }
                    else
                    {
                        samoloty.Add(new AirplaneClass(SpawnedPlanes, GameSpeed));
                        Console.WriteLine(SpawnedPlanes);
                        PlaneStopwatch.Restart();
                        SpawnedPlanes++;
                    }                    
                }
                    

                window.DispatchEvents();
                window.Clear(new Color(200, 255, 255));
                text.Rotation = angle;


                position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));

                //
                if (Keyboard.IsKeyPressed(Keyboard.Key.P))
                {
                    MenuAction = 1;
                }
                if (Mouse.IsButtonPressed(Mouse.Button.Left) && wasMousePressed == false)
                {
                    selectedPlane = int.MaxValue;
                    foreach (var o in samoloty)
                    {
                        if (o.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                            selectedPlane = o.number;
                    }
                }

                foreach (var o in samoloty.ToList())
                {
                    if (selectedPlane == o.number)
                        o.GetLines(wasMousePressed, window);
                    o.MoveMath(window);
                    if (!FlyableArea.GetGlobalBounds().Intersects(o.sprite.GetGlobalBounds()))
                        o.ReverseFlight();
                    if (CheckCollision(lotnisko1, o))
                        samoloty.Remove(o);

                    foreach (var o2 in samoloty.ToList())
                    {
                        if (o != o2 && o2.sprite.GetGlobalBounds().Intersects(o.sprite.GetGlobalBounds()))
                        {
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

                window.Draw(lotnisko1.sprite);
                //window.Draw(lotnisko1.rectangle);
                //window.Draw(FlyableArea);
                foreach (var o in samoloty)
                {
                    window.Draw(o.lines);
                    window.Draw(o.sprite);
                }

                //window.Draw(text);
                window.Display();
            }
            return MenuAction;
        }
        public int Menu()
        {

            Setup();
            MenuButton button1;
            MenuButton button7;
            button1 = new MenuButton("button_wybor-poziomu.png", -120, "", 0);
            button7 = new MenuButton("button_wyjscie.png", 0, "", 0);

            Text TitleText = new Text("SkyMaster: Symulacja Lotniska", font);
            TitleText.CharacterSize = 70;
            TitleText.Position = new Vector2f(30, window.Size.Y / 9f);

            Text ScoreText = new Text("Twój czas: "+LastTime.ToString()+"s", font);
            if(LastTime == -1) ScoreText = new Text("Porażka! Spróbuj jeszcze raz.", font);
            ScoreText.CharacterSize = 40;
            ScoreText.Position = new Vector2f(window.Size.X / 2f - 120, window.Size.Y / 1.5f);

            while (MenuAction == 1)
            {
                window.DispatchEvents();
                window.Clear(new Color(0, 160, 255));
                Vector2i position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    if (button1.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 2;
                    if (button7.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 9;
                }




                window.Draw(button1.sprite);
                window.Draw(button1.text);
                window.Draw(button7.sprite);
                window.Draw(button7.text);
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
            MenuButton button1 = new MenuButton("button(1).png", 60, "", -120);
            MenuButton button2 = new MenuButton("button(2).png", 60, "", 0);
            MenuButton button3 = new MenuButton("button(3).png", 60, "", 120);
            MenuButton button7 = new MenuButton("button_menu.png", 180, "", 0);

            Text TitleText = new Text("SkyMaster: Symulacja Lotniska", font);
            TitleText.CharacterSize = 70;
            TitleText.Position = new Vector2f(30, window.Size.Y / 9f);

            while (MenuAction == 2)
            {
                window.DispatchEvents();
                window.Clear(new Color(0, 160, 255));
                Vector2i position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    if (button1.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                    {
                        MenuAction = 0;
                        GameSpeed = 1;
                    }
                    if (button2.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                    {
                        MenuAction = 0;
                        GameSpeed = 1.1f;
                    }
                    if (button3.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                    {
                        MenuAction = 0;
                        GameSpeed = 1.2f;
                    }
                    if (button7.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 1;
                }




                window.Draw(button1.sprite);
                window.Draw(button1.text);
                window.Draw(button2.sprite);
                window.Draw(button2.text);
                window.Draw(button3.sprite);
                window.Draw(button3.text);
                window.Draw(button7.sprite);
                window.Draw(button7.text);
                window.Draw(TitleText);
                window.Display();
            }
            return MenuAction;
        }
    }
}
