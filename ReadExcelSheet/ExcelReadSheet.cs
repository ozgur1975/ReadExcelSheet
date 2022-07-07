using ExcelDataReader;
using System.Data;

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

        enum SLKolonlari
        {
            Hafta = 0,
            Tarih = 1,
            Kolon1 = 2,
            Kolon2 = 3,
            Kolon3 = 4,
            Kolon4 = 5,
            Kolon5 = 6,
            Kolon6 = 7,            
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

            Console.WriteLine($"---Çılgın Sayısal Loto Hesaplanan Hafta: {CSLCekilisSonuclariListesi.Count}---");

            List<CikanNumara> CSLCikanNumaraListesi = getCSLCikanNumaraListesi(CSLCekilisSonuclariListesi);

            var KolonAdat = getCSLKolonAdat(CSLCikanNumaraListesi);                                
            Console.WriteLine($"   ---Kolon En Az Çıkma Adatına göre: {string.Join(",", KolonAdat)}");

            var KolonveJokerAdat = getCSLKolonveJokerAdat(CSLCikanNumaraListesi);
            Console.WriteLine($"   ---Kolon ve Joker En Az Çıkma Adatına göre: {string.Join(",", KolonveJokerAdat)}");

            var ary = GetCSLSuperStarAdat(CSLCikanNumaraListesi, KolonAdat);
            Console.WriteLine($"   ---SuperStar En Az Çıkma Adatına göre: {string.Join(",", ary)}");

            var Tumu = getCSLTumuAdat(CSLCikanNumaraListesi);
            Console.WriteLine($"   ---Tümü  En Az Çıkma Adatına göre: {string.Join(",", Tumu)}");

            Console.WriteLine("");

            var KolonSayi = getCSLKolonSayi(CSLCikanNumaraListesi);            
            Console.WriteLine($"   ---Kolon En Az Çıkma Sayısına göre: {string.Join(",", KolonSayi)}");

            var KolonveJokersayi = getCSLKolonveJokerSayi(CSLCikanNumaraListesi);
            Console.WriteLine($"   ---Kolon ve Joker En Az Çıkma Sayısına göre: {string.Join(",", KolonveJokersayi)}");


            ary = GetCSLSuperStarSayi(CSLCikanNumaraListesi, KolonSayi);            
            Console.WriteLine($"   ---SuperStar En Az Çıkma Sayısına göre: {string.Join(",", ary)}");
            
            ary = getCSLTumuSayi(CSLCikanNumaraListesi);            
            Console.WriteLine($"   ---Tümü  En Az Çıkma Sayısına göre: {string.Join(",", ary)}");

            Console.WriteLine("");
            Console.WriteLine($"---Süper Loto Hesaplanan Hafta: {SLCekilisSonuclariListesi.Count}---");

            List<CikanNumara> SLCikanNumaraListesi = getSLCikanNumaraListesi(SLCekilisSonuclariListesi);

            var SLTumu = getSLTumuAdat(SLCikanNumaraListesi);
            Console.WriteLine($"   ---Süper Loto En Az Çıkma Adatına göre: {string.Join(",", SLTumu)}");
            SLTumu = getSLTumuSayi(SLCikanNumaraListesi);
            Console.WriteLine($"   ---Süper Loto En Az Çıkma sayısına göre: {string.Join(",", SLTumu)}");

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
                                UseHeaderRow = true,                                
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
               Hafta = x.Field<double>(SLKolonlari.Hafta.ToString()),
               Tarih = DateOnly.Parse(x.Field<DateTime>(CSLKolonlari.Tarih.ToString()).ToString("dd.MM.yyy")),
               Kolon1 = x.Field<double>(SLKolonlari.Kolon1.ToString()),
               Kolon2 = x.Field<double>(SLKolonlari.Kolon2.ToString()),
               Kolon3 = x.Field<double>(SLKolonlari.Kolon3.ToString()),
               Kolon4 = x.Field<double>(SLKolonlari.Kolon4.ToString()),
               Kolon5 = x.Field<double>(SLKolonlari.Kolon5.ToString()),
               Kolon6 = x.Field<double>(SLKolonlari.Kolon6.ToString())
           })
           .ToList();
        }

        public static List<CikanNumara> getCSLCikanNumaraListesi(List<CSLCekilisSonucu> CSLCekilisSonuclariListesi)
        {
            List<CikanNumara> CSLCikanNumaraListesi = new List<CikanNumara>();

            CSLCekilisSonuclariListesi.ForEach(x =>
            {
                var Hafta = double.Parse(x.GetType().GetProperty("Hafta")?.GetValue(x)?.ToString() ?? "0");

                x.GetType()
                .GetProperties()
                .ToList()
                .ForEach(y =>
                {
                    string KolonTipi = "";
                    CikanNumara? cikanno;
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
                                var yeniNo = new CikanNumara()
                                {
                                    KolonTipi = KolonTipi,
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
                            }
                        }
                    }
                });
            });
            return CSLCikanNumaraListesi;
        }

        public static IEnumerable<string> getCSLKolonAdat(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                   .OrderBy(x => x.CikmaAdati / x.CikmaSayisi)
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());
            
        }
        public static IEnumerable<string> getCSLKolonSayi(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .OrderBy(x => x.Numara)
                       .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getCSLKolonveJokerAdat(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.SuperStar.ToString())
                .GroupBy(x=> x.Numara)
                .Select(group=> new {Numara = group.Key, CikmaAdati = group.Sum(item => item.CikmaAdati), CikmaSayisi= group.Sum(item=> item.CikmaSayisi) })
                   .OrderBy(x => x.CikmaAdati / x.CikmaSayisi)
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());

        }
        public static IEnumerable<string> getCSLKolonveJokerSayi(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.SuperStar.ToString())
                .GroupBy(x => x.Numara)
                .Select(group => new { Numara = group.Key, CikmaAdati = group.Sum(item => item.CikmaAdati), CikmaSayisi = group.Sum(item => item.CikmaSayisi) })
                .OrderBy(x => x.CikmaSayisi)
                .Take(6)
                .OrderBy(x => x.Numara)
                .Select(x => x.Numara.ToString());

        }
        public static IEnumerable<string> GetCSLSuperStarAdat(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> KolonAdat)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString() && !KolonAdat.Any(a => a == x.Numara.ToString()))
                               .OrderBy(x => x.CikmaAdati/x.CikmaSayisi)
                               .Take(6)
                               .Select(x => x.Numara.ToString());            
        }
        public static IEnumerable<string> GetCSLSuperStarSayi(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> KolonSayi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString() && !KolonSayi.Any(a => a == x.Numara.ToString()))
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .Select(x => x.Numara.ToString());            
        }     

        public static IEnumerable<string> getCSLTumuAdat(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi
                .GroupBy(x => x.Numara)
                .Select(group => new { Numara = group.Key, CikmaAdati = group.Sum(item => item.CikmaAdati), CikmaSayisi = group.Sum(item => item.CikmaSayisi) })
                .OrderBy(x => x.CikmaAdati / x.CikmaSayisi)
                .Take(6)
                .OrderBy(x => x.Numara)
                .Select(x => x.Numara.ToString());
        }
        public static IEnumerable<string> getCSLTumuSayi(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi
                .GroupBy(x => x.Numara)
                .Select(group => new { Numara = group.Key, CikmaAdati = group.Sum(item => item.CikmaAdati), CikmaSayisi = group.Sum(item => item.CikmaSayisi) })
                .OrderBy(x => x.CikmaSayisi)
                .Take(6)
                .OrderBy(x => x.Numara)
                .Select(x => x.Numara.ToString());
        }

        public static List<CikanNumara> getSLCikanNumaraListesi(List<SLCekilisSonucu> SLCekilisSonuclariListesi)
        {
            List<CikanNumara> SLCikanNumaraListesi = new List<CikanNumara>();

            SLCekilisSonuclariListesi.ForEach(x =>
            {
                var Hafta = double.Parse(x.GetType().GetProperty("Hafta")?.GetValue(x)?.ToString() ?? "0");

                x.GetType()
                .GetProperties()
                .ToList()
                .ForEach(y =>
                {
                    string KolonTipi = "";
                    CikanNumara? cikanno;

                    if (y.Name.Contains("Kolon"))
                        KolonTipi = "Kolon";

                    if (!string.IsNullOrEmpty(KolonTipi))
                    {
                        var numara = (double)(y.GetValue(x) ?? 0);
                        if (numara != 0)
                        {
                            cikanno = SLCikanNumaraListesi
                                                        .Where(z => z.Numara == numara && z.KolonTipi == KolonTipi)
                                                        .FirstOrDefault();
                            if (cikanno == null)
                            {
                                var yeniNo = new CikanNumara()
                                {
                                    KolonTipi = KolonTipi,
                                    Numara = numara,
                                    CikmaAdati = Hafta,
                                    CikmaSayisi = 1
                                };
                                SLCikanNumaraListesi.Add(yeniNo);
                            }
                            else
                            {
                                cikanno.CikmaSayisi++;
                                cikanno.CikmaAdati += Hafta;
                            }
                        }
                    }
                });
            });
            return SLCikanNumaraListesi;
        }

        public static IEnumerable<string> getSLTumuAdat(List<CikanNumara> SLCikanNumaraListesi)
        {
            return SLCikanNumaraListesi
                               .OrderBy(x => x.CikmaAdati / x.CikmaSayisi)
                               .Take(6)
                               .OrderBy(x => x.Numara)
                               .Select(x => x.Numara.ToString());
        }
        public static IEnumerable<string> getSLTumuSayi(List<CikanNumara> SLCikanNumaraListesi)
        {
            return SLCikanNumaraListesi
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .OrderBy(x => x.Numara)
                       .Select(x => x.Numara.ToString());
        }
    }
}


