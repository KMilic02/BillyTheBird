public interface IDamageable
{
    public int health { get; set; }

    public void IOnDamage(int damage);
    public void IOnDeath();
}
