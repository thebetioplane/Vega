
namespace Vega.GameModePlay
{
    public class Slave
        : Enemy
    {
        public Enemy Master;

        public Slave(GameModePlay parent, Enemy master, bool mirror, EnemyScriptRunner runner)
            : base(parent, mirror, false, master.Position.X, master.Position.Y, runner)
        {
            this.Master = master;
            this.IsSlave = true;
            // we are un-doing the shift that enemy inherits when it is mirrored
            // this way it still spawns on top of the master
            if (this.Mirrored)
                this.Position.X = 900.0f - this.Position.X;
        }
    }
}
