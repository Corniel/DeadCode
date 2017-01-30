Public Class SimpleClass

	Public Property Prop As Integer

	Public ReadOnly Property Twice As Integer
		Get
			Return Prop * 2
		End Get
	End Property

	Public Function Method(num As Integer) As Integer
		Return num * Prop
	End Function

End Class
