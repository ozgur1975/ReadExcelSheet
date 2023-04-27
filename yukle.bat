
set Tarih = %date:.=%


git add .
git commit -a -m '%Tarih%'
git push

 timeout /t 10