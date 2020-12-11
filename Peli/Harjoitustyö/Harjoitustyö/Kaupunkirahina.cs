using Jypeli;
using Jypeli.Effects;
using Jypeli.Assets;
using Jypeli.Widgets;

// TODO: funktio, ks: https://tim.jyu.fi/view/kurssit/tie/ohj1/2020s/demot/demo6?answerNumber=10&b=zKDlPxnfUKS5&size=1&task=miidijaetsilahin&user=pokoivuu
// Tehtävässä esimerkki funktiosta, sillä pelissä ei sellaista ole

/// @author Pauli Koivuniemi
/// @version 11.12.2020
/// <summary>
/// Pelissä pelaajan eli hirviön tehtävä on tuhota kaikki rakennukset, ja välttää vihollisten luoteja. Pelaaja voittaa jos saa kaikki rakennukset tuhottua
/// </summary>
public class Kaupunkirahina : PhysicsGame
{        
    private static Image[] ukkelinLyonti = LoadImages("hirviö", "hirviölyö");
    private static Image pelaajanKuva = LoadImage("hirviö");
    private static Image vihunKuva = LoadImage("rotta");
    private static Image helinKuva = LoadImage("heli");
       
    private bool liikkuiVasemmalle = false;

    private int palikkaHealth = 3;
    private int talonHealth1 = 5;
    private int talonHealth2 = 5;
    private int talonHealth3 = 5;
    private IntMeter pisteLaskuri;

    private SoundEffect osumisAani = LoadSoundEffect("sfx_sounds_impact7");
    private SoundEffect lyontiAani = LoadSoundEffect("pickaxe");
    private SoundEffect hyppyAani = LoadSoundEffect("sfx_movement_jump19");
    private DoubleMeter elamaLaskuri;

    /// <summary>
    /// Begin aliohjelma
    /// </summary>
    public override void Begin()
    {                
        PhysicsObject pelaaja = new PhysicsObject(100, 100);
        pelaaja.Image = pelaajanKuva;                      
        pelaaja.LinearDamping = 0.95;
        pelaaja.CanRotate = false;
        pelaaja.IgnoresExplosions = true;
        pelaaja.CollisionIgnoreGroup = 1;
        pelaaja.Tag = "pelaaja";
           
        AddCollisionHandler(pelaaja, "vihu", PelaajaCollides);
        LuoElamaLaskuri();
        LuoPistelaskuri();
        Add(pelaaja);;

        LuoKentta();
        LuoLentavaVihollinen(pelaaja, helinKuva, new Vector(200, 200), "vihu");       
        LuoVihollinen(pelaaja, vihunKuva, new Vector(RandomGen.NextDouble(-150, -100),  -100), "vihu");      
        
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


    /// <summary>
    /// aliohjelma, jolla pelaaja liikkuu vaakasuorassa x-akselin mukaan
    /// </summary>
    /// <param name="pelaaja">pelaajan kuva</param>
    /// <param name="voima">voima millä liikutetaan pelaajaa</param>    
    private void LiikutaPelaajaa(PhysicsObject pelaaja, Vector voima)
    {   
        if(voima.X < 0 && !liikkuiVasemmalle)
        {
            pelaaja.MirrorImage();
            liikkuiVasemmalle = true;
        }

        else if(voima.X > 0 && liikkuiVasemmalle)
        {
            pelaaja.MirrorImage();
            liikkuiVasemmalle = false;
        }
        pelaaja.Hit(voima);
    }


    /// <summary>
    /// aliohjelma, jolla pelaaja hyppää
    /// </summary>
    /// <param name="pelaaja">pelaajan kuva</param>
    /// <param name="voima">voima jolla pelaaja liikkuu kun hypätään</param>    
    private void PelaajaHyppaa(PhysicsObject pelaaja, Vector voima)
    {
        pelaaja.Hit(voima);       
        hyppyAani.Play();
    }


    /// <summary>
    /// aliohjelma, jolla pelaaja lyö
    /// </summary>
    /// <param name="pelaaja">pelaajan kuva</param>    
    private void PelaajaLyo(PhysicsObject pelaaja)
    {       
        lyontiAani.Play();
        pelaaja.Animation = new Animation(ukkelinLyonti);
        pelaaja.Animation.Start(1);
        pelaaja.Animation.FPS = 5;
        
        foreach (PhysicsObject obj in GetObjectsAt(pelaaja.Position, 50)) // etsitään listasta kaikki palikat, jotka ovat parametrin välisellä alueella
        {
            PalikkaCollides(pelaaja, obj);           
        }
        
    }


    /// <summary>
    /// +luo peliin kentän
    /// </summary>    
    private void LuoKentta()
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
  "                         ",
  "   ===   ++             ",
  "   ===   ++      *       ",
  "   ===   ++     **       ",
  "   ===   ++    ***       ",
  "   ===   ++   ****       ",
  "   ===   ++   ****       ",
  "-------------------------"


        };
        // aliohjelma arpoo kolmesta eri kenttävaihtoehdosta yhden
        // ja luo sen pelattavaksi kentäksi
        TileMap ruudut = TileMap.FromStringArray(RandomGen.SelectOne<string[]>(kentta1, kentta2, kentta3));
        ruudut.SetTileMethod('=', LuoPalikka, "=");       
        ruudut.SetTileMethod('*', LuoPalikka, "*");
        ruudut.SetTileMethod('+', LuoPalikka, "+");        
        ruudut.SetTileMethod('-', LuoAsfaltti);
        ruudut.Execute();

