using Vega.GameModePlay.StageScript;
using MathHelper = OpenTK.MathHelper;

namespace Vega.GameModePlay
{
    public class EnemyScriptRunner : StageScriptRunner
    {
        public Enemy Parent { get; set; }
        private int BulletAttribEntry = 0;
        private float BulletAttribLaunchAngle = 0.0f;

        public EnemyScriptRunner(StageScript.StageScript ss, int entryPoint)
            : base(ss, entryPoint, 20)
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
                case OpCode.Speed:
                    this.Parent.Velocity = this.Stack.Pop().ArgF;
                    break;
                case OpCode.GetAngle:
                    this.Stack.Push(new ScriptStackEntry(0, this.Parent.Angle, true));
                    break;
                case OpCode.Angle:
                    if (this.Parent.Mirrored)
                        this.Parent.Angle = MathHelper.Pi - this.Stack.Pop().ArgF;
                    else
                        this.Parent.Angle = this.Stack.Pop().ArgF;
                    break;
                case OpCode.AngleSpeed:
                    if (this.Parent.Mirrored)
                        this.Parent.AngleSpeed = -this.Stack.Pop().ArgF;
                    else
                        this.Parent.AngleSpeed = this.Stack.Pop().ArgF;
                    break;
                case OpCode.MoveLinear:
                    this.Parent.MoveLinear(this.Stack.Pop().ArgI, this.Stack.Pop().ArgF, this.Stack.Pop().ArgF);
                    break;
                case OpCode.MoveCubic:
                    this.Parent.MoveCubic(this.Stack.Pop().ArgI, this.Stack.Pop().ArgF, this.Stack.Pop().ArgF);
                    break;
                case OpCode.Delete:
                    this.Parent.Delete();
                    break;
                case OpCode.Delay:
                    this.Parent.Delay = this.Stack.Pop().ArgI;
                    break;
                case OpCode.GetPlayerAngle:
                    this.Stack.Push(new ScriptStackEntry(0, this.Parent.GetPlayerAngle(), true));
                    break;
                case OpCode.BulletType:
                    this.BulletAttribEntry = this.Stack.Pop().ArgI;
                    break;
                case OpCode.BulletAngle:
                    this.BulletAttribLaunchAngle = this.Stack.Pop().ArgF;
                    break;
                case OpCode.SingleBullet:
                    this.Parent.CreateBullet(0, this.BulletAttribEntry, 2, 1, this.BulletAttribLaunchAngle);
                    break;
                case OpCode.LeftBullet:
                    this.Parent.CreateBullet(0, this.BulletAttribEntry, this.Stack.Pop().ArgI,
                        this.Stack.Pop().ArgI, this.BulletAttribLaunchAngle);
                    break;
                case OpCode.RightBullet:
                    this.Parent.CreateBullet(1, this.BulletAttribEntry, this.Stack.Pop().ArgI,
                        this.Stack.Pop().ArgI, this.BulletAttribLaunchAngle);
                    break;
                case OpCode.CenterBullet:
                    this.Parent.CreateBullet(2, this.BulletAttribEntry, this.Stack.Pop().ArgI,
                        this.Stack.Pop().ArgI, this.BulletAttribLaunchAngle);
                    break;
                case OpCode.Health:
                    this.Parent.Health = this.Stack.Pop().ArgI;
                    break;
                case OpCode.GetHealth:
                    this.Stack.Push(new ScriptStackEntry(this.Parent.Health, 0, false));
                    break;
                case OpCode.MaxHealth:
                    this.Parent.MaxHealth = this.Stack.Pop().ArgI;
                    break;
                case OpCode.GetMaxHealth:
                    this.Stack.Push(new ScriptStackEntry(this.Parent.MaxHealth, 0, false));
                    break;
                case OpCode.PlaySound:
                    this.Parent.Parent.PlaySound(this, this.Stack.Pop().ArgI);
                    break;
                case OpCode.SlaveSpawn:
                    this.Parent.Parent.AddSlave(this.Stack.Pop().ArgI, this.Parent, false, 0.0f);
                    break;
                case OpCode.SlaveMirror:
                    this.Parent.Parent.AddSlave(this.Stack.Pop().ArgI, this.Parent, true, 0.0f);
                    break;
                case OpCode.SlaveSymmetricSpawn:
                    {
                        int n = this.Stack.Pop().ArgI;
                        int entry = this.Stack.Pop().ArgI;
                        float angleDelta = MathHelper.TwoPi / n;
                        for (int i = 0; i < n; i++)
                            this.Parent.Parent.AddSlave(entry, this.Parent, false, this.BulletAttribLaunchAngle + angleDelta * i);
                    }
                    break;
                default:
                    this.ThrowException(string.Format("Operation `{0}` cannot be called in enemy segment", op.OpCode));
                    break;
            }
        }
    }
}
