using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Harjoitustyö : PhysicsGame
{
    private Image[] ukkelinKavely = LoadImages("hirviö", "hirviöliikkuu");
    public override void Begin()

    {

        PhysicsObject pelaaja = new PhysicsObject(100, 100, Shape.Rectangle);        
        pelaaja.Image = LoadImage("hirviö.png");
        pelaaja.Animation = new Animation(ukkelinKavely);
        pelaaja.LinearDamping = 0.95;

        Add(pelaaja);
        

        Level.Background.Image = LoadImage("Pelintausta.jpg");

        Gravity = new Vector(0, -1000);
        
        Level.CreateBorders(1.0, false);

        Camera.ZoomToLevel();
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Move left", pelaaja, new Vector(-10, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Move right", pelaaja, new Vector(10, 0));
        Keyboard.Listen(Key.Up, ButtonState.Pressed, LiikutaPelaajaa, "Move up", pelaaja, new Vector(0, 500));


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    void LiikutaPelaajaa(PhysicsObject pelaaja, Vector voima)
    {
        pelaaja.Hit(voima);
    }
}

