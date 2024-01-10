using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Color = SFML.Graphics.Color;
using Text = SFML.Graphics.Text;
using System.Diagnostics;

// Autor: Daniel Niepla s188828, 10 Styczeń 2024

namespace Gra_JPWP
{
    class Program // Główna klasa programu
    {
        static void Main(string[] args)
        {
            int sel = 1;
            GameWindow Game = new GameWindow();
            while (sel != 9) // Wybór trybu działania programu - domyślnie menu główne, 9 = wyjście
            {
                if (sel == 0) // Gra
                    sel = Game.ShowGame();
                else if (sel == 1) // Menu główne
                    sel = Game.Menu();
                else if (sel == 2) // Menu wyboru poziomu
                    sel = Game.Levels();
            }
        }
    }

    class AirportClass // Klasa Lotniska
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

    class AirplaneClass // Klasa samolotu
    {
        public Texture texture;
        public Sprite sprite;

        public double x = 1, y = -1, rotation = 0; // Wektory prędkości i rotacji
        public int IdNumber; // Numer porządkowy samolotu
        float GameSpeed = 1; // Mnożnik prędkości samolotu
        public Stopwatch deatchwatch = new Stopwatch(); // Licznik odliczający czas w przypadku rozpoczęcia lądowania na lotnisku
        
        public VertexArray lines = new VertexArray(PrimitiveType.Lines);// Tablica linii, za których punktami ma podążać samolot
        uint VertexCounter = 0; // Licznik aktualnej linii
        Vector2i MousePos = new Vector2i(0, 0);

        public AirplaneClass(int num, float GmS) // Konstruktor
        {
            GameSpeed = GmS;
            IdNumber = num;
            texture = new Texture("samlot.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f(40, 40);
            Random rnd = new Random();

            int a = rnd.Next(0, 100); // Losowanie miejsca, z którego nadleci samolot
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
            
            sprite.Position = new Vector2f(bx, cy); // Ustawienie wylosowanej pozycji
            double dx = 512 - bx;
            double dy = 384 - cy;
            rotation = (Math.Atan2(dy, dx)) * 180 / Math.PI; // Obliczanie kąta obrotu w kierunku lotniska
            rotation += 180;

            x = -1 * Math.Cos((360 - rotation) * Math.PI / 180); // Obliczanie wektora przyspieszenia
            y = 1 * Math.Sin((360 - rotation) * Math.PI / 180);

            Move(x, y); // Wykonanie pierwszego ruchu
            sprite.Rotation = (float)rotation; // Obrócenie samolotu na obliczony kąt
        }

        public void Move(double x, double y) // Proste ruszenie o wcześniej wyliczone przyspieszenie
        {
            sprite.Position += new Vector2f((float)x * GameSpeed, (float)y * GameSpeed);
        }

        public void ReverseFlight() // Odwrócenie lotu, stosowane w wypadku wylecenia poza mapę
        {
            x = -x;
            y = -y;
            sprite.Rotation = (float)rotation;
        }

        public void MoveMath(RenderWindow window) // Wyliczanie kąta i wektora przyspieszenia w kierunku następnego punktu
        {
            if ((int)lines.VertexCount > (int)VertexCounter + 1) // Upewnienie się czy są następne punkty..
            {
                // Sprawdzenie, czy samolot osiągnął punkt
                if (Math.Abs(lines[VertexCounter].Position.X - sprite.Position.X) < 2 && Math.Abs(lines[VertexCounter].Position.Y - sprite.Position.Y) < 2)
                    VertexCounter += 1;

                double dx = sprite.Position.X - lines[VertexCounter].Position.X;
                double dy = sprite.Position.Y - lines[VertexCounter].Position.Y;

                rotation = (Math.Atan2(dy, dx)) * 180 / Math.PI; // Obliczanie kąta obrotu w kierunku punktu
                rotation += 180;

                x = 1 * Math.Cos((360 - rotation) * Math.PI / 180); // Obliczanie wektora przyspieszenia
                y = -1 * Math.Sin((360 - rotation) * Math.PI / 180);

                Move(x, y);
                sprite.Rotation = (float)rotation + 180;
            }
            else Move(x, y); // Jeśli nie ma następnego punktu, leć z dotychczasowym kierunkiem
        }

        public void GetLines(bool wasMousePressed, RenderWindow window) // Metoda służąca do rysowania linii
        {
            MousePos = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));
            if (wasMousePressed == false && Mouse.IsButtonPressed(Mouse.Button.Left)) // Jeżeli myszko została dopiero co wciśnięta, zacznij rysowanie od nowa
            {
                lines.Clear();
                VertexCounter = 0;
            }
            else if (Mouse.IsButtonPressed(Mouse.Button.Left) && !sprite.GetGlobalBounds().Contains(MousePos.X, MousePos.Y))
            {
                if (lines.VertexCount == 0)
                    lines.Append(new Vertex(new Vector2f(MousePos.X, MousePos.Y), new Color(255, 0, 0)));

                // Akceptowanie następnych punktów tylko w wypadku, jak są oddalone od ostatniego punktu o min. 4 pixele
                else if (Math.Abs(Math.Abs(lines[lines.VertexCount - 1].Position.X) + Math.Abs(lines[lines.VertexCount - 1].Position.Y) - Math.Abs(MousePos.X) - Math.Abs(MousePos.Y)) > 4)
                    if (!sprite.GetGlobalBounds().Contains(lines[lines.VertexCount - 1].Position.X, lines[lines.VertexCount - 1].Position.Y)) 
                        lines.Append(new Vertex(new Vector2f(MousePos.X, MousePos.Y), new Color(255, 0, 0)));
            }
        }
        public void Land() // Uruchumienie licznika lądowania na lotnisku
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

