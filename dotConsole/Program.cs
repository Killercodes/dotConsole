using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace dotConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string readCode = File.ReadAllText(@"Scripts\PortScan.cs");

            Object[] parameters = new Object[2];
            parameters[0] = "192.168.1.1";
            parameters[1] = 69;

            Plugin.Status += (s) => Console.WriteLine(s);
            Plugin.Execute(readCode, "dotConsole.Network.PortScan", parameters);

            Console.ReadKey();
        }
    }


    //
    public static class Plugin
    {
        //stus
        public delegate void CompilerStatus(object state);
        public static event CompilerStatus Status;
        private static void _(object stat)
        {
            Status?.Invoke(stat);
        }

        //Execute
        public static void Execute(string sourceCode, string className, object[] args)
        {
            _(string.Format("- Compiling {0}...", className));
            var compParms = new CompilerParameters
            {
                // True - exe file generation, false - dll file generation
                GenerateExecutable = false,
                // True - memory generation, false - external file generation
                GenerateInMemory = true,
                //OutputAssembly = exeName,
                TreatWarningsAsErrors = false
            };

            //Add references
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => a.Location);
            compParms.ReferencedAssemblies.AddRange(assemblies.ToArray());
            //assemblies.ToList().ForEach(x => _(string.Format("- refering {0}", x.ToString())));

            //compile
            _(string.Format("Building..."));
            var csProvider = new CSharpCodeProvider();
            CompilerResults compilerResults = csProvider.CompileAssemblyFromSource(compParms, sourceCode);
            if (compilerResults.Errors.Count > 0)
            {
                // Display compilation errors.
                _(string.Format("- Errors building {0} into {1}", className, compilerResults.PathToAssembly));
                foreach (CompilerError ce in compilerResults.Errors)
                    _(string.Format("- {0}", ce.ToString()));
            }
            else
            {
                // Display a successful compilation message.
                _(string.Format("- Source {0} built into {1} successfully.", className, compilerResults.PathToAssembly));
            }

            //Execute
            _("Executing...");
            try
            {
                object typeInstance = compilerResults.CompiledAssembly.CreateInstance(className);
                MethodInfo mi = typeInstance.GetType().GetMethod("Execute");
                ParameterInfo[] parameters = mi.GetParameters();
                object classInstance = Activator.CreateInstance(typeInstance.GetType(), null);

                if (parameters.Length == 0)
                {
                    // This works fine
                    var result = mi.Invoke(classInstance, null);
                }
                else
                {
                    //object[] parametersArray = new object[] { "Hello","bye" };
                    var result = mi.Invoke(classInstance, new object[] { args });
                }
            }
            catch (Exception ex)
            {

                _(ex.Message);
                _(ex.StackTrace);
            }
        }

        //Compile Exe
        public static void Compile(string sourceCode, string className, bool isExe)
        {
            var compParms = new CompilerParameters
            {
                GenerateExecutable = true,
                GenerateInMemory = false,
                OutputAssembly = String.Format(@"{0}\{1}.exe", System.Environment.CurrentDirectory, className),
                TreatWarningsAsErrors = false
            };

            //compile
            var csProvider = new CSharpCodeProvider();
            CompilerResults compilerResults = csProvider.CompileAssemblyFromSource(compParms, sourceCode);
            if (compilerResults.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}",
                    className, compilerResults.PathToAssembly);
                foreach (CompilerError ce in compilerResults.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                // Display a successful compilation message.
                Console.WriteLine("Source {0} built into {1} successfully.",
                    className, compilerResults.PathToAssembly);
            }

        }

        // compile dll/exe
        public static void Compile(string sourceCode, string className)
        {
            // DLL compiler 
            var compParms = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                OutputAssembly = String.Format(@"{0}\{1}.dll", System.Environment.CurrentDirectory, className),
                TreatWarningsAsErrors = false,
                //CompilerOptions = "/optimize",
                //IncludeDebugInformation = true,
                //TempFiles = new TempFileCollection(".", true),
                WarningLevel = 3,

            };
            //Exe compilation
            /* var compParmsExe = new CompilerParameters{
                GenerateExecutable = true, 
                GenerateInMemory = false,
                OutputAssembly = String.Format(@"{0}\{1}.exe", System.Environment.CurrentDirectory,className),
                TreatWarningsAsErrors = false,
                //CompilerOptions = "/optimize",
                //IncludeDebugInformation = true,
                //TempFiles = new TempFileCollection(".", true),
                WarningLevel = 3,

                };
            */


            //Add all references
            var assemblies = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Where(a => !a.IsDynamic)
                                .Select(a => a.Location);

            compParms.ReferencedAssemblies.AddRange(assemblies.ToArray());

            //compile
            var csProvider = new CSharpCodeProvider();
            CompilerResults compilerResults = csProvider.CompileAssemblyFromSource(compParms, sourceCode);
            if (compilerResults.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}", className, compilerResults.PathToAssembly);
                foreach (CompilerError ce in compilerResults.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                // Display a successful compilation message.
                Console.WriteLine("Source {0} built into {1} successfully.", className, compilerResults.PathToAssembly);
            }
        }
    }
}
