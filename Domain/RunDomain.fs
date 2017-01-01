﻿namespace R4nd0mApps.TddStud10.Common.Domain

open System

type RunData = 
    | NoData
    | TestCases of PerDocumentLocationDTestCases
    | SequencePoints of PerDocumentSequencePoints
    | TestRunOutput of PerTestIdDResults * PerDocumentLocationTestFailureInfo * PerSequencePointIdTestRunId

type RunDataFiles = 
    { SequencePointStore : FilePath
      CoverageSessionStore : FilePath
      TestResultsStore : FilePath
      DiscoveredUnitTestsStore : FilePath
      TestFailureInfoStore : FilePath
      DiscoveredUnitDTestsStore : FilePath }

type SolutionPaths = 
    { Path : FilePath
      SnapshotPath : FilePath
      BuildRoot : FilePath }

[<CLIMutable>]
type RunStartParams = 
    { SnapShotRoot : FilePath
      StartTime : DateTime
      TestHostPath : FilePath
      Solution : SolutionPaths
      DataFiles : RunDataFiles
      // TODO: Merge this with EngineConfig, otherwise we will keep duplicating parameters
      IgnoredTests : string
      AdditionalMSBuildProperties : string[] }

type RunStepInfo = 
    { name : RunStepName
      kind : RunStepKind
      subKind : RunStepSubKind }

type RunStepResult = 
    { status : RunStepStatus
      runData : RunData
      addendum : RunStepStatusAddendum }

exception RunStepFailedException of RunStepResult

[<CLIMutable>]
type RunStepStartingEventArg = 
    { sp : RunStartParams
      info : RunStepInfo }

[<CLIMutable>]
type RunStepErrorEventArg = 
    { sp : RunStartParams
      info : RunStepInfo
      rsr : RunStepResult }

[<CLIMutable>]
type RunStepEndedEventArg = 
    { sp : RunStartParams
      info : RunStepInfo
      rsr : RunStepResult }

type RunStepEvents = 
    { onStart : Event<RunStepStartingEventArg>
      onError : Event<RunStepErrorEventArg>
      onFinish : Event<RunStepEndedEventArg> }

type RunStepFunc = IRunExecutorHost -> RunStartParams -> RunStepInfo -> RunStepEvents -> RunStepResult

type RunStepFuncWrapper = RunStepFunc -> RunStepFunc

type RunStep = 
    { info : RunStepInfo
      func : RunStepFunc }

type RunSteps = RunStep array
