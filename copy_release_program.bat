chcp 1251

rem ������� �����
rd "D:\Clouds\GDrive\������\���������\Data\ETOPO2\SQL" /Q /S
rd "D:\Clouds\GDrive\������\���������\ru" /Q /S
rd "D:\Clouds\GDrive\������\���������\Images" /Q /S
rd "D:\Clouds\GDrive\������\���������\Templates" /Q /S
rd "D:\Clouds\GDrive\������\���������\Docs" /Q /S
rd "D:\Clouds\GDrive\������\���������\x64 /Q /S
rd "D:\Clouds\GDrive\������\���������\x86 /Q /S

rem �������� ������
del "D:\Clouds\GDrive\������\���������\Res.dll"
del "D:\Clouds\GDrive\������\���������\ZedGraph.dll"
del "D:\Clouds\GDrive\������\���������\Lib.dll"
del "D:\Clouds\GDrive\������\���������\ICSharpCode.SharpZipLib.dll"
del "D:\Clouds\GDrive\������\���������\template.dot"
del "D:\Clouds\GDrive\������\���������\GMap.NET.WindowsForms.dll"
del "D:\Clouds\GDrive\������\���������\GMap.NET.Core.dll"
del "D:\Clouds\GDrive\������\���������\TrackConverter.exe"
del "D:\Clouds\GDrive\������\���������\System.Data.SQLite.dll"
del "D:\Clouds\GDrive\������\���������\Newtonsoft.Json.dll"
del "D:\Clouds\GDrive\������\���������\license.txt"
del "D:\Clouds\GDrive\������\���������\readme.txt"

rem �����
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Images\"*.* "D:\Clouds\GDrive\������\���������\Images\" /Y /E 
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Docs\"*.* "D:\Clouds\GDrive\������\���������\Docs\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ru\"*.* "D:\Clouds\GDrive\������\���������\ru\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Data\ETOPO2\SQL\"*.* "D:\Clouds\GDrive\������\���������\Data\ETOPO2\SQL\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\x64\"*.* "D:\Clouds\GDrive\������\���������\x64\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\x86\"*.* "D:\Clouds\GDrive\������\���������\x86\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Templates\"*.* "D:\Clouds\GDrive\������\���������\Templates\" /Y

rem �����
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\GMap.NET.Core.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\GMap.NET.WindowsForms.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ICSharpCode.SharpZipLib.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Lib.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Res.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\TrackConverter.exe" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ZedGraph.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\license.txt" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\System.Data.SQLite.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Newtonsoft.Json.dll" "D:\Clouds\GDrive\������\���������\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\readme.txt" "D:\Clouds\GDrive\������\���������\" /Y


rem pause