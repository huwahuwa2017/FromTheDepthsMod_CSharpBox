using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Texts;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Tips;
using System.IO;

namespace CSharpBox
{
    public class CSharpBoxUiTab_0 : SuperScreen<CSharpBoxClass>
    {
        public CSharpBoxUiTab_0(ConsoleWindow window, CSharpBoxClass focus) : base(window, focus)
        {
        }

        public override Content Name
        {
            get
            {
                return new Content("File read settings", new ToolTip(""), "CSharpBoxUiTab_1");
            }
        }

        public override void Build()
        {
            CreateHeader("File read settings");

            ScreenSegmentStandard screenSegment_0 = CreateStandardSegment(InsertPosition.OnCursor);

            TextInput<CSharpBoxClass> textInput_0 = TextInput<CSharpBoxClass>.Quick(_focus, M.m((CSharpBoxClass c) => c.FilePath), "FilePath : ", new ToolTip(""), (CSharpBoxClass c, string s) => c.FilePath = s);

            SubjectiveButton<CSharpBoxClass> SubjectiveButton_0 = SubjectiveButton<CSharpBoxClass>.Quick(_focus, "Compile & Start", new ToolTip("Compile"),
                (CSharpBoxClass c) =>
                {
                    if (File.Exists(c.FilePath))
                    {
                        c.SetText(File.ReadAllText(c.FilePath));
                    }
                });
            SubjectiveButton_0.FadeOut = M.m<CSharpBoxClass>(c => !File.Exists(c.FilePath));

            screenSegment_0.AddInterpretter(textInput_0);
            screenSegment_0.AddInterpretter(SubjectiveButton_0);
            screenSegment_0.AddInterpretter(new Empty());



            ScreenSegmentTable screenSegment_1 = CreateTableSegment(2, 1);
            screenSegment_1.SqueezeTable = false;

            SubjectiveButton<CSharpBoxClass> SubjectiveButton_1 = SubjectiveButton<CSharpBoxClass>.Quick(_focus, "Activation", new ToolTip("Start running"),
                (CSharpBoxClass c) =>
                {
                    c.Running = true;
                });
            SubjectiveButton_1.FadeOut = M.m((CSharpBoxClass c) => c.Running);

            SubjectiveButton<CSharpBoxClass> SubjectiveButton_2 = SubjectiveButton<CSharpBoxClass>.Quick(_focus, "Invalidation", new ToolTip("Stop running"),
                (CSharpBoxClass c) =>
                {
                    c.Running = false;
                });
            SubjectiveButton_2.FadeOut = M.m((CSharpBoxClass c) => !c.Running);

            screenSegment_1.AddInterpretter(SubjectiveButton_1);
            screenSegment_1.AddInterpretter(SubjectiveButton_2);



            CreateHeader("Output Log");

            ScreenSegmentStandard screenSegment_2 = CreateStandardSegment(InsertPosition.OnCursor);

            TextInput<CSharpBoxClass> textInput_1 = TextInput<CSharpBoxClass>.Quick(_focus, M.m((CSharpBoxClass c) => c.OutputLog), string.Empty, null, (CSharpBoxClass c, string s) => { });

            screenSegment_2.AddInterpretter(textInput_1);
        }





    }
}
