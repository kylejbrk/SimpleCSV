Public Class DEV_Window

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If TextBox1.Text = My.Settings.DevPassword Then
            Main.devStatus = 1
            Main.BackColor = Color.FromArgb(51, 51, 51)
            Main.Label1.ForeColor = Color.FloralWhite
            Main.Label2.ForeColor = Color.FloralWhite
            Main.Label3.ForeColor = Color.FloralWhite
            Main.Label4.ForeColor = Color.FloralWhite
            Main.Label5.ForeColor = Color.FloralWhite
            Main.Label7.ForeColor = Color.FloralWhite
            Main.lblStatus.ForeColor = Color.Red
            Main.lblStatus.Text = "Development"
            Main.Connection = My.Settings.DevConnection
            Main.Main_Load(e, e)
        Else
            MessageBox.Show("Password is incorrect", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        End If

        TextBox1.Clear()
        Me.Close()
    End Sub
End Class