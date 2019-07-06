Global $argc = $CmdLine[0]
Global $argPath = $argc > 0 ? $CmdLine[1] : ""

$title = "[REGEXPTITLE:(Open|打开)]"

If Not WinActivate($title) Then _
    WinActivate($title)
WinWaitActive($title)

Send($argPath)
Sleep(500)
Send("{Enter}")
;~ MsgBox("OpenFileDialog", "Info", @WindowsDir)
;~ FileChangeDir(@WindowsDir)
;~ ControlClick("Open", "Default.rdp", "left")
;~ ControlClick("Open", "Open", "left")