chcp 1251

rem ������� �����
rd "D:\Clouds\google.vasily.mett\Documents\������\���������\Data\ETOPO2\SQL" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\������\���������\ru" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\������\���������\Images" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\������\���������\Schemes" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\������\���������\Docs" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\������\���������\x64 /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\������\���������\x86 /Q /S

rem �������� ������
del "D:\Clouds\google.vasily.mett\Documents\������\���������\Res.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\ZedGraph.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\Lib.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\ICSharpCode.SharpZipLib.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\GMap.NET.WindowsForms.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\GMap.NET.Core.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\TrackConverter.exe"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\System.Data.SQLite.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\Newtonsoft.Json.dll"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\license.txt"
del "D:\Clouds\google.vasily.mett\Documents\������\���������\readme.txt"

rem �����
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Images\"*.* "D:\Clouds\google.vasily.mett\Documents\������\���������\Images\" /Y /E 
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Docs\"*.* "D:\Clouds\google.vasily.mett\Documents\������\���������\Docs\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Schemes\"*.* "D:\Clouds\google.vasily.mett\Documents\������\���������\Schemes\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\ru\"*.* "D:\Clouds\google.vasily.mett\Documents\������\���������\ru\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Data\ETOPO2\SQL\"*.* "D:\Clouds\google.vasily.mett\Documents\������\���������\Data\ETOPO2\SQL\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\x64\"*.* "D:\Clouds\google.vasily.mett\Documents\������\���������\x64\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\x86\"*.* "D:\Clouds\google.vasily.mett\Documents\������\���������\x86\" /Y

rem �����
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\GMap.NET.Core.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\GMap.NET.WindowsForms.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\ICSharpCode.SharpZipLib.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Lib.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Res.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\TrackConverter.exe" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\ZedGraph.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\license.txt" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\System.Data.SQLite.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Newtonsoft.Json.dll" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\readme.txt" "D:\Clouds\google.vasily.mett\Documents\������\���������\" /Y



rem pause