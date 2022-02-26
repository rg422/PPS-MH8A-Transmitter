'-----------------------------------------------------------------------------------
' Oceanic Simulator Copyright, 2022 Nick Clark
' Wave code converted from "Tones Test" by David M. Hitchner, originating on www.TheScarms.com
'
' This program is free software: you can redistribute it and/or modify it under the terms of the
' GNU General Public License as published by the Free Software Foundation, either version 3 of
' the License, or (at your option) any later version.
'
' This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
' See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with this program.
' If not, see <https://www.gnu.org/licenses/>.


'-----------------------------------------------------------------------------------
' Wave File Format
'-----------------------------------------------------------------------------------
' RIFF Chunk   ( 12 bytes )
' 00 00 - 03  "RIFF"
' 04 04 - 07  Total Length to Follow  (Length of File - 8)
' 08 08 - 11  "WAVE"
'
' FORMAT Chunk ( 24 bytes )
' 0C 12 - 15  "fmt_"
' 10 16 - 19  Length of FORMAT Chunk  Always 0x10
' 14 20 - 21  Audio Format            Always 0x01
' 16 22 - 23  Channels                1 = Mono, 2 = Stereo
' 18 24 - 27  Sample Rate             In Hertz
' 1C 28 - 31  Bytes per Second        Sample Rate * Channels * Bits per Sample / 8
' 20 32 - 33  Bytes per Sample        Channels * Bits per Sample / 8
'                                       1 = 8 bit Mono
'                                       2 = 8 bit Stereo or 16 bit Mono
'                                       4 = 16 bit Stereo
' 22 34 - 35  Bits per Sample
'
' DATA Chunk
' 24 36 - 39  "data"
' 28 40 - 43  Length of Data          Samples * Channels * Bits per Sample / 8
' 2C 44 - End Data Samples
'              8 Bit = 0 to 255             unsigned bytes
'             16 Bit = -32,768 to 32,767    2's-complement signed integers
'-----------------------------------------------------------------------------------

