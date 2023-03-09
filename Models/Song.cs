namespace MathGame
{
    public class Song
    {
        public string Name { get; private set; }
        public int FileName { get; private set; }

        public Song(string name, int fileName)
        {
            Name = name;
            FileName = fileName;
        }
    }
}