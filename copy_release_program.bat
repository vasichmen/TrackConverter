chcp 1251

rem очистка папок
rd "D:\Clouds\GDrive\Походы\Конвертер\Data\ETOPO2\SQL" /Q /S
rd "D:\Clouds\GDrive\Походы\Конвертер\ru" /Q /S
rd "D:\Clouds\GDrive\Походы\Конвертер\Images" /Q /S
rd "D:\Clouds\GDrive\Походы\Конвертер\Templates" /Q /S
rd "D:\Clouds\GDrive\Походы\Конвертер\Docs" /Q /S
rd "D:\Clouds\GDrive\Походы\Конвертер\x64 /Q /S
rd "D:\Clouds\GDrive\Походы\Конвертер\x86 /Q /S

rem удаление файлов
del "D:\Clouds\GDrive\Походы\Конвертер\Res.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\ZedGraph.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\Lib.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\ICSharpCode.SharpZipLib.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\template.dot"
del "D:\Clouds\GDrive\Походы\Конвертер\GMap.NET.WindowsForms.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\GMap.NET.Core.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\TrackConverter.exe"
del "D:\Clouds\GDrive\Походы\Конвертер\System.Data.SQLite.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\Newtonsoft.Json.dll"
del "D:\Clouds\GDrive\Походы\Конвертер\license.txt"
del "D:\Clouds\GDrive\Походы\Конвертер\readme.txt"

rem папки
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Images\"*.* "D:\Clouds\GDrive\Походы\Конвертер\Images\" /Y /E 
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Docs\"*.* "D:\Clouds\GDrive\Походы\Конвертер\Docs\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ru\"*.* "D:\Clouds\GDrive\Походы\Конвертер\ru\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Data\ETOPO2\SQL\"*.* "D:\Clouds\GDrive\Походы\Конвертер\Data\ETOPO2\SQL\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\x64\"*.* "D:\Clouds\GDrive\Походы\Конвертер\x64\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\x86\"*.* "D:\Clouds\GDrive\Походы\Конвертер\x86\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Templates\"*.* "D:\Clouds\GDrive\Походы\Конвертер\Templates\" /Y

rem файлы
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\GMap.NET.Core.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\GMap.NET.WindowsForms.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ICSharpCode.SharpZipLib.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Lib.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Res.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\TrackConverter.exe" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\ZedGraph.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\license.txt" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\System.Data.SQLite.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\Newtonsoft.Json.dll" "D:\Clouds\GDrive\Походы\Конвертер\" /Y
xcopy "D:\Clouds\GDrive\Projects\CS\TrackConverter\UI\bin\Release\readme.txt" "D:\Clouds\GDrive\Походы\Конвертер\" /Y


rem pause