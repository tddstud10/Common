namespace R4nd0mApps.TddStud10.Common.Domain

open Microsoft.FSharp.Reflection
open System
open System.Diagnostics
open System.Reflection
open System.Runtime.Serialization
open System.ComponentModel

[<KnownType("KnownTypes")>]
type AssemblyId = 
    | AssemblyId of Guid
    
    override x.ToString() = 
        match x with
        | AssemblyId g -> g.ToString()
    static member KnownTypes() = 
        typeof<AssemblyId>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion

[<KnownType("KnownTypes")>]
type MdTokenRid = 
    | MdTokenRid of uint32
    
    override x.ToString() = 
        match x with
        | MdTokenRid m -> m.ToString()
    static member KnownTypes() = 
        typeof<MdTokenRid>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion

[<KnownType("KnownTypes")>]
type DocumentCoordinate = 
    | DocumentCoordinate of int
    
    override x.ToString() = 
        match x with
        | DocumentCoordinate dc -> dc.ToString()
    static member KnownTypes() = 
        typeof<DocumentCoordinate>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion

type TestRunInstanceId = 
    | TestRunInstanceId of int
    
    override x.ToString() = 
        match x with
        | TestRunInstanceId t -> t.ToString()

[<CLIMutable>]
[<TypeConverter(typeof<DocumentLocationConverter>)>]
type DocumentLocation = 
    { document : FilePath
      line : DocumentCoordinate }
    
    override x.ToString() = 
        sprintf "%O|%O" x.document x.line

    static member FromString s =
        let fields = String.split '|' s
        { document = FilePath fields.[0]
          line = DocumentCoordinate(int fields.[1]) }

and DocumentLocationConverter() = 
    inherit DomainTypeConverter<DocumentLocation>(DocumentLocation.FromString)

[<KnownType("KnownTypes")>]
type StackFrame = 
    | ParsedFrame of string * DocumentLocation
    | UnparsedFrame of string
    static member KnownTypes() = 
        typeof<StackFrame>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion

[<CLIMutable>]
type TestFailureInfo = 
    { message : string
      stack : StackFrame array }

[<CLIMutable>]
[<TypeConverter(typeof<TestIdConverter>)>]
type TestId = 
    { source : FilePath
      location : DocumentLocation }
    
    override x.ToString() = 
        sprintf "%O|%O" x.source x.location

    static member FromString s =
        let fields = String.split '|' s
        { source = FilePath fields.[0]
          location = 
              { document = FilePath fields.[1]
                line = DocumentCoordinate(int fields.[2]) } }

and TestIdConverter() = 
    inherit DomainTypeConverter<TestId>(TestId.FromString)

[<CLIMutable>]
type TestRunId = 
    { testId : TestId
      testRunInstanceId : TestRunInstanceId }

[<CLIMutable>]
type MethodId = 
    { assemblyId : AssemblyId
      mdTokenRid : MdTokenRid }
    
    override x.ToString() = 
        sprintf "%O|%O" x.assemblyId x.mdTokenRid

[<CLIMutable>]
[<TypeConverter(typeof<SequencePointIdConverter>)>]
type SequencePointId = 
    { methodId : MethodId
      uid : int }
    
    override x.ToString() = 
        sprintf "%O|%O" x.methodId x.uid

    static member FromString s =
        let fields = String.split '|' s
        { methodId = 
              { assemblyId = AssemblyId(System.Guid.Parse fields.[0])
                mdTokenRid = MdTokenRid(uint32 fields.[1]) }
          uid = int fields.[2] }

and SequencePointIdConverter() = 
    inherit DomainTypeConverter<SequencePointId>(SequencePointId.FromString)

[<CLIMutable>]
type SequencePoint = 
    { id : SequencePointId
      document : FilePath
      startLine : DocumentCoordinate
      startColumn : DocumentCoordinate
      endLine : DocumentCoordinate
      endColumn : DocumentCoordinate }

[<CLIMutable>]
type DTestCase =
    { DtcId : Guid
      FullyQualifiedName : string
      DisplayName : string
      Source : FilePath
      CodeFilePath : FilePath
      LineNumber : DocumentCoordinate }

[<KnownType("KnownTypes")>]
type DTestOutcome =
    | TONone
    | TOPassed
    | TOFailed
    | TOSkipped
    | TONotFound
    static member KnownTypes() = 
        typeof<DTestOutcome>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion

[<CLIMutable>]
type DTestResult =
    { DisplayName : string
      TestCase : DTestCase
      Outcome : DTestOutcome
      ErrorStackTrace : string
      ErrorMessage : string }
