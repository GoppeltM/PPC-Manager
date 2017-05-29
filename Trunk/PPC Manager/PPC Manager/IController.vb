Imports PPC_Manager

Public Interface IController

    ReadOnly Property ExcelPfad As String
    Function NächsteRunde(rundenName As String) As SpielRunde
    Function DruckeSchiedsrichterzettel(seitenFormat As SeitenEinstellung) As FixedDocument
    Function DruckeNeuePaarungen(seitenFormat As SeitenEinstellung) As FixedDocument
    Function DruckeRangliste(seitenFormat As SeitenEinstellung) As FixedDocument
    Function DruckeSpielergebnisse(seitenFormat As SeitenEinstellung) As FixedDocument
    Sub ExcelExportieren(p1 As String)
    Sub SaveXML()
    Sub SaveExcel()
End Interface
