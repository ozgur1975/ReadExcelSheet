// See https://aka.ms/new-console-template for more information
using ReadExcelSheet;


var getSonuclar = new GetSonuclar();

await getSonuclar.GetCSL();

await getSonuclar.GetSL();
await getSonuclar.GetST();


//Kombinasyon.Konbinasyonlar(new List<int>() { 1, 2, 3, 4, 5, 6, 7 },6);
//ExcelReadSheet.Read02(@"C:\Users\ozgur.yurtsever\OneDrive - teknosol.com.tr\Masaüstü\SLIStatistik.xlsx");
ExcelReadSheet.Read02(@"..\..\..\..\SLIStatistik.xlsx");
//ExcelReadSheet.Read01(@"C:\Users\ozgur.yurtsever\OneDrive - teknosol.com.tr\Masaüstü\SLIStatistik.xlsx");
