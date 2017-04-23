Imports PPC_Manager

Public Interface IFixedPageFabrik
    Function ErzeugeSchiedsrichterZettelSeiten(seiteneinstellung As ISeiteneinstellung) As IEnumerable(Of FixedPage)
    Function ErzeugeSpielErgebnisse(seiteneinstellung As ISeiteneinstellung) As IEnumerable(Of FixedPage)
    Function ErzeugeRanglisteSeiten(seiteneinstellung As ISeiteneinstellung) As IEnumerable(Of FixedPage)
    Function ErzeugePaarungen(seitenEinstellung As ISeiteneinstellung) As IEnumerable(Of FixedPage)
End Interface
