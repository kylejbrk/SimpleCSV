Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Public Class Main
    Dim fp As String
    Dim fn As String
    Dim dt As DataTable = New DataTable()
    Dim dtH As DataTable = New DataTable()
    Dim dtCount As Integer
    Dim dtS As DataTable = New DataTable()
    Dim rowMax As Integer = 99
    Dim columnNames As New List(Of String)()
    Public ProjectName_ID As String
    Public Connection As String = My.Settings.ProdConnection
    Dim SqlTableResult As String
    Dim FileErrMsg As String
    Public devStatus As Integer = 0
    Dim SQLtblFN As New List(Of String)()
    Dim SQLtblSP As New List(Of String)()

    Public Sub Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ComboBox1.Items.Clear()
        columnNames.Clear()
        dt.Columns.Clear()
        dt.Rows.Clear()
        TextBox1.Clear()
        Dim nullResults As New List(Of String)()
        Dim countRows As Integer = 0
        Dim SQLtblNames As String = "SELECT [ListName], [ImportType] FROM [TableNamesList] ORDER BY [ImportType] ASC, [ListName] ASC"
        Dim SQLtblNameWFN As String = "SELECT [ListName], [ImportType] FROM [TableNamesList] WHERE [FNameAdded] LIKE ('Y') ORDER BY [ImportType] ASC, [ListName] ASC"
        Dim SQLtblNamesS As String = "SELECT [ListName], [ImportType] FROM [TableNamesList] WHERE [FNameAdded] LIKE ('S') ORDER BY [ImportType] ASC, [ListName] ASC"

        Using Conn As SqlConnection = New SqlConnection(Connection)
            'Add tables to ComboBox
            Using cmd3 = New SqlCommand(SQLtblNames, Conn)
                Conn.Open()
                Try
                    Dim dr = cmd3.ExecuteReader()
                    While dr.Read()
                        Me.ComboBox1.Items.Add(dr("ListName").ToString())
                    End While
                Catch ex As Exception
                End Try
                Conn.Close()
            End Using

            'Get tables that require filenames
            Using cmd4 = New SqlCommand(SQLtblNameWFN, Conn)
                Conn.Open()
                Try
                    Dim dr2 = cmd4.ExecuteReader()
                    While dr2.Read()
                        SQLtblFN.Add(dr2("ListName").ToString())
                    End While
                Catch ex As Exception
                    Console.WriteLine(ex)
                End Try
                Conn.Close()
            End Using

            'Get tables with special requirements
            Using cmd5 = New SqlCommand(SQLtblNamesS, Conn)
                Conn.Open()
                Try
                    Dim dr3 = cmd5.ExecuteReader()
                    While dr3.Read()
                        SQLtblSP.Add(dr3("ListName").ToString())
                    End While
                Catch ex As Exception
                    Console.WriteLine(ex)
                End Try
            End Using
            Conn.Close()
        End Using

    End Sub

    'Selects file and populates datagridview
    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        columnNames.Clear()
        dt.Columns.Clear()
        dt.Rows.Clear()
        TextBox1.Clear()
        Dim nullResults As New List(Of String)()
        Dim countRows As Integer = 0

        ofd.Filter = "Excel|*.csv;*.xlsx"
        If (ofd.ShowDialog() = DialogResult.OK) Then
            txtbxFilePath.Text = ofd.FileName
            fp = txtbxFilePath.Text
            fn = Path.GetFileName(fp)
            If IsValidFileNameOrPath(fn) = False Then
                MessageBox.Show(FileErrMsg.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End If

        End If

        'Specify Filename
        If SQLtblFN.Contains(ComboBox1.Text) Then
            dt.Columns.Add("FileName").DefaultValue = fn
        ElseIf SQLtblSP.Contains(ComboBox1.Text) Then
            'Add special conditions here. Mark the FNameAdded column with the letter S
        End If

        dt = Me.ConvertCSVToDataTable(fp)
        dtCount = dt.Rows.Count()

        If dt.Columns.Contains("FileNameRow_ID") Then
            For Each dr As DataRow In dt.Rows
                Dim ir = (dt.Rows.IndexOf(dr) + 2).ToString().PadLeft(3, "0")
                dr(0) = "R" & ir & "_" & fn
            Next
        End If

        Dim dt2 = dt.Clone()

        If dt.Rows.Count >= rowMax Then
            For i = 0 To rowMax
                dt2.ImportRow(dt.Rows(i))
            Next
            DataGridView1.DataSource = dt2
        Else
            DataGridView1.DataSource = dt
        End If
        Me.DataGridView1.Refresh()


        For Each column As DataColumn In dt.Columns
            columnNames.Add(column.ColumnName.ToString)
        Next

        For i = 0 To DataGridView1.RowCount - 1
            DataGridView1.Rows(i).HeaderCell.Value = CStr(i + 1)
        Next

        For Each dgvRow As DataGridViewRow In DataGridView1.Rows
            For Each dgvCell As DataGridViewCell In dgvRow.Cells
                If IsDBNull(dgvCell.Value) Then
                    nullResults.Add(dgvRow.HeaderCell.Value)
                    dgvRow.HeaderCell.Style.BackColor = Color.Red
                    dgvCell.Style.BackColor = Color.Red
                    countRows += 1
                End If
            Next
        Next

        TextBox1.Text = "There are (" + countRows.ToString + ") Null Values in your Data" & Environment.NewLine & Environment.NewLine
        For Each Null In nullResults.Distinct()
            TextBox1.Text &= ("Null value(s) found at Row: " + Null & Environment.NewLine)
        Next

    End Sub

    Function IsValidFileNameOrPath(ByVal name As String) As Boolean
        ' Determines if the name is Nothing.
        If name Is Nothing Then
            FileErrMsg = "No Filename"
            Return False
        End If

        ' Determines if there are bad characters in the name.
        For Each badChar As Char In System.IO.Path.GetInvalidPathChars
            If InStr(name, badChar) > 0 Then
                FileErrMsg = "Filename has invalid Characters"
                Return False
            End If
        Next

        ' Checks if file is CSV
        If Path.GetExtension(name) = ".csv" Then

            ' Checks length
            If Path.GetFileNameWithoutExtension(name).Length() > 60 Then
                FileErrMsg = "Filename must be less than 60 Characters"
                Return False
            End If

            ' Check for periods
            If Path.GetFileNameWithoutExtension(name).Contains(".") Then
                FileErrMsg = "Filename cannot have period in name"
                Return False
            End If

        End If

        ' The name passes basic validation.
        Return True
    End Function

    'Converts CSV and Excel to datatable
    Private Function ConvertCSVToDataTable(ByVal path As String) As DataTable
        Using con As OleDb.OleDbConnection = New OleDb.OleDbConnection()
            Try
                If System.IO.Path.GetExtension(path) = ".csv" Then
                    con.ConnectionString = String.Format("Provider={0};Data Source={1};Extended Properties=""Text;HDR=YES;FMT=Delimited""", "Microsoft.Jet.OLEDB.4.0", IO.Path.GetDirectoryName(path))
                    Using cmd As OleDb.OleDbCommand = New OleDb.OleDbCommand("SELECT * FROM [" & IO.Path.GetFileName(path) & "]", con)
                        Using da As OleDb.OleDbDataAdapter = New OleDb.OleDbDataAdapter(cmd)
                            con.Open()
                            da.Fill(dt)
                            con.Close()
                        End Using
                    End Using
                ElseIf System.IO.Path.GetExtension(path) = ".xlsx" Then
                    con.ConnectionString = String.Format("Provider={0};Data Source={1};Extended Properties=""Excel 12.0 Xml;HDR=Yes;IMEX=1""", "Microsoft.ACE.OLEDB.12.0", path)
                    con.Open()
                    Dim dbSchema As DataTable = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
                    Dim firstSheetname As String = dbSchema.Rows(0)("TABLE_NAME").ToString
                    Using cmd As OleDb.OleDbCommand = New OleDb.OleDbCommand("SELECT * FROM [" & firstSheetname & "]", con)
                        Using da As OleDb.OleDbDataAdapter = New OleDb.OleDbDataAdapter(cmd)
                            da.Fill(dt)
                            con.Close()
                        End Using
                    End Using
                End If

            Catch ex As Exception
                MessageBox.Show(ex.ToString(), "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            Finally
                If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End Using
        Return dt
    End Function

    'Bulk Copy to Sql Server and Error Checking
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim SqlTableQry As String = "Select Distinct [Table] FROM [TableNamesList] WHERE [ListName] = '" + ComboBox1.Text + "'"
        Dim result As Integer = MessageBox.Show("Are you sure you want to import File Data into [" + ComboBox1.Text + "] ?", "Warning!", MessageBoxButtons.OKCancel)
        Dim output As New List(Of String)()
        If result = DialogResult.OK Then
            Using Conn As SqlConnection = New SqlConnection(Connection)

                Using cmd1 = New SqlCommand(SqlTableQry, Conn)
                    Conn.Open()
                    Try
                        Dim dr = cmd1.ExecuteReader()
                        While dr.Read()
                            SqlTableResult = dr("Table").ToString()
                        End While
                    Catch ex As Exception
                    End Try
                    Conn.Close()
                End Using

                Dim SQL As String = "Select column_name from information_schema.columns where table_name = " &
                            "(Select Distinct [Table] FROM [TableNamesList] WHERE [ListName] = '" + ComboBox1.Text + "')" &
                            "and COLUMNPROPERTY(object_id(TABLE_SCHEMA +'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 0 " &
                            "and COLUMNPROPERTY(object_id(TABLE_SCHEMA +'.'+TABLE_NAME), COLUMN_NAME, 'IsComputed') = 0"

                Using cmd2 = New SqlCommand(SQL, Conn)
                    Conn.Open()
                    Try
                        Dim dr = cmd2.ExecuteReader()
                        While dr.Read()
                            output.Add(dr("column_name").ToString())
                        End While
                    Catch ex As Exception
                    End Try
                    Conn.Close()
                End Using

                Using s As SqlBulkCopy = New SqlBulkCopy(Conn, SqlBulkCopyOptions.FireTriggers Or SqlBulkCopyOptions.CheckConstraints, Nothing)
                    Conn.Open()
                    Try
                        If ComboBox1.Text IsNot "" And txtbxFilePath.Text IsNot "" Then
                            s.DestinationTableName = SqlTableResult.ToString()
                            For i = 0 To columnNames.Count() - 1
                                If columnNames(i).ToString.ToUpper.Trim() = output(i).ToString.ToUpper.Trim() Then
                                    s.ColumnMappings.Add(columnNames(i).ToString(), output(i).ToString())
                                Else
                                    MessageBox.Show("Incorrect spelling/amount of Column Names." & Environment.NewLine & "Error at: [" + columnNames(i).ToString.ToUpper.Trim() + "] --> [" + output(i).ToString.ToUpper.Trim() + "]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                                    Conn.Close()
                                    Exit Sub
                                End If
                            Next
                            s.WriteToServer(dt)
                            s.Close()
                            MessageBox.Show("[" + dtCount.ToString() + "] Rows of Data Successfully Imported into: [" + ComboBox1.Text + "]", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                        Else
                            MessageBox.Show("Please select File and Table to import.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                        End If
                    Catch ex As Exception
                        MessageBox.Show(ex.ToString(), "SQL Bulk Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    End Try
                    Conn.Close()
                End Using
            End Using
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        dt.Clear()
        dt.Columns.Clear()
        dt.Rows.Clear()
        dtH.Clear()
        dtH.Columns.Clear()
        dtH.Rows.Clear()
        DataGridView1.ClearSelection()

        Dim output As New List(Of String)()

        Dim SQL As String = "Select column_name from information_schema.columns where table_name = " &
                            "(Select Distinct [Table] FROM [TableNamesList] WHERE [ListName] = '" + ComboBox1.Text + "')" &
                            "and COLUMNPROPERTY(object_id(TABLE_SCHEMA +'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 0 " &
                            "and COLUMNPROPERTY(object_id(TABLE_SCHEMA +'.'+TABLE_NAME), COLUMN_NAME, 'IsComputed') = 0"
        Using Conn As SqlConnection = New SqlConnection(Connection)

            Using cmd2 = New SqlCommand(SQL, Conn)
                Conn.Open()
                Try
                    Dim dr = cmd2.ExecuteReader()
                    While dr.Read()
                        output.Add(dr("column_name").ToString())
                    End While
                Catch ex As Exception
                End Try
                Conn.Close()
            End Using
        End Using

        For Each i As String In output
            dtH.Columns.Add(i.ToString())
        Next

        DataGridView1.DataSource = dtH
        DataGridView1.Refresh()

    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("https://github.com/kylejbrk/SimpleCSV")
    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If devStatus = 0 Then
                DEV_Window.ShowDialog()

            ElseIf devStatus = 1 Then
                Dim result As Integer = MessageBox.Show("Would you like to revert back to Production?", "Production Mode", MessageBoxButtons.OKCancel)
                If result = DialogResult.OK Then
                    devStatus = 0
                    BackColor = Color.White
                    Label1.ForeColor = Color.Black
                    Label2.ForeColor = Color.Black
                    Label3.ForeColor = Color.Black
                    Label4.ForeColor = Color.Black
                    Label5.ForeColor = Color.Black
                    lblStatus.ForeColor = Color.Black
                    lblStatus.Text = "Production"
                    Connection = "Server=localhost\SQLEXPRESS;Database=BluthCompany;Trusted_Connection=True;"
                    Me.Controls.Clear() 'removes all the controls on the form
                    InitializeComponent() 'load all the controls again
                    Main_Load(e, e) 'Load everything in your form load event again
                End If
            End If
        End If
    End Sub
End Class
