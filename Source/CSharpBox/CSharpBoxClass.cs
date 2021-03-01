using BrilliantSkies.Core.Timing;
using BrilliantSkies.Ui.Displayer;
using BrilliantSkies.Ui.Tips;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CSharpBox
{
    public class CSharpBoxClass : BlockWithText
    {
        private bool RunStartMethod;

        private string SourceCode;

        private MethodInfo CompiledUpdateMethod;

        private MethodInfo CompiledStartMethod;

        public bool Running { get; set; } = true;
        public string FilePath { get; set; } = ModPlugin.MyModDirPath + "/Source/CSharpScript/Program.cs";
        public string OutputLog { get; set; }

        public override string SetText(string str)
        {
            if (str != SourceCode)
            {
                if (string.IsNullOrEmpty(str))
                {
                    SourceCode = string.Empty;
                }
                else
                {
                    SourceCode = str;
                }

                GetConstructableOrSubConstructable().iMultiplayerSyncroniser.RPCRequest_SyncroniseBlock(this, SourceCode);
            }

            Compile();

            return str;
        }

        public override string GetText()
        {
            return SourceCode;
        }

        public override void SyncroniseUpdate(string s1)
        {
            SetText(s1);
        }

        public override void StateChanged(IBlockStateChange change)
        {
            base.StateChanged(change);

            if (change.InitiatedOrInitiatedInUnrepairedState_OnlyCalledOnce)
            {
                GetConstructableOrSubConstructable().iBlocksWithText.BlocksWithText.Add(this);
                MainConstruct.SchedulerRestricted.RegisterForFixedUpdate(new Action<ITimeStep>(FixedStep));
            }

            if (change.IsPermanentlyRemovedOrConstructDestroyed)
            {
                GetConstructableOrSubConstructable().iBlocksWithText.BlocksWithText.Remove(this);
                MainConstruct.SchedulerRestricted.UnregisterForFixedUpdate(new Action<ITimeStep>(FixedStep));
            }
        }

        protected override void AppendToolTip(ProTip tip)
        {
            string text1 = "CSharp Box";
            string text2 = "It realizes dynamic generation and execution of CSharp.";
            string text3 = "Press <<Q>> to change CSharp Box settings";

            tip.Add(new ProTipSegment_TitleSubTitle(text1, text2), Position.First);
            tip.Add(new ProTipSegment_Text(400, text3));
        }

        public override void Secondary(Transform T)
        {
            new CSharpBoxUi(this).ActivateGui(GuiActivateType.Standard);
        }

        public void FixedStep(ITimeStep t)
        {
            if (!Running || (CompiledStartMethod == null && CompiledUpdateMethod == null))
            {
                return;
            }

            try
            {
                if (RunStartMethod)
                {
                    RunStartMethod = false;
                    CompiledStartMethod?.Invoke(null, new object[] { this });
                }

                CompiledUpdateMethod?.Invoke(null, new object[] { this });
            }
            catch (Exception e)
            {
                Running = false;
                Log("FixedStep method error catch");
                ErrorOutput(e);
            }
        }

        public void Compile()
        {
            if (string.IsNullOrEmpty(SourceCode)) return;

            ClearLogs();

            CompiledStartMethod = null;
            CompiledUpdateMethod = null;

            try
            {
                string[] assemblyNames = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => a.FullName).ToArray();

                CompilerParameters compiler =
                    new CompilerParameters
                    {
                        GenerateInMemory = true,
                        TreatWarningsAsErrors = false
                    };

                compiler.ReferencedAssemblies.AddRange(assemblyNames);

                CompilerResults results = new CSharpCompiler.CodeCompiler().CompileAssemblyFromSource(compiler, SourceCode);



                if (results.Errors.Count > 0)
                {
                    Log("CSharpCompiler Error");

                    foreach (CompilerError ce in results.Errors)
                    {
                        Log(ce.ToString());
                    }
                }
                else
                {
                    string className = new StringReader(SourceCode).ReadLine().Remove(0, 2);
                    Console.WriteLine("Class name : " + className);

                    Assembly assembly = results.CompiledAssembly;
                    Type type = assembly.GetType(className);

                    CompiledStartMethod = type.GetMethod("Start", BindingFlags.Static | BindingFlags.NonPublic);
                    CompiledUpdateMethod = type.GetMethod("Update", BindingFlags.Static | BindingFlags.NonPublic);

                    RunStartMethod = true;
                    Running = true;
                }
            }
            catch (Exception e)
            {
                Log("Compile method error catch");
                ErrorOutput(e);
            }
        }

        public void Log(string text)
        {
            OutputLog += text + "\n";
        }

        public void ClearLogs()
        {
            OutputLog = string.Empty;
        }

        private void ErrorOutput(Exception e)
        {
            Log($"\nMessage ---\n{e.Message}");
            Log($"\nHelpLink ---\n{e.HelpLink}");
            Log($"\nSource ---\n{e.Source}");
            Log($"\nStackTrace ---\n{e.StackTrace}");
            Log($"\nTargetSite ---\n{e.TargetSite}");
        }
    }
}
