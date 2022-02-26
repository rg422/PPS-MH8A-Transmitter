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


'Typical Transmitter Specifications
' Rated Air Pressure 300 bar / 4350 psi
' Pressure resolution 0.14 bar / 2 psi
' Pressure reporting intervals:
' Swift   4.8 – 5.2 seconds
' Yellow  5.2 seconds
' Grey    5.0 seconds
' Green   4.8 seconds
'-----------------------------------------------------------------------------------
Imports System.ComponentModel

Public Class Form1

    Private Tick As Integer 'timer tick counter
    Private Ticks As Integer 'ticks for transmission interval
    Private ReadOnly rnd As New Random

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim toolTip1 As New ToolTip()
        toolTip1.SetToolTip(LabelTXCode, "DoubleClick to copy to clipboard")

        TextBoxID.Text = DefaultID
        NumericUpDownPVal.Minimum = 0
        NumericUpDownPVal.Maximum = MaxPSI
        NumericUpDownPVal.Increment = PSIIncrement
        NumericUpDownPVal.Value = DefaultPSI

        Dim i As Integer
        For i = 0 To Battery.NumStates
            ComboBoxBVal.Items.Add(Battery.StateName(i))
        Next
        ComboBoxBVal.SelectedIndex = Battery.DefaultState 'good
        For i = 0 To Tx.NumModels
            ComboBoxTXType.Items.Add(Tx.ModelName(i))
        Next
        ComboBoxTXType.SelectedIndex = Tx.DefaultModel 'yellow
        PictureBox1.Image = Tx.Image(ComboBoxTXType.Text)
        LabelTXInterval.Text = "TX Code (" & Tx.IntervalText(ComboBoxTXType.Text) & ") :"

        LabelTXCode.Text = OceanicEncode(TextBoxID.Text, Convert.ToInt32(NumericUpDownPVal.Value), ComboBoxBVal.Text)
    End Sub

    'Calculate data stream and save to wav file
    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click

        'Check the data input is valid and calculate the tx code
        If Not ValidateTextBoxes() = True Then Exit Sub

        'Save the TX code
        Call NECSave(LabelTXCode.Text, MODE_MONO, RATE_384000, BITS_16)

    End Sub

    'Calculate data stream play wav file
    Private Sub ButtonTX_Click(sender As Object, e As EventArgs) Handles ButtonTX.Click

        If ButtonTX.Text = "Stop Sending" Then
            Timer1.Stop() 'Stop the repetitive output and restore the default buttons
            ButtonTX.Text = "Transmit Once"
            ButtonTXCont.Text = "Transmit Continuous"
            Wav_Stop()
            Exit Sub
        End If

        'Check the data input is valid and calculate the tx code
        If Not ValidateTextBoxes() = True Then Exit Sub

        'Transmit the TX code
        Call NECPlay(LabelTXCode.Text, MODE_MONO, RATE_384000, BITS_16)

    End Sub

    'Calculate data stream play wav file every ~5 seconds
    Private Sub ButtonTXCont_Click(sender As Object, e As EventArgs) Handles ButtonTXCont.Click

        If ButtonTXCont.Text = "Update Message" Then
            ValidateTextBoxes()
            Exit Sub
        End If

        'Check the data input is valid and calculate the tx code
        If Not ValidateTextBoxes() = True Then Exit Sub

        ButtonTX.Text = "Stop Sending"
        ButtonTXCont.Text = "Update Message"
        '' call this method to start your asynchronous Task.
        Tick = 0
        Timer1.Interval = 50 '0.05s, 100 ticks = 5 seconds
        Timer1.Enabled = True
        Timer1.Start() 'Start transmitting

    End Sub

    'Validate the form data, convert to the binary data string and display on the form
    Private Function ValidateTextBoxes() As Boolean
        Dim sID As String
        Dim sBV As String
        Dim sTX As String
        Dim iPVal As Int32

        ValidateTextBoxes = False

        sID = TextBoxID.Text
        sBV = ComboBoxBVal.Text

        If Len(sID) <> 6 Or Not IsNumeric(sID) Then
            MessageBox.Show("Transmitter ID error:" & vbCrLf & "Please enter 6 digits")
            Exit Function
        End If

        iPVal = Convert.ToInt32(NumericUpDownPVal.Value)

        If iPVal < 0 Or iPVal > MaxPSI Then
            MessageBox.Show("Pressure value error:" & vbCrLf & "Please enter a number between 0 and " & MaxPSI)
            Exit Function
        End If

        If (iPVal Mod 2) > 0 Then
            MessageBox.Show("Pressure value error:" & vbCrLf & "Please enter an even number!")
            Exit Function
        End If

        If ComboBoxBVal.SelectedIndex < 0 And (Len(sBV) <> 4 Or Not IsNumeric(sBV)) Then
            MessageBox.Show("Battery value error:" & vbCrLf & "Please select from drop down or enter a 4 digit binary number")
            Exit Function
        End If

        sTX = OceanicEncode(sID, iPVal, sBV)
        LabelTXCode.Text = sTX

        ValidateTextBoxes = True

    End Function

    Private Sub TextBoxID_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxID.KeyPress
        Select Case True
            Case Char.IsDigit(e.KeyChar)
            Case e.KeyChar = Chr(Keys.Back)
            Case Else
                e.Handled = True
        End Select
    End Sub

    Private Sub ComboBoxBVal_KeyPress(sender As Object, e As KeyPressEventArgs) Handles ComboBoxBVal.KeyPress
        Select Case e.KeyChar
            Case "0"
            Case "1"
            Case Chr(Keys.Back)
            Case Else
                e.Handled = True
        End Select
    End Sub

    Private Sub LabelTXCode_DoubleClick(sender As Object, e As EventArgs) Handles LabelTXCode.DoubleClick
        Clipboard.SetText(LabelTXCode.Text)
        Beep()
    End Sub

    Private Sub TextBoxID_TextChanged(sender As Object, e As EventArgs) Handles TextBoxID.TextChanged
        LabelTXCode.Text = "..."
    End Sub

    Private Sub NumericUpDownPVal_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDownPVal.ValueChanged
        LabelTXCode.Text = "..."
    End Sub

    Private Sub ComboBoxBVal_TextChanged(sender As Object, e As EventArgs) Handles ComboBoxBVal.TextChanged
        LabelTXCode.Text = "..."
    End Sub

    Private Sub ComboBoxTXType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxTXType.SelectedIndexChanged
        PictureBox1.Image = Tx.Image(ComboBoxTXType.Text)
        LabelTXInterval.Text = "TX Code (" & Tx.IntervalText(ComboBoxTXType.Text) & ") :"
    End Sub

    'return an interval (including random variation if the model supports it)
    Private Function TXRepeat(Model As String) As Single
        Dim Interval As Single = Tx.Interval(Model)
        Dim Variation As Single = Tx.Variation(Model)
        Dim Steps As Integer = Tx.Steps()
        TXRepeat = Interval + Variation * (-1 + 2 * rnd.Next(0, Steps + 1) / Steps)
    End Function

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        TXGreen.Dispose()
        TXGrey.Dispose()
        TXSwift.Dispose()
        TXYellow.Dispose()
        TXNull.Dispose()
    End Sub

    'timer to send repeat transmissions
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Tick += 1

        If Tick = 1 Then
            'Transmit the TX code
            If Len(LabelTXCode.Text) = 58 Then Call NECPlay(LabelTXCode.Text, MODE_MONO, RATE_384000, BITS_16)
            Ticks = TXRepeat(ComboBoxTXType.Text) * 1000 / Timer1.Interval
        End If

        If Tick >= Ticks Then
            Tick = 0
            Exit Sub
        End If
    End Sub



    '-----------------------------------------------------------------------------------
    ' How to call the original "Tones Test" functions
    '-----------------------------------------------------------------------------------

    'MODE_MONO, MODE_LR, MODE_L, MODE_R
    'RATE_8000, RATE_11025, RATE_22050, RATE_32000, RATE_44100, RATE_48000, RATE_88000, RATE_96000
    'BITS_8, BITS_16

    'Call CmdTrain_Click(MODE_MONO, RATE_44100, BITS_8)
    'Call CmdPlay_Click(500, MODE_LR, RATE_44100, BITS_16)
    'Call CmdDTMF_Click(MODE_MONO, RATE_44100, BITS_8)
    '-----------------------------------------------------------------------------------

End Class
