using ExamenProject.Entities.Player;

namespace ExamenProject.Interfaces
{
    public interface IHazard : IGameObject
    {
        void OnHit(Hero hero);
    }
}
