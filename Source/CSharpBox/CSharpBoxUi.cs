using BrilliantSkies.Ui.Consoles;

namespace CSharpBox
{
    public class CSharpBoxUi : ConsoleUi<CSharpBoxClass>
    {
        public CSharpBoxUi(CSharpBoxClass focus) : base(focus)
        {
        }

        protected override ConsoleWindow BuildInterface(string suggestedName = "")
        {
            ConsoleWindow consoleWindow_0 = NewWindow("CSharp Box Ui", new ScaledRectangle(100f, 100f, 1080f, 600f));
            consoleWindow_0.DisplayTextPrompt = false;
            consoleWindow_0.SetMultipleTabs(new CSharpBoxUiTab_0(consoleWindow_0, _focus), new CSharpBoxUiTab_1(consoleWindow_0, _focus));

            return consoleWindow_0;
        }
    }
}
