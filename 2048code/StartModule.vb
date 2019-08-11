Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports System.Drawing

Class MainForm
    Inherits Form

    Private g As Graphics

    Private Const Smooth As Integer = 10
    Private Const Cnt As Integer = 3, SZ As Integer = 100, AngleSZ As Integer = 6, BorderSZ As Integer = 4
    Private Const TrueSZ As Integer = SZ - 2 * BorderSZ

    Private Table(Cnt, Cnt), TmpTable(Cnt, Cnt) As Byte
    Private _Sign((Cnt + 1) * (Cnt + 1)) As Boolean
    Private MoveDistance(Cnt, Cnt) As Integer
    Private BlockTable(Cnt, Cnt) As Point
    Private Ri, Rj As Integer

    Private Strs() As String = {"", "2", "4", "8", "16", "32", "64", "128", "256", "512", "1024", "2048"}
    Private Bs() As Brush = {Brushes.White, Brushes.LightSeaGreen, Brushes.Orange, Brushes.Yellow, Brushes.LightBlue, Brushes.MediumVioletRed, Brushes.Gray, Brushes.Salmon, Brushes.Green, Brushes.Purple, Brushes.DarkOrange, Brushes.Gold}

    Private WithEvents Clock As New System.Windows.Forms.Timer
    Private Count As Integer

    Private Property Sign(i As Integer, j As Integer) As Boolean
        Get
            Return _Sign(i * Cnt + j)
        End Get
        Set(value As Boolean)
            _Sign(i * Cnt + j) = value
        End Set
    End Property

    Sub New()
        MyBase.New()
        Me.KeyPreview = True
        Me.DoubleBuffered = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D
        Me.ClientSize = New Size((1 + Cnt) * SZ, (1 + Cnt) * SZ)
        Me.BackColor = Color.LightYellow
        Clock.Interval = 10
        Clock.Enabled = False
        g = Me.CreateGraphics
        Init()
    End Sub

    Private Sub Clear()
        For j As Integer = 0 To Cnt
            For i As Integer = 0 To Cnt
                SetT(i, j) = 0
            Next
        Next
    End Sub

    Private WriteOnly Property SetT(i As Integer, j As Integer) As Byte
        Set(value As Byte)
            Table(i, j) = value
            TmpTable(i, j) = value
            BlockTable(i, j) = New Point(i * SZ, j * SZ)
        End Set
    End Property

    Private Sub Init()
        Clear()
        Rand()
        Rand()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        For i As Integer = 0 To Cnt
            For j As Integer = 0 To Cnt
                If TmpTable(i, j) > 0 Then
                    DrawBlock(BlockTable(i, j), TmpTable(i, j))
                End If
            Next
        Next
    End Sub

    Private Sub Form_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                d = 0
            Case Keys.Down
                d = 1
            Case Keys.Left
                d = 2
            Case Keys.Right
                d = 3
            Case Keys.Escape
                Init()
                Me.Invalidate()
                Exit Sub
            Case Else
                Exit Sub
        End Select
        Run()
    End Sub

    Private Sub Print()
        For j As Integer = 0 To Cnt
            For i As Integer = 0 To Cnt
                Console.Write(Table(i, j).ToString + " ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub

    Private d As Integer

    Private Sub Run()
        Dim k As Integer
        Dim Moved As Boolean = False
        Select Case d
            Case 0
                For i As Integer = 0 To Cnt
                    For j As Integer = 0 To Cnt
                        TmpTable(i, j) = Table(i, j)
                        Sign(i, j) = False
                        k = j
                        If Table(i, j) > 0 Then
                            Do
                                k -= 1
                                If k < 0 OrElse Sign(i, k) Then Exit Do
                                If Table(i, j) = Table(i, k) Then
                                    Table(i, j) += Convert.ToByte(1)
                                    Sign(i, k) = True
                                    k -= 1
                                    Exit Do
                                ElseIf Table(i, k) <> 0 Then
                                    Exit Do
                                End If
                            Loop
                            k += 1
                            If k <> j Then
                                Moved = True
                                Table(i, k) = Table(i, j)
                                Table(i, j) = 0
                            End If
                        End If
                        MoveDistance(i, j) = (j - k) * Smooth
                    Next
                Next
            Case 1
                For i As Integer = 0 To Cnt
                    For j As Integer = Cnt To 0 Step -1
                        TmpTable(i, j) = Table(i, j)
                        Sign(i, j) = False
                        k = j
                        If Table(i, j) > 0 Then
                            Do
                                k += 1
                                If k > Cnt OrElse Sign(i, k) Then Exit Do
                                If Table(i, j) = Table(i, k) Then
                                    Table(i, j) += Convert.ToByte(1)
                                    Sign(i, k) = True
                                    k += 1
                                    Exit Do
                                ElseIf Table(i, k) <> 0 Then
                                    Exit Do
                                End If
                            Loop
                            k -= 1
                            If k <> j Then
                                Moved = True
                                Table(i, k) = Table(i, j)
                                Table(i, j) = 0
                            End If
                        End If
                        MoveDistance(i, j) = (k - j) * Smooth
                    Next
                Next
            Case 2
                For j As Integer = 0 To Cnt
                    For i As Integer = 0 To Cnt
                        TmpTable(i, j) = Table(i, j)
                        Sign(i, j) = False
                        k = i
                        If Table(i, j) > 0 Then
                            Do
                                k -= 1
                                If k < 0 OrElse Sign(k, j) Then Exit Do
                                If Table(i, j) = Table(k, j) Then
                                    Table(i, j) += Convert.ToByte(1)
                                    Sign(k, j) = True
                                    k -= 1
                                    Exit Do
                                ElseIf Table(k, j) <> 0 Then
                                    Exit Do
                                End If
                            Loop
                            k += 1
                            If k <> i Then
                                Moved = True
                                Table(k, j) = Table(i, j)
                                Table(i, j) = 0
                            End If
                        End If
                        MoveDistance(i, j) = (i - k) * Smooth
                    Next
                Next
            Case 3
                For j As Integer = 0 To Cnt
                    For i As Integer = Cnt To 0 Step -1
                        TmpTable(i, j) = Table(i, j)
                        Sign(i, j) = False
                        k = i
                        If Table(i, j) > 0 Then
                            Do
                                k += 1
                                If k > Cnt OrElse Sign(k, j) Then Exit Do
                                If Table(i, j) = Table(k, j) Then
                                    Table(i, j) += Convert.ToByte(1)
                                    Sign(k, j) = True
                                    k += 1
                                    Exit Do
                                ElseIf Table(k, j) <> 0 Then
                                    Exit Do
                                End If
                            Loop
                            k -= 1
                            If k <> i Then
                                Moved = True
                                Table(k, j) = Table(i, j)
                                Table(i, j) = 0
                            End If
                        End If
                        MoveDistance(i, j) = (k - i) * Smooth
                    Next
                Next
        End Select
        If Moved Then
            RemoveHandler Me.KeyDown, AddressOf Form_KeyDown
            Count = SZ \ Smooth
            Clock.Enabled = True
        End If
    End Sub



    Private Sub Animate(sender As Object, e As EventArgs) Handles Clock.Tick
        If Count > 0 Then
            Select Case d
                Case 0
                    For i As Integer = 0 To Cnt
                        For j As Integer = 0 To Cnt
                            If MoveDistance(i, j) <> 0 Then
                                BlockTable(i, j).Y -= MoveDistance(i, j)
                                Me.Invalidate(New Rectangle(BlockTable(i, j), New Size(SZ, SZ + 3 * Smooth)))
                            End If
                        Next
                    Next
                Case 1
                    For i As Integer = 0 To Cnt
                        For j As Integer = 0 To Cnt
                            If MoveDistance(i, j) <> 0 Then
                                BlockTable(i, j).Y += MoveDistance(i, j)
                                Me.Invalidate(New Rectangle(New Point(BlockTable(i, j).X, BlockTable(i, j).Y - 3 * Smooth), New Size(SZ, SZ + 3 * Smooth)))
                            End If
                        Next
                    Next
                Case 2
                    For i As Integer = 0 To Cnt
                        For j As Integer = 0 To Cnt
                            If MoveDistance(i, j) <> 0 Then
                                BlockTable(i, j).X -= MoveDistance(i, j)
                                Me.Invalidate(New Rectangle(BlockTable(i, j), New Size(SZ + 3 * Smooth, SZ)))
                            End If
                        Next
                    Next
                Case 3
                    For i As Integer = 0 To Cnt
                        For j As Integer = 0 To Cnt
                            If MoveDistance(i, j) <> 0 Then
                                BlockTable(i, j).X += MoveDistance(i, j)
                                Me.Invalidate(New Rectangle(New Point(BlockTable(i, j).X - 3 * Smooth, BlockTable(i, j).Y), New Size(SZ + 3 * Smooth, SZ)))
                            End If
                        Next
                    Next
            End Select
            Count -= 1
        Else
            Dim Max As Byte = 0
            For i As Integer = 0 To Cnt
                For j As Integer = 0 To Cnt
                    SetT(i, j) = Table(i, j)
                    If Table(i, j) > Max Then
                        Max = Table(i, j)
                    End If
                Next
            Next
            Clock.Enabled = False
            If Max >= UBound(Strs) Then
                Me.Invalidate()
                MessageBox.Show("学霸！" + vbCrLf + "你获得了" + Strs(Max))
                Init()
            Else
                Rand()
                Me.Invalidate(New Rectangle(New Point(Ri, Rj), New Size(SZ, SZ)))
            End If
            AddHandler Me.KeyDown, AddressOf Form_KeyDown
        End If
    End Sub

    Private Sub Rand()
        Static R As New Random
        Do
            Ri = R.Next(Cnt + 1)
            Rj = R.Next(Cnt + 1)
        Loop While Table(Ri, Rj) > 0
        If R.Next(6) = 0 Then
            SetT(Ri, Rj) = 2
        Else
            SetT(Ri, Rj) = 1
        End If
    End Sub

    Private Sub DrawBlock(p As Point, i As Integer)
        If i < 0 OrElse i >= Strs.Length Then
            Exit Sub
        End If
        Dim b As Brush = Bs(i)
        Dim x, y As Integer
        x = p.X : y = p.Y
        Static f1 As New Font("Calibri", 40.0F, FontStyle.Bold)
        Static f2 As New Font("微软雅黑", 25.0F, FontStyle.Bold)
        x += BorderSZ
        y += BorderSZ
        g.FillRectangle(b, x + AngleSZ, y, TrueSZ - 2 * AngleSZ, TrueSZ)
        g.FillRectangle(b, x, y + AngleSZ, TrueSZ, TrueSZ - 2 * AngleSZ)
        g.FillEllipse(b, x, y, AngleSZ * 2, AngleSZ * 2)
        g.FillEllipse(b, x + TrueSZ - AngleSZ * 2, y, AngleSZ * 2, AngleSZ * 2)
        g.FillEllipse(b, x, y + TrueSZ - AngleSZ * 2, AngleSZ * 2, AngleSZ * 2)
        g.FillEllipse(b, x + TrueSZ - AngleSZ * 2, y + TrueSZ - AngleSZ * 2, AngleSZ * 2, AngleSZ * 2)
        If i Mod 2 = 0 Then
            b = Brushes.Black
        Else
            b = Brushes.White
        End If
        If Strs(i).Length = 1 Then
            g.DrawString(Strs(i), f1, b, x + TrueSZ \ 6, y + TrueSZ \ 16 + 5)
        ElseIf Strs(i).Length = 2 Then
            g.DrawString(Strs(i), f1, b, x + TrueSZ \ 10, y + TrueSZ \ 10 + 7)
        ElseIf Strs(i).Length = 3 Then
            g.DrawString(Strs(i), f2, b, x + TrueSZ \ 16, y + TrueSZ \ 16 + 15)
        ElseIf Strs(i).Length > 3 Then
            g.DrawString(Strs(i), f2, b, x + TrueSZ \ 22, y + TrueSZ \ 22 + 20)
        End If
    End Sub

End Class

Module StartModule
    Sub Main()
        Application.Run(New MainForm)
    End Sub
End Module
