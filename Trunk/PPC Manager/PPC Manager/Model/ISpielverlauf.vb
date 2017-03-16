Public Interface ISpielverlauf(Of T)
    Function BerechnePunkte(t As T) As Integer

    Function HatFreilos(t As T) As Boolean
End Interface
