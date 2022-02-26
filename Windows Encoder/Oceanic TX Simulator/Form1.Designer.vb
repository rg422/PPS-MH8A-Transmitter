<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxID = New System.Windows.Forms.TextBox()
        Me.ButtonSave = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LabelTXInterval = New System.Windows.Forms.Label()
        Me.LabelTXCode = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ButtonTX = New System.Windows.Forms.Button()
        Me.ComboBoxBVal = New System.Windows.Forms.ComboBox()
        Me.ButtonTXCont = New System.Windows.Forms.Button()
        Me.ComboBoxTXType = New System.Windows.Forms.ComboBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.NumericUpDownPVal = New System.Windows.Forms.NumericUpDown()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDownPVal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(36, 44)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(122, 25)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Transmitter ID"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(36, 96)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(118, 25)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Pressure (PSI)"
        '
        'TextBoxID
        '
        Me.TextBoxID.Location = New System.Drawing.Point(184, 44)
        Me.TextBoxID.Name = "TextBoxID"
        Me.TextBoxID.Size = New System.Drawing.Size(150, 31)
        Me.TextBoxID.TabIndex = 1
        '
        'ButtonSave
        '
        Me.ButtonSave.Location = New System.Drawing.Point(36, 305)
        Me.ButtonSave.Name = "ButtonSave"
        Me.ButtonSave.Size = New System.Drawing.Size(190, 49)
        Me.ButtonSave.TabIndex = 5
        Me.ButtonSave.Text = "Save Code to WAV"
        Me.ButtonSave.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(36, 153)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(120, 25)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Battery Status"
        '
        'LabelTXInterval
        '
        Me.LabelTXInterval.AutoSize = True
        Me.LabelTXInterval.Location = New System.Drawing.Point(36, 235)
        Me.LabelTXInterval.Name = "LabelTXInterval"
        Me.LabelTXInterval.Size = New System.Drawing.Size(83, 25)
        Me.LabelTXInterval.TabIndex = 8
        Me.LabelTXInterval.Text = "TX Code:"
        '
        'LabelTXCode
        '
        Me.LabelTXCode.AutoSize = True
        Me.LabelTXCode.Font = New System.Drawing.Font("Lucida Console", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.LabelTXCode.Location = New System.Drawing.Point(49, 260)
        Me.LabelTXCode.Name = "LabelTXCode"
        Me.LabelTXCode.Size = New System.Drawing.Size(37, 16)
        Me.LabelTXCode.TabIndex = 9
        Me.LabelTXCode.Tag = ""
        Me.LabelTXCode.Text = "..."
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(423, 22)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(181, 164)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 10
        Me.PictureBox1.TabStop = False
        '
        'ButtonTX
        '
        Me.ButtonTX.Location = New System.Drawing.Point(450, 305)
        Me.ButtonTX.Name = "ButtonTX"
        Me.ButtonTX.Size = New System.Drawing.Size(190, 49)
        Me.ButtonTX.TabIndex = 7
        Me.ButtonTX.Text = "Transmit Once"
        Me.ButtonTX.UseVisualStyleBackColor = True
        '
        'ComboBoxBVal
        '
        Me.ComboBoxBVal.FormattingEnabled = True
        Me.ComboBoxBVal.Location = New System.Drawing.Point(184, 153)
        Me.ComboBoxBVal.Name = "ComboBoxBVal"
        Me.ComboBoxBVal.Size = New System.Drawing.Size(150, 33)
        Me.ComboBoxBVal.TabIndex = 3
        '
        'ButtonTXCont
        '
        Me.ButtonTXCont.Location = New System.Drawing.Point(243, 305)
        Me.ButtonTXCont.Name = "ButtonTXCont"
        Me.ButtonTXCont.Size = New System.Drawing.Size(190, 49)
        Me.ButtonTXCont.TabIndex = 6
        Me.ButtonTXCont.Text = "Transmit Continuous"
        Me.ButtonTXCont.UseVisualStyleBackColor = True
        '
        'ComboBoxTXType
        '
        Me.ComboBoxTXType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxTXType.FormattingEnabled = True
        Me.ComboBoxTXType.Location = New System.Drawing.Point(423, 201)
        Me.ComboBoxTXType.Name = "ComboBoxTXType"
        Me.ComboBoxTXType.Size = New System.Drawing.Size(182, 33)
        Me.ComboBoxTXType.TabIndex = 4
        '
        'Timer1
        '
        '
        'NumericUpDownPVal
        '
        Me.NumericUpDownPVal.Location = New System.Drawing.Point(184, 96)
        Me.NumericUpDownPVal.Name = "NumericUpDownPVal"
        Me.NumericUpDownPVal.Size = New System.Drawing.Size(150, 31)
        Me.NumericUpDownPVal.TabIndex = 2
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(678, 371)
        Me.Controls.Add(Me.NumericUpDownPVal)
        Me.Controls.Add(Me.ComboBoxTXType)
        Me.Controls.Add(Me.ButtonTXCont)
        Me.Controls.Add(Me.ComboBoxBVal)
        Me.Controls.Add(Me.ButtonTX)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.LabelTXCode)
        Me.Controls.Add(Me.LabelTXInterval)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ButtonSave)
        Me.Controls.Add(Me.TextBoxID)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Oceanic Transmitter Simulator"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDownPVal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBoxID As TextBox
    Friend WithEvents ButtonSave As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents LabelTXInterval As Label
    Friend WithEvents LabelTXCode As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ButtonTX As Button
    Friend WithEvents ComboBoxBVal As ComboBox
    Friend WithEvents ButtonTXCont As Button
    Friend WithEvents ComboBoxTXType As ComboBox
    Friend WithEvents Timer1 As Timer
    Friend WithEvents NumericUpDownPVal As NumericUpDown
End Class
