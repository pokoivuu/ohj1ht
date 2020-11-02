using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Harjoitustyö : PhysicsGame
{
    private Image[] ukkelinKavely = LoadImages("hirviö", "hirviöliikkuu2", "hirviöliikkuu");
    private Image[] ukkelinLyonti = LoadImages("hirviö", "hirviölyö");
    public override void Begin()

    {

        

         
        PhysicsObject pelaaja = new PhysicsObject(150, 150, Shape.FromImage(LoadImage("hirviö")));        
        pelaaja.Image = LoadImage("hirviö.png");
        
        // animaatio
        pelaaja.Animation = new Animation(ukkelinKavely);       
              

        pelaaja.LinearDamping = 0.95;
        pelaaja.CanRotate = false;

        Add(pelaaja);
        LuoKentta();

        Level.Background.Image = LoadImage("Pelintausta.jpg");
        Level.Background.FitToLevel();

        Gravity = new Vector(0, -1000);
        
        Level.CreateBorders(1.0, false);
       

        Camera.ZoomToLevel(-50);
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Move left", pelaaja, new Vector(-10, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Move right", pelaaja, new Vector(10, 0));
        Keyboard.Listen(Key.Up, ButtonState.Pressed, PelaajaHyppaa, "Move up", pelaaja, new Vector(0, 500));
        Keyboard.Listen(Key.Space, ButtonState.Pressed, PelaajaLyo, "Lyo", pelaaja);


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    void LiikutaPelaajaa(PhysicsObject pelaaja, Vector voima)
    {
        pelaaja.Hit(voima);
        pelaaja.Animation.Start();
        pelaaja.Animation.FPS = 10;

        
    }
    void PelaajaHyppaa(PhysicsObject pelaaja, Vector voima)
    {
        pelaaja.Hit(voima);
    }

    void PelaajaLyo(PhysicsObject pelaaja)
    {
        pelaaja.Animation = new Animation(ukkelinLyonti);
        pelaaja.Animation.Start(1);
        pelaaja.Animation.FPS = 5;
    }

    public void LuoKentta()
    {
        string[] kentta =
            {

  
  "                        ",
  "                        ",
  "                        ",
  "   ==         ==        ",
  "   ==         ==        ",
  "   ==   ==   ====       ",
  "   ==   ==   ====       ",
  "   ==   ==   ====       ",
  "------------------------"


        };
        TileMap ruudut = TileMap.FromStringArray(kentta);
        ruudut.SetTileMethod('=', LuoPalikka);
        ruudut.SetTileMethod('-', LuoAsfaltti);
        ruudut.Execute();


        void LuoAsfaltti(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject asfaltti = PhysicsObject.CreateStaticObject(leveys, korkeus);
            asfaltti.Position = paikka;
            asfaltti.Shape = Shape.Rectangle;
            asfaltti.Image = LoadImage("tie");
            Add(asfaltti);
        }

        void LuoPalikka(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject palikka = PhysicsObject.CreateStaticObject(leveys, korkeus);
            palikka.Position = paikka;
            palikka.Shape = Shape.Rectangle;
            palikka.Image = LoadImage("tiili");
            Add(palikka);
        }


    }
}

