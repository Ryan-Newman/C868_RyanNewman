namespace C868_RyanNewman.Models
{
    // Use of this class is an example of Polymorphism. Both Course and Term classes inherits this class and uses it as polymorphism
    public abstract class BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
