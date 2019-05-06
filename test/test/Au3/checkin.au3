#include <MsgBoxConstants.au3>
#include <Debug.au3>
#include <GetDPI.au3>

;~ _DebugSetup("checkin")

Global $argc = $CmdLine[0]
Global $argUrl = $argc > 1 ? $CmdLine[1] : "http://qyhgateway.ihxlife.com/api/v1/other/query/authorize?timestamp=1546523890746&nonce=7150788195ff4a4fa0ae73d56a4245d0&trade_source=TMS&signature=D5CE85CD68327998A7C78EB0D48B806F&data=%7B%22redirectURL%22%3A%22http%3A%2F%2Ftms.ihxlife.com%2Ftms%2Fhtml%2F1_kqlr%2Fsign.html%22%2C%22attach%22%3A%2200000000000000105723%22%7D"
Global $userCount = $argc > 2 ? $CmdLine[2] : 1

;~ $argUrl = "http://www.baidu.com/"

;~ _DebugOut("argc, argv:" & $argc & $userCount)

Global $colorSelectedUser = 0xa9a9ab
Global $colorNormalUser = 0xa7c2f7
Global $colorFileTransfer = 0x2ba245
Global $colorMessageBox = 0x9eea6a

Global $titleDuoliao = "多聊"
Global $titleFileTransfer = "[CLASS:ChatWnd;REGEXPTITLE:(File Transfer|文件传输助手)]"
Global $titleWeChatBrowser = "[CLASS:CefWebViewWnd;REGEXPTITLE:(WeChat|微信)]"

;~ Global $dpiFactor = RegRead("HKEY_CURRENT_CONFIG\Software\Fonts", "LogPixels") / 96
Global $dpiFactor = _GetDPI()[2]
;~ MsgBox($MB_SYSTEMMODAL, "", $dpiFactor)
;~ _DebugOut("dpiFactor: " & $dpiFactor & _GetDPI()[2])

Global $hWnd = openMainWindow($titleDuoliao)
Global $aWindowPosition = WinGetPos($titleDuoliao)

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
   Local $sessionArea[4]
   $sessionArea[0] = $aWindowPosition[0] + 120  ; x
   $sessionArea[1] = $aWindowPosition[1]        ; y
   $sessionArea[2] = 300                        ; width
   $sessionArea[3] = $aWindowPosition[3]        ; height

   For $i = 1 To $userCount Step +1
      checkin($sessionArea, $argUrl)
      Send("!~")
   Next
   ConsoleWrite(0)
   Exit(0)
EndFunc

Func checkin($sessionArea, $url)
   ;FileTransfer Session
   Local $aFileTransferIcon = findColorByPosition($sessionArea, $colorFileTransfer, $hWnd)
   If Not($aFileTransferIcon = Null) Then
      MouseMove($aFileTransferIcon[0], $aFileTransferIcon[1])
      Sleep(100)

      MouseClick("left")
      MouseClick("left")
      Sleep(200)

      WinWaitActive($titleFileTransfer)

      ; todo: Ensure Textbox had been auto-focused
      openUrl($url)
      Sleep(500)

      WinClose($titleFileTransfer)
   Else
      ConsoleWriteError(-1)
      Exit(-1)
   EndIf
EndFunc

Func openUrl($url)
   ; Add new data to the clipboard.
   ClipPut($url)
   Send("^v")
   Sleep(200)

   ; Send Message
   Send("!s")
   Sleep(500)

   ; Click url in message box
   Local $hFileTransferWnd = WinGetHandle($titleFileTransfer)
   Local $aFileTransferArea = WinGetPos($titleFileTransfer)

   ; MsgBox($MB_SYSTEMMODAL, "", $hFileTransferWnd)
   ; MsgBox($MB_SYSTEMMODAL, "", $aFileTransferArea[0] & "," & $aFileTransferArea[1] & "," & $aFileTransferArea[2] & "," & $aFileTransferArea[3])
   ;~ drawRect($aFileTransferArea)

   Local $aMessageBox = findColorByPositionReverse($aFileTransferArea, $colorMessageBox, $hFileTransferWnd)
   If Not($aMessageBox = Null) Then
	  MouseMove($aMessageBox[0] + (12 * $dpiFactor), $aMessageBox[1] - (15 * $dpiFactor))
	  Sleep(100)

	  MouseClick("left")
	  Sleep(200)

	  WinWaitActive($titleWeChatBrowser)
	  Sleep(5000)
	  WinClose($titleWeChatBrowser)
   Else
      ConsoleWriteError(-1)
      Exit(-1)
   EndIf
