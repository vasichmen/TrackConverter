chcp 1251
set day=%DATE:~0,2%
set month=%DATE:~3,2%
set year=%DATE:~6,4%
"C:\Program Files\7-Zip\7z.exe" a -tzip -ssw -mx7 -r0 -x@exclusions.txt "D:\Clouds\google.vasily.mett\Documents\Projects\CS\TrackConverter\TrackConverter_%day%-%month%-%year%.zip" "D:\Clouds\google.vasily.mett\Documents\Projects\CS\TrackConverter"