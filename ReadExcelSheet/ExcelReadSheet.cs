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

            DataSet ds = new();
            List<CSLCekilisSonucu> CSLCekilisSonuclariListesi = new List<CSLCekilisSonucu>();
            List<SLCekilisSonucu> SLCekilisSonuclariListesi = new List<SLCekilisSonucu>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    ds = reader.AsDataSet(
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

            CSLCekilisSonuclariListesi = ds.Tables[(int)SheetName.CSL_Sonuclari]
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

            SLCekilisSonuclariListesi = ds.Tables[(int)SheetName.SL_Sonuclari]
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

            ds.Dispose();



            List<CSLCikanNumara> CSLCikanNumaraListesi = new List<CSLCikanNumara>();

            CSLCekilisSonuclariListesi.ForEach(x =>
            {
                var Hafta = double.Parse(x.GetType().GetProperty("Hafta").GetValue(x).ToString());

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
            Console.WriteLine("---Kolon En Az Adat");
            CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                   .OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .ToList()
                   .ForEach(x => Console.WriteLine(x.Numara));

            Console.WriteLine("---Kolon En Az Çıkma Sayısı");
            CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                   .OrderBy(x => x.CikmaSayisi)
                   .Take(6)
                   .ToList()
                   .ForEach(x => Console.WriteLine(x.Numara));            

            Console.WriteLine("---SüperStar En Az Çıkma Adat");
            CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString())
                   .OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .ToList()
                   .ForEach(x => Console.WriteLine(x.Numara));

            Console.WriteLine("---SüperStar En Az Çıkma Sayısı");
            CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString())
                   .OrderBy(x => x.CikmaSayisi)
                   .Take(6)
                   .ToList()
                   .ForEach(x => Console.WriteLine(x.Numara));

            Console.WriteLine("---Tümü En Az Çıkma Adat");
            CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Tumu")
                   .OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .ToList()
                   .ForEach(x => Console.WriteLine(x.Numara));

            Console.WriteLine("---Tümü En Az Çıkma Sayisi");
            CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Tumu")
                   .OrderBy(x => x.CikmaSayisi)
                   .Take(6)
                   .ToList()
                   .ForEach(x => Console.WriteLine(x.Numara));




            Console.ReadLine();
        }




        public static List<T> ReadData<T>(DataTable dt)
        {

            var configuration = new MapperConfiguration(cfg => { });
            Mapper mp = new Mapper(configuration);

            List<T> result = mp.Map<List<T>>(dt);


            //aa.Map(dt, typeof(DataTable), T);


            //return Mapper.DynamicMap<IDataReader, List<T>>(dt.CreateDataReader());
            return result;
        }



        public static void Read01(string xlsxPath)
        {


            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {


                    do
                    {
                        while (reader.Read()) //Each ROW
                        {
                            for (int column = 0; column < reader.FieldCount; column++)
                            {
                                //Console.WriteLine(reader.GetString(column));//Will blow up if the value is decimal etc. 
                                Console.WriteLine(reader.GetValue(column));//Get Value returns object
                            }
                        }
                    } while (reader.NextResult()); //Move to NEXT SHEET
                }
            }

        }


    }
}


