Imports PPC_Manager

Public Interface IFixedPageFabrik
    Function ErzeugeSchiedsrichterZettelSeiten(seiteneinstellung As SeitenEinstellung) As IEnumerable(Of FixedPage)
    Function ErzeugeSpielErgebnisse(seiteneinstellung As SeitenEinstellung) As IEnumerable(Of FixedPage)
    Function ErzeugeRanglisteSeiten(seiteneinstellung As SeitenEinstellung) As IEnumerable(Of FixedPage)
    Function ErzeugePaarungen(seitenEinstellung As SeitenEinstellung) As IEnumerable(Of FixedPage)
End Interface
