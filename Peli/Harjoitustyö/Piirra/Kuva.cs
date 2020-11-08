using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
// using Jypeli.Effects;
using Jypeli.Widgets;
using System.Text;

namespace Demo9
{
    /// @author  Vesa Lappalainen
    /// @version 30.10.2011
    /// <summary>
    /// Tehdään erilaisia kuvaoperaatioita
    /// Laskee kuvan sisätulon painomatriisin kanssa väreittäin.
    /// Sisätulon arvo jaetaan jakajalla ja väri rajoitetaan [0,255] arvojoukkoon.
    /// </summary>
    /// <remarks>
    /// Konvoluutiossa kuvamatriin päällä liutetaan painomatriisia ja kukin 
    /// painomatriisn alle jäävä väriarvo (komponenteittain) kerrotaan
    /// vastaavalla painomatrisin arvolla ja summataan.  Lopuksi summa
    /// skaalataan ja uuteen matriisin sijoitettn vastinpaikkaan skaalattu
    /// summa.
    /// 
    /// Katso:
    ///   - http://www.niksula.cs.hut.fi/~tjpartan/Studio4/Harjoitukset/Harjoitus3/
    ///   - http://users.tkk.fi/~hjkarkka/studio4portfolio/tehtava3/Hahmo.pdf
    ///   - http://www.pages.drexel.edu/~weg22/edge.html
    ///   - http://en.wikipedia.org/wiki/Sobel_operator
    /// 
    /// </remarks>
    /// 
    public class Kuva : PhysicsGame
    
        public static void Main()
    {

    }
        private string kuvannimi = "vesa";

        /// <summary>
        /// Tehdään kuvalle erilaisia operaatioita ja näytetään kuvat
        /// </summary>
        public override void Begin()
        {
            // Image.SetLineCorrection(0); // Linux käyttäjät, ottakaa tämä kommenteista
            int[,] painot = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };          // Keskiarvo 3x3
            Image kuva = LoadImage(kuvannimi);
            SetWindowSize(2 * kuva.Width, 2 * kuva.Height);
            Level.Width = 2 * kuva.Width;
            Level.Height = 2 * kuva.Height;

            AddKuva(kuva, 0, 0);

            Image kuva2 = kuva.Clone();
            HarmaasavyTaulukolla(kuva2);
            AddKuva(kuva2, 0, 1);
            // TODO: Tehtävä 5 ota seuraavat kommentit pois ja laita kolme edellistä rivi kommentteihin ja toteuta puuttuva aliohjelma
            // Image kuva2 = kuva.Clone();
            // PoistaPunainen(kuva2, 150);
            // AddKuva(kuva2, 0, 1);

            Image kuva3 = kuva.Clone();
            Punaiseksi(kuva3);
            AddKuva(kuva3, 1, 0);

