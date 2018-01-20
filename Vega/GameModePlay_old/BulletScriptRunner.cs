using Vega.GameModePlay.StageScript;

namespace Vega.GameModePlay
{
    public class BulletScriptRunner : StageScriptRunner
    {
        public Bullet Parent { get; set; }

        public BulletScriptRunner(StageScript.StageScript script, int entryPoint)
            : base(script, entryPoint, 10)
        {
        }

        protected override void DoOp(StageScriptOp op)
        {
            switch (op.OpCode)
            {
                case OpCode.Animation:
                    this.Parent.SetTexture(this.Stack.Pop().ArgI);
                    break;
                case OpCode.DefaultHitbox:
                    this.Parent.DefaultHitbox();
                    break;
                case OpCode.Hitbox:
                    this.Parent.SetHitbox(this.Stack.Pop().ArgF, this.Stack.Pop().ArgF, this.Stack.Pop().ArgF);
                    break;
                case OpCode.Color:
                    this.Parent.Color = this.Int32ToColor(this.Stack.Pop().ArgI);
                    break;
                case OpCode.Velocity:
                    this.Parent.SetVelocity(this.Stack.Pop().ArgI, this.Stack.Pop().ArgF, this.Stack.Pop().ArgF);
                    break;
                case OpCode.GetAngle:
                    this.Stack.Push(new ScriptStackEntry(0, this.Parent.Angle, true));
                    break;
                case OpCode.Angle:
                    this.Parent.Angle = this.Stack.Pop().ArgF;
                    break;
                case OpCode.AngleSpeed:
                    this.Parent.AngleSpeed = this.Stack.Pop().ArgF;
                    break;
                case OpCode.Delete:
                    this.Parent.Delete();
                    break;
                case OpCode.GetLaunchAngle:
                    this.Stack.Push(new ScriptStackEntry(0, this.Parent.LaunchAngle, true));
                    break;
                case OpCode.GetPlayerAngle:
                    this.Stack.Push(new ScriptStackEntry(0, this.Parent.GetPlayerAngle(), true));
                    break;
                case OpCode.GetParentAngle:
                    this.Stack.Push(new ScriptStackEntry(0, this.Parent.ParentAngle, true));
                    break;
                default:
                    this.ThrowException(string.Format("Operation `{0}` cannot be called in bullet segment", op.OpCode));
                    break;
            }
        }
    }
}
