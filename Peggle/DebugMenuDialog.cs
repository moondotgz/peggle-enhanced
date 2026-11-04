using System;

namespace Sexy
{
    /// <summary>
    /// Port debug aid (not in the original): SEXY_DEBUG_MENU=1 lets F1 open this menu during play, to set up
    /// things that are slow to reach by playing: Extreme Fever and Ultra Extreme Fever on the next peg hit, the
    /// fever text on its own, and more balls. Not shown in normal play.
    /// </summary>
    public class DebugMenuDialog : ThunderDialog
    {
        public static readonly bool gEnabled = Environment.GetEnvironmentVariable("SEXY_DEBUG_MENU") == "1";

        public const int DebugMenu_ExtremeFever = 0;

        public const int DebugMenu_UltraFever = 1;

        public const int DebugMenu_ExtremeText = 2;

        public const int DebugMenu_UltraText = 3;

        public const int DebugMenu_Balls = 4;

        public const int DebugMenu_Close = 5;

        private static readonly string[] LABELS =
        {
            "Extreme Fever next hit", "Ultra Fever next hit", "Show Extreme Fever text", "Show Ultra Fever text",
            "+10 balls", "Close",
        };

        private const int BUTTON_GAP = 5;

        public ThunderballApp mApp;

        public ThunderButton[] mButtons = new ThunderButton[LABELS.Length];

        public DebugMenuDialog(ThunderballApp theApp)
            : base(DialogId.DIALOG_DEBUG_MENU, true, "DEBUG", "", "", 0)
        {
            mApp = theApp;
            for (int i = 0; i < mButtons.Length; i++)
            {
                mButtons[i] = SexyUtil.MakeButton(i, this, LABELS[i]);
                AddWidget(mButtons[i]);
            }
        }

        public override void Dispose()
        {
            RemoveAllWidgets(true, false);
            base.Dispose();
        }

        private int GetButtonsTop()
        {
            return mBackgroundInsets.mTop + mContentInsets.mTop + mHeaderFont.GetHeight() + mSpaceAfterHeader;
        }

        public override int GetPreferredHeight(int theWidth)
        {
            return GetButtonsTop() + mButtons.Length * (mButtons[0].mHeight + BUTTON_GAP) + mBackgroundInsets.mBottom + mContentInsets.mBottom;
        }

        public override void Resize(int theX, int theY, int theWidth, int theHeight)
        {
            base.Resize(theX, theY, theWidth, theHeight);
            int aY = GetButtonsTop();
            foreach (ThunderButton aButton in mButtons)
            {
                aButton.Resize((mWidth - aButton.mWidth) / 2, aY, aButton.mWidth, aButton.mHeight);
                aY += aButton.mHeight + BUTTON_GAP;
            }
        }

        public override void ButtonDepress(int theId)
        {
            if (IsScrolling())
            {
                return;
            }
            Board aBoard = mApp.mBoard;
            LogicMgr aLogic = aBoard?.mLogicMgr;
            if (aLogic != null)
            {
                switch (theId)
                {
                case DebugMenu_ExtremeFever:
                    aLogic.mDebugFeverNext = 1;
                    break;
                case DebugMenu_UltraFever:
                    aLogic.mDebugFeverNext = 2;
                    break;
                case DebugMenu_ExtremeText:
                case DebugMenu_UltraText:
                    aLogic.AddExtremeFeverEffect(theId == DebugMenu_UltraText);
                    break;
                case DebugMenu_Balls:
                    aLogic.mBallsLeft[aLogic.mCurPlayer] += 10;
                    aBoard.mInterfaceMgr.SetNumBalls(aLogic.mBallsLeft[aLogic.mCurPlayer]);
                    break;
                }
            }
            mApp.KillDialog(DialogId.DIALOG_DEBUG_MENU);
            mApp.PauseBoard(false);
        }
    }
}
