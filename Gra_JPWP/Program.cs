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
            MyWindow window = new MyWindow();
            window.Show();
        }
    }

    class Airport 
    {
        public Texture texture;
        public Sprite sprite;
        public RectangleShape rectangle;
        public Airport()
        {
            texture = new Texture("Lotnisko_kradziony2.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f(40, 40);
            sprite.Position = new Vector2f(400, 300);
            rectangle = new RectangleShape(new Vector2f(250, 70)){
                Position = new Vector2f(sprite.Position.X + 150, sprite.Position.Y-25)
            };

        }
    }

    class obiekt
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
        public obiekt(int num) {
            this.number = num;
            x *= 1/(number+1);
            texture = new Texture("samlot_kradziony2.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f(40, 40);
            sprite.Position = new Vector2f(900, 600);
        }
        public void Instantiate()
        {
            
        }
        public void Move(double x, double y) { 
            sprite.Position += new Vector2f((float)x, (float)y);
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
                //Console.WriteLine(x + " " + y);
                Move(x, y);
                sprite.Rotation = (float)rotation + 180;
            } else Move(x, y);


        }
        public void GetLines(bool wasMousePressed, RenderWindow window)
        {
            positionOld = position;
            position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
            //if (Mouse.IsButtonPressed(Mouse.Button.Left) && sprite.GetGlobalBounds().Contains(position.X, position.Y))
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
                    Console.WriteLine(position.X + " " + position.Y);
                    Console.WriteLine(number);
                    lines.Append(new Vertex(new Vector2f(Mouse_x[Mouse_x.Count() - 1], Mouse_y[Mouse_x.Count() - 1]), new Color(0, 255, 0)));
                }
                else if (Math.Abs(Math.Abs(lines[lines.VertexCount - 1].Position.X) + Math.Abs(lines[lines.VertexCount - 1].Position.Y) - Math.Abs(position.X) - Math.Abs(position.Y)) > 4)
                    if (!sprite.GetGlobalBounds().Contains(lines[lines.VertexCount - 1].Position.X, lines[lines.VertexCount - 1].Position.Y))
                    {

                        Mouse_x.Add(position.X);
                        Mouse_y.Add(position.Y);

                        lines.Append(new Vertex(new Vector2f(Mouse_x[Mouse_x.Count() - 1], Mouse_y[Mouse_x.Count() - 1]), new Color(0, 255, 0)));
                    }
            }
        }

    }

    class MyWindow
    {
        bool sprawdzKolizje(Airport p1, obiekt p2)
        {
            if (p1.rectangle.GetGlobalBounds().Intersects(p2.sprite.GetGlobalBounds()) )
            {
                return true;
            }
            return false;
        }
        public void Show()
        {
            VideoMode mode = new VideoMode(1024, 768);
            RenderWindow window = new RenderWindow(mode, "SFML.NET");

            window.SetFramerateLimit(60);

            window.Closed += (obj, e) => { window.Close(); };
            window.KeyPressed +=
                (sender, e) =>
                {
                    Window window1 = (Window)sender;
                    if (e.Code == Keyboard.Key.Escape)
                    {
                        window1.Close();
                    }
                };
            Airport lotnisko1 = new Airport();

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

            Vector2i position = new Vector2i(0, 0);
            List<obiekt> samoloty = new List<obiekt>();
            samoloty.Add(new obiekt(samoloty.Count()));
            samoloty.Add(new obiekt(samoloty.Count()));
            samoloty.Add(new obiekt(samoloty.Count()));
            samoloty.Add(new obiekt(samoloty.Count()));

            foreach (var o in samoloty)
            {
                Console.WriteLine(o.number);
            }

            int selectedPlane = 99999;

            while (window.IsOpen)
            {
                delta = clock.Restart().AsSeconds();
                angle += angleSpeed * delta;
                window.DispatchEvents();
                window.Clear();
                text.Rotation = angle;
                

                position = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));

                //
                if (Mouse.IsButtonPressed(Mouse.Button.Left) && wasMousePressed == false)
                {
                    selectedPlane = 99999;
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
                    if (sprawdzKolizje(lotnisko1, o))
                        o.sprite.Position = new Vector2f(400, 300);
                }

                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    wasMousePressed = true;
                else wasMousePressed = false;

                window.Draw(lotnisko1.sprite);
                window.Draw(lotnisko1.rectangle);
                foreach (var o in samoloty)
                {
                    window.Draw(o.lines);
                    if (o.visible)
                        window.Draw(o.sprite);
                }

                //window.Draw(text);
                window.Display();
            }
        }
    }
}