        // luodaan keskelle tasoa "näkymätön" palkki, jonka päällä vihollinen liikkuu

        PhysicsObject palikka = PhysicsObject.CreateStaticObject(1000, 40);
        palikka.Position = Level.Center;
        palikka.Shape = Shape.Rectangle;
        palikka.Image = LoadImage("nimetön");
        palikka.CollisionIgnoreGroup = 1;
        Add(palikka);
    }

    /// <summary>
    /// luodaan peliin grafiikkaa eli asfaltti, joka tulee tason pohjalle
    /// </summary>
    /// <param name="paikka">paikka minne asfaltti sijoitetaan</param>
    /// <param name="leveys">tien leveys</param>
    /// <param name="korkeus">tien korkeus</param>    
    private void LuoAsfaltti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject asfaltti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        asfaltti.Position = paikka;
        asfaltti.Shape = Shape.Rectangle;
        asfaltti.Image = LoadImage("tie");
        Add(asfaltti);
    }

    /// <summary>
    /// luodaan rakkenusten palikat, joita pelaaja yrittää tuhota
    /// </summary>
    /// <param name="paikka">paikka minne palikka sijoitetaan</param>
    /// <param name="leveys">palikan leveys</param>
    /// <param name="korkeus">palikan korkeus</param>
    /// <param name="tag">palikan tagi</param>    
    private void LuoPalikka(Vector paikka, double leveys, double korkeus, string tag)
    {
        PhysicsObject palikka = PhysicsObject.CreateStaticObject(leveys, korkeus);
        palikka.Position = paikka;
        palikka.Shape = Shape.Rectangle;
        palikka.Image = LoadImage("tiili");
        palikka.Tag = tag;
        palikka.CollisionIgnoreGroup = 2;        
        Add(palikka);
    }


    /// <summary>
    /// luodaan törmäys pelaajan ja palikoiden välille
    /// joka kerta kun pelaaja törmää palikan kanssa palikan kuva vaihtuu ja siltä lähtee yksi "elämä pois" 
    /// </summary>
    /// <param name="pelaaja">pelaaja jolla törmätään</param>
    /// <param name="palikka">palikka mihin törmätään</param>
    private void PalikkaCollides(PhysicsObject pelaaja, PhysicsObject palikka)
    {                  
        if (palikka.Tag.ToString() == "=" || palikka.Tag.ToString() == "+" || palikka.Tag.ToString() == "*")
        {
            palikkaHealth--;            
            osumisAani.Play();
            if (palikkaHealth == 2)
            {
                palikka.Image = LoadImage("tiili2");
            }
            if (palikkaHealth == 1)
            {
                palikka.Image = LoadImage("tiili 3");
            }
            if (palikkaHealth <= 0)
            {
                palikkaHealth = 3;
                pisteLaskuri.Value += 50;
                
                Flame liekki = new Flame(LoadImage("flame"));
                liekki.Position = palikka.Position;
                Add(liekki);

                 if (palikka.Tag.ToString() == "=")
                {
                    talonHealth1--;
                    if (talonHealth1 == 0)
                    {
                        TuhoaPalikat(palikka, "=");                        
                    }
                }
                if (palikka.Tag.ToString() == "+")
                {
                    talonHealth2--;
                    if (talonHealth2 == 0)
                    {
                        TuhoaPalikat(palikka, "+");                        
                    }
                }
                if (palikka.Tag.ToString() == "*")
                {
                    talonHealth3--;
                    if (talonHealth3 == 0)
                    {
                        TuhoaPalikat(palikka, "*");                        
                    }
                }
                if (talonHealth1 == 0 && talonHealth2 == 0 && talonHealth3 == 0)
                {
                    PelaajaVoittaa();
                }               
            }
        }               
    }


    /// <summary>
    /// Aliohjelma jota kutsutaan kun taloja on löyty tarpeeksi. Tuhoaa talon kokonaan
    /// </summary>
    /// <param name="palikka">palikka joka tuhotaan</param>
    /// <param name="tag">palikan tagi, josta tarkistetaan mitkä palikat tuhotaan</param>
    private void TuhoaPalikat(PhysicsObject palikka, string tag)
    {
        foreach (PhysicsObject obj in GetObjectsWithTag(tag))
        {                   
            obj.Destroy();           
        }
        pisteLaskuri.Value += 1000;
    }


    /// <summary>
    /// Aliohjelma pelaajan ja vihun törmäykselle, kun pelaaja törmää viholliseen vihollinen tuhoutuu
    /// </summary>
    /// <param name="pelaaja">pelaaja jolla törmätään</param>
    /// <param name="vihollinen">vihollinen johon törmätään</param>
    private void PelaajaCollides(PhysicsObject pelaaja, PhysicsObject vihollinen)
    {
        vihollinen.Destroy();
        SoundEffect vihuKuolee = LoadSoundEffect("sfx_deathscream_android7");
        vihuKuolee.Play();
        pisteLaskuri.Value += 100;
    }

    /// <summary>
    /// aliohjelma jossa luodaan lentävä vihollinen
    /// </summary>
    /// <param name="pelaaja">aliohjelma ottaa pelaajan parametrinä, jotta se voi ottaa sen sijainnin ampuakseen</param>
    /// <param name="kuva">vihun kuva</param>
    /// <param name="sijainti">vihun sijainti</param>
    /// <param name="tag">vihun tagi</param>    
    private void LuoLentavaVihollinen(PhysicsObject pelaaja, Image kuva, Vector sijainti, string tag)
    {
        LuoVihollinen(pelaaja, kuva, sijainti, tag);
    }


    /// <summary>
    /// Aliohjelma, jossa luodaan vihollinen ja sille myös aivot
    /// </summary>
    /// <param name="pelaaja">aliohjelma ottaa pelaajan parametrinä, jotta se voi ottaa sen sijainnin ampuakseen</param>
    /// <param name="kuva">vihun kuva</param>
    /// <param name="sijainti">vihun sijainti</param>
    /// <param name="tag">vihun tagi</param>   
    private void LuoVihollinen(PhysicsObject pelaaja, Image kuva, Vector sijainti, string tag)
    {
        
        PlatformCharacter vihu = new PlatformCharacter(40.0, 40.0);
        vihu.Position = sijainti;
        vihu.Tag = tag;
        vihu.Image = kuva;
        vihu.Shape = Shape.FromImage(kuva);
        vihu.CollisionIgnoreGroup = 2;
        Add(vihu, 1);

        // luodaan viholliselle "aivot" 
        PlatformWandererBrain tasoAivot = new PlatformWandererBrain();
        tasoAivot.Speed = 100;

        vihu.Brain = tasoAivot;

        // lisätään viholliselle ase
        vihu.Weapon = new AssaultRifle(30, 10);
        vihu.Weapon.Image = LoadImage("Nimetön");
        vihu.Weapon.X = 10.0;
        vihu.Weapon.Y = 5.0;
        vihu.Weapon.Ammo.Value = 1000;

        // ajastetaan vihollinen ampumaan joka kolmas sekunti
        Timer ajastin = new Timer();
        ajastin.Interval = 5.0;
        ajastin.Timeout += delegate { AmmuAseella(pelaaja, vihu); };
        ajastin.Start();

        vihu.Weapon.ProjectileCollision = AmmusOsui;
                
    }

    /// <summary>
    /// Vihollinen ampuu pelaajan suuntaan
    /// </summary>
    /// <param name="pelaaja">pelaaja jota ammutaan</param>
    /// <param name="vihu">vihollinen joka ampuu</param>
    private void AmmuAseella(PhysicsObject pelaaja, PlatformCharacter vihu)
    {
        Vector suunta = (pelaaja.Position - vihu.Position).Normalize();
        vihu.Weapon.Angle = suunta.Angle;
        PhysicsObject ammus = vihu.Weapon.Shoot();
    }


    /// <summary>
    /// jos ammus osuu pelaajan pelaajalta lähtee yksi elämä pois
    /// </summary>
    /// <param name="ammus">ammus joka osuu</param>
    /// <param name="pelaaja">pelaaja johon osutaan</param>    
    private void AmmusOsui(PhysicsObject ammus, PhysicsObject pelaaja)
    {
        if (pelaaja.Tag.ToString() == "pelaaja")
        {
            elamaLaskuri.Value -= 1;
            if (elamaLaskuri.Value == 0)
            {
                SoundEffect pelaajaKuolee = LoadSoundEffect("sfx_deathscream_alien2");
                pelaajaKuolee.Play();
                pelaaja.Destroy();
            }
            ammus.Destroy();
        }
        ammus.Destroy();
    }


    /// <summary>
    /// lisätään pelaajalle elämälaskuri
    /// </summary>    
    void LuoElamaLaskuri()
    {
        elamaLaskuri = new DoubleMeter(10);
        elamaLaskuri.MaxValue = 10;
        

        ProgressBar elamaPalkki = new ProgressBar(150, 20);
        elamaPalkki.BarColor = Color.Green;
        elamaPalkki.X = Screen.Left + 500;
        elamaPalkki.Y = Screen.Top - 20;
        elamaPalkki.BindTo(elamaLaskuri);
        Add(elamaPalkki);
    }
    

    /// <summary>
    /// Luo peliin pistelaskurin
    /// </summary>    
    void LuoPistelaskuri()
    {
        pisteLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.IntFormatString = "Score: {0:D1}";

        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }


    /// <summary>
    /// aliohjelma sille, mitä tapahtuu kun pelaaja voittaa
    /// </summary>    
    private void PelaajaVoittaa()
    {
        MessageDisplay.Add("Voitit pelin. Hyvä!");        
    }

    
}





