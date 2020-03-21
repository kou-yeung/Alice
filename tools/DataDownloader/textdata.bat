SET sheetid=1nzEU5Vrv-eSsULHLBdwi0nsfscMWLfHhS1tYVjapXF0
SET apikey=AIzaSyChKdUxx0lPQsb_LF08bJ0XqVlvS4b5aKw
SET sheetname=TextData
SET dataStart=4
REM SET crypt_iv=Q7V10/1qKgkV61CGk25PKA==
REM SET crypt_key=FYxcgJgSVVrO5hMGd+zehpM8FI/y8e0+eAJ+KQKsmZg=

REM à√çÜâªÇµÇ»Ç¢
REM CALL DataDownloader.exe sheetid=%sheetid% apikey=%apikey% sheetname=%sheetname% flagLine=2 dataStart=%dataStart% output="./../../app/client/Contrib.Gate/Assets/Resources/Entities/" crypt_iv=%crypt_iv% crypt_key=%crypt_key%

CALL DataDownloader.exe sheetid=%sheetid% apikey=%apikey% sheetname=%sheetname% flagLine=2 dataStart=%dataStart% output="./../../prog/client/Alice/Assets/AddressableAssets/Entities/"
CALL DataDownloader.exe sheetid=%sheetid% apikey=%apikey% sheetname=%sheetname% flagLine=2 dataStart=%dataStart% output="./../../resources/masterdata/cl/"
CALL DataDownloader.exe sheetid=%sheetid% apikey=%apikey% sheetname=%sheetname% flagLine=1 dataStart=%dataStart% output="./../../resources/masterdata/sv/"