EndFunc

Func drawRect($aRectArea)
   MouseMove($aRectArea[0], $aRectArea[1])
   Sleep(300)
   MouseMove($aRectArea[0] + $aRectArea[2], $aRectArea[1])
   Sleep(300)
   MouseMove($aRectArea[0], $aRectArea[1] + $aRectArea[3])
   Sleep(300)
   MouseMove($aRectArea[0] + $aRectArea[2], $aRectArea[1] + $aRectArea[3])
   Sleep(300)
EndFunc

Func findColorByPositionReverse($aWindowPosition, $color, $hWnd = Null)
   Local $aCoord[4]
   $aCoord[0] = $aWindowPosition[0]
   $aCoord[3] = $aWindowPosition[1]
   $aCoord[2] = $aWindowPosition[0] + $aWindowPosition[2]
   $aCoord[1] = $aWindowPosition[1] + $aWindowPosition[3]

   Return findColorByCoord($aCoord, $color, $hWnd)
EndFunc

; findColor by x, y, width, height
Func findColorByPosition($aWindowPosition, $color, $hWnd = Null)
   Local $aCoord[4]
   $aCoord[0] = $aWindowPosition[0]
   $aCoord[1] = $aWindowPosition[1]
   $aCoord[2] = $aWindowPosition[0] + $aWindowPosition[2]
   $aCoord[3] = $aWindowPosition[1] + $aWindowPosition[3]
   Return findColorByCoord($aCoord, $color, $hWnd)
EndFunc

; findColor by left, top, right, bottom & dpi factor need to be considered.
Func findColorByCoord($aWindowCoord, $color, $hWnd = Null)
   ; _DebugOut("left:" & $aWindowCoord[0] * $dpiFactor & ", top:" & $aWindowCoord[1] * $dpiFactor & "right:" & $aWindowCoord[2] * $dpiFactor & "bottom:" & $aWindowPosition[3] * $dpiFactor)
   ; Find a pure red pixel in the range 0,0-20,300
   ;MsgBox($MB_SYSTEMMODAL, "", $hWnd)
   ;~ _DebugOut("hWnd" & $hWnd)
   Local $aCoord = PixelSearch($aWindowCoord[0] * $dpiFactor, $aWindowCoord[1] * $dpiFactor, $aWindowCoord[2] * $dpiFactor, $aWindowCoord[3] * $dpiFactor, $color, 0, 1, $hWnd)
   ;Local $aCoord = PixelSearch(0, 0, @DesktopWidth, @DesktopHeight, 0xFF0000)

   ; MsgBox($MB_SYSTEMMODAL, "", "findColor")

   If Not @error Then
	  ; MsgBox($MB_SYSTEMMODAL, "", "X and Y are: " & $aCoord[0] & "," & $aCoord[1])
	  ;~ MouseMove($aCoord[0] / $dpiFactor, $aCoord[1] / $dpiFactor)

	  Local $normalizedCoord[2]
	  $normalizedCoord[0] = $aCoord[0] / $dpiFactor
	  $normalizedCoord[1] = $aCoord[1] / $dpiFactor
	  Return $normalizedCoord
   Else
     $error = "Error: Cannot find color" & $color & "in:" & "left: " & $aWindowCoord[0] * $dpiFactor & ", top:" & $aWindowCoord[1] * $dpiFactor & ", right: " & $aWindowCoord[2] * $dpiFactor & ", bottom:" & $aWindowPosition[3] * $dpiFactor
     ;~ _DebugOut($error)
	  MsgBox($MB_SYSTEMMODAL, "", $error)
	  Return Null
   EndIf
EndFunc

Func _findchecksum($checksum, $width, $height, $pcolor, $x= 0, $y = 0, _
   $d_width = @DesktopWidth, $d_height = @DesktopHeight)

   $current_y = $d_height - 1
   While 1
	  $xy = PixelSearch($x, $y, $d_width - 1, $current_y, $pcolor)
	  If @error AND $current_y = ($d_height - 1) Then
		 Return 0
	  ElseIf @error Then
		 $x = 0
		 $y = $current_y + 1
		 $current_y = ($d_height - 1)
	  ElseIf $checksum = PixelCheckSum($xy[0], $xy[1], $xy[0] + $width, _
		 $xy[1] + $height) Then
		 Return $xy
	  Else
		 $x = $xy[0] + 1
		 $y = $xy[1]
		 $current_y = $y
	  EndIf
   WEnd
EndFunc
