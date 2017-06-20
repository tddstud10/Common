namespace R4nd0mApps.TddStud10.Common.Domain

open System.ComponentModel

type DomainTypeConverter<'T>(fromStr : string -> 'T) = 
    inherit TypeConverter()
    
    override __.CanConvertFrom(_, sourceType) = 
        sourceType = typeof<string>
    
    override __.ConvertFrom(_, _, value) = 
        value :?> string |> fromStr :> _
    
    override __.CanConvertTo(_, destinationType) = 
        destinationType = typeof<string>
    
    override __.ConvertTo(_, _, value, _) = 
        value :?> 'T |> Prelude.toStr :> _

