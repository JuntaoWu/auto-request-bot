Global $argc = $CmdLine[0]
Global $argPath = $argc > 0 ? $CmdLine[1] : ""

$title = "[REGEXPTITLE:(Open|打开)]"

If Not $argPath Then _
    MsgBox("Error", "Error", "No face image provided")

If Not WinActivate($title) Then _
    WinActivate($title)
Local $hWnd = WinWaitActive($title)

If Not $hWnd Then _
	MsgBox("Error", "Error", "No window found")

Sleep(1000)

ControlFocus($hWnd, "", "[CLASS:Edit;INSTANCE:1]")
ControlSend($hWnd, "", "[CLASS:Edit;INSTANCE:1]", $argPath)

;~ Send($argPath)
Sleep(1000)
ControlFocus($hWnd, "", "[CLASS:Button;INSTANCE:1]")
ControlClick($hWnd, "", "[CLASS:Button;INSTANCE:1]")

;~ Send("{Enter}")
;~ MsgBox("OpenFileDialog", "Info", @WindowsDir)
;~ FileChangeDir(@WindowsDir)
;~ ControlClick("Open", "Default.rdp", "left")
;~ ControlClick("Open", "Open", "left")