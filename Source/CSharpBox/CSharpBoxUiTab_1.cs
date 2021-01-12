using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Texts;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Tips;

namespace CSharpBox
{
    public class CSharpBoxUiTab_1 : SuperScreen<CSharpBoxClass>
    {
        public CSharpBoxUiTab_1(ConsoleWindow window, CSharpBoxClass focus) : base(window, focus)
        {
        }

        public override Content Name
        {
            get
            {
                return new Content("SourceCode", new ToolTip(""), "CSharpBoxUiTab_2");
            }
        }

        public override void Build()
        {
            ScreenSegmentStandard screenSegment_0 = CreateStandardSegment(InsertPosition.OnCursor);

            TextInput<CSharpBoxClass> textInput_1 = TextInput<CSharpBoxClass>.Quick(_focus, M.m((CSharpBoxClass c) => c.GetText()), string.Empty, null, (CSharpBoxClass c, string s) => { });

            screenSegment_0.AddInterpretter(textInput_1);
        }
    }
}
