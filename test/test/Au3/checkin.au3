#include <MsgBoxConstants.au3>


$hWnd = openDuoliao()
WinActivate($hWnd)

   ;WinWait("多聊", "")
   ;If Not WinActivate("多聊", "") Then _
   ;WinActivate("多聊", "")
   ;WinWaitActive("多聊", "")

checkin()
Local $aWindowPosition = WinGetPos("多聊")

;MouseMove($aWindowPosition[0], $aWindowPosition[1] + 80)

;MsgBox($MB_SYSTEMMODAL, "", $aWindowPosition[0] & "," & $aWindowPosition[1] & "," & $aWindowPosition[2] & "," & $aWindowPosition[3])

Global $avatarArea[4]
$avatarArea[0]= $aWindowPosition[0]
$avatarArea[1]= $aWindowPosition[1] + 80
$avatarArea[2]= $aWindowPosition[0] + 100
$avatarArea[3]= $aWindowPosition[3]

Global $sessionArea[4]
$sessionArea[0] = $aWindowPosition[0] + 120
$sessionArea[1] = $aWindowPosition[1]
$sessionArea[2] = $aWindowPosition[0] + 300
$sessionArea[3] = $aWindowPosition[3]

Global $url = "http://qyhgateway.ihxlife.com/api/v1/other/query/authorize?timestamp=1546523890746&nonce=7150788195ff4a4fa0ae73d56a4245d0&trade_source=TMS&signature=D5CE85CD68327998A7C78EB0D48B806F&data=%7B%22redirectURL%22%3A%22http%3A%2F%2Ftms.ihxlife.com%2Ftms%2Fhtml%2F1_kqlr%2Fsign.html%22%2C%22attach%22%3A%2200000000000000105723%22%7D"

;selected user
If Not(findColor($avatarArea, 0xa9a9ab) = Null) Then
   MouseClick("left")
   Sleep(200)
   ;FileTransfer Session
   If Not(findColor($sessionArea, 0x2ba245) = Null) Then
	  MouseClick("left")
	  MouseClick("left")

	  Sleep(200)

	  WinWaitActive("File Transfer")
	  ; Add new data to the clipboard.
	  ClipPut($url)
	  Send("^v")
	  Sleep(200)

	  Send("!s")
	  Sleep(200)

	  openUrl()

	  WinClose("File Transfer")
   EndIf
EndIf
Sleep(200)

;normal user
If Not(findColor($avatarArea, 0xa7c2f7) = Null) Then
   MouseClick("left")
   Sleep(200)
   ;FileTransfer Session
   If Not(findColor($sessionArea, 0x2ba245) = Null) Then
	  MouseClick("left")
	  MouseClick("left")

	  Sleep(200)

	  WinWaitActive("File Transfer")
	  ; Add new data to the clipboard.
	  ClipPut($url)
	  Send("^v")
	  Sleep(200)

	  Send("!s")
	  Sleep(200)

	  openUrl()

	  WinClose("File Transfer")
   EndIf
EndIf
Sleep(200)

Func openUrl()
   ; 0x9eea6a message box color
   Local $hFileTransferWnd = WinGetHandle("File Transfer")
   Local $aFileTransferArea = WinGetPos("File Transfer")
   $aCoord = findColorReverse($aFileTransferArea, 0x9eea6a)
   If Not($aCoord = Null) Then
	  MouseMove($aCoord[0] - 200, $aCoord[1] - 50)
	  Sleep(200)
	  MouseClick("left")
	  Sleep(200)

	  WinWaitActive("WeChat")
	  Sleep(3000)
	  WinClose("WeChat")
   EndIf
EndFunc

Func openDuoliao()
   Return WinGetHandle("多聊", "")
EndFunc

Func checkin()
   WinWait("WeChat", "")
   If Not WinActivate("WeChat", "") Then _
   WinActivate("WeChat", "")
   WinWaitActive("WeChat", "")
   WinClose("WeChat", "")
EndFunc

Func findColorReverse($aWindowPosition, $color)
   Global $aPos[4]
   $aPos[0] = $aWindowPosition[2]
   $aPos[1] = $aWindowPosition[3]
   $aPos[2] = $aWindowPosition[0]
   $aPos[3] = $aWindowPosition[1]
   Return findColor($aPos, $color)
EndFunc

Func findColor($aWindowPosition, $color)
   ; Find a pure red pixel in the range 0,0-20,300
   ;MsgBox($MB_SYSTEMMODAL, "", $hWnd)
   Local $aCoord = PixelSearch($aWindowPosition[0] * 2, $aWindowPosition[1] * 2, $aWindowPosition[2] * 2, $aWindowPosition[3] * 2, $color, 0, 1, $hWnd)
   ;Local $aCoord = PixelSearch(0, 0, @DesktopWidth, @DesktopHeight, 0xFF0000)

   If Not @error Then

	  ;MsgBox($MB_SYSTEMMODAL, "", "X and Y are: " & $aCoord[0] & "," & $aCoord[1])
	  MouseMove($aCoord[0] / 2, $aCoord[1] / 2)

	  Local $normalizedCoord[2]
	  $normalizedCoord[0] = $aCoord[0] / 2
	  $normalizedCoord[1] = $aCoord[1] / 2
	  Return $normalizedCoord
	  ;MsgBox($MB_SYSTEMMODAL, "", "X and Y are: " & $aCoord[0] & "," & $aCoord[1])
   Else
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
