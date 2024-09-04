// See https://aka.ms/new-console-template for more information
using ReadExcelSheet;


var getSonuclar = new GetSonuclar();



//do
//{
//    Console.Clear();

//    Console.WriteLine("1 = Çılgın sayısal Loto");
//    Console.WriteLine("2 = Süper Loto");
//    Console.WriteLine("3 = Şans Topu");
//    Console.WriteLine("4 = Excel");

//    var ret = Console.ReadLine();
   
//    if (ret == "1")
//    {
//        try
//        {
//            await getSonuclar.GetCSL();
//        }
//        catch (Exception)
//        {

//            Console.WriteLine("Hata yüzünden listeleyemedi");
//            Console.WriteLine($"--------------------------------------------------------------------------");
//        }
//    }
//    if(ret == "2")

//} while (true);


try
{
    await getSonuclar.GetCSL();
}
catch (Exception ex)
{

    Console.WriteLine("Hata yüzünden listeleyemedi");
    Console.WriteLine(ex.Message);
    Console.WriteLine($"--------------------------------------------------------------------------");
}

try
{
    await getSonuclar.GetSL();
}
catch (Exception)
{

    Console.WriteLine("Hata yüzünden listeleyemedi");
    Console.WriteLine($"--------------------------------------------------------------------------");
}


try
{
    await getSonuclar.GetST();
}
catch (Exception)
{
    Console.WriteLine("Hata yüzünden listeleyemedi");
    Console.WriteLine($"--------------------------------------------------------------------------");
}





//Kombinasyon.Konbinasyonlar(new List<int>() { 1, 2, 3, 4, 5, 6, 7 },6);
//ExcelReadSheet.Read02(@"C:\Users\ozgur.yurtsever\OneDrive - teknosol.com.tr\Masaüstü\SLIStatistik.xlsx");
try
{
    ExcelReadSheet.Read02(@"..\..\..\..\SLIStatistik.xlsx");
}
catch (Exception ex)
{

    Console.WriteLine("Hata yüzünden listeleyemedi");
    Console.WriteLine($"--------------------------------------------------------------------------");
}




//ExcelReadSheet.Read01(@"C:\Users\ozgur.yurtsever\OneDrive - teknosol.com.tr\Masaüstü\SLIStatistik.xlsx");
