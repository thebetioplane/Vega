using Vega.GameModePlay.StageScript;

namespace Vega.GameModePlay
{
    public class MainScriptRunner : StageScriptRunner
    {
        private GameModePlay Parent;
        private bool _BossFightWaiting = false;
        public bool BossFightWaiting
        {
            get { return this._BossFightWaiting; }
            set
            {
                if (value == this._BossFightWaiting)
                    return;
                this._BossFightWaiting = value;
                this.Suspended = value;
            }
        }

        public MainScriptRunner(GameModePlay parent, StageScript.StageScript script)
            : base(script)
        {
            this.Parent = parent;
        }

        protected override void DoOp(StageScriptOp op)
        {
            switch (op.OpCode)
            {
                case OpCode.Spawn:
                    this.Parent.AddEnemy(this.Stack.Pop().ArgI, this.Stack.Pop().ArgF, this.Stack.Pop().ArgF, false, false);
                    break;
                case OpCode.Mirror:
                    this.Parent.AddEnemy(this.Stack.Pop().ArgI, this.Stack.Pop().ArgF, this.Stack.Pop().ArgF, true, false);
                    break;
                case OpCode.BossSpawn:
                    this.Parent.AddEnemy(this.Stack.Pop().ArgI, this.Stack.Pop().ArgF, this.Stack.Pop().ArgF, false, true);
                    this.Parent.BossFightInProgress = true;
                    break;
                case OpCode.BackgroundColor:
                    this.Parent.SetBackgroundColor(this.Int32ToColor(this.Stack.Pop().ArgI));
                    break;
                case OpCode.BackgroundTransition:
                    this.Parent.TransitionBackground(this.Stack.Pop().ArgI, this.Int32ToColor(this.Stack.Pop().ArgI));
                    break;
                case OpCode.BackgroundSpeed:
                    this.Parent.SetBackgroundSpeed(this.Stack.Pop().ArgF);
                    break;
                case OpCode.WaitForBoss:
                    if (this.Parent.BossFightInProgress)
                        this.BossFightWaiting = true;
                    else
                        this.ThrowException("There is not a boss fight in progress");
                    break;
                case OpCode.PlaySound:
                    this.Parent.PlaySound(this, this.Stack.Pop().ArgI);
                    break;
                case OpCode.DeleteBullets:
                    this.Parent.BulletsToDelete.AddRange(this.Parent.Bullets);
                    break;
                case OpCode.BackgroundPattern:
                    this.Parent.SetBackground(this, this.Stack.Pop().ArgI);
                    break;
                case OpCode.Dialog:
                    this.ThrowException("Not implemented");
                    break;
                default:
                    this.ThrowException(string.Format("Operation `{0}` cannot be called in main segment", op.OpCode));
                    break;
            }
        }
    }
}
