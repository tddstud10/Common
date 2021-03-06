﻿namespace R4nd0mApps.TddStud10.Common.Domain

open Microsoft.FSharp.Reflection
open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.Serialization
open System.ComponentModel

[<CustomEquality; CustomComparison>]
[<DebuggerDisplay("{ToString()}")>]
[<KnownType("KnownTypes")>]
[<TypeConverter(typeof<FilePathConverter>)>]
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

and FilePathConverter() = 
    inherit DomainTypeConverter<FilePath>(FilePath)


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
    
    let combine : FilePath list -> _ = 
        List.map Prelude.toStr
        >> Path.combine
        >> FilePath
    
    let fileExists (FilePath p) = p |> File.Exists
    let directoryExists (FilePath p) = p |> Directory.Exists
