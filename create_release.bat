chcp 1251
rem �����������  �� release � ����� �������� ������
rem ������� ����� � ���������� � ������


Set /P ver=  ������� ������ ��� ����������� ^>
echo %ver%
Set n=TrackConverter_
Set ext=.zip
Set name=%n%%ver%%ext%
echo %name%
call copy_release_program.bat
"C:\Program Files\7-Zip\7z.exe" a -tzip -ssw -mx7 -r0 -x@exclusions_arhive_program.txt "D:\Clouds\Share\track converter\%name%" "D:\Clouds\GDrive\Trips\Converter\"