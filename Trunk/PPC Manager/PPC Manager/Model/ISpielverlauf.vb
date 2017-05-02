Imports PPC_Manager

Public Interface ISpielverlauf(Of In T)
    Function BerechnePunkte(t As T) As Integer

    Function HatFreilos(t As T) As Boolean

    Function BerechneBuchholzPunkte(t As T) As Integer

    Function BerechneSonnebornBergerPunkte(t As T) As Integer
    Function IstAusgeschieden(t As T) As Boolean

    Function BerechneGewonneneSätze(t As T) As Integer

    Function BerechneVerloreneSätze(t As T) As Integer

    Function BerechneSatzDifferenz(t As T) As Integer

    Function Habengegeneinandergespielt(a As T, b As T) As Boolean
    Function BerechneGegnerProfil(s As T) As IEnumerable(Of String)
End Interface
