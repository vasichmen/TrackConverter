chcp 1251

rem ������� �����
rd "D:\Clouds\GDrive\Documents\������\���������\Data\ETOPO2\SQL" /Q /S
rd "D:\Clouds\GDrive\Documents\������\���������\ru" /Q /S
rd "D:\Clouds\GDrive\Documents\������\���������\Images" /Q /S
rd "D:\Clouds\GDrive\Documents\������\���������\Schemes" /Q /S
rd "D:\Clouds\GDrive\Documents\������\���������\Docs" /Q /S
rd "D:\Clouds\GDrive\Documents\������\���������\x64 /Q /S
rd "D:\Clouds\GDrive\Documents\������\���������\x86 /Q /S

rem �������� ������
del "D:\Clouds\GDrive\Documents\������\���������\Res.dll"
del "D:\Clouds\GDrive\Documents\������\���������\ZedGraph.dll"
del "D:\Clouds\GDrive\Documents\������\���������\Lib.dll"
del "D:\Clouds\GDrive\Documents\������\���������\ICSharpCode.SharpZipLib.dll"
del "D:\Clouds\GDrive\Documents\������\���������\GMap.NET.WindowsForms.dll"
del "D:\Clouds\GDrive\Documents\������\���������\GMap.NET.Core.dll"
del "D:\Clouds\GDrive\Documents\������\���������\TrackConverter.exe"
del "D:\Clouds\GDrive\Documents\������\���������\System.Data.SQLite.dll"
del "D:\Clouds\GDrive\Documents\������\���������\Newtonsoft.Json.dll"
del "D:\Clouds\GDrive\Documents\������\���������\license.txt"
del "D:\Clouds\GDrive\Documents\������\���������\readme.txt"

rem �����
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Images\"*.* "D:\Clouds\GDrive\Documents\������\���������\Images\" /Y /E 
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Docs\"*.* "D:\Clouds\GDrive\Documents\������\���������\Docs\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Schemes\"*.* "D:\Clouds\GDrive\Documents\������\���������\Schemes\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ru\"*.* "D:\Clouds\GDrive\Documents\������\���������\ru\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Data\ETOPO2\SQL\"*.* "D:\Clouds\GDrive\Documents\������\���������\Data\ETOPO2\SQL\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\x64\"*.* "D:\Clouds\GDrive\Documents\������\���������\x64\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\x86\"*.* "D:\Clouds\GDrive\Documents\������\���������\x86\" /Y

rem �����
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\GMap.NET.Core.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\GMap.NET.WindowsForms.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ICSharpCode.SharpZipLib.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Lib.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Res.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\TrackConverter.exe" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ZedGraph.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\license.txt" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\System.Data.SQLite.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Newtonsoft.Json.dll" "D:\Clouds\GDrive\Documents\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\readme.txt" "D:\Clouds\GDrive\Documents\������\���������\" /Y



rem pause