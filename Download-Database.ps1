$appName = 'mobi.futurestate.breathingroom'
$dbName  = 'fsm-breathingroom'

adb shell "run-as $appName chmod 644 /data/data/$appName/files/$dbName"
adb pull "/data/data/$appName/files/$dbName" "./$dbName.sqlite3"
Invoke-Item ".\$dbName.sqlite3"
