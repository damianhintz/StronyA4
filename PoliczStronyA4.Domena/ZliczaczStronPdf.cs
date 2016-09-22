﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PoliczStronyA4.Domena
{
    public class ZliczaczStronPdf
    {
        public int LiczbaPlików { get { return _pliki.Length; } }
        string[] _pliki;
        public int SumaStron { get { return _sumaStron; } }
        int _sumaStron;
        public int SumaStronA4 { get { return _sumaStronA4; } }
        int _sumaStronA4;

        IAnalizatorFormatuStrony _analizator = new MetrycznyAnalizatorFormatuStrony();
        Encoding _kodowanie = Encoding.GetEncoding(1250);
        StreamWriter _writer;

        public ZliczaczStronPdf(string roboczyKatalog)
        {
            _pliki = Directory.GetFiles(roboczyKatalog, "*.pdf", SearchOption.AllDirectories);
        }

        public void Zapisz(string fileName)
        {
            using (_writer = new StreamWriter(fileName, false, _kodowanie))
            {
                _writer.WriteLine("Nazwa pliku\tNumer strony\tSzerokość[punkty]\tWysokość[punkty]\tSzerokość[mm]\tWysokość[mm]\tFormat arkusza\tLiczba stron A4");
                AnalizujStronyPlikówPdf();
            }
        }

        void AnalizujStronyPlikówPdf()
        {
            foreach (var plik in _pliki)
            {
                //catch (Exception ex) { _błędy.Add(plik + " (" + ex.Message + ")"); continue; } //Błędny plik
                try
                {
                    var liczbaStron = OdczytajStronyPlikuPdf(plik);
                    _sumaStron += liczbaStron;
                }
                catch { }
            }
        }

        int OdczytajStronyPlikuPdf(string fileName)
        {
            //var teraz = DateTime.Now;
            //var różnica = teraz - początek;
            //var sekundy = różnica.TotalSeconds;
            //var liczbaPlikówNaSekundę = nr / sekundy;
            //var przewidywanyCzas = (_pliki.Length - nr) / liczbaPlikówNaSekundę;
            //Console.WriteLine("{0}, {1} stron[y] #{2}/{3} {4:F0}s {5:F0}pliki/s {6:F0}min. ({7})", plik, liczbaStron, nr++, _pliki.Length, sekundy, liczbaPlikówNaSekundę, przewidywanyCzas / 60, _błędy.Count);
            var czytnik = new CzytnikStronPdf(fileName);
            foreach (var strona in czytnik.Strony)
            {
                var format = _analizator.ObliczFormatStrony(strona.RozmiarMilimetry);
                _writer.WriteLine("{0}\t{1}\t{2:F0}\t{3:F0}\t{4:F0}\t{5:F0}\t{6}\t{7}", strona.FileName, strona.NumerStrony,
                    strona.RozmiarPunkty.Szerokość, strona.RozmiarPunkty.Wysokość,
                    strona.RozmiarMilimetry.Szerokość, strona.RozmiarMilimetry.Wysokość,
                    format, format.StronyA4);
                _sumaStronA4 += format.StronyA4;
            }
            return czytnik.LiczbaStron;
        }
    }
}