namespace KT5_1.Classes
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Category(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
