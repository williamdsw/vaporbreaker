
namespace MVC.Models
{
    public class Track
    {
        private long ID;
        private string TITLE;
        private string ARTIST;
        private string COVER;
        private string FILENAME;

        public long Id { get => ID; set => ID = value; }
        public string Title { get => TITLE; set => TITLE = value; }
        public string Artist { get => ARTIST; set => ARTIST = value; }
        public string Cover { get => COVER; set => COVER = value; }
        public string FileName { get => FILENAME; set => FILENAME = value; }
    }
}
