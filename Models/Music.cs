namespace MathGame
{
    public class Song
    {
        public string Name { get; private set; }
        public int File { get; private set; }

        public Song(string name, int file)
        {
            Name = name;
            File = file;
        }
    }
}