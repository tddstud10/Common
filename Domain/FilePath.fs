namespace R4nd0mApps.TddStud10.Common.Domain

open Microsoft.FSharp.Reflection
open System
open System.Diagnostics
open System.IO
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

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FilePath = 
    let getDirectoryName (FilePath p) = 
        p
        |> Path.GetDirectoryName
        |> FilePath
    
    let getFullPath (FilePath p) = 
        p
        |> Path.GetFullPath
        |> FilePath
    
    let getFileName (FilePath p) = 
        p
        |> Path.GetFileName
        |> FilePath
    
    let getFileNameWithoutExtension (FilePath p) = 
        p
        |> Path.GetFileName
        |> FilePath
    
    let getPathWithoutRoot (FilePath p) = p.Substring(Path.GetPathRoot(p).Length) |> FilePath
    let combine (FilePath p1) (FilePath p2) = Path.Combine(p1, p2) |> FilePath
    let createDirectory (FilePath p) = p |> Directory.CreateDirectory
    let writeAllText text (FilePath p) = (p, text) |> File.WriteAllText
    let readAllText (FilePath p) = p |> File.ReadAllText
    let enumerateFiles so pattern (FilePath path) = Directory.EnumerateFiles(path, pattern, so) |> Seq.map FilePath
    
    let makeRelativePath (FilePath folder) (FilePath file) = 
        let file = Uri(file, UriKind.Absolute)
        
        let folder = 
            if folder.EndsWith(@"\") then folder
            else folder + @"\"
        
        let folder = Uri(folder, UriKind.Absolute)
        folder.MakeRelativeUri(file).ToString().Replace('/', Path.DirectorySeparatorChar)
        |> Uri.UnescapeDataString
        |> FilePath
    
    let getExtension (FilePath path) = Path.GetExtension(path)
