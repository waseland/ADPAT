
namespace DPA_Musicsheets.Commands
{
    public interface CommandTarget
    {
        void OpenFile();
        void SaveFileToLilypond();
        void SaveFileToPdf();
        void AddTekstAtSelection(string _text);
        void SetNewState();
        void AddBarlinesToEditor();
        string UpdateBarlinesFromLilypond();
        
    }
}
