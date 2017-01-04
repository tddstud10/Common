namespace R4nd0mApps.TddStud10.Common.Domain

open Microsoft.FSharp.Reflection
open System
open System.Diagnostics
open System.Reflection
open System.Runtime.Serialization

[<CustomEquality; CustomComparison>]
[<DebuggerDisplay("{ToString()}")>]
[<KnownType("KnownTypes")>]
type FilePath = 
    | FilePath of string
    static member KnownTypes() = 
        typeof<FilePath>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion
    
    override x.ToString() = 
        match x with
        | FilePath s -> s
    
    override x.GetHashCode() = 
        match x with
        | FilePath s -> s.ToUpperInvariant().GetHashCode()
    
    override x.Equals(yObj) = 
        match yObj with
        | :? FilePath as y -> 
            let FilePath x, FilePath y = x, y
            x.ToUpperInvariant().Equals(y.ToUpperInvariant(), StringComparison.Ordinal)
        | _ -> false
    
    interface IComparable<FilePath> with
        member x.CompareTo(y : FilePath) : int = 
            let FilePath x, FilePath y = x, y
            x.ToUpperInvariant().CompareTo(y.ToUpperInvariant())
    
    interface IComparable with
        member x.CompareTo(yObj : obj) : int = 
            match yObj with
            | :? FilePath as y -> (x :> IComparable<FilePath>).CompareTo(y)
            | _ -> 1
    
    interface IEquatable<FilePath> with
        member x.Equals(y : FilePath) : bool = x.Equals(y)

type AssemblyId = 
    | AssemblyId of Guid
    
    override x.ToString() = 
        match x with
        | AssemblyId g -> g.ToString()

type MdTokenRid = 
    | MdTokenRid of uint32
    
    override x.ToString() = 
        match x with
        | MdTokenRid m -> m.ToString()

type DocumentCoordinate = 
    | DocumentCoordinate of int
    
    override x.ToString() = 
        match x with
        | DocumentCoordinate dc -> dc.ToString()

type TestRunInstanceId = 
    | TestRunInstanceId of int
    
    override x.ToString() = 
        match x with
        | TestRunInstanceId t -> t.ToString()

[<CLIMutable>]
type DocumentLocation = 
    { document : FilePath
      line : DocumentCoordinate }
    
    override x.ToString() = 
        sprintf "%O.%O" x.document x.line

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
type TestId = 
    { source : FilePath
      location : DocumentLocation }
    
    override x.ToString() = 
        sprintf "%O.%O" x.source x.location

[<CLIMutable>]
type TestRunId = 
    { testId : TestId
      testRunInstanceId : TestRunInstanceId }

[<CLIMutable>]
type MethodId = 
    { assemblyId : AssemblyId
      mdTokenRid : MdTokenRid }
    
    override x.ToString() = 
        sprintf "%O.%O" x.assemblyId x.mdTokenRid

[<CLIMutable>]
type SequencePointId = 
    { methodId : MethodId
      uid : int }
    
    override x.ToString() = 
        sprintf "%O.%O" x.methodId x.uid

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
