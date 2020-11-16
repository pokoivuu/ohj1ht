using Jypeli;
using Jypeli.Effects;
using Jypeli.Physics;
using Jypeli.Assets;
using Jypeli.WP7;
using Jypeli.Devices;
using Jypeli.Controls;
using Jypeli.GameObjects;
using Jypeli.Physics2d;
using Jypeli.Widgets;

public class Harjoitustyö : PhysicsGame
{
    private static Image[] ukkelinKavely = LoadImages("hirviö", "hirviöliikkuu2", "hirviöliikkuu");
    private static Image[] ukkelinLyonti = LoadImages("hirviö", "hirviölyö");
    private Animation kavely = new Animation(ukkelinKavely);
    int palikkaHealth = 3;

    SoundEffect explosionSound = LoadSoundEffect("exp");

    public override void Begin()

    {                
        PhysicsObject pelaaja = new PhysicsObject(150, 150, Shape.FromImage(LoadImage("hirviö")));        
        pelaaja.Image = LoadImage("hirviö.png");
                                    
        pelaaja.LinearDamping = 0.95;
        pelaaja.CanRotate = false;
        pelaaja.IgnoresExplosions = true;

        Add(pelaaja);
        LuoKentta();

        Level.Background.Image = LoadImage("Pelintausta.jpg");
        Level.Background.FitToLevel();

        Gravity = new Vector(0, -1000);
        
        Level.CreateBorders(1.0, false);
       

        Camera.ZoomToLevel(-50);
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Move left", pelaaja, new Vector(-10, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Move right", pelaaja, new Vector(10, 0));
        Keyboard.Listen(Key.Up, ButtonState.Pressed, PelaajaHyppaa, "Move up", pelaaja, new Vector(0, 1000));
        Keyboard.Listen(Key.Space, ButtonState.Pressed, PelaajaLyo, "Lyo", pelaaja);


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    // aliohjelma, jolla pelaaja liikkuu vaakasuorassa
    private void LiikutaPelaajaa(PhysicsObject pelaaja, Vector voima)
    {
        pelaaja.Hit(voima);             
    }

    // luodaan aliohjelma, jolla pelaaja hyppää
    private void PelaajaHyppaa(PhysicsObject pelaaja, Vector voima)
    {
        pelaaja.Hit(voima);
        pelaaja.Animation.Stop();
    }

    // luodaan aliohjelma, jolla pelaaja lyö
    private void PelaajaLyo(PhysicsObject pelaaja)
    {
        pelaaja.Animation = new Animation(ukkelinLyonti);
        pelaaja.Animation.Start(1);
        pelaaja.Animation.FPS = 5;       
    }


   // seuraava aliohjelma luo peliin kentän
   
    public void LuoKentta()
    {
        string[] kentta1 =
            {

  
  "                        ",
  "                        ",
  "                        ",
  "   ==         **        ",
  "   ==         **        ",
  "   ==   ++   ****       ",
  "   ==   ++   ****       ",
  "   ==   ++   ****       ",
  "------------------------"


        };
        string[] kentta2 =        
        {


  "                         ",
  "                         ",
  "         ++              ",
  "         ++              ",
  "   ===   ++    **        ",
  "   ===   ++   ****       ",
  "   ===   ++   ****       ",
  "   ===   ++   ****       ",
  "-------------------------"
        };

        string[] kentta3 =
{


  "                         ",
  "   ===                   ",
  "   ===   ++      *       ",
  "   ===   ++     **       ",
  "   ===   ++    ***       ",
  "   ===   ++   ****       ",
  "   ===   ++   ****       ",
  "   ===   ++   ****       ",
  "-------------------------"


        };
        // aliohjelma arpoo kolmesta eri kenttävaihtoehdosta yhden
        // ja luo sen pelattavaksi kentäksi
        TileMap ruudut = TileMap.FromStringArray(RandomGen.SelectOne<string[]>(kentta1, kentta2, kentta3));
        ruudut.SetTileMethod('=', LuoPalikka);       
        ruudut.SetTileMethod('*', LuoPalikka);
        ruudut.SetTileMethod('+', LuoPalikka);
        ruudut.SetTileMethod('-', LuoAsfaltti);
        ruudut.Execute();

   } 

    // luodaan peliin grafiikkaa eli asfaltti, joka tulee tason pohjalle
    private void LuoAsfaltti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject asfaltti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        asfaltti.Position = paikka;
        asfaltti.Shape = Shape.Rectangle;
        asfaltti.Image = LoadImage("tie");
        Add(asfaltti);
    }

    // luodaan rakkenusten palikat, joita pelaaja yrittää tuhota
    private void LuoPalikka(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject palikka = PhysicsObject.CreateStaticObject(leveys, korkeus);
        palikka.Position = paikka;
        palikka.Shape = Shape.Rectangle;
        palikka.Image = LoadImage("tiili");        
        AddCollisionHandler(palikka, PalikkaCollides);
        Add(palikka);
    }

    // luodaan törmäys pelaajan ja palikoiden välille
    // joka kerta kun pelaaja törmää palikan kanssa palikan kuva vaihtuu ja siltä lähtee yksi "elämä pois"
    private void PalikkaCollides(PhysicsObject palikka, PhysicsObject target)
    {
        
        palikkaHealth--;
        if (palikkaHealth == 2)
        {
            palikka.Image = LoadImage("tiili 2");
        }
        if (palikkaHealth == 1)
        {
            palikka.Image = LoadImage("tiili 3");
        }
        if (palikkaHealth <= 0)
        {
            palikkaHealth = 3;
            Explosion rajahdys = new Explosion(50);
            rajahdys.Position = palikka.Position;
            Add(rajahdys);
            palikka.Destroy();
        }
    }
}

