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

namespace Gra_JPWP
{

    class Program
    {
        static void Main(string[] args)
        {
            int sel = 0;
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
            rectangle = new RectangleShape(new Vector2f(320, 60)) {
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

        List<float> Mouse_x = new List<float>();
        List<float> Mouse_y = new List<float>();

        public VertexArray lines = new VertexArray(PrimitiveType.Lines);
        uint VertexCounter = 0;
        Vector2f curPlanePos;
        Vector2i positionOld = new Vector2i(0, 0);
        Vector2i position = new Vector2i(0, 0);
        public AirplaneClass(int num) {
            number = num;
            x = 1;
            texture = new Texture("samlot.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f(40, 40);
            sprite.Position = new Vector2f(100 * number, 600);
        }
        public void Move(double x, double y) {
            sprite.Position += new Vector2f((float)x, (float)y);
        }
        public void ReverseFlight()
        {
            x = -x;
            y = -y;
            sprite.Rotation = (float)rotation;
        }
        public void MoveMath(RenderWindow window) {
            if ((int)lines.VertexCount > (int)VertexCounter + 1)
            {
                if (Math.Abs(lines[VertexCounter].Position.X - curPlanePos.X) < 2 && Math.Abs(lines[VertexCounter].Position.Y - curPlanePos.Y) < 2)
                    VertexCounter += 1;
                double dx = curPlanePos.X - lines[VertexCounter].Position.X;
                double dy = curPlanePos.Y - lines[VertexCounter].Position.Y;


                curPlanePos = sprite.Position;

                rotation = (Math.Atan2(dy, dx)) * 180 / Math.PI;
                rotation += 180;

                x = 2 * Math.Cos((360 - rotation) * Math.PI / 180);
                y = -2 * Math.Sin((360 - rotation) * Math.PI / 180);

                Move(x, y);
                sprite.Rotation = (float)rotation + 180;
            } else Move(x, y);


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
            sprite = new Sprite(new Texture(typ+".png"));
            sprite.Origin = new Vector2f(sprite.GetLocalBounds().Width/2, sprite.GetLocalBounds().Height/2);
            sprite.Position = new Vector2f(512, 500 + offset);

            text = new Text(title, new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf"));
            text.CharacterSize = 40;
            text.Position = new Vector2f(362 + offset2, 515 + offset);
            text.FillColor = Color.White;
        }
        
    }

    class GameWindow
    {
        RenderWindow window = new RenderWindow(new VideoMode(1024, 768), "SFML.NET");
        int MenuAction = 0;
        bool CheckCollision(AirportClass p1, AirplaneClass p2)
        {
            if (p1.rectangle.GetGlobalBounds().Intersects(p2.sprite.GetGlobalBounds()) )
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



            SFML.Graphics.Font font = new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");
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

            FlyableArea = new RectangleShape(new Vector2f(1500, 1000)){
                Position = new Vector2f(-238, -116)
            };

            Vector2i position = new Vector2i(0, 0);
            List<AirplaneClass> samoloty = new List<AirplaneClass>();
            samoloty.Add(new AirplaneClass(samoloty.Count()));
            samoloty.Add(new AirplaneClass(samoloty.Count()));
            //samoloty.Add(new obiekt(samoloty.Count()));
            //samoloty.Add(new obiekt(samoloty.Count()));

            foreach (var o in samoloty)
            {
                Console.WriteLine(o.number);
            }

            int selectedPlane = 99999;

            while (window.IsOpen && MenuAction==0)
            {
                delta = clock.Restart().AsSeconds();
                angle += angleSpeed * delta;
                window.DispatchEvents();
                window.Clear(new Color(200,255,255));
                text.Rotation = angle;
                

                position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));

                //
                if(Keyboard.IsKeyPressed(Keyboard.Key.P))
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

                foreach (var o in samoloty)
                {
                    if (selectedPlane == o.number)
                        o.GetLines(wasMousePressed, window);
                    o.MoveMath(window);
                    if (!FlyableArea.GetGlobalBounds().Intersects(o.sprite.GetGlobalBounds()))
                        o.ReverseFlight();
                    if (CheckCollision(lotnisko1, o))
                        o.visible = false;
                    foreach (var o2 in samoloty)
                    {
                        if (o!=o2&&o2.sprite.GetGlobalBounds().Intersects(o.sprite.GetGlobalBounds()))
                        {
                            if(o2.visible && o.visible)
                            {
                                o2.visible = false;
                                o.visible = false;
                            }
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
                    if (o.visible)
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
            MenuButton button1 = new MenuButton("button_wybor-poziomu", 0, "", 35);
            MenuButton button2 = new MenuButton("button_wyjscie", 120, "", 35);
            while (true)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.L))
                {
                    return 0;
                }
                if (MenuAction == 9)
                    return MenuAction;
                window.DispatchEvents();
                window.Clear(new Color(0, 160, 255));
                


                
                window.Draw(button1.sprite);
                window.Draw(button1.text);
                window.Draw(button2.sprite);
                window.Draw(button2.text);
                window.Display();
            }
        }
    }
}
