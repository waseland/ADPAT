
namespace DPA_Musicsheets.Editor
{
    public class Memento
    {
        private string state;

        public Memento(string newState)
        {
            this.state = newState;
        }
        public string getState()
        {
            return state;
        }
    }
}