    class GameWindow // Główne okno programu
    {
        RenderWindow window = new RenderWindow(new VideoMode(1024, 768), "SkyMaster: Symulacja Lotniska");
        Font font = new Font("C:/Windows/Fonts/arial.ttf");
        int MenuAction = 1;
        float GameSpeed = 1; // Prędkość gry
        float LastTime = 0; // Zmienna zawierająca czas ostatniego podejścia

        bool CheckCollision(AirportClass p1, AirplaneClass p2) // Metoda do sprawdzania kolizji z lotniskiem
        {
            if (p1.rectangle.GetGlobalBounds().Intersects(p2.sprite.GetGlobalBounds()))
                return true;
            return false;
        }

        void Setup() // Podstawowa konfiguracja zachowania programu
        {
            window.SetFramerateLimit(60);

            window.Closed += (obj, e) => { window.Close(); };
            window.KeyPressed +=
                (sender, e) =>{
                    Window window1 = (Window)sender;
                    if (e.Code == Keyboard.Key.Escape) // Escape wyłącza grę
                        MenuAction = 9;
                };
        }

        public int ShowGame() // Metoda gry
        {
            Setup();
            MenuAction = 0;

            AirportClass lotnisko1 = new AirportClass();
            int PlaneAmount = 10, SpawnedPlanes = 0;
            int selectedPlane = int.MaxValue;

            var PlaneStopwatch = new Stopwatch(); // Stoper służący do odliczania czasu do wygenerowania kolejnego samolotu
            var stopwatch = new Stopwatch(); // Stoper służący do odliczania czasu gry
            PlaneStopwatch.Start();
            stopwatch.Start();

            Text text = new Text();
            Text text2 = new Text();

            bool wasMousePressed = false; // Typ logiczny przechowujący stan wciśnięcia lewego przycisku myszy w poprzedniej klatce


            RectangleShape FlyableArea; // Dozwolony obszar poruszania się samolotów
            List<AirplaneClass> samoloty = new List<AirplaneClass>(); // Lista zawierająca wszystkie samoloty

            FlyableArea = new RectangleShape(new Vector2f(1500, 1100)){ // Dozwolony obszar lotów
                Position = new Vector2f(-238, -116)
            };
            

            while (window.IsOpen && MenuAction == 0) // Główna pętla gry
            {
                if(PlaneStopwatch.ElapsedMilliseconds > 5000 / GameSpeed / GameSpeed / GameSpeed) // Warunek dodający kolejne samoloty co 5 sekund lub mniej, zależne od prędkości gry
                {
                    if (PlaneAmount <= SpawnedPlanes){ // Sprawdzenie warunku końca gry
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
                Vector2i MousePos = (Vector2i)window.MapPixelToCoords(Mouse.GetPosition(window));

                if (Keyboard.IsKeyPressed(Keyboard.Key.P))
                    MenuAction = 1;

                if (Mouse.IsButtonPressed(Mouse.Button.Left) && wasMousePressed == false){ // Rozpoczęcie rysowania linii po wybraniu konkretnego samolotu
                    selectedPlane = int.MaxValue;
                    foreach (var o in samoloty){
                        if (o.sprite.GetGlobalBounds().Contains(MousePos.X, MousePos.Y))
                            selectedPlane = o.IdNumber;
                    }
                }

                foreach (var o in samoloty.ToList()){ // Pętla wykonująca wymagane czynności na samolotach
                    if (selectedPlane == o.IdNumber)
                        o.GetLines(wasMousePressed, window);

                    o.MoveMath(window);

                    if (!FlyableArea.GetGlobalBounds().Intersects(o.sprite.GetGlobalBounds()))
                        o.ReverseFlight();

                    if (CheckCollision(lotnisko1, o))
                        o.Land();

                    if (o.deatchwatch.ElapsedMilliseconds > 0) // Proces lądowania samolotu na lotnisku
                    {
                        float coeff = 1 - (o.deatchwatch.ElapsedMilliseconds / 1000f);
                        o.sprite.Scale = new Vector2f(coeff, coeff);
                        if(o.deatchwatch.ElapsedMilliseconds > 1000)
                            samoloty.Remove(o);
                    }

                    foreach (var o2 in samoloty.ToList()){ // Sprawdzanie kolizji z wszystkimi innymi samolotami na planszy
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

                // Generowanie tekstów wyświetlanych na ekranie
                text = new Text("Pozostała ilość samolotów: " + (PlaneAmount - SpawnedPlanes), font);
                text2 = new Text("Czas: " + stopwatch.ElapsedMilliseconds / 1000 + "s", font);
                text.CharacterSize = 40;
                text2.CharacterSize = 40;
                text.Position = new Vector2f(10, 10);
                text2.Position = new Vector2f(820, 10);
                text.FillColor = Color.Black;
                text2.FillColor = Color.Black;

                // Rysowanie obiektów
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
        public int Menu() // Metoda menu głównego
        {
            Setup();
            MenuButton ChoiceButton;
            MenuButton ExitButton;
            ChoiceButton = new MenuButton("button_wybor-poziomu.png", -80, 0);
            ExitButton = new MenuButton("button_wyjscie.png", 40, 0);

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
                    if (ChoiceButton.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 2; // Przejście do wyboru poziomu
                    if (ExitButton.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 9; // Wyjście z gry
                }

                // Rysowanie obiektów
                window.Draw(ChoiceButton.sprite);
                window.Draw(ExitButton.sprite);
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
            MenuButton MenuButton = new MenuButton("button_menu.png", 180, 0);

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
                        MenuAction = 0; // Rozpoczęcie gry, prędkość domyślna
                        GameSpeed = 1;
                    }
                    else if (button2.sprite.GetGlobalBounds().Contains(position.X, position.Y)){
                        MenuAction = 0; // Rozpoczęcie gry, prędkość przyspieszona
                        GameSpeed = 1.1f;
                    }
                    else if (button3.sprite.GetGlobalBounds().Contains(position.X, position.Y)){
                        MenuAction = 0; // Rozpoczęcie gry, prędkość najwyższa
                        GameSpeed = 1.2f;
                    }
                    else if (MenuButton.sprite.GetGlobalBounds().Contains(position.X, position.Y))
                        MenuAction = 1; // Powrót do menu głównego
                }

                // Rysowanie obiektów
                window.Draw(button1.sprite);
                window.Draw(button2.sprite);
                window.Draw(button3.sprite);
                window.Draw(MenuButton.sprite);
                window.Draw(TitleText);
                window.Display();
            }
            return MenuAction;
        }
    }
}