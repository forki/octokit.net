#r @"tools\FAKE.Core\tools\FakeLib.dll"
open Fake 

let buildDir = "./Octokit/bin"
let reactiveBuildDir = "./Octokit.Reactive/bin"
let packageDir = "./packaging/octokit/"
let reactivePackageDir = "./packaging/octokit/reactive"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; reactiveBuildDir; packageDir; reactivePackageDir]
)

Target "BuildApp" (fun _ ->
    MSBuildWithDefaults "Build" ["./Octokit.sln"]
    |> Log "AppBuild-Output: "
)

Target "CreateOctokitPackage" (fun _ ->
    let net45Dir = packageDir @@ "lib/net45/"
    let netcore45Dir = packageDir @@ "lib/netcore45/"
    CleanDirs [net45Dir; netcore45Dir]

    CopyFile net45Dir (buildDir @@ "Release/Net40/Octokit.dll") // TODO: this a bug in the sln?!
    CopyFile netcore45Dir (buildDir @@ "Release/NetCore45/Octokit.dll")
    CopyFiles packageDir ["LICENSE.txt"; "README.md"]
)

Target "CreateOctokitReactivePackage" (fun _ ->
    let net45Dir = reactivePackageDir @@ "lib/net45/"
    CleanDirs [net45Dir]

    CopyFile net45Dir (reactiveBuildDir @@ "Release/Net40/Octokit.Reactive.dll") // TODO: this a bug in the sln?!    
    CopyFiles packageDir ["LICENSE.txt"; "README.md"]
)

Target "Default" DoNothing

"Clean"
   ==> "BuildApp"
   ==> "CreateOctokitPackage"
   ==> "CreateOctokitReactivePackage"
   ==> "Default"

RunTargetOrDefault "Default"