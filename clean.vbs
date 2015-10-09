Option Explicit

Dim fso
Set fso = CreateObject("Scripting.FileSystemObject")

Wscript.echo "Start"
Dim sCurPath: sCurPath = CreateObject("Scripting.FileSystemObject").GetAbsolutePathName(".")
DirWalk(sCurPath)  ' and/or like this.
Wscript.echo "End"

Sub DirWalk(parmPath)
    Dim oSubDir, oSubFolder, oFile, n

   On Error Resume Next         ' We'll handle any errors ourself, thank you very much

   Set oSubFolder = fso.getfolder(parmPath)

   for each oFile in oSubFolder.Files
    if (oFile.name = "StyleCop.Cache") then fso.deletefile ofile
   next
   
    For Each oSubDir In oSubFolder.Subfolders
        if (oSubDir.name = "bin" or oSubDir.name = "obj") then
            Wscript.echo oSubDir
            fso.DeleteFolder oSubDir, true
        end if
        DirWalk oSubDir.Path      ' recurse the DirWalk sub with the subdir paths
    Next

   On Error Goto 0              ' Resume letting system handle errors.
End Sub