            Image kuva4 = Konvoluutio(kuvannimi, painot);
            AddKuva(kuva4, 1, 1);
        }


        /// <summary>
        /// Lisää kuvan näyttöön valittuun "ruutuun"
        /// </summary>
        /// <param name="kuva">Lisättävä kuva</param>
        /// <param name="nx">ruudun x-paikka</param>
        /// <param name="ny">ruudun y-paikka</param>
        private void AddKuva(Image kuva, int nx = 0, int ny = 0)
        {
            GameObject k = new GameObject(kuva);
            k.Position = new Vector(Level.Left + k.Width / 2 + k.Width * nx, Level.Top - k.Height / 2 - k.Height * ny);
            Add(k);
        }


        /// <summary>
        /// Tehdään konvoluutio käyttäen uint-tyyppiä ja [,] taulukkoa. 
        /// </summary>
        /// <param name="nimi">ladattavan kuvan nimi</param>
        /// <param name="painot">painokertoimien matriisi</param>
        /// <returns>konvoloitu kuva</returns>
        private Image Konvoluutio(string nimi, int[,] painot)
        {
            Image kuva = LoadImage(nimi);
            uint[,] bmp = kuva.GetDataUInt();
            uint[,] bmp2 = Muunna(bmp, painot);
            kuva.SetData(bmp2);
            return kuva;
        }


        /// <summary>
        /// Muutetaan kuva harmaasävykuvaksi.
        /// </summary>
        /// <param name="kuva">Muutettava kuva.  Tämä muuttuu.</param>
        public static void Harmaasavy(Image kuva)
        {
            for (int iy = 0; iy < kuva.Height; iy++)
                for (int ix = 0; ix < kuva.Width; ix++)
                {
                    Color c = kuva[iy, ix];
                    byte b = (byte)((c.RedComponent + c.GreenComponent + c.BlueComponent) / 3);
                    kuva[iy, ix] = new Color(b, b, b);
                }
        }


        /// <summary>
        /// Muutetaan kuva harmaasävykuvaksi Color-taulukon avulla
        /// </summary>
        /// <param name="kuva">Muutettava kuva.  Tämä muuttuu.</param>
        public static void HarmaasavyTaulukolla(Image kuva)
        {
            Color[,] bmp = kuva.GetData();
            int ny = kuva.Height;
            int nx = kuva.Width;
            for (int iy = 0; iy < ny; iy++)
                for (int ix = 0; ix < nx; ix++)
                {
                    Color c = bmp[iy, ix];
                    byte b = (byte)((c.RedComponent + c.GreenComponent + c.BlueComponent) / 3);
                    bmp[iy, ix] = new Color(b, b, b);
                }
            kuva.SetData(bmp);
        }


        /// <summary>
        /// Otetaan kuvasta vain punainen osuus käyttäen uint-taulukkoa
        /// </summary>
        /// <param name="kuva">Muutettava kuva.  Tämä muuttuu.</param>
        public static void Punaiseksi(Image kuva)
        {
            uint[,] bmp = kuva.GetDataUInt();
            int ny = kuva.Height;
            int nx = kuva.Width;
            for (int iy = 0; iy < ny; iy++)
                for (int ix = 0; ix < nx; ix++)
                {
                    uint c = bmp[iy, ix];
                    byte r = Color.GetRed(c);
                    byte g = 0;
                    byte b = 0;
                    bmp[iy, ix] = Color.PackRGB(r, g, b);
                }
            kuva.SetData(bmp);
        }


        /// <summary>
        /// Lasketaan konvoluutio uint[,] kuva-matriisille käytten painomatriisia.
        /// Konvoluutio lasketaan kunkin pikselin kohdalla kullekin värille (R,B,G)
        /// erikseen.
        /// Reunat unohdetaan. 
        /// </summary>
        /// <param name="kuva">kuvamatriisi jolle konvoluutio lasketaan</param>
        /// <param name="painot">käytettävä painomatriisi</param>
        /// <returns>uusi kuva-matriisi konvoloituna</returns>
        /// <example>
        /// <pre name="test">
        ///     uint[,] kuva  = { { 0x010905,0x55020202,0x7f030303,0x040404 }, 
        ///                       { 0x010101,0x66020202,0x7f030803,0x050505 }, 
        ///                       { 0x010102,0x77090909,0x7f030303,0x060606 } };
        ///     int[,] painot   = {{1,0,0},{0,1,0},{0,0,2}};
        ///     
        ///     uint[,] kuva2 = Muunna(kuva,painot);
        ///     kuva2[0,0] === 0U;  // reunat jäävät mustiksi
        ///     kuva2[2,1] === 0U;
        ///     Color.GetBlue(kuva2[1,1]) === 3;
        ///     kuva2[1,2] === 0x7f040504U;        
        ///     kuva2[1,1] === 0x66020403U;        
        /// </pre>
        /// </example>
        public static uint[,] Muunna(uint[,] kuva, int[,] painot)
        {
            // TODO: Tehtävä B1-3 täydennä tämä toimivaksi
            int nky = kuva.GetLength(0);
            int nkx = kuva.GetLength(1);
            uint[,] tulos = new uint[nky, nkx];
            // Tähän kovasti koodia tyyliin GameOfLife
            return tulos;
        }

    }
}

