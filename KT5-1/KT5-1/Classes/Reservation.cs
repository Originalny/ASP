namespace KT5_1.Classes
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateTime { get; set; }

        public Reservation(int id, string name, int categoryId, DateTime dateTime)
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            DateTime = dateTime;
        }
    }
}
