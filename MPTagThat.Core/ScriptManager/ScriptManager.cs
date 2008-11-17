using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using CSScriptLibrary;

namespace MPTagThat.Core
{
  public class ScriptManager : IScriptManager
  {

    private string _sharedAsemblyDir;
    private Assembly _assembly;
    private ArrayList _tagScripts = new ArrayList();


    public ScriptManager()
    {
      //script (to be compiled) must be in the same folder with Core (MPTagThat.Core.dll) 
      _sharedAsemblyDir = Path.GetDirectoryName(GetLoadedAssemblyLocation("MPTagThat.core"));
      LoadAvailableScripts();
    }

    #region Initialisation
    public ArrayList GetScripts()
    {
      return _tagScripts;
    }

    private void LoadAvailableScripts()
    {
      DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(_sharedAsemblyDir, "Scripts"));
      FileInfo[] files = dirInfo.GetFiles();
      foreach (FileInfo file in files)
      {
        string ext = Path.GetExtension(file.Name);
        string[] desc = GetDescription(file.FullName);
        if (desc == null)
          continue;   // we had an error reading the file

        if (desc[1] == String.Empty)
          desc[1] = Path.GetFileNameWithoutExtension(file.Name); // Use the Filename as Title            

        if (ext == ".sct")
          _tagScripts.Add(desc);
      }
    }

    private string[] GetDescription(string fileName)
    {
      string[] description = new string[3];
      StreamReader file;
      try
      {
        file = File.OpenText(fileName);
        string line1 = file.ReadLine();
        string line2 = file.ReadLine();

        description[0] = Path.GetFileName(fileName);
        if (line1.StartsWith("// Title:"))
          description[1] = line1.Substring(9).Trim();
        else
          description[1] = String.Empty;

        if (line2.StartsWith("// Description:"))
          description[2] = line2.Substring(15).Trim();
        else
          description[2] = String.Empty;

        return description;
      }
      catch (Exception)
      {
        return null;
      }
    }
    #endregion

    #region Script Compiling and Loading
    public Assembly Load(string script)
    {
      string scriptFile = String.Format(@"scripts\{0}", script);

      try
      {
        string finalName = String.Format(@"scripts\Compiled\{0}.dll", Path.GetFileNameWithoutExtension(scriptFile));
        finalName = Path.Combine(_sharedAsemblyDir, finalName);
        string name = String.Format(@"{0}.dll", Path.GetFileNameWithoutExtension(scriptFile));

        // The script file could have been deleted while already executing the progrsm
        if (!File.Exists(scriptFile) && !File.Exists(finalName))
          return null;

        DateTime lastwriteSourceFile = File.GetLastWriteTime(scriptFile);

        // LOad the compiled Assembly only, if it is newer than the source file, to get changes done on the file
        if (File.Exists(finalName) && File.GetLastWriteTime(finalName) > lastwriteSourceFile)
        {
          _assembly = Assembly.LoadFile(finalName);
        }
        else
        {
          string scriptCopy = Path.Combine(_sharedAsemblyDir, name);
          File.Copy(scriptFile, scriptCopy, true);
          _assembly = CSScript.Load(scriptCopy, finalName, true);
          File.Delete(scriptCopy);
          // And reload the assembly from the compiled dir, so that we may execute it
          Assembly.LoadFile(finalName);
        }
        return _assembly;
      }
      catch (Exception ex)
      {
        ServiceScope.Get<ILogger>().Error("Error loading script: {0} {1}", scriptFile, ex.Message);
        return null;
      }
    }

    private string GetLoadedAssemblyLocation(string name)
    {
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
      {
        string asmName = asm.FullName.Split(",".ToCharArray())[0];
        if (string.Compare(name, asmName, true) == 0)
          return asm.Location;
      }
      return "";
    }
    #endregion
  }
}