Module Module1

    ' Force the file save when in debug mode
    'Public Const SaveFilePath = "C:\OceanicTXtest"
    Public Const SaveFilePath = ""
    Public Const SaveFileName = "OceanicTX.wav"
    Public Const MaxPSI = 5000 ' Teric stops at 5000 psi, binary max is 8190
    Public Const PSIIncrement = 2
    Public Const DefaultID = "104308"
    Public Const DefaultPSI = 248

    Public Const MODE_MONO = 0      ' Mono
    Public Const MODE_LR = 1        ' Stereo L+R
    Public Const MODE_L = 2         ' Stereo L
    Public Const MODE_R = 3         ' Stereo R

    Public Const RATE_8000 = 8000
    Public Const RATE_11025 = 11025
    Public Const RATE_22050 = 22050
    Public Const RATE_32000 = 32000
    Public Const RATE_44100 = 44100
    Public Const RATE_48000 = 48000
    Public Const RATE_88000 = 88000
    Public Const RATE_96000 = 96000
    Public Const RATE_192000 = 192000
    Public Const RATE_384000 = 384000

    Public Const BITS_8 = 8
    Public Const BITS_16 = 16

    Public Const NEC_Carrier = 38000 '38 kHz
    Public Const NEC_Volume = 1 ' wav volume
    Public Const NEC_PreGap = 0.003
    Public Const NEC_Preamble As String = "0010000001"

    ' Values by experimental analysys
    Public Const NEC_0_Burst = 0.0008502155
    Public Const NEC_0_Space = 0.0011354167
    Public Const NEC_0_TX_time = 0.0019856322
    Public Const NEC_1_Burst = 0.0008502155
    Public Const NEC_1_Space = 0.0021386218
    Public Const NEC_1_TX_time = 0.0029888373

    ' Potential 'ideal' values
    'Public Const NEC_0_Burst = 0.001 '1 ms
    'Public Const NEC_0_Space = 0.001 '1 ms
    'Public Const NEC_0_TX_time = 0.002 '2 ms
    'Public Const NEC_1_Burst = 0.001 '1 ms
    'Public Const NEC_1_Space = 0.002 '2 ms
    'Public Const NEC_1_TX_time = 0.003 '20 ms

    ' NEC standard values
    ' Logical '0' – a 562.5µs pulse burst followed by a 562.5µs space, with a total transmit time of 1.125ms
    ' Logical '1' – a 562.5µs pulse burst followed by a 1.6875ms space, with a total transmit time of 2.25ms
    'Public Const NEC_0_Burst = 0.0005625 '562.5 µs
    'Public Const NEC_0_Space = 0.0005625 '562.5 µs
    'Public Const NEC_0_TX_time = 0.001125 '1.125 ms
    'Public Const NEC_1_Burst = 0.0005625 '562.5 µs
    'Public Const NEC_1_Space = 0.0000016875 '1.6875 µs
    'Public Const NEC_1_TX_time = 0.00225 '2.25 ms

    Public Class ID
        Public Shared Function Code(Digit As String)
            Select Case Digit
                Case "0"
                    Return "1010"
                Case "1"
                    Return "1011"
                Case "2"
                    Return "1100"
                Case "3"
                    Return "0011"
                Case "4"
                    Return "1101"
                Case "5"
                    Return "0101"
                Case "6"
                    Return "0110"
                Case "7"
                    Return "0111"
                Case "8"
                    Return "1110"
                Case "9"
                    Return "1001"
                Case Else
                    Return "0000"
            End Select
        End Function
    End Class

    Public Class Battery
        Public Shared Function NumStates() As Integer
            Return 2
        End Function
        Public Shared Function DefaultState() As Integer
            Return 0 'Good
        End Function
        Public Shared Function StateName(i As Integer) As String
            Select Case i
                Case 0
                    Return "Good"
                Case 1
                    Return "Low"
                Case 2
                    Return "Critical"
                Case Else
                    MessageBox.Show("Program error, battery status index too large")
                    Return ""
            End Select
        End Function
        Public Shared Function Code(Status As String)
            Select Case Status
                Case "Good"
                    Return "0000"
                Case "Low"
                    Return "0010"
                Case "Critical"
                    Return "0001"
                Case Else
                    Return Status
            End Select
        End Function

    End Class

    Public Class Tx
        Public Shared Function NumModels() As Integer
            Return 3
        End Function
        Public Shared Function DefaultModel() As Integer
            Return 1 'Yellow
        End Function
        Public Shared Function Steps() As Integer
            Return 100
        End Function
        Public Shared Function ModelName(i As Integer) As String
            Select Case i
                Case 0
                    Return "Swift"  ' 4.8 to 5.2 seconds
                Case 1
                    Return "Yellow" ' 5.2 seconds
                Case 2
                    Return "Grey"   ' 5.0 seconds
                Case 3
                    Return "Green"  ' 4.8 seconds
                Case Else
                    MessageBox.Show("Program error, TX Model index too large")
                    Return ""
            End Select
        End Function
        Public Shared Function IntervalText(Model As String) As String
            Select Case Model
                Case "Swift"
                    Return "4.8 to 5.2 seconds"
                Case "Yellow"
                    Return "5.2 seconds"
                Case "Grey"
                    Return "5.0 seconds"
                Case "Green"
                    Return "4.8 seconds"
                Case Else
                    MessageBox.Show("Program error, TX Model not defined")
                    Return ""
            End Select
        End Function
        Public Shared Function Interval(Model As String) As Single
            Select Case Model
                Case "Swift"
                    Return 5.0 ' centre interval time
                Case "Yellow"
                    Return 5.2
                Case "Grey"
                    Return 5.0
                Case "Green"
                    Return 4.8
                Case Else
                    MessageBox.Show("Program error, TX Model not defined")
                    Return 0
            End Select
        End Function
        Public Shared Function Variation(Model As String) As Single
            Select Case Model
                Case "Swift"
                    Return 0.2 ' +/- 0.2 seconds
                Case "Yellow"
                    Return 0
                Case "Grey"
                    Return 0
                Case "Green"
                    Return 0
                Case Else
                    MessageBox.Show("Program error, TX Model not defined")
                    Return 0
            End Select
        End Function
        Public Shared Function Image(Model As String) As Image
            Select Case Model
                Case "Swift"
                    Return TXSwift
                Case "Yellow"
                    Return TXYellow
                Case "Grey"
                    Return TXGrey
                Case "Green"
                    Return TXGreen
                Case Else
                    MessageBox.Show("Program error, TX Model not defined")
                    Return TXNull
            End Select
        End Function
    End Class

    Public ReadOnly TXGreen As Image = My.Resources.TXGreen
    Public ReadOnly TXGrey As Image = My.Resources.TXGrey
    Public ReadOnly TXYellow As Image = My.Resources.TXYellow
    Public ReadOnly TXSwift As Image = My.Resources.TXSwift
    Public ReadOnly TXNull As Image = My.Resources.TXNull

    Public Structure SINEWAVE
        Public dblFrequency As Double
        Public dblDataSlice As Double
        Public dblAmplitudeL As Double
        Public dblAmplitudeR As Double
    End Structure

    Private Structure DTMF
        Public bytTones() As Byte
    End Structure

    Dim bytSound() As Byte 'byte array for constructing wav sound
    Dim bytPlay() As Byte  'byte array for playing wav sound

    Private PI As Double
    Private intBits As Integer
    Private lngSampleRate As Long
    Private intSampleBytes As Integer
    Private intAudioMode As Integer
    Private dblFrequency As Double
    Private dblVolumeL As Double
    Private dblVolumeR As Double
    Private intAudioWidth As Integer

    Private Const SND_ALIAS = &H10000
    Private Const SND_ALIAS_ID = &H110000
    Private Const SND_ALIAS_START = 0
    Private Const SND_APPLICATION = &H80
    Private Const SND_ASYNC = &H1
    Private Const SND_FILENAME = &H20000
    Private Const SND_LOOP = &H8
    Private Const SND_MEMORY = &H4
    Private Const SND_NODEFAULT = &H2
    Private Const SND_NOSTOP = &H10
    Private Const SND_NOWAIT = &H2000
    Private Const SND_PURGE = &H40
    Private Const SND_RESERVED = &HFF000000
    Private Const SND_RESOURCE = &H40004
    Private Const SND_SYNC = &H0
    Private Const SND_TYPE_MASK = &H170007
    Private Const SND_VALID = &H1F
    Private Const SND_VALIDFLAGS = &H17201F

    'Gain access to the medial player
    Private Declare Unicode Function PlaySoundFile Lib "winmm.dll" Alias "PlaySoundA" _
    (ByVal lpszName As String, ByVal hModule As Long, ByVal dwFlags As Long) As Long
    Private Declare Function PlaySoundMemory Lib "winmm.dll" Alias "PlaySoundA" _
    (ByRef ptrMemory As Byte, ByVal hModule As Long, ByVal dwFlags As Long) As Long

    '-----------------------------------------------------------------------------------
    ' Wav_Play - Plays the wav file from memory.
    '-----------------------------------------------------------------------------------
    Public Function Wav_Play(ByRef WavArray() As Byte) As Boolean
        Dim lngStatus As Long

        'Setup protected buffer to play from
        ReDim bytPlay(UBound(WavArray))
        bytPlay = bytSound

        lngStatus = PlaySoundMemory(bytPlay(0), 0, SND_MEMORY Or SND_APPLICATION Or
        SND_SYNC Or SND_NODEFAULT)

        If lngStatus = 0 Then
            Wav_Play = False
        Else
            Wav_Play = True
        End If
    End Function

    '-----------------------------------------------------------------------------------
    ' Wav_PlayLoop - Continuously plays the wav file from memory.
    '-----------------------------------------------------------------------------------
    Public Function Wav_PlayLoop(ByRef WavArray() As Byte) As Boolean
        Dim lngStatus As Long

        'Setup protected buffer to play from
        ReDim bytPlay(UBound(WavArray))
        bytPlay = bytSound

        lngStatus = PlaySoundMemory(bytPlay(0), 0&, SND_MEMORY Or SND_APPLICATION Or
        SND_ASYNC Or SND_LOOP Or SND_NODEFAULT)

        If lngStatus = 0 Then
            Wav_PlayLoop = False
        Else
            Wav_PlayLoop = True
        End If
    End Function

    '-----------------------------------------------------------------------------------
    ' Wav_Play - Plays the wav file from a file.
    '-----------------------------------------------------------------------------------
    Public Function Wav_PlayFile(FileName As String) As Boolean
        Dim lngStatus As Long

        lngStatus = PlaySoundFile(FileName, 0, SND_FILENAME Or SND_APPLICATION Or
        SND_SYNC Or SND_NODEFAULT)

        If lngStatus = 0 Then
            Wav_PlayFile = False
        Else
            Wav_PlayFile = True
        End If
    End Function

    '-----------------------------------------------------------------------------------
    ' Wav_PlayFileLoop - Continuously plays the wav file from a file.
    '-----------------------------------------------------------------------------------
    Public Function Wav_PlayFileLoop(FileName As String) As Boolean
        Dim lngStatus As Long

        lngStatus = PlaySoundFile(FileName, 0, SND_FILENAME Or SND_APPLICATION Or
        SND_ASYNC Or SND_LOOP Or SND_NODEFAULT)

        If lngStatus = 0 Then
            Wav_PlayFileLoop = False
        Else
            Wav_PlayFileLoop = True
        End If
    End Function

    '-----------------------------------------------------------------------------------
    ' Wav_BuildHeader - Builds the WAV file header based on the sample rate, resolution,
    '                   audio mode.  Also sets the volume level for other routines.
    '-----------------------------------------------------------------------------------
    Public Sub Wav_BuildHeader(ByRef WavArray() As Byte, SampleRate As Long,
    Resolution As Integer, AudioMode As Integer, VolumeL As Double, VolumeR As Double)
        Dim lngBytesASec As Long

        PI = 4.0# * Math.Atan(1.0#)

        ' Save parameters.
        lngSampleRate = SampleRate
        intBits = Resolution
        intAudioMode = AudioMode
        dblVolumeL = VolumeL
        dblVolumeR = VolumeR

        ReDim WavArray(0 To 43)

        '-------------------------------------------------------------------------------
        ' Fixed Data
        '-------------------------------------------------------------------------------
        WavArray(0) = 82   ' R
        WavArray(1) = 73   ' I
        WavArray(2) = 70   ' F
        WavArray(3) = 70   ' F
        WavArray(8) = 87   ' W
        WavArray(9) = 65   ' A
        WavArray(10) = 86  ' V
        WavArray(11) = 69  ' E
        WavArray(12) = 102 ' f
        WavArray(13) = 109 ' m
        WavArray(14) = 116 ' t
        WavArray(15) = 32  ' .
        WavArray(16) = 16  ' Length of Format Chunk
        WavArray(17) = 0   ' Length of Format Chunk
        WavArray(18) = 0   ' Length of Format Chunk
        WavArray(19) = 0   ' Length of Format Chunk
        WavArray(20) = 1   ' Audio Format
        WavArray(21) = 0   ' Audio Format
        WavArray(36) = 100 ' d
        WavArray(37) = 97  ' a
        WavArray(38) = 116 ' t
        WavArray(39) = 97  ' a

        '-------------------------------------------------------------------------------
        ' Bytes 22 - 23  Channels   1 = Mono, 2 = Stereo
        '-------------------------------------------------------------------------------
        Select Case intAudioMode
            Case MODE_MONO
                WavArray(22) = 1
                WavArray(23) = 0
                intAudioWidth = 1
            Case MODE_LR
                WavArray(22) = 2
                WavArray(23) = 0
                intAudioWidth = 2
            Case MODE_L
                WavArray(22) = 2
                WavArray(23) = 0
                intAudioWidth = 2
            Case MODE_R
                WavArray(22) = 2
                WavArray(23) = 0
                intAudioWidth = 2
        End Select

        '-------------------------------------------------------------------------------
        ' 24 - 27  Sample Rate             In Hertz
        '-------------------------------------------------------------------------------
        WavArray(24) = ExtractByte(lngSampleRate, 0)
        WavArray(25) = ExtractByte(lngSampleRate, 1)
        WavArray(26) = ExtractByte(lngSampleRate, 2)
        WavArray(27) = ExtractByte(lngSampleRate, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 34 - 35  Bits per Sample
        '-------------------------------------------------------------------------------
        Select Case intBits
            Case 8
                WavArray(34) = 8
                WavArray(35) = 0
                intSampleBytes = 1
            Case 16
                WavArray(34) = 16
                WavArray(35) = 0
                intSampleBytes = 2
        End Select

        '-------------------------------------------------------------------------------
        ' Bytes 28 - 31  Bytes per Second   Sample Rate * Channels * Bits per Sample / 8
        '-------------------------------------------------------------------------------
        lngBytesASec = lngSampleRate * intAudioWidth * intSampleBytes

        WavArray(28) = ExtractByte(lngBytesASec, 0)
        WavArray(29) = ExtractByte(lngBytesASec, 1)
        WavArray(30) = ExtractByte(lngBytesASec, 2)
        WavArray(31) = ExtractByte(lngBytesASec, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 32 - 33 Bytes per Sample     Channels * Bits per Sample / 8
        '                                       1 = 8 bit Mono
        '                                       2 = 8 bit Stereo or 16 bit Mono
        '                                       4 = 16 bit Stereo
        '-------------------------------------------------------------------------------
        If (intAudioMode = MODE_MONO) And (intBits = 8) Then
            WavArray(32) = 1
            WavArray(33) = 0
        End If

        If ((intAudioMode = MODE_LR) Or (intAudioMode = MODE_L) Or
        (intAudioMode = MODE_R)) And (intBits = 8) Then
            WavArray(32) = 2
            WavArray(33) = 0
        End If

        If (intAudioMode = MODE_MONO) And (intBits = 16) Then
            WavArray(32) = 2
            WavArray(33) = 0
        End If

        If ((intAudioMode = MODE_LR) Or (intAudioMode = MODE_L) Or
        (intAudioMode = MODE_R)) And (intBits = 16) Then
            WavArray(32) = 4
            WavArray(33) = 0
        End If

    End Sub

    '-----------------------------------------------------------------------------------
    ' Wav_WriteToFile - Writes the wav file to disk.
    '-----------------------------------------------------------------------------------
    Public Function Wav_WriteToFile(FileName As String, ByRef WavArray() As Byte) As Long
        Static blnInitialized As Boolean

        Dim fd As Integer
        Dim i As Long, lngFileSize As Long

        Wav_WriteToFile = 0
        On Error GoTo Routine_Error

        fd = FreeFile()
        FileOpen(fd, FileName, OpenMode.Binary)

        lngFileSize = UBound(WavArray)
        For i = 0 To lngFileSize
            FilePut(fd, WavArray(i))
        Next

Routine_Exit:
        On Error Resume Next
        FileClose(fd)
        Exit Function

Routine_Error:
        Wav_WriteToFile = Err.Number
        Resume Routine_Exit
    End Function

    '-----------------------------------------------------------------------------------
    ' Wav_SineWave - Builds a sine wave that may be played in a continuous loop.
    '-----------------------------------------------------------------------------------
    Public Sub Wav_SineWave(ByRef WavArray() As Byte, Frequency As Double)
        Dim i As Long
        Dim lngLimit As Long
        Dim lngDataL As Long, lngDataR As Long
        Dim dblDataPt As Double, blnPositive As Boolean
        Dim intCycles As Integer, intCycleCount As Integer
        Dim lngFileSize As Long
        Dim lngSamples As Long
        Dim lngDataSize As Long

        Dim dblDataSlice As Double
        Dim dblWaveTime As Double
        Dim dblTotalTime As Double
        Dim dblSampleTime As Double

        dblFrequency = Frequency

        If dblFrequency > 1000 Then
            intCycles = 100
        Else
            intCycles = 10
        End If

        dblWaveTime = 1 / dblFrequency
        dblTotalTime = dblWaveTime * intCycles
        dblSampleTime = 1 / CDbl(lngSampleRate)
        dblDataSlice = (2 * PI) / (dblWaveTime / dblSampleTime)

        lngSamples = 0
        intCycleCount = 0
        blnPositive = True
        Do
            dblDataPt = Math.Sin(lngSamples * dblDataSlice)
            If lngSamples > 0 Then
                If dblDataPt < 0 Then
                    blnPositive = False
                Else
                    ' Detect Zero Crossing
                    If Not blnPositive Then
                        intCycleCount += 1
                        If intCycleCount >= intCycles Then Exit Do
                        blnPositive = True
                    End If
                End If
            End If
            lngSamples += 1
        Loop

        '-------------------------------------------------------------------------------
        ' Bytes 40 - 43  Length of Data   Samples * Channels * Bits per Sample / 8
        '-------------------------------------------------------------------------------
        lngDataSize = lngSamples * intAudioWidth * (intBits / 8)
        ReDim Preserve WavArray(0 To 43 + lngDataSize)

        WavArray(40) = ExtractByte(lngDataSize, 0)
        WavArray(41) = ExtractByte(lngDataSize, 1)
        WavArray(42) = ExtractByte(lngDataSize, 2)
        WavArray(43) = ExtractByte(lngDataSize, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 04 - 07  Total Length to Follow  (Length of File - 8)
        '-------------------------------------------------------------------------------
        lngFileSize = lngDataSize + 36

        WavArray(4) = ExtractByte(lngFileSize, 0)
        WavArray(5) = ExtractByte(lngFileSize, 1)
        WavArray(6) = ExtractByte(lngFileSize, 2)
        WavArray(7) = ExtractByte(lngFileSize, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 44 - End   Data Samples
        '-------------------------------------------------------------------------------

        If intBits = 8 Then
            lngLimit = 127
        Else
            lngLimit = 32767
        End If

        For i = 0 To lngSamples - 1

            If intBits = 8 Then
                '-----------------------------------------------------------------------
                ' 8 Bit Data
                '-----------------------------------------------------------------------
                ' Calculate data point.
                dblDataPt = Math.Sin(i * dblDataSlice) * lngLimit
                lngDataL = Int(dblDataPt * dblVolumeL) + lngLimit
                lngDataR = Int(dblDataPt * dblVolumeR) + lngLimit

                ' Place data point in wave tile.
                If intAudioMode = MODE_MONO Then _
                WavArray(i + 44) = ExtractByte(lngDataL, 0)

                If intAudioMode = MODE_LR Then       'L+R stereo
                    WavArray((2 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + 45) = ExtractByte(lngDataR, 0)
                End If

                If intAudioMode = MODE_L Then       ' L only stereo
                    WavArray((2 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + 45) = 0
                End If

                If intAudioMode = MODE_R Then       ' R only stereo
                    WavArray((2 * i) + 44) = 0
                    WavArray((2 * i) + 45) = ExtractByte(lngDataR, 0)
                End If

            Else

                '-----------------------------------------------------------------------
                ' 16 Bit Data
                '-----------------------------------------------------------------------
                ' Calculate data point.
                dblDataPt = Math.Sin(i * dblDataSlice) * lngLimit
                lngDataL = Int(dblDataPt * dblVolumeL)
                lngDataR = Int(dblDataPt * dblVolumeR)

                ' Place data point in wave tile.
                If intAudioMode = MODE_MONO Then
                    WavArray((2 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + 45) = ExtractByte(lngDataL, 1)
                End If

                If intAudioMode = MODE_LR Then
                    WavArray((4 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((4 * i) + 45) = ExtractByte(lngDataL, 1)
                    WavArray((4 * i) + 46) = ExtractByte(lngDataR, 0)
                    WavArray((4 * i) + 47) = ExtractByte(lngDataR, 1)
                End If

                If intAudioMode = MODE_L Then
                    WavArray((4 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((4 * i) + 45) = ExtractByte(lngDataL, 1)
                    WavArray((4 * i) + 46) = 0
                    WavArray((4 * i) + 47) = 0
                End If

                If intAudioMode = MODE_R Then
                    WavArray((4 * i) + 44) = 0
                    WavArray((4 * i) + 45) = 0
                    WavArray((4 * i) + 46) = ExtractByte(lngDataR, 0)
                    WavArray((4 * i) + 47) = ExtractByte(lngDataR, 1)
                End If

            End If

        Next
    End Sub

    '-----------------------------------------------------------------------------------
    ' Wav_MultiSineWave - Builds a complex wave form from one or more sine waves.
    '-----------------------------------------------------------------------------------
    Public Sub Wav_MultiSineWave(ByRef WavArray() As Byte, SineWaves() As SINEWAVE,
    Seconds As Double)

        Dim i As Long, j As Long
        Dim lngLimit As Long
        Dim lngDataL As Long, lngDataR As Long
        Dim dblDataPtL As Double, dblDataPtR As Double
        Dim dblWaveTime As Double
        Dim dblSampleTime As Double
        Dim lngSamples As Long
        Dim lngFileSize As Long, lngDataSize As Long

        Dim intSineCount As Integer

        intSineCount = UBound(SineWaves)

        For i = 0 To intSineCount
            dblWaveTime = 1 / SineWaves(i).dblFrequency
            dblSampleTime = 1 / CDbl(lngSampleRate)
            SineWaves(i).dblDataSlice = (2 * PI) / (dblWaveTime / dblSampleTime)
        Next

        lngSamples = CLng(Seconds / dblSampleTime)

        '-------------------------------------------------------------------------------
        ' Bytes 40 - 43  Length of Data   Samples * Channels * Bits per Sample / 8
        '-------------------------------------------------------------------------------
        lngDataSize = lngSamples * intAudioWidth * (intBits / 8)
        ReDim Preserve WavArray(0 To 43 + lngDataSize)

        WavArray(40) = ExtractByte(lngDataSize, 0)
        WavArray(41) = ExtractByte(lngDataSize, 1)
        WavArray(42) = ExtractByte(lngDataSize, 2)
        WavArray(43) = ExtractByte(lngDataSize, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 04 - 07  Total Length to Follow  (Length of File - 8)
        '-------------------------------------------------------------------------------
        lngFileSize = lngDataSize + 36

        WavArray(4) = ExtractByte(lngFileSize, 0)
        WavArray(5) = ExtractByte(lngFileSize, 1)
        WavArray(6) = ExtractByte(lngFileSize, 2)
        WavArray(7) = ExtractByte(lngFileSize, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 44 - End   Data Samples
        '-------------------------------------------------------------------------------

        If intBits = 8 Then
            lngLimit = 127
        Else
            lngLimit = 32767
        End If

        For i = 0 To lngSamples - 1

            If intBits = 8 Then
                '-----------------------------------------------------------------------
                ' 8 Bit Data
                '-----------------------------------------------------------------------
                dblDataPtL = 0
                dblDataPtR = 0
                For j = 0 To intSineCount
                    dblDataPtL += (Math.Sin(i * SineWaves(j).dblDataSlice) *
                    SineWaves(j).dblAmplitudeL)
                    dblDataPtR += (Math.Sin(i * SineWaves(j).dblDataSlice) *
                    SineWaves(j).dblAmplitudeR)
                Next

                lngDataL = Int(dblDataPtL * dblVolumeL * lngLimit) + lngLimit
                lngDataR = Int(dblDataPtL * dblVolumeR * lngLimit) + lngLimit

                If intAudioMode = MODE_MONO Then _
                WavArray(i + 44) = ExtractByte(lngDataL, 0)

                If intAudioMode = MODE_LR Then       'L+R stereo
                    WavArray((2 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + 45) = ExtractByte(lngDataR, 0)
                End If

                If intAudioMode = MODE_L Then       ' L only stereo
                    WavArray((2 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + 45) = 0
                End If

                If intAudioMode = MODE_R Then       ' R only stereo
                    WavArray((2 * i) + 44) = 0
                    WavArray((2 * i) + 45) = ExtractByte(lngDataR, 0)
                End If

            Else

                '-----------------------------------------------------------------------
                ' 16 Bit Data
                '-----------------------------------------------------------------------
                dblDataPtL = 0
                dblDataPtR = 0
                For j = 0 To intSineCount
                    dblDataPtL += (Math.Sin(i * SineWaves(j).dblDataSlice) *
                    SineWaves(j).dblAmplitudeL)
                    dblDataPtR += (Math.Sin(i * SineWaves(j).dblDataSlice) *
                    SineWaves(j).dblAmplitudeR)
                Next

                lngDataL = Int(dblDataPtL * dblVolumeL * lngLimit)
                lngDataR = Int(dblDataPtL * dblVolumeR * lngLimit)

                If intAudioMode = MODE_MONO Then
                    WavArray((2 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + 45) = ExtractByte(lngDataL, 1)
                End If

                If intAudioMode = MODE_LR Then
                    WavArray((4 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((4 * i) + 45) = ExtractByte(lngDataL, 1)
                    WavArray((4 * i) + 46) = ExtractByte(lngDataR, 0)
                    WavArray((4 * i) + 47) = ExtractByte(lngDataR, 1)
                End If

                If intAudioMode = MODE_L Then
                    WavArray((4 * i) + 44) = ExtractByte(lngDataL, 0)
                    WavArray((4 * i) + 45) = ExtractByte(lngDataL, 1)
                    WavArray((4 * i) + 46) = 0
                    WavArray((4 * i) + 47) = 0
                End If

                If intAudioMode = MODE_R Then
                    WavArray((4 * i) + 44) = 0
                    WavArray((4 * i) + 45) = 0
                    WavArray((4 * i) + 46) = ExtractByte(lngDataR, 0)
                    WavArray((4 * i) + 47) = ExtractByte(lngDataR, 1)
                End If

            End If

        Next
    End Sub

    '-------------------------------------------------------------------------------------
    ' ExtractByte - Extracts the high or low byte from a short (16 bit) VB integer.
    '
    '   intWord     - VB Integer from which to extract byte.
    '   intByte     - Returned high or low byte.
    '   intPosition - |                    Word                   |
    '                 | Byte = 3 | Byte = 2 | Byte = 1 | Byte = 0 |
    '-------------------------------------------------------------------------------------
    Private Function ExtractByte(lngWord As Long, intPosition As Integer) As Byte
        Dim lngTemp As Long
        Dim intByte As Integer

        If intPosition = 3 Then
            ' Byte 2
            lngTemp = lngWord

            ' Mask off byte and shift right 24 bits.
            '   Mask  -> 2130706432 = &H7F000000
            '   Shift -> Divide by 16777216
            lngTemp = (lngTemp And 2130706432) / 16777216

            ' Cast back to integer.
            intByte = lngTemp

        ElseIf intPosition = 2 Then
            ' Byte 2
            lngTemp = lngWord

            ' Mask off byte and shift right 16 bits.
            '   Mask  -> 16711680 = &HFF0000
            '   Shift -> Divide by 65536
            lngTemp = (lngTemp And 16711680) / 65536

            ' Cast back to integer.
            intByte = lngTemp

        ElseIf intPosition = 1 Then
            ' Byte 1
            lngTemp = lngWord

            ' Mask off high byte and shift right 8 bits.
            '   Mask  -> 65290 = &HFF00
            '   Shift -> Divide by 256
            lngTemp = (lngTemp And 65290) / 256

            ' Cast back to integer.
            intByte = lngTemp
        Else
            ' Byte 0
            intByte = lngWord And &HFF
        End If

        ExtractByte = intByte
    End Function

    '-----------------------------------------------------------------------------------
    ' Wav_Stop - Stop the currently playing wav.
    '-----------------------------------------------------------------------------------
    Public Sub Wav_Stop()
        Dim lngStatus As Long

        lngStatus = PlaySoundMemory(0&, 0&, SND_MEMORY Or SND_PURGE Or SND_NODEFAULT)

    End Sub

    '-----------------------------------------------------------------------------------
    ' Create and Play all 16 DTMF Tones
    '-----------------------------------------------------------------------------------
    Public Sub CmdDTMF_Click(ByRef intAudioMode As Integer, ByRef lngSampleRate As Long, ByRef intBits As Integer)
        Dim i As Integer
        'Dim intAudioMode As Integer
        'Dim lngSampleRate As Long
        'Dim intBits As Integer
        'Dim intIndex As Integer
        Dim udtSineWaves(0 To 1) As SINEWAVE

        Dim udtDTMF(16) As DTMF

        'intAudioMode = cboAudioMode.ItemData(cboAudioMode.ListIndex)
        'lngSampleRate = cboSampleRate.ItemData(cboSampleRate.ListIndex)
        'intBits = cboBits.ItemData(cboBits.ListIndex)

        Wav_Stop()

        '---------------------------------------------------
        '              DTMF Tones
        ' Freq  1209  1336  1477  1633
        '  697    1     2     3     A
        '  770    4     5     6     B
        '  852    7     8     9     C
        '  941    *     0     #     D
        '---------------------------------------------------
        For i = 1 To 16
            With udtSineWaves(0)
                .dblAmplitudeL = 0.25
                .dblAmplitudeR = 0.25
            End With

            With udtSineWaves(1)
                .dblAmplitudeL = 0.25
                .dblAmplitudeR = 0.25
            End With

            udtSineWaves(0).dblFrequency = Choose(i,
            697, 697, 697, 697,
            770, 770, 770, 770,
            852, 852, 852, 852,
            941, 941, 941, 941)

            udtSineWaves(1).dblFrequency = Choose(i,
            1209, 1336, 1477, 1633,
            1209, 1336, 1477, 1633,
            1209, 1336, 1477, 1633,
            1209, 1336, 1477, 1633)

            Wav_BuildHeader(udtDTMF(i).bytTones, lngSampleRate, intBits, intAudioMode, 0.5, 0.5)
            Wav_MultiSineWave(udtDTMF(i).bytTones, udtSineWaves, 0.25)
        Next

        For i = 1 To 16
            Wav_Play(udtDTMF(i).bytTones) 'play tone
            'Wav_WriteToFile("C:\Looks\Test" & i & ".wav", udtDTMF(i).bytTones) 'save tone file
        Next

    End Sub

    '-----------------------------------------------------------------------------------
    ' Create and Play Sinewave based on current settings.
    '-----------------------------------------------------------------------------------
    Public Sub CmdPlay_Click(ByRef dblFrequency As Double, ByRef intAudioMode As Integer, ByRef lngSampleRate As Long, ByRef intBits As Integer)
        'Dim dblFrequency As Double
        'Dim intAudioMode As Integer
        'Dim lngSampleRate As Long
        'Dim intBits As Integer
        'Dim bytSound() As Byte

        'dblFrequency = Val(txtFrequency)
        'intAudioMode = cboAudioMode.ItemData(cboAudioMode.ListIndex)
        'lngSampleRate = cboSampleRate.ItemData(cboSampleRate.ListIndex)
        'intBits = cboBits.ItemData(cboBits.ListIndex)

        Wav_Stop()
        Wav_BuildHeader(bytSound, lngSampleRate, intBits, intAudioMode, 0.5, 0.5)
        Wav_SineWave(bytSound, dblFrequency)

        'Wav_WriteToFile("C:\Looks\Test.wav", bytSound)
        Wav_PlayLoop(bytSound)

    End Sub

    '-----------------------------------------------------------------------------------
    ' Create and Play Wave using frequencies of a train horn.
    '-----------------------------------------------------------------------------------
    Public Sub CmdTrain_Click(ByRef intAudioMode As Integer, ByRef lngSampleRate As Long, ByRef intBits As Integer)
        'Dim i As Integer
        'Dim intAudioMode As Integer
        'Dim lngSampleRate As Long
        'Dim intBits As Integer
        'Dim intIndex As Integer
        Dim udtSineWaves(0 To 2) As SINEWAVE
        'Dim bytSound() As Byte

        'intAudioMode = cboAudioMode.ItemData(cboAudioMode.ListIndex)
        'lngSampleRate = cboSampleRate.ItemData(cboSampleRate.ListIndex)
        'intBits = cboBits.ItemData(cboBits.ListIndex)

        Wav_Stop()

        With udtSineWaves(0)
            .dblFrequency = 255
            .dblAmplitudeL = 0.25
            .dblAmplitudeR = 0.25
        End With

        With udtSineWaves(1)
            .dblFrequency = 311
            .dblAmplitudeL = 0.25
            .dblAmplitudeR = 0.25
        End With

        With udtSineWaves(2)
            .dblFrequency = 440
            .dblAmplitudeL = 0.25
            .dblAmplitudeR = 0.25
        End With

        Wav_BuildHeader(bytSound, lngSampleRate, intBits, intAudioMode, 0.5, 0.5)
        Wav_MultiSineWave(bytSound, udtSineWaves, 2) '2 seconds

        Wav_Play(bytSound)

    End Sub

    ' Create and play once NEC style On-off_keying (OOK) 38 kHz data stream.
    Public Sub NECPlay(ByRef sTX As String, ByRef intAudioMode As Integer, ByRef lngSampleRate As Long, ByRef intBits As Integer)

        'build wav
        Wav_NEC(bytSound, sTX, intAudioMode, lngSampleRate, intBits)
        'play once
        Wav_Play(bytSound)

    End Sub

    ' Create and play repeating NEC style On-off_keying (OOK) 38 kHz data stream (add silence method).
    Public Sub NECPlayLoop(ByRef sTX As String, ByRef intAudioMode As Integer, ByRef lngSampleRate As Long, ByRef intBits As Integer, Optional ByRef dblFillSilence As Double = 5)

        'build wav
        Wav_NEC(bytSound, sTX, intAudioMode, lngSampleRate, intBits)
        'add silence
        Wav_NECBit(bytSound, , dblFillSilence) 'add silence
        'continuous play
        Wav_PlayLoop(bytSound) 'continuous play  

    End Sub

    ' Create and save as wav NEC style On-off_keying (OOK) 38 kHz data stream.
    Public Sub NECSave(ByRef sTX As String, ByRef intAudioMode As Integer, ByRef lngSampleRate As Long, ByRef intBits As Integer)
        Dim appPath As String

        'build wav
        Wav_NEC(bytSound, sTX, intAudioMode, lngSampleRate, intBits)

        'get file path
        If SaveFilePath <> "" Then
            appPath = SaveFilePath
        Else
            appPath = Application.StartupPath 'My.Application.Info.DirectoryPath doesn't work in 'ClickOnce'
        End If
        'save wav
        Wav_WriteToFile(appPath & SaveFileName, bytSound)
        MessageBox.Show("Accoustic file saved to:" & vbCrLf & vbCrLf & appPath & SaveFileName)

    End Sub

    'Convert transmission settings into 'binary' string format
    '00 - 01 Preamble 2 bits "00"
    '02 - 05 Sync1 "1000"
    '06 - 09 Sync2 "0001"
    '10 - 13 ID6 4 bits "1011"  1
    '14 - 17 ID5 4 bits "1010"  0
    '18 - 21 ID4 4 bits "1101"  4
    '22 - 25 ID3 4 bits "0011"  3
    '26 - 29 ID2 4 bits "1010"  0
    '30 - 33 ID1 4 bits "1110"  8
    '34 - 45 Pressure (PSI/2) 12 bits unsigned integer	0 to 5000 PSI
    '46 - 49 Battery 4 bits  0000 Good / 0010 Low / 0001 Critical
    '50 - 57 Checksum 8 bits (unsigned sum of previous 12 nibbles)
    Public Function OceanicEncode(ByRef sID As String, ByRef dblPressure As Int32, ByRef sBattery As String) As String
        Dim i As Integer

        'Begin with preamble
        OceanicEncode = NEC_Preamble
        'Add ID data
        For i = 0 To 5
            OceanicEncode += IDEncode(sID(i))
        Next
        'Add pressure psi \ 2
        dblPressure \= 2
        OceanicEncode += Right("000000000000" & Convert.ToString(dblPressure , 2), 12)
        'Add battery value
        OceanicEncode += BattEncode(sBattery)
        'Add checksum
        OceanicEncode += Checksum(OceanicEncode)

    End Function

    'Return TX code for an ID digit
    Private Function IDEncode(ByVal sDigit As String) As String

        IDEncode = ID.Code(sDigit)

    End Function

    'Return Battery code for a given status
    Private Function BattEncode(ByVal sBatt As String) As String

        BattEncode = Battery.Code(sBatt)

    End Function

    'Return the 8-bit checksum
    Private Function Checksum(ByRef sData As String) As String
        Dim i As Int32
        Dim iSum As Int32
        Dim sNibble As String

        If Len(sData) <> 50 Then
            MessageBox.Show("Checksum calculation error" & vbCrLf & "Data string only " & Len(sData) & "digits")
            Checksum = "00000000"
            Exit Function
        End If

        For i = 0 To 11
            sNibble = Mid(sData, 3 + (i * 4), 4)
            iSum += Convert.ToInt32(sNibble, 2)
        Next
        Checksum = Right("00000000" & Convert.ToString(iSum, 2), 8)
    End Function

    ' Convert binary string to an NEC style accoustic sequence in an array of byte
    Private Sub Wav_NEC(ByRef WavArray() As Byte, ByRef sTX As String, ByRef intAudioMode As Integer, ByRef lngSampleRate As Long, ByRef intBits As Integer)
        Dim i As Integer

        'begin array
        Wav_BuildHeader(WavArray, lngSampleRate, intBits, intAudioMode, NEC_Volume, NEC_Volume)
        'add lead in silence
        Wav_NECBit(WavArray, , NEC_PreGap)
        'add data bits
        For i = 0 To Len(sTX) - 1
            Wav_NECBit(WavArray, sTX(i) = "1")
        Next
        'add stop bit
        Wav_NECBit(WavArray, False)

    End Sub

    ' Add NEC style accoustic sequence for data bit to end of byte array
    Private Sub Wav_NECBit(ByRef WavArray() As Byte, Optional ByRef blnDataBit As Boolean = True, Optional ByRef dblFillSilence As Double = 0)
        Dim i As Long
        Dim lngLimit As Long
        Dim lngDataL As Long, lngDataR As Long
        Dim dblDataPt As Double
        Dim lngFileSize As Long
        Dim lngSamples As Long
        Dim lngDataSize As Long

        Dim dblWaveTime As Double
        Dim dblTotalTime As Double
        Dim dblBurstTime As Double
        Dim dblSampleTime As Double

        Dim intWavArraySize As Integer
        Dim intWavDataSize As Integer

        dblFrequency = NEC_Carrier '38Khz carrier
        dblWaveTime = 1 / dblFrequency 'time for 1 carrier cycle

        Select Case blnDataBit
            Case True
                dblTotalTime = NEC_1_TX_time 'time to transmit 1
                dblBurstTime = NEC_1_Burst 'burst time for 1
            Case False
                dblTotalTime = NEC_0_TX_time 'time to transmit 0
                dblBurstTime = NEC_0_Burst 'burst time for 0
        End Select

        '-------------------------------------------------------------------------------
        ' Bytes 40 - 43  Length of Data   Samples * Channels * Bits per Sample / 8
        '-------------------------------------------------------------------------------
        intWavArraySize = UBound(WavArray) 'current array size (0 to X)
        intWavDataSize = intWavArraySize - 43 'current data size (0 to X)
        dblSampleTime = 1 / CDbl(lngSampleRate) 'sample time

        If dblFillSilence > 0 Then 'add silence to equal total play time specified
            lngSamples = (dblFillSilence / dblSampleTime) - (intWavDataSize / intAudioWidth / (intBits / 8)) 'number of samples to fill gap
            dblBurstTime = -1 'no burst
        Else
            lngSamples = dblTotalTime / dblSampleTime 'number of samples in total
        End If
        lngDataSize = lngSamples * intAudioWidth * (intBits / 8) 'additional data size

        ReDim Preserve WavArray(0 To intWavArraySize + lngDataSize) '43

        WavArray(40) = ExtractByte(intWavDataSize + lngDataSize, 0)
        WavArray(41) = ExtractByte(intWavDataSize + lngDataSize, 1)
        WavArray(42) = ExtractByte(intWavDataSize + lngDataSize, 2)
        WavArray(43) = ExtractByte(intWavDataSize + lngDataSize, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 04 - 07  Total Length to Follow  (Length of File - 8)
        '-------------------------------------------------------------------------------
        lngFileSize = intWavDataSize + lngDataSize + 36

        WavArray(4) = ExtractByte(lngFileSize, 0)
        WavArray(5) = ExtractByte(lngFileSize, 1)
        WavArray(6) = ExtractByte(lngFileSize, 2)
        WavArray(7) = ExtractByte(lngFileSize, 3)

        '-------------------------------------------------------------------------------
        ' Bytes 44 - End   Data Samples
        '-------------------------------------------------------------------------------

        If intBits = 8 Then
            lngLimit = 127
        Else
            lngLimit = 32767
        End If

        For i = 0 To lngSamples - 1

            ' Calculate data point.
            If (i * dblSampleTime <= dblBurstTime) Then
                If (i * dblSampleTime) Mod dblWaveTime < (dblWaveTime / 2) Then
                    dblDataPt = lngLimit ' burst peak
                Else
                    dblDataPt = -lngLimit ' burst trough / silence
                End If
            Else
                dblDataPt = 0 'space
            End If

            If intBits = 8 Then
                '-----------------------------------------------------------------------
                ' 8 Bit Data
                '-----------------------------------------------------------------------
                ' Scale data point.
                lngDataL = Int(dblDataPt * dblVolumeL) + lngLimit
                lngDataR = Int(dblDataPt * dblVolumeR) + lngLimit

                ' Place data point in wave tile.
                If intAudioMode = MODE_MONO Then _
                WavArray(i + intWavDataSize + 44) = ExtractByte(lngDataL, 0)

                If intAudioMode = MODE_LR Then       'L+R stereo
                    WavArray((2 * i) + intWavDataSize + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + intWavDataSize + 45) = ExtractByte(lngDataR, 0)
                End If

                If intAudioMode = MODE_L Then       ' L only stereo
                    WavArray((2 * i) + intWavDataSize + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + intWavDataSize + 45) = 0
                End If

                If intAudioMode = MODE_R Then       ' R only stereo
                    WavArray((2 * i) + intWavDataSize + 44) = 0
                    WavArray((2 * i) + intWavDataSize + 45) = ExtractByte(lngDataR, 0)
                End If

            Else

                '-----------------------------------------------------------------------
                ' 16 Bit Data
                '-----------------------------------------------------------------------
                ' Scale data point.
                lngDataL = Int(dblDataPt * dblVolumeL)
                lngDataR = Int(dblDataPt * dblVolumeR)

                ' Place data point in wave tile.
                If intAudioMode = MODE_MONO Then
                    WavArray((2 * i) + intWavDataSize + 44) = ExtractByte(lngDataL, 0)
                    WavArray((2 * i) + intWavDataSize + 45) = ExtractByte(lngDataL, 1)
                End If

                If intAudioMode = MODE_LR Then
                    WavArray((4 * i) + intWavDataSize + 44) = ExtractByte(lngDataL, 0)
                    WavArray((4 * i) + intWavDataSize + 45) = ExtractByte(lngDataL, 1)
                    WavArray((4 * i) + intWavDataSize + 46) = ExtractByte(lngDataR, 0)
                    WavArray((4 * i) + intWavDataSize + 47) = ExtractByte(lngDataR, 1)
                End If

                If intAudioMode = MODE_L Then
                    WavArray((4 * i) + intWavDataSize + 44) = ExtractByte(lngDataL, 0)
                    WavArray((4 * i) + intWavDataSize + 45) = ExtractByte(lngDataL, 1)
                    WavArray((4 * i) + intWavDataSize + 46) = 0
                    WavArray((4 * i) + intWavDataSize + 47) = 0
                End If

                If intAudioMode = MODE_R Then
                    WavArray((4 * i) + intWavDataSize + 44) = 0
                    WavArray((4 * i) + intWavDataSize + 45) = 0
                    WavArray((4 * i) + intWavDataSize + 46) = ExtractByte(lngDataR, 0)
                    WavArray((4 * i) + intWavDataSize + 47) = ExtractByte(lngDataR, 1)
                End If

            End If

        Next
    End Sub

End Module
