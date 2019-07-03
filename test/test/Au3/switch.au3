#include <MsgBoxConstants.au3>
#include <Debug.au3>
#include <GetDPI.au3>

;~ _DebugSetup("checkin")

Global $colorSelectedUser = 0xa9a9ab
Global $colorNormalUser = 0xa7c2f7
Global $colorFileTransfer = 0x2ba245
Global $colorMessageBox = 0x9eea6a

Global $titleDuoliao = "多聊"
Global $titleFileTransfer = "[CLASS:ChatWnd;REGEXPTITLE:(File Transfer|文件传输助手)]"
Global $titleWeChatBrowser = "[CLASS:CefWebViewWnd;REGEXPTITLE:(WeChat|微信)]"

;~ Global $dpiFactor = RegRead("HKEY_CURRENT_CONFIG\Software\Fonts", "LogPixels") / 96
;~ Global $dpiFactor = _GetDPI()[2]
;~ MsgBox($MB_SYSTEMMODAL, "", $dpiFactor)
;~ _DebugOut("dpiFactor: " & $dpiFactor & _GetDPI()[2])

Global $hWnd = openMainWindow($titleDuoliao)

; Main process
Main()

;drawRect($aWindowPosition)
;MouseMove($aWindowPosition[0], $aWindowPosition[1] + 80)
;MsgBox($MB_SYSTEMMODAL, "", $aWindowPosition[0] & "," & $aWindowPosition[1] & "," & $aWindowPosition[2] & "," & $aWindowPosition[3])

;~ Global $avatarArea[4]
;~ $avatarArea[0]= $aWindowPosition[0] 		   ; x
;~ $avatarArea[1]= $aWindowPosition[1] + 80 	   ; y
;~ $avatarArea[2]= 100							      ; width
;~ $avatarArea[3]= $aWindowPosition[3] - 80	   ; height

; checkin selectedUser
; checkin($sessionArea)
;~ If Not(findColorByPosition($avatarArea, $colorSelectedUser) = Null) Then
;~    MouseClick("left")
;~    Sleep(200)
;~    checkin()
;~ EndIf

; checkin normalUser
;~ If Not(findColorByPosition($avatarArea, $colorNormalUser) = Null) Then
;~    MouseClick("left")
;~    Sleep(200)
;~    checkin()
;~ EndIf

Func openMainWindow($title)
   WinWait($title)
   If Not WinActivate($title) Then _
      WinActivate($title)
   WinWaitActive($title)
   Return WinGetHandle($title)
EndFunc

Func Main()
   openMainWindow($titleDuoliao)
   Send("!`")
   Sleep(200)
   Exit(0)
EndFunc
