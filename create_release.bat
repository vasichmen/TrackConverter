chcp 1251

Set /P ver=  введите версию для продолжения ^>
echo %ver%
Set n=TrackConverter_
Set ext=.zip
Set name=%n%%ver%%ext%
echo %name%
"C:\Program Files\7-Zip\7z.exe" a -tzip -ssw -mx7 -r0 -x@exclusions_arhive_program.txt "D:\Clouds\Share\track converter\%name%" "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\"
rem del "D:\Clouds\GDrive\Projects\Web\htdocs\downloads\TrackConverter_"*
copy "D:\Clouds\Share\track converter\%name%" "D:\Clouds\GDrive\Projects\Web\htdocs\downloads"

rem обновить данные о новом релизе в бд сайта
rem загрузить файл релиза на сайт 

pause