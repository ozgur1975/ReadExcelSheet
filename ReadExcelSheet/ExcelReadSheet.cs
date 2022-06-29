using AutoMapper;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelSheet
{
    public static class ExcelReadSheet
    {

        enum SheetName
        {
            CSL_Sonuclari = 0,
            SL_Sonuclari = 1
        }

        enum CSLKolonlari
        {
            Hafta = 0,
            Tarih = 1,
            Kolon1 = 2,
            Kolon2 = 3,
            Kolon3 = 4,
            Kolon4 = 5,
            Kolon5 = 6,
            Kolon6 = 7,
            Joker = 8,
            SuperStar = 9
        }
        public static void Read02(string xlsxPath)
        {

            List<CSLCekilisSonucu> CSLCekilisSonuclariListesi;
            List<SLCekilisSonucu> SLCekilisSonuclariListesi;
            using (DataSet ds = GetExcelToDataSet(xlsxPath))
            {
                CSLCekilisSonuclariListesi = GetCSLCekilisSonuclariListesi(ds.Tables[(int)SheetName.CSL_Sonuclari]);
                SLCekilisSonuclariListesi = getSLCekilisSonuclariListesi(ds.Tables[(int)SheetName.SL_Sonuclari]);
            }

            List<CSLCikanNumara> CSLCikanNumaraListesi = getCSLCikanNumaraListesi(CSLCekilisSonuclariListesi);

            var KolonAdat = getKolonAdat(CSLCikanNumaraListesi);                                
            Console.WriteLine($"---Kolon En Az Çıkma Adatına göre: {string.Join(",", KolonAdat)}");

            var KolonSayi = getKolonSayi(CSLCikanNumaraListesi);            
            Console.WriteLine($"---Kolon En Az Çıkma Sayısına göre: {string.Join(",", KolonSayi)}");

            var ary = GetSuperStarAdat(CSLCikanNumaraListesi, KolonAdat);            
            Console.WriteLine($"---SuperStar En Az Çıkma Adatına göre: {string.Join(",", ary)}");

            ary = GetSuperStarSayi(CSLCikanNumaraListesi, KolonSayi);            
            Console.WriteLine($"---SuperStar En Az Çıkma Sayısına göre: {string.Join(",", ary)}");

            var KolonTumu = getTumuAdat(CSLCikanNumaraListesi);
            Console.WriteLine($"---Tümü  En Az Çıkma Adatına göre: {string.Join(",", KolonTumu)}");

            ary = getTumuSayi(CSLCikanNumaraListesi);            
            Console.WriteLine($"---Tümü  En Az Çıkma Sayısına göre: {string.Join(",", ary)}");

            Console.ReadLine();
        }


        public static DataSet GetExcelToDataSet(string xlsxPath)
        {

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    return reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        }
                        );
                }
            }
        }


        public static List<CSLCekilisSonucu> GetCSLCekilisSonuclariListesi(DataTable dt)
        {
            return dt
            .AsEnumerable()
            .Select(x => new CSLCekilisSonucu
            {
                Hafta = x.Field<double>(CSLKolonlari.Hafta.ToString()),
                Tarih = DateOnly.Parse(x.Field<DateTime>(CSLKolonlari.Tarih.ToString()).ToString("dd.MM.yyy")),
                Kolon1 = x.Field<double>(CSLKolonlari.Kolon1.ToString()),
                Kolon2 = x.Field<double>(CSLKolonlari.Kolon2.ToString()),
                Kolon3 = x.Field<double>(CSLKolonlari.Kolon3.ToString()),
                Kolon4 = x.Field<double>(CSLKolonlari.Kolon4.ToString()),
                Kolon5 = x.Field<double>(CSLKolonlari.Kolon5.ToString()),
                Kolon6 = x.Field<double>(CSLKolonlari.Kolon6.ToString()),
                Joker = x.Field<double>(CSLKolonlari.Joker.ToString()),
                SuperStar = double.TryParse(x[CSLKolonlari.SuperStar.ToString()]?.ToString(), out double val) ? val : 0
            })
            .ToList();
        }

        public static List<SLCekilisSonucu> getSLCekilisSonuclariListesi(DataTable dt)
        {
            return dt
           .AsEnumerable()
           .Select(x => new SLCekilisSonucu
           {
               Hafta = x.Field<double>(CSLKolonlari.Hafta.ToString()),
               Tarih = DateOnly.Parse(x.Field<DateTime>(CSLKolonlari.Tarih.ToString()).ToString("dd.MM.yyy")),
               Kolon1 = x.Field<double>(CSLKolonlari.Kolon1.ToString()),
               Kolon2 = x.Field<double>(CSLKolonlari.Kolon2.ToString()),
               Kolon3 = x.Field<double>(CSLKolonlari.Kolon3.ToString()),
               Kolon4 = x.Field<double>(CSLKolonlari.Kolon4.ToString()),
               Kolon5 = x.Field<double>(CSLKolonlari.Kolon5.ToString()),
               Kolon6 = x.Field<double>(CSLKolonlari.Kolon6.ToString())
           })
           .ToList();
        }

        public static List<CSLCikanNumara> getCSLCikanNumaraListesi(List<CSLCekilisSonucu> CSLCekilisSonuclariListesi)
        {
            List<CSLCikanNumara> CSLCikanNumaraListesi = new List<CSLCikanNumara>();

            CSLCekilisSonuclariListesi.ForEach(x =>
            {
                var Hafta = double.Parse(x.GetType().GetProperty("Hafta")?.GetValue(x)?.ToString() ?? "0");

                x.GetType()
                .GetProperties()
                .ToList()
                .ForEach(y =>
                {
                    string KolonTipi = "";
                    CSLCikanNumara? cikanno;
                    if (y.Name == CSLKolonlari.SuperStar.ToString())
                        KolonTipi = CSLKolonlari.SuperStar.ToString();
                    else if (y.Name == CSLKolonlari.Joker.ToString())
                        KolonTipi = CSLKolonlari.Joker.ToString();
                    else if (y.Name.Contains("Kolon"))
                        KolonTipi = "Kolon";

                    if (!string.IsNullOrEmpty(KolonTipi))
                    {
                        var numara = (double)(y.GetValue(x) ?? 0);
                        if (numara != 0)
                        {
                            cikanno = CSLCikanNumaraListesi
                                                        .Where(z => z.Numara == numara && z.KolonTipi == KolonTipi)
                                                        .FirstOrDefault();
                            if (cikanno == null)
                            {
                                var yeniNo = new CSLCikanNumara()
                                {
                                    KolonTipi = KolonTipi,
                                    Numara = numara,
                                    CikmaAdati = Hafta,
                                    CikmaSayisi = 1
                                };
                                CSLCikanNumaraListesi.Add(yeniNo);
                                yeniNo = new CSLCikanNumara()
                                {
                                    KolonTipi = "Tumu",
                                    Numara = numara,
                                    CikmaAdati = Hafta,
                                    CikmaSayisi = 1
                                };
                                CSLCikanNumaraListesi.Add(yeniNo);
                            }
                            else
                            {
                                cikanno.CikmaSayisi++;
                                cikanno.CikmaAdati += Hafta;
                                cikanno = CSLCikanNumaraListesi
                                                       .Where(z => z.Numara == numara && z.KolonTipi == "Tumu")
                                                       .FirstOrDefault();
                                cikanno.CikmaSayisi++;
                                cikanno.CikmaAdati += Hafta;
                            }
                        }
                    }
                });
            });
            return CSLCikanNumaraListesi;
        }

        public static IEnumerable<string> getKolonAdat(List<CSLCikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                   .OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());
            
        }

        public static IEnumerable<string> getKolonSayi(List<CSLCikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .OrderBy(x => x.Numara)
                       .Select(x => x.Numara.ToString());            
        }
        public static IEnumerable<string> GetSuperStarAdat(List<CSLCikanNumara> CSLCikanNumaraListesi, IEnumerable<string> KolonAdat)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString() && !KolonAdat.Any(a => a == x.Numara.ToString()))
                               .OrderBy(x => x.CikmaAdati)
                               .Take(6)
                               .Select(x => x.Numara.ToString());            
        }
        public static IEnumerable<string> GetSuperStarSayi(List<CSLCikanNumara> CSLCikanNumaraListesi, IEnumerable<string> KolonSayi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString() && !KolonSayi.Any(a => a == x.Numara.ToString()))
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .Select(x => x.Numara.ToString());            
        }

        public static IEnumerable<string> getTumuAdat(List<CSLCikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Tumu")
                               .OrderBy(x => x.CikmaAdati)
                               .Take(6)
                               .OrderBy(x => x.Numara)
                               .Select(x => x.Numara.ToString());
        }
        public static IEnumerable<string> getTumuSayi(List<CSLCikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Tumu")
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .OrderBy(x => x.Numara)
                       .Select(x => x.Numara.ToString());            
        }

        //---
        
        
        //---

    }
}


