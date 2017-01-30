Public Class ClassWithConstructors

	Public Sub New(empty As EmptyClass)
		Me.Empty = empty
	End Sub

	Public Sub New(empty As EmptyClass, number As Integer)
		Me.Empty = If(number Mod 2 = 1, empty, Nothing)
	End Sub

	Public Property Empty As EmptyClass

End Class
