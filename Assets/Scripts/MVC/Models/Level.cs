
namespace MVC.Models
{
    public class Level
    {
        private long ID = 0;
        private string NAME = string.Empty;
        private long IS_UNLOCKED = 0;
        private long IS_COMPLETED = 0;
        private string LAYOUT = string.Empty;

        public long Id { get => ID; set => ID = value; }
        public string Name { get => NAME; set => NAME = value; }
        public bool IsUnlocked { get => IS_UNLOCKED == 1; set => IS_UNLOCKED = (value ? 1 : 0); }
        public bool IsCompleted { get => IS_COMPLETED == 1; set => IS_COMPLETED = (value ? 1 : 0); }
        public string Layout { get => LAYOUT; set => LAYOUT = value; }
    }
}
