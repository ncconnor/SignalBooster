using Synapse.SignalBoosterExample.Models;

namespace Synapse.SignalBoosterExample.NoteOrderProcessor
{
    public interface IPhysicianNoteOrderProcessor
    {
        MedicalOrder Parse(string noteText);
    }
}