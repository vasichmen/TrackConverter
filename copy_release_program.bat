chcp 1251

rem очистка папок
rd "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Data\ETOPO2\SQL" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\ru" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Images" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Schemes" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Docs" /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\x64 /Q /S
rd "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\x86 /Q /S

rem удаление файлов
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Res.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\ZedGraph.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Lib.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\ICSharpCode.SharpZipLib.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\GMap.NET.WindowsForms.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\GMap.NET.Core.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\TrackConverter.exe"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\System.Data.SQLite.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Newtonsoft.Json.dll"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\license.txt"
del "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\readme.txt"

rem папки
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Images\"*.* "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Images\" /Y /E 
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Docs\"*.* "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Docs\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Schemes\"*.* "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Schemes\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\ru\"*.* "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\ru\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Data\ETOPO2\SQL\"*.* "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\Data\ETOPO2\SQL\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\x64\"*.* "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\x64\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\x86\"*.* "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\x86\" /Y

rem файлы
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\GMap.NET.Core.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\GMap.NET.WindowsForms.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\ICSharpCode.SharpZipLib.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Lib.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Res.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\TrackConverter.exe" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\ZedGraph.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\license.txt" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\System.Data.SQLite.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\Newtonsoft.Json.dll" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y
xcopy "D:\Clouds\google.vasily.mett\Documents\Projects\C#\TrackConverter\UI\bin\Release\readme.txt" "D:\Clouds\google.vasily.mett\Documents\Походы\Конвертер\" /Y



rem pause