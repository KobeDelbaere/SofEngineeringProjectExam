using ExamenProject.Entities.Player;

namespace ExamenProject.Interfaces
{
    public interface ILevelEnd : IGameObject
    {
        void OnHit(Hero hero);
    }
}
