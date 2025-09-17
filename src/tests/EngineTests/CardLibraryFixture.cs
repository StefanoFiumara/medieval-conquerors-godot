using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Tests.EngineTests;

[assembly: AssemblyFixture(typeof(CardLibraryFixture))]
namespace MedievalConquerors.Tests.EngineTests;
public class CardLibraryFixture
{
    public CardLibrary Library { get; private set; }

    public CardLibraryFixture()
    {
        using var db = new CardDatabase();
        Library = new CardLibrary(db.Query.ToList());
    }
}